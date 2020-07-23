using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Unity.Tiny;
using Unity.Tiny.Assertions;

namespace Unity.Tiny.Rendering
{
    public struct LightMatrices : IComponentData
    {
        public float4x4 projection;
        public float4x4 view;
        public float4x4 mvp;
        public Frustum frustum;
    }

    public struct Light : IComponentData
    {
        // always points in z direction 
        public float clipZNear; // near clip, applies only for mapped lights 
        public float clipZFar;
        
        public float intensity;
        public float3 color;
        // if no other components are not to a light, it's a simple non-shadowed omni light
    }

    public struct ShadowmappedLight : IComponentData // next to light
    {
        public int shadowMapResolution;     // for auto creation, this is the texture resolution, so if there are multiple cascades in the map this includes all of them 
        public Entity shadowMap;            // the shadow map texture
        public Entity shadowMapRenderNode;  // node used for shadow map creation
    }

    public struct CascadeShadowmappedLight : IComponentData // next to light and ShadowMappedLight, AutoMovingDirectionalLight, and DirectionalLight
    {
        public float3 cascadeScale;      // The four cascades are scaled according to these weights, the largest cascade has an implicit weight of 1
                                         // 1>x>y>z>0. z is the scale of the highest detail cascade.
        public float cascadeBlendWidth;  // Blend width for blending between cascades: 0=no blending, 1=maximum blending 
        public Entity camera;            // The camera this cascade is computed from - must match the camera rendering the shadows 
    }

    public struct CascadeData
    {
        public float4x4 view;
        public float4x4 proj;
        public Frustum frustum;
        public float2 offset;
        public float scale;
    }

    public struct CascadeShadowmappedLightCache : IComponentData // next to CascadeShadowmappedLight
    {
        public CascadeData c0;
        public CascadeData c1;
        public CascadeData c2;
        public CascadeData c3;

        public CascadeData GetCascadeData(int idx)
        {
            switch ( idx ) {
                default: Assert.IsTrue(idx==0); return c0;
                case 1: return c1;
                case 2: return c2;
                case 3: return c3;
            }
        }

        public void SetCascadeData(int idx, in CascadeData cd)
        {
            switch ( idx ) {
                case 0: c0 = cd; break;
                case 1: c1 = cd; break;
                case 2: c2 = cd; break;
                case 3: c3 = cd; break;
                default: Assert.IsTrue(false); break;
            }
        }
    }

    public struct SpotLight : IComponentData // next to light
    {
        // always points in z direction
        public float fov; // in degrees 
        public float innerRadius; // [0..1[, start of circle falloff 1=sharp circle, 0=smooth, default 0
        public float ratio; // ]0..1] 1=circle, 0=line, default 1
    }

    public struct DirectionalLight : IComponentData // next to light
    {
    }

    // This component automatically updates a directional lights position & size 
    // so the shadow map covers the intersection of the bounds of interest and the cameras frustum
    // because it changes the size and position of the directional light it is not suitable for projection textures in the light
    // Also requires a NonUniformScale component next to it
    public struct AutoMovingDirectionalLight : IComponentData // next to mapped directional light 
    {
        public AABB bounds;                 // bounds of the world to track (world space)
        public bool autoBounds;             // automatically get bounds from world bounds of renderers
        public Entity clipToCamera;         // if not Entity.Null, clip the shadow receivers bounds to the frustum of the camera
                                            // entity here. The entity pointed to here must have a Frustum component.
        public AABB boundsClipped;          // set to the clipped receiver bounds if clipToCamera is set (world space)
    }
    
    public struct LightToBGFXLightingSetup : IBufferElementData
    {
        public Entity e; // the light should add itself to this lighting setup
    }

    /// <summary>
    /// Ambient light. To add next to entity with a Light component on it.
    /// The ambient light color and intensity must be set in the Light Component.
    /// </summary>
    public struct AmbientLight : IComponentData { }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateBefore(typeof(UpdateLightMatricesSystem))]
    [UpdateAfter(typeof(UpdateWorldBoundsSystem))]
    public class UpdateAutoMovingLightSystem : ComponentSystem
    {
        private AABB RotateBounds (ref float4x4 tx, ref AABB b)
        {
            WorldBounds wBounds;
            Culling.AxisAlignedToWorldBounds(ref tx, ref b, out wBounds);
            // now turn those bounds back to axis aligned.. 
            AABB aab;
            Culling.WorldBoundsToAxisAligned(ref wBounds, out aab);
            return aab;
        }

        static bool ClipLinePlane(ref float3 p0, ref float3 p1, float4 plane)
        {
            bool p0inside = math.dot(plane.xyz, p0) >= -plane.w;
            bool p1inside = math.dot(plane.xyz, p1) >= -plane.w;
            if (!p0inside && !p1inside) 
                return false; // both outside
            if (p0inside && p1inside)
                return true; // both inside, no need to change p0 and p1 
            // clip 
            float3 dp = p1 - p0;
            float dp0 = math.dot(plane.xyz, p0);
            float dpd = math.dot(plane.xyz, dp);
            float t = -(plane.w + dp0) / dpd;
            if ( !(t>0.0f && t<1.0f) ) { // if dpd == 0, point on plane
                if (p0inside) p1 = p0;
                else p0 = p1;
                return true;
            }
            float3 p = p0 + t * dp;
            if (p0inside) p1 = p;
            else p0 = p;
            return true;
        }

        static unsafe int ClipLineFrustum(float3 p0, float3 p1, in Frustum f, float3 *dest)
        {
            for ( int i=0; i<f.PlanesCount; i++ ) {
                if (!ClipLinePlane(ref p0, ref p1, f.GetPlane(i)))
                    return 0;
            }
            dest[0] = p0;
            dest[1] = p1;
            return 2;
        }

        static AABB ClipAABBByFrustum(in AABB b, in Frustum f, in Camera cam, in float4x4 camTx) 
        {
            AABB r = default;

            float3 bMin = b.Min;
            float3 bMax = b.Max;
            unsafe
            {
                float3* insidePoints = stackalloc float3[48];
                int nInsidePoints = 0;
                // clip the 12 edge lines of the aab into the frustum, and add their end points
                // this is not optimal, but robust 
                for ( int i=0; i<Culling.EdgeTable.Length; i++ ) {
                    float3 p0 = Culling.SelectCoordsMinMax(bMin, bMax, Culling.EdgeTable[i]&7);
                    float3 p1 = Culling.SelectCoordsMinMax(bMin, bMax, Culling.EdgeTable[i]>>3);
                    nInsidePoints += ClipLineFrustum(p0, p1, in f, insidePoints + nInsidePoints);
                }
                // clip the 12 edge lines of the furstum into the aab, and add their end points 
                Frustum f2;
                ProjectionHelper.FrustumFromAABB(b, out f2);
                WorldBounds wb = UpdateCameraMatricesSystem.BoundsFromCamera(in cam);
                Culling.TransformWorldBounds(in camTx, ref wb);
                for ( int i=0; i<Culling.EdgeTable.Length; i++ ) {
                    float3 p0 = wb.GetVertex(Culling.EdgeTable[i]&7);
                    float3 p1 = wb.GetVertex(Culling.EdgeTable[i]>>3);
                    nInsidePoints += ClipLineFrustum(p0, p1, in f2, insidePoints + nInsidePoints);
                }
                if (nInsidePoints > 0) {
                    float3 bbMin = insidePoints[0];
                    float3 bbMax = bbMin;
                    for ( int i=1; i<nInsidePoints; i++ ) {
                        bbMin = math.min(insidePoints[i], bbMin);
                        bbMax = math.max(insidePoints[i], bbMax);
                    }
                    r.Center = (bbMax+bbMin)*.5f;
                    r.Extents = (bbMax-bbMin)*.5f;
                }
            }
            return r;
        }

        void AssignCascades(ref AutoMovingDirectionalLight amdl, ref LocalToWorld ltw, ref Rotation rx, ref CascadeShadowmappedLight csm, ref CascadeShadowmappedLightCache csmDest)
        {
            Assert.IsTrue(amdl.clipToCamera == csm.camera || amdl.clipToCamera == Entity.Null);
            Assert.IsTrue(csm.cascadeBlendWidth >= 0.0f && csm.cascadeBlendWidth <= 1.0f);
            Assert.IsTrue(0.0f < csm.cascadeScale.z && csm.cascadeScale.z < csm.cascadeScale.y && csm.cascadeScale.y < csm.cascadeScale.x && csm.cascadeScale.x < 1.0f);

            var camTx = EntityManager.GetComponentData<LocalToWorld>(csm.camera);
            var invLight = math.inverse(ltw.Value);
            // transform camera to light space, that's where we want to have the most samples! 
            float3 camPos = math.transform(invLight, camTx.Value.c3.xyz);

            for (int cascadeIndex = 0; cascadeIndex < 4; cascadeIndex++) {
                float ratio = 1.0f;
                float2 useOffset = camPos.xy;
                switch (cascadeIndex) {
                    case 0:
                        //ratio = 1.0f;
                        useOffset = new float2(0);
                        break;
                    case 1:
                        ratio = csm.cascadeScale.x;
                        break;
                    case 2:
                        ratio = csm.cascadeScale.y;
                        break;
                    case 3: // highest res
                        ratio = csm.cascadeScale.z;
                        break;
                    default:
                        Assert.IsTrue(false);
                        break;
                }
                float invRatio = 1.0f / ratio;
                useOffset = useOffset * -invRatio;

                CascadeData cd = default;
                // this is used for RENDERING the cascade
                cd.proj = ProjectionHelper.ProjectionMatrixUnitOrthoOffset(useOffset, invRatio);
                cd.view = invLight;
                // this is used for SAMPLING the cascade
                cd.scale = invRatio;
                cd.offset = useOffset;
                ProjectionHelper.FrustumFromMatrices(cd.proj, cd.view, out cd.frustum);
                csmDest.SetCascadeData(cascadeIndex, cd);
            }
        }

        void AssignSimpleAutoBounds(ref AutoMovingDirectionalLight amdl, ref LocalToWorld ltw, ref Rotation rx, ref Translation tx, ref NonUniformScale sc) { 
            AABB bounds = amdl.bounds;
            if (amdl.clipToCamera!=Entity.Null) {
                var camMatrices =  EntityManager.GetComponentData<CameraMatrices>(amdl.clipToCamera);
                var cam = EntityManager.GetComponentData<Camera>(amdl.clipToCamera);
                var camTx = EntityManager.GetComponentData<LocalToWorld>(amdl.clipToCamera);
                amdl.boundsClipped = ClipAABBByFrustum(in bounds, in camMatrices.frustum, in cam, in camTx.Value);
                //Assert.IsTrue(recvBounds.Contains(amdl.boundsClippedReceivers));
                bounds = amdl.boundsClipped;
            }

            // transform bounds into light space rotation
            float4x4 rotOnlyTx = new float4x4(rx.Value, new float3(0));
            float4x4 rotOnlyTxInv = new float4x4(math.inverse(rx.Value), new float3(0));

            AABB lsBounds = RotateBounds(ref rotOnlyTxInv, ref bounds);

            float3 posls;
            posls.x = lsBounds.Center.x;
            posls.y = lsBounds.Center.y;
            posls.z = lsBounds.Center.z - lsBounds.Extents.z;
            tx.Value = math.transform(rotOnlyTx, posls); // back to world space
            float size = math.max(lsBounds.Extents.x, lsBounds.Extents.y);
            sc.Value.x = size;
            sc.Value.y = size;
            sc.Value.z = lsBounds.Extents.z * 2.0f;

            // also write back to local to world, as it's going to get used later
            ltw.Value = math.mul ( new float4x4(rx.Value, tx.Value), float4x4.Scale(sc.Value) );
        }

        protected override void OnUpdate() 
        {
#if DEBUG
            // debug check that csm lights are AutoMovingDirectionalLight
            Entities.WithNone<AutoMovingDirectionalLight>().WithAll<CascadeShadowmappedLight>().ForEach((Entity e) => {
                Assert.IsTrue(false, "Lights with CascadeShadowmappedLight must include AutoMovingDirectionalLight component for bounds." );
            });
#endif
            // add csm caches to lights
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            Entities.WithNone<CascadeShadowmappedLightCache>().WithAll<CascadeShadowmappedLight>().ForEach((Entity e) => 
            {
                ecb.AddComponent<CascadeShadowmappedLightCache>(e);
            });
            ecb.Playback(EntityManager);
            ecb.Dispose();

            var sysBounds = World.GetExistingSystem<UpdateWorldBoundsSystem>();
            Entities.WithAll<DirectionalLight>().ForEach((Entity eLight, ref AutoMovingDirectionalLight amdl,
                ref Light l, ref LocalToWorld ltw, ref Rotation rx, ref Translation tx, ref NonUniformScale sc) => 
            {
                Assert.IsTrue(!EntityManager.HasComponent<Parent>(eLight), "Auto moving directional lights can not have a parent transform" );
                if (amdl.autoBounds)
                    amdl.bounds =  sysBounds.m_wholeWorldBounds;
                // TODO: split into two loops, BUT can not have that many components in ForEach 
                l.clipZFar = 1.0f; 
                l.clipZNear = 0.0f;
                AssignSimpleAutoBounds(ref amdl, ref ltw, ref rx, ref tx, ref sc);
                if ( EntityManager.HasComponent<CascadeShadowmappedLight>(eLight)) {
                    var csm = EntityManager.GetComponentData<CascadeShadowmappedLight>(eLight);
                    var csmDest = EntityManager.GetComponentData<CascadeShadowmappedLightCache>(eLight);
                    AssignCascades(ref amdl, ref ltw, ref rx, ref csm, ref csmDest);
                    EntityManager.SetComponentData<CascadeShadowmappedLightCache>(eLight, csmDest);
                }
            });
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class UpdateLightMatricesSystem : ComponentSystem
    {
        protected override void OnUpdate() 
        {
            // add matrices component if needed 
            Entities.WithNone<LightMatrices>().WithAll<Light>().ForEach((Entity e) =>
            {
                EntityManager.AddComponent<LightMatrices>(e);
            });
            
            // update 
            Entities.ForEach((ref Light c, ref LocalToWorld tx, ref LightMatrices m, ref SpotLight sl) =>
            { // spot light
                m.projection = ProjectionHelper.ProjectionMatrixPerspective(c.clipZNear, c.clipZFar, sl.fov, 1.0f);
                m.view = math.inverse(tx.Value);
                m.mvp = math.mul(m.projection, m.view);
                ProjectionHelper.FrustumFromMatrices(m.projection, m.view, out m.frustum);
            });
            Entities.ForEach((ref Light c, ref LocalToWorld tx, ref LightMatrices m, ref DirectionalLight dr) =>
            { // directional
                m.projection = ProjectionHelper.ProjectionMatrixOrtho(0.0f, 1.0f, 1.0f, 1.0f);
                m.view = math.inverse(tx.Value);
                m.mvp = math.mul(m.projection, m.view);
                ProjectionHelper.FrustumFromMatrices(m.projection, m.view, out m.frustum);
            });
            Entities.WithNone<DirectionalLight, SpotLight>().ForEach((ref Light c, ref LocalToWorld tx, ref LightMatrices m) =>
            { // point
                m.projection = float4x4.identity;
                m.view = math.inverse(tx.Value);
                m.mvp = math.mul(m.projection, m.view);
                // build furstum from bounds 
                ProjectionHelper.FrustumFromCube(tx.Value.c3.xyz, c.clipZFar, out m.frustum);
            });
        }
    }
}

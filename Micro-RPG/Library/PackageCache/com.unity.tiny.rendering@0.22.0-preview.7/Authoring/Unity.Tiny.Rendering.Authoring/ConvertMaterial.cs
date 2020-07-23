using Unity.Entities;
using Unity.Tiny;
using Unity.Tiny.Rendering;
using Unity.Mathematics;
using UnityEngine;
using Unity.Entities.Runtime.Build;
using UnityEditor;

namespace Unity.TinyConversion
{
    [UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    class MaterialDeclareAssets : GameObjectConversionSystem
    {
        protected override void OnUpdate() =>
            Entities.ForEach((UnityEngine.Material uMaterial) =>
            {
                int[] ids = uMaterial.GetTexturePropertyNameIDs();
                for (int i = 0; i < ids.Length; i++)
                {
                    DeclareReferencedAsset(uMaterial.GetTexture(ids[i]));
                }
            });
    }

    [UpdateInGroup(typeof(GameObjectConversionGroup))]
    public class MaterialConversion : GameObjectConversionSystem
    {
        private BlendOp GetBlending(float blend)
        {
            switch (blend)
            {
                case 0:
                    return BlendOp.Alpha;
                case 2:
                    return BlendOp.Add;
                case 3:
                    return BlendOp.Multiply;
                default:
                    return BlendOp.Alpha;
            }
        }

        private Entity GetTextureEntity(Material mat, string textureName)
        {
            if (mat.HasProperty(textureName))
                return GetPrimaryEntity(mat.GetTexture(textureName) as Texture2D);
            return Entity.Null;
        }

        private bool IsTwoSided(Material uMaterial)
        {
            //both = 0, back = 1, front = 2
            if (uMaterial.GetInt("_Cull") == 0)
                return true;
            else if(uMaterial.GetInt("_Cull") == 1)
            {
                UnityEngine.Debug.LogWarning("Setting a value of Back for Render Face is not supported. Choose either Both or Front in material: " + uMaterial.name);
            }
            return false;
        }

        private void ConvertUnlitMaterial(Entity entity, Material uMaterial)
        {
            //Do the conversion
            Vector2 textScale = uMaterial.GetTextureScale("_BaseMap");
            Vector2 textTrans = uMaterial.GetTextureOffset("_BaseMap");
            UnityEngine.Color baseColor = uMaterial.GetColor("_BaseColor").linear;
            DstEntityManager.AddComponentData<SimpleMaterial>(entity, new SimpleMaterial()
            {
                texAlbedoOpacity = GetTextureEntity(uMaterial, "_MainTex"),
                constAlbedo = new float3(baseColor.r, baseColor.g, baseColor.b),
                constOpacity = baseColor.a,
                blend = GetBlending(uMaterial.GetFloat("_Blend")),
                twoSided = IsTwoSided(uMaterial),
                transparent = uMaterial.GetInt("_Surface") == 1,
                scale = new float2(textScale[0], textScale[1]),
                offset = new float2(textTrans[0], 1 - textTrans[1]) // Invert the offset as well
            });
        }

        private void ConvertLitMaterial(Entity entity, Material uMaterial)
        {
            //Check any unsupported properties
            if (uMaterial.GetInt("_WorkflowMode") == 0)
                UnityEngine.Debug.LogWarning("Specular workflow mode is not supported on material: " + uMaterial.name);
            Texture t = uMaterial.GetTexture("_OcclusionMap");
            if (!((uMaterial.GetTexture("_OcclusionMap") as Texture2D) is null))
                UnityEngine.Debug.LogWarning("_OcclusionMap is not supported on material: " + uMaterial.name);
            if (!((uMaterial.GetTexture("_EmissionMap") as Texture2D) is null))
                UnityEngine.Debug.LogWarning("_EmissionMap is not supported on material: " + uMaterial.name);

            //Do the conversion
            var texAlbedo = GetTextureEntity(uMaterial, "_BaseMap");
            var texMetal = GetTextureEntity(uMaterial, "_MetallicGlossMap");
            Vector2 textScale = uMaterial.GetTextureScale("_BaseMap");
            Vector2 textTrans = uMaterial.GetTextureOffset("_BaseMap");

            //Check if _Emission shader keyword has been enabled for that material
            float3 emissionColor = new float3(0.0f);
            if (uMaterial.IsKeywordEnabled("_EMISSION"))
            {
                UnityEngine.Color uEmissionColor = uMaterial.GetColor("_EmissionColor").linear;
                emissionColor = new float3(uEmissionColor.r, uEmissionColor.g, uEmissionColor.b);
            }

            UnityEngine.Color baseColor = uMaterial.GetColor("_BaseColor").linear;
            DstEntityManager.AddComponentData<LitMaterial>(entity, new LitMaterial()
            {
                texAlbedoOpacity = texAlbedo,
                constAlbedo = new float3(baseColor.r, baseColor.g, baseColor.b),
                constOpacity = baseColor.a,
                constEmissive = emissionColor,
                texMetal = texMetal,
                texSmoothness = uMaterial.GetFloat("_Smoothness") > 0.0f ? texAlbedo : texMetal,
                texNormal = GetTextureEntity(uMaterial, "_BumpMap"),
                texEmissive = GetTextureEntity(uMaterial, "_EmissionMap"),
                constSmoothness = uMaterial.GetFloat("_Smoothness"),
                normalMapZScale = uMaterial.GetFloat("_BumpScale"),
                twoSided = IsTwoSided(uMaterial),
                transparent = uMaterial.GetInt("_Surface") == 1,
                scale = new float2(textScale[0], textScale[1]),
                offset = new float2( textTrans[0], 1 - textTrans[1]) // Invert the offset as well
            });
        }

        public override bool ShouldRunConversionSystem()
        {
            //Workaround for running the tiny conversion systems only if the BuildSettings have the DotsRuntimeBuildProfile component, so these systems won't run in play mode
            if (GetBuildSettingsComponent<DotsRuntimeBuildProfile>() == null)
                return false;
            return base.ShouldRunConversionSystem();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((UnityEngine.Material uMaterial) =>
            {
                var entity = GetPrimaryEntity(uMaterial);
                switch (uMaterial.shader.name)
                {
                    case "Universal Render Pipeline/Unlit":
                        ConvertUnlitMaterial(entity, uMaterial);
                        break;
                    case "Universal Render Pipeline/Lit":
                        ConvertLitMaterial(entity, uMaterial);
                        break;
                    case "Sprites/Default":    // Sprite material conversion is handled by Unity.U2D.Entities.MaterialProxyConversion
                    case "Universal Render Pipeline/2D/Sprite-Lit-Default":
                        break;
                    default:
                        UnityEngine.Debug.LogWarning("No material conversion yet for shader " + uMaterial.shader.name);
                        break;
                }
            });
        }
    }
}

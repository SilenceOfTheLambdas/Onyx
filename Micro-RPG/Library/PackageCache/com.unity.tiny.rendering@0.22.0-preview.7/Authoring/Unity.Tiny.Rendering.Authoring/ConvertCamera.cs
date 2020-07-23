using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Rendering;
using Unity.Transforms;
using Unity.Entities.Runtime.Build;

namespace Unity.TinyConversion
{
    internal static partial class ConversionUtils
    {
        public static CameraClearFlags ToTiny(this UnityEngine.CameraClearFlags flags)
        {
            switch (flags)
            {
                case UnityEngine.CameraClearFlags.Skybox:
                case UnityEngine.CameraClearFlags.Color:
                    return CameraClearFlags.SolidColor;
                case UnityEngine.CameraClearFlags.Depth:
                case UnityEngine.CameraClearFlags.Nothing:
                    return CameraClearFlags.Nothing;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flags), flags, null);
            }
        }
    }

    [UpdateInGroup(typeof(GameObjectBeforeConversionGroup))]
    public class CameraConversion : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((UnityEngine.Camera uCamera) =>
            {
                var entity = GetPrimaryEntity(uCamera);

                var camera = new Unity.Tiny.Rendering.Camera();
                camera.clearFlags = uCamera.clearFlags.ToTiny();
                camera.backgroundColor = uCamera.backgroundColor.ToTiny();
                camera.viewportRect = uCamera.rect.ToTiny();
                camera.fov =  uCamera.orthographic ? uCamera.orthographicSize : uCamera.fieldOfView;
                camera.mode = uCamera.orthographic ? ProjectionMode.Orthographic : ProjectionMode.Perspective;
                camera.clipZNear = uCamera.nearClipPlane;
                camera.clipZFar = uCamera.farClipPlane;
                camera.aspect = (float)1920 / (float)1080; //fixed for now
                DstEntityManager.AddComponentData(entity, camera);

                // For CameraSettings2D
                float3 customSortAxisSetting = new float3(0,0,1.0f);
                if (UnityEngine.Rendering.GraphicsSettings.transparencySortMode == UnityEngine.TransparencySortMode.CustomAxis)
                    customSortAxisSetting = UnityEngine.Rendering.GraphicsSettings.transparencySortAxis;
                DstEntityManager.AddComponentData(entity, new Unity.Tiny.Rendering.CameraSettings2D
                    { customSortAxis = customSortAxisSetting });
                
            });
        }
    }
}

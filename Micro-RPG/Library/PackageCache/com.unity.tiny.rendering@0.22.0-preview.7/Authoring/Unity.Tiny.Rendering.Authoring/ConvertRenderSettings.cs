using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;
using Unity.Entities.Runtime.Build;

namespace Unity.TinyConversion
{
    public class RenderSettingsConversion : GameObjectConversionSystem
    {
        public override bool ShouldRunConversionSystem()
        {
            //Workaround for running the tiny conversion systems only if the BuildSettings have the DotsRuntimeBuildProfile component, so these systems won't run in play mode
            if (GetBuildSettingsComponent<DotsRuntimeBuildProfile>() == null)
                return false;
            return base.ShouldRunConversionSystem();
        }

        protected override void OnUpdate()
        {
            //Get the ambient light color from the current active scene
            Entity e = DstEntityManager.CreateEntity();
            DstEntityManager.AddComponentData<Unity.Tiny.Rendering.Light>(e, new Unity.Tiny.Rendering.Light()
            {
                color = new float3(RenderSettings.ambientLight.r, RenderSettings.ambientLight.g, RenderSettings.ambientLight.b),
                intensity = 1.0f
            });
            DstEntityManager.AddComponent<Unity.Tiny.Rendering.AmbientLight>(e);
        }
    }
}

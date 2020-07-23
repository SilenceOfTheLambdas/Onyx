using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Rendering;
using Unity.Entities.Runtime.Build;

namespace Unity.Tiny.Authoring
{
    [AddComponentMenu("Tiny/CascadedShadowMappedLight")]
    public class CascadedShadowMappedLight : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float3 cascadeScale = new float3(.5f, .15f, .020f);
        public GameObject mainCamera;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!conversionSystem.TryGetBuildSettingsComponent<DotsRuntimeBuildProfile>(out _))
                return;

            if (mainCamera == null)
                throw new ArgumentException($"No camera found in the CascadedShadowMappedLight authoring component of the gameobject: {name}. Please assign one for cascade shadow mapping.");

            bool scale = 0.0f < cascadeScale.z && cascadeScale.z < cascadeScale.y && cascadeScale.y < cascadeScale.x && cascadeScale.x < 1.0f;
            if (!scale)
                throw new ArgumentException($"Cascade scale values on the game object: {name} should be clamped between 0 and 1 with cascadeScale.z < cascadeScale.y <cascadeScale.x"); 

            var entityCamera = conversionSystem.GetPrimaryEntity(mainCamera);
            var comp = new CascadeShadowmappedLight();
            comp.cascadeScale = cascadeScale;
            comp.cascadeBlendWidth = 0.0f;
            comp.camera = entityCamera;
            dstManager.AddComponentData(entity, comp);
        }
    }
}

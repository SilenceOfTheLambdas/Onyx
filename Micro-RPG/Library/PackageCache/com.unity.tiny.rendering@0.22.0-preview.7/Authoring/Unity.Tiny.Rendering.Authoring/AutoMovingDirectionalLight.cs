using System;
using UnityEngine;
using Unity.Entities;
using Unity.Entities.Runtime.Build;

namespace Unity.Tiny.Authoring
{
    [AddComponentMenu("Tiny/AutoMovingDirectionalLight")]
    public class AutoMovingDirectionalLight : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool autoBounds = true;
        public GameObject mainCamera;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!conversionSystem.TryGetBuildSettingsComponent<DotsRuntimeBuildProfile>(out _))
                return;

            if (mainCamera == null)
                throw new ArgumentException($"No camera found in the AutoMovingDirectionalLight authoring component of the gameobject: {name}. Please assign one");

            var entityCamera = conversionSystem.GetPrimaryEntity(mainCamera);
            dstManager.AddComponentData(entity, new Tiny.Rendering.AutoMovingDirectionalLight()
            {
                autoBounds = autoBounds,
                clipToCamera = entityCamera
            });
        }
    }
}

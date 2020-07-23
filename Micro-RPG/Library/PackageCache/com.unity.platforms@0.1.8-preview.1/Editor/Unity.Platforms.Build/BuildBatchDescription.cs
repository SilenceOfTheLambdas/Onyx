using System;
using UnityEngine.Events;
using Unity.Build;

namespace Unity.Platforms.Build
{
    public struct BuildBatchItem
    {
        public BuildSettings BuildSettings;
        public Action<BuildContext> Mutator;
    }

    public struct BuildBatchDescription
    {
        public BuildBatchItem[] BuildItems;
        public UnityAction<BuildPipelineResult[]> OnBuildCompleted;
    }
}

using UnityEngine;
using Unity.Build;

namespace Unity.Platforms.Build
{
    public sealed class GraphicsSettings : IBuildSettingsComponent
    {
        public ColorSpace ColorSpace = ColorSpace.Uninitialized;
    }
}

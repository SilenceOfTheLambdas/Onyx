using Unity.Build;
using UnityEngine;
using PropertyAttribute = Unity.Properties.PropertyAttribute;

namespace Unity.Tiny.Rendering.Settings
{
    //TODO Need to find a way to retrieve project settings from runtime component without bringing a dependency to runtime packages
    public class TinyRenderingSettings : IBuildSettingsComponent
    {
        [Property]
        public int ResolutionX = 1280; //TODO: switch to VectorInt when there will be a Vector2Int Built-in inspector

        [Property]
        public int ResolutionY = 720;

        [Property]
        public bool AutoResizeFrame = true;

        [Property]
        public bool DisableVsync = false;

        [Property]
        [HideInInspector]
        public bool DisableSRGB = false;
    }
}

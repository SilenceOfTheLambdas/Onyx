using Unity.Properties;
using Unity.Build;

namespace Unity.Platforms.Build
{
    /// <summary>
    /// Used for generating debugging environment for players, only usable if Unity source is available.
    /// </summary>
    public sealed class SourceBuildConfiguration : IBuildSettingsComponent
    {
        [Property] public bool Enabled { get; set; }
    }
}

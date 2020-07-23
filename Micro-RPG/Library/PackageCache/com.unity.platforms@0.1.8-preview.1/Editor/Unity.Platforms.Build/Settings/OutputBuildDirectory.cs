using System;
using Unity.Properties;
using Unity.Build;
using System.Linq;

namespace Unity.Platforms.Build
{
    /// <summary>
    /// Overrides the default output directory of Builds/NameOfBuildSettingsAsset to an arbitrary location. 
    /// </summary>
    public class OutputBuildDirectory : IBuildSettingsComponent
    {
        [Property] public string OutputDirectory { get; set; }
    }

    public static class BuildSettingsExtensions
    {
        /// <summary>
        /// Get the output build directory for this <see cref="BuildSettings"/>.
        /// The output build directory can be overridden using a <see cref="OutputBuildDirectory"/> component.
        /// </summary>
        /// <param name="settings">This build settings.</param>
        /// <returns>The output build directory.</returns>
        public static string GetOutputBuildDirectory(this BuildSettings settings)
        {
            if (settings.TryGetComponent<OutputBuildDirectory>(out var outBuildDir))
            {
                return outBuildDir.OutputDirectory;
            }
            return $"Builds/{settings.name}";
        }
    }

    public static class BuildStepExtensions
    {
        /// <summary>
        /// Get the output build directory for this <see cref="BuildStep"/>.
        /// The output build directory can be overridden using a <see cref="OutputBuildDirectory"/> component.
        /// </summary>
        /// <param name="step">This build step.</param>
        /// <param name="context">The build context used throughout this build.</param>
        /// <returns>The output build directory.</returns>
        public static string GetOutputBuildDirectory(this BuildStep step, BuildContext context)
        {
            if (step.HasOptionalComponent<OutputBuildDirectory>(context))
            {
                return step.GetOptionalComponent<OutputBuildDirectory>(context).OutputDirectory;
            }
            // TODO: Robert, how we should approach this
            var buildInternals = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Contains("Unity.Build.Internals"));
            var buildContext = buildInternals.GetType("Unity.Build.Internals.BuildContextInternals");
            var method = buildContext.GetMethod("GetBuildSettings", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            //var settings = BuildContextInternals.GetBuildSettings(context);
            var settings = (BuildSettings) method.Invoke(null, new[] { context });
            return $"Builds/{settings.name}";
        }
    }
}

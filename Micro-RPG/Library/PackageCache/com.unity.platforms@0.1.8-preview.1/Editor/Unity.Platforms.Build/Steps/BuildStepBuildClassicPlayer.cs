using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Unity.Build;
using BuildStep = Unity.Build.BuildStep;

namespace Unity.Platforms.Build
{
    [BuildStep(description = k_Description, category = "Classic")]
    public sealed class BuildStepBuildClassicPlayer : BuildStep
    {
        const string k_Description = "Build Player";
        const string k_BootstrapFilePath = "Assets/StreamingAssets/livelink-bootstrap.txt";

        TemporaryFileTracker m_TemporaryFileTracker;

        public override string Description => k_Description;

        public override Type[] RequiredComponents => new[]
        {
            typeof(ClassicBuildProfile),
            typeof(SceneList),
            typeof(GeneralSettings)
        };

        public override Type[] OptionalComponents => new[]
        {
            typeof(OutputBuildDirectory),
            typeof(SourceBuildConfiguration)
        };

        public static bool Prepare(BuildContext context, BuildStep step, TemporaryFileTracker tracker, out BuildStepResult failure, out BuildPlayerOptions buildPlayerOptions)
        {
            buildPlayerOptions = default;
            var profile = step.GetRequiredComponent<ClassicBuildProfile>(context);
            if (profile.Target <= 0)
            {
                failure = BuildStepResult.Failure(step, $"Invalid build target '{profile.Target.ToString()}'.");
                return false;
            }

            if (profile.Target != EditorUserBuildSettings.activeBuildTarget)
            {
                failure = BuildStepResult.Failure(step, $"{nameof(EditorUserBuildSettings.activeBuildTarget)} must be switched before {nameof(BuildStepBuildClassicPlayer)} step.");
                return false;
            }

            var scenesList = step.GetRequiredComponent<SceneList>(context).GetScenePathsForBuild();
            if (scenesList.Length == 0)
            {
                failure = BuildStepResult.Failure(step, "There are no scenes to build.");
                return false;
            }

            var outputPath = step.GetOutputBuildDirectory(context);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var productName = step.GetRequiredComponent<GeneralSettings>(context).ProductName;
            var extension = profile.GetExecutableExtension();
            var locationPathName = Path.Combine(outputPath, productName + extension);

            buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = scenesList,
                target = profile.Target,
                locationPathName = locationPathName,
                targetGroup = UnityEditor.BuildPipeline.GetBuildTargetGroup(profile.Target),
            };

            buildPlayerOptions.options = BuildOptions.None;
            switch (profile.Configuration)
            {
                case BuildConfiguration.Debug:
                    buildPlayerOptions.options |= BuildOptions.AllowDebugging | BuildOptions.Development;
                    break;
                case BuildConfiguration.Develop:
                    buildPlayerOptions.options |= BuildOptions.Development;
                    break;
            }

            var sourceBuild = step.GetOptionalComponent<SourceBuildConfiguration>(context);
            if (sourceBuild.Enabled)
            {
                buildPlayerOptions.options |= BuildOptions.InstallInBuildFolder;
            }


            failure = default;
            return true;
        }

        public override BuildStepResult RunBuildStep(BuildContext context)
        {
            m_TemporaryFileTracker = new TemporaryFileTracker();
            if (!Prepare(context, this, m_TemporaryFileTracker, out var failure, out var options))
                return failure;
            else
                m_TemporaryFileTracker.EnsureFileDoesntExist(k_BootstrapFilePath);

            var report = UnityEditor.BuildPipeline.BuildPlayer(options);
            context.SetValue(report);
            return Success();
        }

        public override BuildStepResult CleanupBuildStep(BuildContext context)
        {
            m_TemporaryFileTracker.Dispose();
            return Success();
        }
    }
}

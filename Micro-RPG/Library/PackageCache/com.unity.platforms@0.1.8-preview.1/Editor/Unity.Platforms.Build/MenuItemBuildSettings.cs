using System.IO;
using UnityEditor;
using Unity.Build;
using BuildPipeline = Unity.Build.BuildPipeline;

namespace Unity.Platforms.Build
{
    public static class MenuItemBuildSettings
    {
        public static BuildSettings CreateNewBuildSettingsAsset(string prefix, params IBuildSettingsComponent[] components)
        {
            var dependency = Selection.activeObject as BuildSettings;
            var path = CreateAssetPathInActiveDirectory(prefix + $"BuildSettings{BuildSettings.AssetExtension}");
            return BuildSettings.CreateAsset(path, (bs) =>
            {
                if (dependency != null)
                {
                    bs.AddDependency(dependency);
                }
                bs.SetComponent(new GeneralSettings());
                bs.SetComponent(new SceneList());
                foreach (var component in components)
                {
                    bs.SetComponent(component.GetType(), component);
                }
            });
        }

        static string CreateAssetPathInActiveDirectory(string defaultFilename)
        {
            string path = null;
            if (Selection.activeObject != null)
            {
                var aoPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (!string.IsNullOrEmpty(aoPath))
                {
                    if (Directory.Exists(aoPath))
                        path = Path.Combine(aoPath, defaultFilename);
                    else
                        path = Path.Combine(Path.GetDirectoryName(aoPath), defaultFilename);
                }
            }
            return AssetDatabase.GenerateUniqueAssetPath(path);
        }
    }
}

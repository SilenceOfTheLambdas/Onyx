using Unity.Entities;
using Unity.Tiny.Rendering;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using System.IO;
using Unity.Build;
using System.Collections.Generic;
using Bgfx;
using Unity.Entities.Runtime.Build;

namespace Unity.TinyConversion
{
    public class ShaderExportSystem : ConfigurationSystemBase
    {
        static string kBinaryShaderFolderPath = "Packages/com.unity.tiny.rendering/Runtime/Unity.Tiny.Rendering.Native/shaderbin~/";

        string GetShaderFileName(string prefix, string type, string backend)
        {
            return prefix + "_" + type + "_" + backend + ".raw";
        }

        unsafe BlobAssetReference<PrecompiledShaderData> AddShaderData(EntityManager em, Entity e, ShaderType type, bgfx.RendererType[] types, string prefix)
        {
            using (var allocator = new BlobBuilder(Allocator.Temp))
            {
                ref var root = ref allocator.ConstructRoot<PrecompiledShaderData>();
                foreach (bgfx.RendererType sl in types)
                {
                    string path = Path.GetFullPath(kBinaryShaderFolderPath);
                    string fsFileName = GetShaderFileName(prefix, type.ToString(), sl.ToString()).ToLower();
                    using (var data = new NativeArray<byte>(File.ReadAllBytes(Path.Combine(path, fsFileName)), Allocator.Temp))
                    {
                        byte* dest = (byte*)(allocator.Allocate(ref root.DataForBackend(sl), data.Length).GetUnsafePtr());
                        UnsafeUtility.MemCpy(dest, (byte*)data.GetUnsafePtr(), data.Length);
                    }
                }
                return allocator.CreateBlobAssetReference<PrecompiledShaderData>(Allocator.Persistent);
            }
        }

        bgfx.RendererType[] GetShaderFormat(string targetName)
        {
            if (targetName == UnityEditor.BuildTarget.StandaloneWindows.ToString() ||
                targetName == UnityEditor.BuildTarget.StandaloneWindows64.ToString())
                return new bgfx.RendererType[] { bgfx.RendererType.OpenGL, bgfx.RendererType.Direct3D9, bgfx.RendererType.Direct3D11, bgfx.RendererType.Vulkan };
            else if (targetName == UnityEditor.BuildTarget.StandaloneLinux64.ToString())
                return new bgfx.RendererType[] { bgfx.RendererType.OpenGL, bgfx.RendererType.Vulkan };
            else if (targetName == UnityEditor.BuildTarget.StandaloneOSX.ToString())
                return new bgfx.RendererType[] { bgfx.RendererType.OpenGL, bgfx.RendererType.Metal, bgfx.RendererType.Vulkan };
            // TODO: get rid of OpenGLES for iOS when problem with Metal on A7/A8 based devices is fixed
            else if (targetName == UnityEditor.BuildTarget.iOS.ToString())
                return new bgfx.RendererType[] { bgfx.RendererType.OpenGL, bgfx.RendererType.OpenGLES, bgfx.RendererType.Metal };
            else if (targetName == UnityEditor.BuildTarget.Android.ToString())
                return new bgfx.RendererType[] { bgfx.RendererType.OpenGL, bgfx.RendererType.OpenGLES, bgfx.RendererType.Vulkan };
            else if (targetName == UnityEditor.BuildTarget.WebGL.ToString())
                return new bgfx.RendererType[] { bgfx.RendererType.OpenGLES };
            else
                //TODO: Should we default to a specific shader type?
                Debug.LogError($"Target: {targetName} is not supported. No shaders will be exported");
            return new bgfx.RendererType[] { };
        }

        Entity CreateShaderDataEntity(EntityManager em, ShaderType type, bgfx.RendererType[] backends)
        {
            var e = em.CreateEntity();
            var blob = AddShaderData(em, e, type, backends, "vs");
            em.AddComponentData(e, new VertexShaderBinData()
            {
                data = blob
            });

            blob = AddShaderData(em, e, type, backends, "fs");
            em.AddComponentData(e, new FragmentShaderBinData()
            {
                data = blob
            });
            return e;
        }

        protected override void OnUpdate()
        {
            if (buildSettings == null || !buildSettings.TryGetComponent<DotsRuntimeBuildProfile>(out var profile))
                return;

            //Export shaders per build target
            var targetName = profile.Target.UnityPlatformName;
            bgfx.RendererType[] types = GetShaderFormat(targetName);
            PrecompiledShaders data = new PrecompiledShaders();

            data.SimpleShader = CreateShaderDataEntity(EntityManager, ShaderType.simple, types);
            data.LitShader = CreateShaderDataEntity(EntityManager, ShaderType.simplelit, types);
            data.LineShader = CreateShaderDataEntity(EntityManager, ShaderType.line, types);
            data.ZOnlyShader = CreateShaderDataEntity(EntityManager, ShaderType.zOnly, types);
            data.BlitSRGBShader = CreateShaderDataEntity(EntityManager, ShaderType.blitsrgb, types);
            data.ShadowMapShader = CreateShaderDataEntity(EntityManager, ShaderType.shadowmap, types);
            data.SpriteShader = CreateShaderDataEntity(EntityManager, ShaderType.sprite, types);

            var singletonEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData<PrecompiledShaders>(singletonEntity, data);
        }
    }
}

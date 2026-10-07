// Source: Module M25 - Editor Tools

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Creates a URP pipeline asset (with a Universal Renderer) and makes it the project's render pipeline.
/// Without it the project renders with the Built-in pipeline: URP Volumes (vignette, color grading,
/// lens distortion...) do nothing and "Universal Render Pipeline/*" materials render magenta.
/// Menu: CursedPortal > Configure URP Render Pipeline. Batch mode: -executeMethod SetupURP.Run
/// </summary>
public static class SetupURP
{
    private const string SettingsFolder = "Assets/Settings";
    private const string PipelineAssetPath = SettingsFolder + "/CursedPortal_URP.asset";
    private const string RendererAssetPath = SettingsFolder + "/CursedPortal_URP_Renderer.asset";

    [MenuItem("CursedPortal/Configure URP Render Pipeline", false, 0)]
    public static void Run()
    {
        UniversalRenderPipelineAsset asset = EnsurePipelineAssigned();
        if (asset != null && !Application.isBatchMode)
        {
            EditorUtility.DisplayDialog("URP Setup",
                $"The project now renders with the Universal Render Pipeline:\n{AssetDatabase.GetAssetPath(asset)}", "OK");
        }
    }

    /// <summary>
    /// Ensures a URP asset is the default render pipeline, creating one if needed.
    /// </summary>
    public static UniversalRenderPipelineAsset EnsurePipelineAssigned()
    {
        if (GraphicsSettings.defaultRenderPipeline is UniversalRenderPipelineAsset current)
        {
            Debug.Log($"[SetupURP] URP already active: {AssetDatabase.GetAssetPath(current)}");
            return current;
        }

        UniversalRenderPipelineAsset asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelineAssetPath);
        if (asset == null)
        {
            CursedPortalEditorUtil.EnsureFolder(SettingsFolder);

            UniversalRendererData rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
            // Post-processing only renders when the renderer has its post-process shaders/data
            rendererData.postProcessData = AssetDatabase.LoadAssetAtPath<PostProcessData>(
                UniversalRenderPipelineAsset.packagePath + "/Runtime/Data/PostProcessData.asset");
            if (rendererData.postProcessData == null)
            {
                Debug.LogWarning("[SetupURP] URP PostProcessData asset not found; post-processing may not render.");
            }
            AssetDatabase.CreateAsset(rendererData, RendererAssetPath);
            ResourceReloader.TryReloadAllNullIn(rendererData, UniversalRenderPipelineAsset.packagePath);

            asset = UniversalRenderPipelineAsset.Create(rendererData);
            AssetDatabase.CreateAsset(asset, PipelineAssetPath);
            Debug.Log($"[SetupURP] Created {PipelineAssetPath}");
        }

        GraphicsSettings.defaultRenderPipeline = asset;
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();

        Debug.Log($"[SetupURP] Default render pipeline set to {PipelineAssetPath}");
        return asset;
    }
}
#endif

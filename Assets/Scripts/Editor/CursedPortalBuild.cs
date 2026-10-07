// Source: Module M25 - Build Automation

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Command-line entry points. Generates any missing scene, configures URP and builds the player.
/// Example:
///   "&lt;Unity Hub&gt;/Editor/2023.2.20f1/Editor/Unity" -batchmode -nographics -quit -logFile - \
///       -projectPath . -executeMethod CursedPortalBuild.BuildWindows
/// </summary>
public static class CursedPortalBuild
{
    [MenuItem("CursedPortal/Build/Windows (64-bit)", false, 200)]
    public static void BuildWindows()
    {
        Build(BuildTarget.StandaloneWindows64, "Builds/Windows/CursedPortal.exe");
    }

    [MenuItem("CursedPortal/Build/Linux (64-bit)", false, 201)]
    public static void BuildLinux()
    {
        Build(BuildTarget.StandaloneLinux64, "Builds/Linux/CursedPortal.x86_64");
    }

    /// <summary>
    /// Configures URP and generates whichever game scene doesn't exist yet. Returns false on failure.
    /// </summary>
    public static bool GenerateMissingScenes()
    {
        SetupURP.EnsurePipelineAssigned();

        if (!CursedPortalEditorUtil.EnsureTMPResources())
        {
            Debug.LogError("[CursedPortalBuild] TextMesh Pro Essential Resources are not imported yet. " +
                           "Open the project once in the editor (or re-run this command) and try again.");
            return false;
        }

        if (!File.Exists(CursedPortalEditorUtil.MainScenePath))
        {
            SceneSetup.SetupMainScene();
        }
        if (!File.Exists(CursedPortalEditorUtil.FinaleScenePath))
        {
            OtherDimensionSetup.CreateOtherDimensionScene();
        }

        CursedPortalEditorUtil.RegisterScenesInBuildSettings();
        return File.Exists(CursedPortalEditorUtil.MainScenePath) && File.Exists(CursedPortalEditorUtil.FinaleScenePath);
    }

    private static void Build(BuildTarget target, string locationPath)
    {
        int exitCode = 1;
        try
        {
            if (!GenerateMissingScenes())
            {
                Debug.LogError("[CursedPortalBuild] Scenes could not be generated; aborting build.");
                return;
            }

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { CursedPortalEditorUtil.MainScenePath, CursedPortalEditorUtil.FinaleScenePath },
                locationPathName = locationPath,
                target = target,
                options = BuildOptions.None,
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            Debug.Log($"[CursedPortalBuild] {target}: {report.summary.result} ({report.summary.totalErrors} errors) -> {locationPath}");
            exitCode = report.summary.result == BuildResult.Succeeded ? 0 : 1;
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(exitCode);
            }
        }
    }
}
#endif

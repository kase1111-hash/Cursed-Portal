// Source: Module M25 - Editor Tools

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Shared helpers for the CursedPortal scene and prefab generators.
/// </summary>
public static class CursedPortalEditorUtil
{
    public const string ScenesFolder = "Assets/Scenes";
    public const string MainScenePath = ScenesFolder + "/CursedPortal.unity";
    public const string FinaleScenePath = ScenesFolder + "/OtherDimension.unity";

    // ---------------------------------------------------------------- assets & folders

    /// <summary>
    /// Creates an asset folder (and its parents) if it doesn't exist.
    /// </summary>
    public static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        if (!AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }
        AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
    }

    // ---------------------------------------------------------------- serialized fields

    /// <summary>
    /// Assigns a private [SerializeField] object reference. Logs an error if the field doesn't exist,
    /// so a renamed field is caught instead of silently leaving the reference empty.
    /// </summary>
    public static void SetRef(Object target, string fieldName, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(fieldName);
        if (prop == null)
        {
            Debug.LogError($"[CursedPortal] {target.GetType().Name} has no serialized field '{fieldName}'");
            return;
        }
        prop.objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    /// <summary>
    /// Assigns a private [SerializeField] string.
    /// </summary>
    public static void SetString(Object target, string fieldName, string value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(fieldName);
        if (prop == null)
        {
            Debug.LogError($"[CursedPortal] {target.GetType().Name} has no serialized field '{fieldName}'");
            return;
        }
        prop.stringValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ---------------------------------------------------------------- scenes

    /// <summary>
    /// Asks to save the open scene(s) and to overwrite an existing generated scene, then opens a new empty scene.
    /// Returns false if the user cancelled.
    /// </summary>
    public static bool BeginNewScene(string scenePath)
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return false;
        }

        if (File.Exists(scenePath) && !Application.isBatchMode &&
            !EditorUtility.DisplayDialog("CursedPortal",
                $"{scenePath} already exists. Regenerate it? (The existing scene will be replaced.)",
                "Regenerate", "Cancel"))
        {
            return false;
        }

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        return true;
    }

    /// <summary>
    /// Saves the active scene to the given path and makes sure both game scenes are in Build Settings
    /// (main scene first, so it is the one a build starts with).
    /// </summary>
    public static void SaveSceneAndRegister(string scenePath)
    {
        EnsureFolder(Path.GetDirectoryName(scenePath).Replace('\\', '/'));
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath);
        RegisterScenesInBuildSettings();
        Debug.Log($"[CursedPortal] Saved {scenePath} and updated Build Settings");
    }

    /// <summary>
    /// Puts the main and finale scenes (whichever exist) at the top of the Build Settings scene list.
    /// </summary>
    public static void RegisterScenesInBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        foreach (string path in new[] { MainScenePath, FinaleScenePath })
        {
            if (File.Exists(path))
            {
                scenes.Add(new EditorBuildSettingsScene(path, true));
            }
        }

        foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
        {
            if (existing.path != MainScenePath && existing.path != FinaleScenePath)
            {
                scenes.Add(existing);
            }
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }

    // ---------------------------------------------------------------- TextMesh Pro

    /// <summary>
    /// Makes sure the TextMesh Pro essential resources (default font, settings) are in the project.
    /// Returns false when they had to be imported and the generator should be run again afterwards.
    /// </summary>
    public static bool EnsureTMPResources()
    {
        if (Resources.Load<TMP_Settings>("TMP Settings") != null) return true;

        TMP_PackageResourceImporter.ImportResources(true, false, false);
        AssetDatabase.Refresh();

        if (Resources.Load<TMP_Settings>("TMP Settings") != null) return true;

        string message = "TextMesh Pro Essential Resources are being imported.\n" +
                         "Run this menu item again once the import has finished.";
        Debug.LogWarning("[CursedPortal] " + message);
        if (!Application.isBatchMode)
        {
            EditorUtility.DisplayDialog("TextMesh Pro", message, "OK");
        }
        return false;
    }

    // ---------------------------------------------------------------- UI

    /// <summary>
    /// The sprites Unity's own GameObject > UI menu uses.
    /// </summary>
    public static DefaultControls.Resources UIResources()
    {
        return new DefaultControls.Resources
        {
            standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"),
            background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"),
            inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd"),
            knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"),
            checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd"),
            dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd"),
            mask = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd"),
        };
    }

    /// <summary>
    /// TMP's equivalent of UIResources for TMP_DefaultControls.
    /// </summary>
    public static TMP_DefaultControls.Resources TMPResources()
    {
        DefaultControls.Resources ui = UIResources();
        return new TMP_DefaultControls.Resources
        {
            standard = ui.standard,
            background = ui.background,
            inputField = ui.inputField,
            knob = ui.knob,
            checkmark = ui.checkmark,
            dropdown = ui.dropdown,
            mask = ui.mask,
        };
    }

    /// <summary>
    /// Creates a screen-space overlay canvas with a 1920x1080 reference scaler.
    /// </summary>
    public static Canvas CreateCanvas(string name, int sortingOrder, Transform parent = null)
    {
        GameObject canvasObj = new GameObject(name, typeof(RectTransform));
        if (parent != null) canvasObj.transform.SetParent(parent, false);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    /// <summary>
    /// Creates a UI child with a RectTransform anchored to the given normalized rectangle.
    /// </summary>
    public static RectTransform CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        RectTransform rt = (RectTransform)obj.transform;
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return rt;
    }

    /// <summary>
    /// Stretches a RectTransform over its parent with a uniform inset.
    /// </summary>
    public static void Stretch(RectTransform rt, float inset = 0f)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(inset, inset);
        rt.offsetMax = new Vector2(-inset, -inset);
    }

    /// <summary>
    /// Creates a full-rect Image.
    /// </summary>
    public static Image CreateImage(string name, Transform parent, Color color, bool raycastTarget = false)
    {
        RectTransform rt = CreateRect(name, parent, Vector2.zero, Vector2.one);
        Image image = rt.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = raycastTarget;
        return image;
    }

    /// <summary>
    /// Creates a TextMeshPro UI text anchored to the given normalized rectangle.
    /// </summary>
    public static TextMeshProUGUI CreateText(string name, Transform parent, string text, float fontSize,
        TextAlignmentOptions alignment, Color color, Vector2 anchorMin, Vector2 anchorMax)
    {
        RectTransform rt = CreateRect(name, parent, anchorMin, anchorMax);
        TextMeshProUGUI tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = color;
        tmp.raycastTarget = false;
        return tmp;
    }

    /// <summary>
    /// Adds an EventSystem with the legacy-input module (needed for the chat input field, buttons and sliders).
    /// </summary>
    public static void CreateEventSystem()
    {
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }
}
#endif

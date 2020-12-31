using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FontReplacer : EditorWindow
{
    private const string EditorPrefsKey = "Utilities.FontReplacer";
    private const string EditorPrefsKeyPro = "Utilities.FontReplacerPro";
    private const string MenuItemName = "Utilities/Replace Fonts...";

    private Font _src;
    private Font _dest;
    private TMP_FontAsset _srcPro;
    private TMP_FontAsset _destPro;
    private bool _includePrefabs;

    [MenuItem(MenuItemName)]
    public static void DisplayWindow()
    {
        var window = GetWindow<FontReplacer>(true, "Replace Fonts");
        var position = window.position;
        position.size = new Vector2(position.size.x, 250);
        position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
        window.position = position;
        window.Show();
    }

    public void OnEnable()
    {
        var path = EditorPrefs.GetString(EditorPrefsKey + ".src");
        if(path != string.Empty)
            _src = AssetDatabase.LoadAssetAtPath<Font>(path) ?? Resources.GetBuiltinResource<Font>(path);

        path = EditorPrefs.GetString(EditorPrefsKey + ".dest");
        if(path != string.Empty)
            _dest = AssetDatabase.LoadAssetAtPath<Font>(path) ?? Resources.GetBuiltinResource<Font>(path);

        //pro
        path = EditorPrefs.GetString(EditorPrefsKeyPro + "_pro.src");
        if(path != string.Empty)
            _srcPro = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path) ?? Resources.GetBuiltinResource<TMP_FontAsset>(path);

        path = EditorPrefs.GetString(EditorPrefsKeyPro + ".dest");
        if(path != string.Empty)
            _destPro = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path) ?? Resources.GetBuiltinResource<TMP_FontAsset>(path);

        _includePrefabs = EditorPrefs.GetBool(EditorPrefsKey + ".includePrefabs", false);
    }

    public void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PrefixLabel("Find:");
        _src = (Font)EditorGUILayout.ObjectField(_src, typeof(Font), false);
        _srcPro = (TMP_FontAsset)EditorGUILayout.ObjectField(_srcPro, typeof(TMP_FontAsset), false);

        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Replace with:");
        _dest = (Font)EditorGUILayout.ObjectField(_dest, typeof(Font), false);
        _destPro = (TMP_FontAsset)EditorGUILayout.ObjectField(_destPro, typeof(TMP_FontAsset), false);

        EditorGUILayout.Space();
        _includePrefabs = EditorGUILayout.ToggleLeft("Include Prefabs", _includePrefabs);
        if(EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(EditorPrefsKey + ".src", GetAssetPath(_src, "ttf"));
            EditorPrefs.SetString(EditorPrefsKey + ".dest", GetAssetPath(_dest, "ttf"));
            EditorPrefs.SetBool(EditorPrefsKey + ".includePrefabs", _includePrefabs);

            //pro
            EditorPrefs.SetString(EditorPrefsKeyPro + ".src", GetAssetPath(_srcPro, "ttf"));
            EditorPrefs.SetString(EditorPrefsKeyPro + ".dest", GetAssetPath(_destPro, "ttf"));
        }

        GUI.color = Color.green;
        if(GUILayout.Button("Replace All", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f)))
        {
            ReplaceFonts(_src, _dest, _srcPro, _destPro, _includePrefabs);
        }
        GUI.color = Color.white;
    }

    private static void ReplaceFonts(Font src, Font dest, TMP_FontAsset srcPro, TMP_FontAsset destPro, bool includePrefabs)
    {
        var sceneMatches = 0;
        for(var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var gos = new List<GameObject>(scene.GetRootGameObjects());
            foreach(var go in gos)
            {
                sceneMatches += ReplaceFonts(src, dest, go.GetComponentsInChildren<Text>(true));
                sceneMatches += ReplaceFonts(srcPro, destPro, go.GetComponentsInChildren<TextMeshProUGUI>(true));
            }
        }

        if(includePrefabs)
        {
            var prefabMatches = 0;
            var prefabs =
                AssetDatabase.FindAssets("t:Prefab")
                    .Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)));
            foreach(var prefab in prefabs)
            {
                prefabMatches += ReplaceFonts(src, dest, prefab.GetComponentsInChildren<Text>(true));
            }

            Debug.LogFormat("Replaced {0} font(s), {1} in scenes, {2} in prefabs", sceneMatches + prefabMatches, sceneMatches, prefabMatches);
        }
        else
        {
            Debug.LogFormat("Replaced {0} font(s) in scenes", sceneMatches);
        }
    }

    private static int ReplaceFonts(Font src, Font dest, IEnumerable<Text> texts)
    {
        if(dest == null)
            return 0;

        var matches = 0;
        var textsFiltered = src != null ? texts.Where(text => text.font == src) : texts;
        foreach(var text in textsFiltered)
        {
            text.font = dest;
            matches++;
        }
        return matches;
    }

    private static int ReplaceFonts(TMP_FontAsset src, TMP_FontAsset dest, IEnumerable<TextMeshProUGUI> textsPro)
    {
        if(dest == null)
            return 0;

        var matches = 0;
        var textsProFiltered = src != null ? textsPro.Where(text => text.font == src) : textsPro;
        foreach(var text in textsProFiltered)
        {
            text.font = dest;
            matches++;
        }
        return matches;
    }

    private static string GetAssetPath(Object assetObject, string defaultExtension)
    {
        var path = AssetDatabase.GetAssetPath(assetObject);
        if(path.StartsWith("Library/", System.StringComparison.InvariantCultureIgnoreCase))
            path = assetObject.name + "." + defaultExtension;
        return path;
    }
}
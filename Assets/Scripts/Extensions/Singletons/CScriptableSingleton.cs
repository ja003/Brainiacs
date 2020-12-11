/*
//=========================================//
AUTHOR:		Karel Brezina
DATE:		2018-08-21
FUNCTION:	
//=========================================//
*/

using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class CScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
{
	//=========================================//
	// VARIABLES
	//=========================================//

	private static T _instance;
	private static readonly string _assetExt = ".asset";

	//=========================================//
	// GET & SET
	//=========================================//

	private static string _filename => typeof(T).Name;
	private static string _filenameExt => _filename + _assetExt;
	private static string _assetDir => "Resources\\" + _filename;
	private static string _filePath => Path.Combine(_assetDir, _filenameExt);
	private static string _fullPath => Path.Combine("Assets", _filePath);
	public static T Instance
	{
		get
		{
			if(!IsInited)
			{
				CreateInstance();
			}
			return _instance;
		}
	}
	public static bool IsInited => _instance != null;

	//=========================================//
	// PRIVATE METHODS
	//=========================================//

	private static void CreateInstance()
	{
#if UNITY_EDITOR
		string[] guids = AssetDatabase.FindAssets(_filename + " t:ScriptableObject");
		int count = guids.Length;

		if(count == 0)
		{
			T asset = CreateInstance<T>();

			string folder = Path.Combine(Application.dataPath, _assetDir);
			if(!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}

			AssetDatabase.CreateAsset(asset, _fullPath);
			EditorUtility.FocusProjectWindow();
			EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<ScriptableObject>(_fullPath));
		}
		else if(count > 1)
		{
			Debug.LogWarning("Asset is already created.");
		}
		_instance = (T)AssetDatabase.LoadAssetAtPath<T>(_fullPath);
#else
			string path = Path.Combine(_filename, _filename);
			_instance = (T) Resources.Load(path, typeof(T));
			//CDebug.Log("Instance je: " + path + " " + _instance);
#endif
	}
}

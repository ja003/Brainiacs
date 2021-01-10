using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(CDebug))]
public class CDebugEditor : Editor
{
	public override void OnInspectorGUI()
	{
		if(GUILayout.Button("Reset"))
		{
			CDebug.Instance.Reset();
		}
		base.OnInspectorGUI();
	}
}
#endif
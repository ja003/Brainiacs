using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

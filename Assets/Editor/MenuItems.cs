using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class MenuItems
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    private static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs deleted");
    }
}
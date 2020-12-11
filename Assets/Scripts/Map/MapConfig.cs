using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Map", menuName = "ScriptableObjects/Map")]
public class MapConfig : ScriptableObject
{
	public EMap Id;

	public Map Prefab;
	public Sprite MapPreview;

	public Sprite Loading1;
	public Sprite Loading2;
	public Sprite Loading3;
	public Sprite Loading4;

}


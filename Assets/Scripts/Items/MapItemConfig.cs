using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class MapItemConfig : ScriptableObject
{
	public Sprite mapSprite;
	public Sprite statusSprite; //icon that will apear after item is picked up
	public string statusText;

	public abstract void OnEnterPlayer(Player pPlayer);
}

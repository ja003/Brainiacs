using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerItemConfig : ScriptableObject
{
	public bool CanDropOnMap = true;

	public Sprite mapSprite;
	
	public Sprite PlayerSpriteUp;
	public Sprite PlayerSpriteRight;
	public Sprite playerSpriteDown;
	public Sprite PlayerSpriteLeft;

	public abstract void OnEnterPlayer(Player pPlayer);
}

//public enum EItemId
//{
//	None,

//	TestGun,
//	TestGun2,
//	TestGun3,
//}

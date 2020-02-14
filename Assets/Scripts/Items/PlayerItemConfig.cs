using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerItemConfig : MapItemConfig
{
	public bool CanDropOnMap = true;
	
	public Sprite PlayerSpriteUp;
	public Sprite PlayerSpriteRight;
	public Sprite playerSpriteDown;
	public Sprite PlayerSpriteLeft;
}

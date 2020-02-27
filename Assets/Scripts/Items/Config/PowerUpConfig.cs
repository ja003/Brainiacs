using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PowerUp", menuName = "ScriptableObjects/PowerUpConfig")]
public class PowerUpConfig : ScriptableObject
{
	public EPowerUp Type;
	public MapItemInfo MapItemInfo;
}




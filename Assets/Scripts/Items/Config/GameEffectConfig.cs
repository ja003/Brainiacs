using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameEffect", menuName = "ScriptableObjects/GameEffectConfig")]
public class GameEffectConfig : ScriptableObject
{
	public EGameEffect Type;
	public MapItemInfo MapItemInfo;
}




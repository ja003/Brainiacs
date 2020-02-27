using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Item that can be held by player
/// </summary>
public abstract class PlayerItemConfig : MapItemConfig
{
	public bool CanDropOnMap = true;
	
	public Sprite PlayerSpriteUp;
	public Sprite PlayerSpriteRight;
	public Sprite playerSpriteDown;
	public Sprite PlayerSpriteLeft;

	//todo: make it work
	//[ConditionalField("CanDropOnMap")]
	//public MapItemConfig mapItem;
}

[AttributeUsage(AttributeTargets.Field)]
public class ConditionalFieldAttribute : PropertyAttribute
{
	public readonly string FieldToCheck;
	public readonly string[] CompareValues;
	public readonly bool Inverse;

	/// <param name="fieldToCheck">String name of field to check value</param>
	/// <param name="inverse">Inverse check result</param>
	/// <param name="compareValues">On which values field will be shown in inspector</param>
	public ConditionalFieldAttribute(string fieldToCheck, bool inverse = false, params object[] compareValues)
	{
		FieldToCheck = fieldToCheck;
		Inverse = inverse;
		CompareValues = compareValues.Select(c => c.ToString().ToUpper()).ToArray();
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages showing players status (pickup weapon, powerup, ...)
/// </summary>
public class UIPlayerStatusManager : MonoBehaviour
{
	[SerializeField] private UIPlayerStatus prefab = null;

	/// <summary>
	/// Insatntiates the status at given position and sets the info (sprite, text).
	/// For now only used for player. If needed alse, make this method public.
	/// </summary>
	public void ShowMapItem(Vector3 pWorldPosition, MapItemInfo pInfo)
	{
		ShowStatus(pWorldPosition, pInfo.StatusText, pInfo.MapSprite);
	}

	public void ShowHealth(Vector3 pWorldPosition, int pIncrement)
	{
		bool isAdd = pIncrement > 0;
		string prefix = isAdd ? "+" : "-";
		Color textColor = isAdd ? Color.green : Color.red;
		ShowStatus(pWorldPosition, prefix + pIncrement, null, textColor);
	}

	public void ShowStatus(Vector3 pWorldPosition, string pText, Sprite pSprite, Color? pTextColor = null)
	{
		//HACK: to hide adding debug weapons
		if(Time.time < 1)
			return;

		//UIPlayerStatus instance = Instantiate(prefab, transform);
		GameObject instance = InstanceFactory.Instantiate(prefab.gameObject, false);
		//instance.transform.parent = transform;

		instance.GetComponent<UIPlayerStatus>().SpawnAt(pWorldPosition, pSprite, pText, pTextColor);
	}

	/// <summary>
	/// DEBUG
	/// </summary>
	//private void Awake()
	//{
	//	UIPlayerStatus instance = Instantiate(prefab, transform);
	//	instance.debug_SpawnAt(Vector3.one);

	//	instance = Instantiate(prefab, transform);
	//	instance.debug_SpawnAt(Vector3.one * 2);
	//}

}


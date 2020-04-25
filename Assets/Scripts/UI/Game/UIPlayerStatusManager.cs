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
	/// Shows status at player status position
	/// </summary>
	public void Show(Player pPlayer, MapItemInfo pInfo)
	{
		//HACK: to hide adding debug weapons
		if(Time.time < 1)
			return;

		Show(pPlayer.Stats.StatusUiPosition.position, pInfo);
	}

	/// <summary>
	/// Insatntiates the status at given position and sets the info (sprite, text).
	/// For now only used for player. If needed alse, make this method public.
	/// </summary>
	private void Show(Vector3 pWorldPosition, MapItemInfo pInfo)
	{
		//Debug.Log("ShowStatus " + pWorldPosition);

		//TODO: pooling
		UIPlayerStatus instance = Instantiate(prefab, transform);

		instance.SpawnAt(pWorldPosition, pInfo.StatusSprite, pInfo.StatusText);
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


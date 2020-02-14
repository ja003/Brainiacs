using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerStatusManager : MonoBehaviour
{
	[SerializeField] private UIPlayerStatus prefab;

	public void SpawnAt(Vector3 pWorldPosition, Sprite pSprite, string pText)
	{
		UIPlayerStatus instance = Instantiate(prefab, transform);

		instance.SpawnAt(pWorldPosition, pSprite, pText);
	}

}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Indicates if player element is local or remote.
/// </summary>
public class UIPlayerTypeIndicator : MainMenuBehaviour
{
	[SerializeField] Image imgPlayerTypeBg = null;
	[SerializeField] Image imgPlayerType = null;
	[SerializeField] Sprite iconPlayerTypeAI = null;
	[SerializeField] Sprite iconPlayerTypeRemote = null;



	/// <summary>
	/// Note: in this context the PlayerType is not same as players config
	/// </summary>
	internal void SetType(EPlayerType pType)
	{
		gameObject.SetActive(pType != EPlayerType.LocalPlayer);
		switch(pType)
		{
			case EPlayerType.None:
				break;
			case EPlayerType.LocalPlayer:
				break;
			case EPlayerType.RemotePlayer:
				imgPlayerType.sprite = iconPlayerTypeRemote;
				break;
			case EPlayerType.AI:
				imgPlayerType.sprite = iconPlayerTypeAI;
				break;
		}
	}

	internal void SetColor(EPlayerColor pColor)
	{
		imgPlayerTypeBg.color = brainiacs.PlayerColorManager.GetColor(pColor, 0.2f);
	}
}

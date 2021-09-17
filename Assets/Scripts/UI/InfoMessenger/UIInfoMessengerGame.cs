using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles showing of info messages in mid-bottom of the screen.
/// Eg.: Game started, Player has left, Player eliminated, ...
/// </summary>
public class UIInfoMessengerGame : UIInfoMessenger
{
	protected override bool CanShow()
	{
		return !Game.Instance.GameEnd.GameEnded;
	}

	protected override UIInfoMessage InstantiateInfoMsg()
	{
		UIInfoMessage infoMessage = InstanceFactory.Instantiate(infoMessagePrefab.gameObject).GetComponent<UIInfoMessage>();
		infoMessage.OnInstantiate(transform);
		return infoMessage;
	}
}

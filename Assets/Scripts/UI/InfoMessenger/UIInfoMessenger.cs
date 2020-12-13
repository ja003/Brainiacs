using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles showing of info messages in mid-bottom of the screen.
/// Eg.: Game started, Player has left, Player eliminated, ...
/// </summary>
public class UIInfoMessenger : MonoBehaviour
{
	[SerializeField] protected UIInfoMessage infoMessagePrefab;

	//protected override void Awake()
	//{
	//    DoInTime(() => Show("TEST test"), 1);
	//    base.Awake();
	//}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.H))
		{
			Show("HHH " + Time.frameCount);
		}
	}

	public void Show(string pText, DebugInfoMsg debug = DebugInfoMsg.None, float pDuration = 3)
	{
		switch(debug)
		{
			case DebugInfoMsg.Normal:
				Debug.Log("ShowMsg:" + pText);
				break;
			case DebugInfoMsg.Error:
				Debug.LogError("ShowMsg:" + pText);
				break;
		}

		if(!CanShow())// game.GameEnd.GameEnded)
		{
			//Debug.Log("Dont show message after game ended");
			return;
		}

		Debug.Log("Show " + pText);
		UIInfoMessage msgInstance = InstantiateInfoMsg();
			
			
		msgInstance.Show(pText, pDuration);
	}

	protected virtual UIInfoMessage InstantiateInfoMsg()
	{
		UIInfoMessage uIInfoMessage = Instantiate(infoMessagePrefab.gameObject).GetComponent<UIInfoMessage>();
		uIInfoMessage.UsePooling = false;
		uIInfoMessage.transform.parent = transform;
		return uIInfoMessage;
	}

	protected virtual bool CanShow()
	{
		return true;
	}
}

public enum DebugInfoMsg
{
	None,
	Normal,
	Error
}

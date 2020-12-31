using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDebug : GameBehaviour
{
	[SerializeField] public Transform AiMoveTarget;

	[SerializeField] Button btnDisconnect;
	[SerializeField] Button btnReconnect;


	public float VolumeReduce = 10;
	private Room leftRoom;

	protected override void Awake()
	{
		base.Awake();
		btnDisconnect.onClick.AddListener(debug_Disconnect);
		btnReconnect.onClick.AddListener(debug_Reconnect);
	}


	private void debug_Disconnect()
	{
		Debug.Log("Disconnect");

		leftRoom = PhotonNetwork.CurrentRoom;
		PhotonNetwork.Disconnect();
	}


	private void debug_Reconnect()
	{
		Debug.Log("Reconnect");

		PhotonNetwork.Reconnect();
	}
}

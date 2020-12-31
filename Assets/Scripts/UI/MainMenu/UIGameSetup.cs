﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameSetup : MainMenuBehaviour
{
	[SerializeField] private Button btnJoin = null;
	[SerializeField] private Button btnHost = null;
	[SerializeField] private Button btnBack = null;
	[SerializeField] private Button btnJoinSearchBack = null;

	[SerializeField] private GameObject setupInit = null;
	[SerializeField] private UIGameSetupSearch setupSearch = null;
	[SerializeField] public UIGameSetupMain SetupMain = null;

	[SerializeField] public UIInfoMessenger InfoMessenger;

	protected override void Awake()
	{
		setupInit.SetActive(true);
		setupSearch.SetActive(false);
		SetupMain.SetActive(false);

		btnJoin.onClick.AddListener(OnBtnJoin);
		btnHost.onClick.AddListener(OnBtnHost);
		btnJoinSearchBack.onClick.AddListener(OnBtnJoinSearchBack);
		btnBack.onClick.AddListener(mainMenu.OnBtnBack);


		base.Awake();
	}

	private void Start()
	{
		brainiacs.AudioManager.PlayMusic(ESound.Music_Menu);
	}

	private void OnBtnJoinSearchBack()
	{
		setupInit.SetActive(true);
		setupSearch.SetActive(false);
	}

	private void OnBtnJoin()
	{
		setupInit.SetActive(false);
		setupSearch.SetActive(true);
	}

	private void OnBtnHost()
	{
		brainiacs.GameInitInfo = new GameInitInfo();
		OpenMain(true);
	}

	//todo: is pIsMaster neccessary? maybe PhotonNetwork.isMaster is enough
	public void OpenMain(bool pIsMaster)
	{
		setupInit.SetActive(false);
		setupSearch.SetActive(false);
		SetupMain.SetActive(true, pIsMaster);
	}

	internal void OpenMain(GameInitInfo pGameInfo)
	{
		OpenMain(false);
		SetupMain.SetGameInfo(pGameInfo);
	}

	public void OnSubMenuBtnBack()
	{
		SetupMain.KickOtherPlayers();
		brainiacs.PhotonManager.LeaveRoom();
		brainiacs.GameInitInfo = null;

		setupInit.SetActive(true);
		setupSearch.SetActive(false);
		SetupMain.SetActive(false);
	}
}

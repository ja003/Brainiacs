using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameSetup : MainMenuBehaviour
{
	[SerializeField] private Button btnJoin;
	[SerializeField] private Button btnHost;
	[SerializeField] private Button btnBack;
	[SerializeField] private Button btnJoinSearchBack;

	[SerializeField] private GameObject setupInit;
	[SerializeField] private UIGameSetupSearch setupSearch;
	[SerializeField] public UIGameSetupMain SetupMain;


	protected override void Awake()
	{
		setupInit.SetActive(true);
		setupSearch.SetActive(false);
		SetupMain.SetActive(false);

		//DEBUG
		//Debug.LogError("DEBUG - remove before build");
		//setupInit.SetActive(false);
		//setupSearch.SetActive(true);


		btnJoin.onClick.AddListener(OnBtnJoin);
		btnHost.onClick.AddListener(OnBtnHost);
		btnJoinSearchBack.onClick.AddListener(OnBtnJoinSearchBack);
		btnBack.onClick.AddListener(mainMenu.OnBtnBack);
		base.Awake();
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
		brainiacs.PhotonManager.LeaveRoom();
		Debug.LogError("todo: reset game info more generally");
		brainiacs.GameInitInfo = null;

		setupInit.SetActive(true);
		setupSearch.SetActive(false);
		SetupMain.SetActive(false);
	}
 }

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
	[SerializeField] private GameObject joinGameSearch;
	[SerializeField] public UIGameSetupMain SetupMain;


	protected override void Awake()
	{
		setupInit.SetActive(true);
		joinGameSearch.SetActive(false);
		SetupMain.SetActive(false);

		//DEBUG
		setupInit.SetActive(false);
		SetupMain.SetActive(true);


		btnJoin.onClick.AddListener(OnBtnJoin);
		btnHost.onClick.AddListener(OnBtnHost);
		btnJoinSearchBack.onClick.AddListener(OnBtnJoinSearchBack);
		btnBack.onClick.AddListener(mainMenu.OnBtnBack);
		base.Awake();
	}

	private void OnBtnJoinSearchBack()
	{
		setupInit.SetActive(true);
		joinGameSearch.SetActive(false);
	}

	private void OnBtnJoin()
	{
		setupInit.SetActive(false);
		joinGameSearch.SetActive(true);
	}

	private void OnBtnHost()
	{
		setupInit.SetActive(false);
		SetupMain.SetActive(true);
	}

	public void OnSubMenuBtnBack()
	{
		setupInit.SetActive(true);
		joinGameSearch.SetActive(false);
		SetupMain.SetActive(false);
	}
 }

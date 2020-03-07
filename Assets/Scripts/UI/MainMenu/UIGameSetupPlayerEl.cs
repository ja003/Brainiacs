using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIGameSetupPlayerEl : MainMenuBehaviour
{
	[SerializeField] Image colorBg;
	[SerializeField] Animator portraitAnimator;
	[SerializeField] Image portrait;
	[SerializeField] Text playerNameText;
	//[SerializeField] UiTextSwapper playerTypeSwapper;
	[SerializeField] UiTextSwapper heroSwapper;
	[SerializeField] Text playerTypeText;
	[SerializeField] Button btnRemove;


	EPlayerColor assignedColor;
	public EPlayerType PlayerType { get; private set; }
	EHero hero;
	int number;

	protected override void Awake()
	{
		btnRemove.onClick.AddListener(OnBtnRemove);
		base.Awake();
	}

	private void OnBtnRemove()
	{
		if(PlayerType == EPlayerType.RemotePlayer)
		{
			Debug.LogError("TODO: are you sure dialog");
		}
		gameObject.SetActive(false);
		mainMenu.GameSetup.SetupMain.OnPlayersChanged();
	}

	/// <summary>
	/// Each player has number as ID <1,4>
	/// </summary>
	public void Init(int pPlayerNumber, EPlayerType pPlayerType)
	{
		number = pPlayerNumber;
		List<string> playerTypes = Utils.GetStrings(typeof(EPlayerType));
		//playerTypeSwapper.Init(playerTypes, OnPlayerTypeChanged, 0);

		List<string> heroes = Utils.GetStrings(typeof(EHero));
		heroSwapper.Init(heroes, OnHeroChanged, 0);

		assignedColor = (EPlayerColor)pPlayerNumber;
		colorBg.color = UIColorDB.GetColor(assignedColor);

		playerNameText.text = "player " + pPlayerNumber;

		SetType(pPlayerType);

		gameObject.SetActive(true);
	}

	/// <summary>
	/// Remote player has to be connected and ready.
	/// Disabled element is not valid.
	/// </summary>
	public bool IsValid()
	{
		bool remoteOK = remoteConnected && remoteReady;
		if(PlayerType == EPlayerType.RemotePlayer && !remoteOK)
			return false;

		return gameObject.activeSelf;
	}

	private void SetType(EPlayerType pPlayerType)
	{
		remoteConnected = false;
		remoteReady = false;
		PlayerType = pPlayerType;
		playerTypeText.text = pPlayerType.ToString();
		//todo: handle remote
		bool isRemote = pPlayerType == EPlayerType.RemotePlayer;
		portraitAnimator.enabled = isRemote;
		heroSwapper.SetEnabled(!isRemote);
		if(isRemote)
		{
			Debug.LogError($"TODO: wait for remote player");
			playerNameText.text = "WAITING...";
			DoInTime(() => OnRemoteConnected("AdamRemote"), 2);
		}

	}

	bool remoteConnected;
	bool remoteReady;
	private void OnRemoteConnected(string pName)
	{
		remoteConnected = true;
		remoteReady = true;
		Debug.Log($"Remote player {pName} connected");
		playerNameText.text = pName;
		portraitAnimator.enabled = false;
	}

	private void OnHeroChanged()
	{
		hero = (EHero)heroSwapper.CurrentIndex;
		portrait.sprite = brainiacs.HeroManager.GetHeroConfig(hero).Portrait;
	}

	//private void OnPlayerTypeChanged()
	//{
	//	playerType = (EPlayerType)playerTypeSwapper.CurrentIndex;
	//	Debug.Log("Set to type: " + playerType);
	//}

	internal PlayerInitInfo GetInitInfo()
	{
		PlayerInitInfo info = new PlayerInitInfo(number, hero, playerNameText.text, assignedColor, PlayerType);
		return info;
	}
}


public enum EPlayerType
{
	None = 0,
	LocalPlayer = 1,
	RemotePlayer = 2,
	AI = 3,
}
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using PhotonPlayer = Photon.Realtime.Player;

public class UIGameSetupPlayerEl : MainMenuBehaviour
{
	[SerializeField] Image color = null;
	[SerializeField] Image state = null; //ready = green, waiting = red
	[SerializeField] Animator portraitAnimator = null;
	[SerializeField] Image portrait = null;
	[SerializeField] Text playerNameText = null;
	//[SerializeField] UiTextSwapper playerTypeSwapper;
	[SerializeField] UiTextSwapper heroSwapper = null;
	[SerializeField] Text playerTypeText = null;
	[SerializeField] Button btnRemove = null;

	public PlayerInitInfo Info;
	public bool IsItMe;

	bool preInited;

	/// <summary>
	/// Called just once
	/// </summary>
	private void PreInit()
	{
		if(preInited)
			return;

		preInited = true;
		btnRemove.onClick.AddListener(OnBtnRemove);

		Info = new PlayerInitInfo();

		//List<string> playerTypes = Utils.GetStrings(typeof(EPlayerType));
		//playerTypeSwapper.Init(playerTypes, OnPlayerTypeChanged, 0);
		List<string> heroes = Utils.GetStrings(typeof(EHero));
		heroSwapper.Init(heroes, OnHeroChanged, 0);
	}

	private void OnBtnRemove()
	{
		if(Info.PlayerType == EPlayerType.RemotePlayer)
		{
			Debug.LogError("TODO: are you sure dialog");
		}
		gameObject.SetActive(false);
		brainiacs.GameInitInfo.Players.Remove(Info);
		mainMenu.GameSetup.SetupMain.OnPlayersChanged();
		//OnElementChanged(true);
	}

	/// <summary>
	/// Each player has number as ID <1,4>
	/// if pPhotonPlayer is null it will be determined in SetType
	/// </summary>
	public void Init(int pPlayerNumber, EPlayerType pPlayerType, PhotonPlayer pPhotonPlayer)
	{
		PreInit();
		gameObject.SetActive(true);

		brainiacs.GameInitInfo.AddPlayer(Info);

		Info.PhotonPlayer = pPhotonPlayer;

		Info.Number = pPlayerNumber;

		AssignColor((EPlayerColor)pPlayerNumber);

		SetName("player " + pPlayerNumber);

		SetType(pPlayerType);

		//if photon player is set then element is being
		//initialized at client
		bool elementCreated = pPhotonPlayer == null;
		//OnElementChanged(elementCreated);
	}

	bool isClientInitializing;

	/// <summary>
	/// Init only for client
	/// </summary>
	public void Init(PlayerInitInfo pPlayer)
	{
		isClientInitializing = true;
		//if(IsItMe)
		//{
		//	Debug.Log("This is me, no need to reinit");
		//	return;
		//}

		//this Init need to be called before any other changes
		Init(pPlayer.Number, pPlayer.PlayerType, pPlayer.PhotonPlayer);
		IsItMe = Info.PlayerType == EPlayerType.RemotePlayer &&
			pPlayer.PhotonPlayer == PhotonNetwork.LocalPlayer;
		SetReady(pPlayer.IsReady); //client has to confirm that he is ready

		if(Info.Color != pPlayer.Color)
		{
			Debug.LogError("Assigned color doesnt match");
		}
		AssignColor(pPlayer.Color);
		SetName(pPlayer.Name);
		heroSwapper.SetValue((int)pPlayer.Hero);

		if(IsItMe)
		{
			Debug.Log("this is me");
			OnRemoteConnected(Info.PhotonPlayer);
		}
		heroSwapper.SetInteractable(IsItMe);
		btnRemove.gameObject.SetActive(false);
		isClientInitializing = false;
	}


	public void UpdateInfo(PlayerInitInfo pPlayerInfo)
	{
		Info = pPlayerInfo;
		Init(pPlayerInfo);
	}

	private void SetName(string pName)
	{
		Info.Name = pName;
		playerNameText.text = pName;
	}

	private void SetType(EPlayerType pPlayerType)
	{
		btnRemove.gameObject.SetActive(true);
		Info.PlayerType = pPlayerType;
		bool isRemote = pPlayerType == EPlayerType.RemotePlayer;
		if(isRemote)
		{
			if(Info.PhotonPlayer == null)
				playerNameText.text = "WAITING...";
		}
		//todo: even AI needs to have assigned photon player?
		else if(pPlayerType == EPlayerType.LocalPlayer ||
			pPlayerType == EPlayerType.AI)
		{
			//local player = master
			if(PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
			{
				//client doesnt change local player
			}
			else
			{
				Info.PhotonPlayer = PhotonNetwork.LocalPlayer;
			}
		}

		playerTypeText.text = pPlayerType.ToString();

		//start animation if player is remote and not connected yet
		portraitAnimator.enabled = isRemote && Info.PhotonPlayer == null;
		heroSwapper.SetInteractable(!isRemote);

		//master is always considered ready
		//client ready state is set in Init
		if(!isRemote)
			SetReady(true); 
	}


	public void SetReady(bool pValue)
	{
		Info.IsReady = pValue;
		state.color = pValue ? Color.green : Color.red;

		bool interactable = !pValue;
		if(Info.PlayerType == EPlayerType.LocalPlayer ||
			Info.PlayerType == EPlayerType.AI)
		{
			//creating game
			if(!PhotonNetwork.InRoom)
			{
				interactable = true;
			}
		}
		else
		{
			interactable &= IsItMe;
		}

		heroSwapper.SetInteractable(interactable);
		SyncInfo();
	}

	private void AssignColor(EPlayerColor pColor)
	{
		Info.Color = pColor;
		color.color = UIColorDB.GetColor(Info.Color);
	}


	/// <summary>
	/// Remote player has to be connected and ready.
	/// Disabled element is not valid.
	/// </summary>
	//public bool IsValidAndReady()
	//{
	//	bool remoteOK = remoteConnected && remoteReady;
	//	if(PlayerType == EPlayerType.RemotePlayer && !remoteOK)
	//		return false;

	//	return gameObject.activeSelf;
	//}

	/// <summary>
	/// If this element is activated, set as a remote player
	/// and no photon player is connected to it
	/// </summary>
	public bool IsRemoteFree()
	{
		return gameObject.activeSelf &&
			Info.PlayerType == EPlayerType.RemotePlayer &&
			Info.PhotonPlayer == null;
	}


	//bool remoteConnected;
	//bool remoteReady;
	public void OnRemoteConnected(PhotonPlayer pPlayer)
	{
		Info.PhotonPlayer = pPlayer;
		//remoteConnected = true;
		//remoteReady = true;
		Debug.Log($"Remote player {pPlayer.UserId} connected");
		playerNameText.text = pPlayer.NickName;
		portraitAnimator.enabled = false;
		//OnElementChanged(true);
	}

	private void OnHeroChanged()
	{
		Info.Hero = (EHero)heroSwapper.CurrentIndex;
		portrait.sprite = brainiacs.HeroManager.GetHeroConfig(Info.Hero).Portrait;
		SyncInfo();
	}

	private void SyncInfo()
	{
		if(isClientInitializing)
			return;

		//no reason to sync
		//should happen only on master
		if(!PhotonNetwork.InRoom)
			return;

		byte[] infoS = Info.Serizalize();
		mainMenu.Photon.Send(EPhotonMsg.MainMenu_SyncPlayerInfo, infoS);
	}

}


public enum EPlayerType
{
	None = 0,
	LocalPlayer = 1,
	RemotePlayer = 2,
	AI = 3,
}
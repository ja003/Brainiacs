using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PhotonPlayer = Photon.Realtime.Player;

public class UIGameSetupPlayerEl : MainMenuBehaviour
{
	[SerializeField] public Image color = null;
	[SerializeField] Image state = null; //ready = green, waiting = red
	[SerializeField] Animator portraitAnimator = null;
	[SerializeField] Image portrait = null;
	//[SerializeField] Text playerNameText = null;
	[SerializeField] InputField playerNameInput = null;
	//[SerializeField] UiTextSwapper playerTypeSwapper;
	[SerializeField] public UiTextSwapper heroSwapper = null;
	[SerializeField] UiTextSwapper keySetSwapper = null;
	[SerializeField] Text playerTypeText = null;
	//[SerializeField] Button btnRemove = null; //replaced by click-hold on 
	//[SerializeField] ButtonObject elementButton = null; //clickable area of this element
	[SerializeField] ButtonObject btnRemove = null;

	public PlayerInitInfo Info;
	public bool IsItMe =>
		(Info.PlayerType == EPlayerType.RemotePlayer ||
		Info.PlayerType == EPlayerType.LocalPlayer)
		&&
		Info.PhotonPlayer == PhotonNetwork.LocalPlayer;

	bool preInited;

	/// <summary>
	/// Called just once
	/// </summary>
	private void PreInit()
	{
		if(preInited)
			return;

		preInited = true;
		//btnRemove.onClick.AddListener(OnBtnRemove);

		//remove player (only master)
		if(IsMaster())
		{
			btnRemove.OnPointerUpAction += OnElementButtonPointerUp;
			btnRemove.OnPointerHoldUpdate += OnElementButtonHoldUpdate;
		}

		Info = new PlayerInitInfo();

		//List<string> playerTypes = Utils.GetStrings(typeof(EPlayerType));
		//playerTypeSwapper.Init(playerTypes, OnPlayerTypeChanged, 0);
		List<string> heroes = Utils.GetStrings(typeof(EHero));
		heroSwapper.Init(heroes, OnHeroChanged, 0);

		List<string> keySets = Utils.GetStrings(typeof(EKeyset));
		keySetSwapper.Init(keySets, OnKeySetChanged, 0);

		//playerNameInput.onValueChanged.AddListener(OnPlayerNameChanged);
		playerNameInput.onEndEdit.AddListener(OnPlayerNameChanged);
	}

	private void OnPlayerNameChanged(string pName)
	{
		Debug.Log($"OnPlayerNameChanged {pName}");
		SetName(pName);
		PlayerNameManager.SaveName(Info.Number, pName);
		SyncInfo();
	}

	/// <summary>
	/// Animate return element to original size - scale up (see OnElementButtonHoldUpdate)
	/// </summary>
	private void Update()
	{
		//gradually scale element up until the original scale (only if it is not being clicked)
		if(transform.localScale.x >= 1 || btnRemove.IsClicked())
			return;

		const float scale_up_speed = 1f;
		transform.localScale += (scale_up_speed * Time.deltaTime) * Vector3.one;
	}

	const float required_hold_time_to_remove = 0.5f;

	/// <summary>
	/// Animate remove player - scale element down
	/// </summary>
	private void OnElementButtonHoldUpdate(float pButtonHoldTime)
	{
		const float min_hold_time = 0.1f; //to start scale anim
		if(pButtonHoldTime < min_hold_time || transform.localScale.x <= 0)
			return;

		float progress = (pButtonHoldTime - min_hold_time) / required_hold_time_to_remove; //0-1
																						   //Debug.Log($"hold {progress}");
		transform.localScale = Vector3.one * (1 - progress);
	}

	/// <summary>
	/// Try remove player
	/// </summary>
	private void OnElementButtonPointerUp(float pButtonHoldTime)
	{
		if(!IsMaster())
		{
			Debug.Log("Client cant remove players");
			return;
		}
		//Debug.Log(pButtonHoldTime);
		if(pButtonHoldTime < required_hold_time_to_remove)
			return;

		if(Info.PlayerType == EPlayerType.RemotePlayer)
		{
			Debug.LogError("TODO: are you sure dialog");
			PhotonNetwork.CloseConnection(Info.PhotonPlayer);
			Debug.Log($"kick player {Info.PhotonPlayer}");
		}
		gameObject.SetActive(false);
		brainiacs.GameInitInfo.Players.Remove(Info);
		mainMenu.SetupMain.OnPlayersChanged();
	}

	//private void OnBtnRemove()
	//{
	//	if(Info.PlayerType == EPlayerType.RemotePlayer)
	//	{
	//		Debug.LogError("TODO: are you sure dialog");
	//		PhotonNetwork.CloseConnection(Info.PhotonPlayer);
	//		Debug.Log($"kick player {Info.PhotonPlayer}");
	//	}
	//	gameObject.SetActive(false);
	//	brainiacs.GameInitInfo.Players.Remove(Info);
	//	mainMenu.GameSetup.SetupMain.OnPlayersChanged();
	//	//OnElementChanged(true);
	//}

	/// <summary>
	/// Each player has number has ID <1,4>
	/// if pPhotonPlayer is null it will be determined in SetType
	/// </summary>
	public void Init(int pPlayerNumber, EPlayerType pPlayerType,
		PhotonPlayer pPhotonPlayer = null, EHero pHero = EHero.None)
	{
		PreInit();
		gameObject.SetActive(true);

		//master can remove every player (even himself - he wont be able to start game)
		btnRemove.gameObject.SetActive(IsMaster());


		//reset remove animation
		transform.localScale = Vector3.one;
		//reset portrait animation for remote player
		portrait.rectTransform.rotation = Quaternion.identity;

		Info.Number = pPlayerNumber;

		AssignColor((EPlayerColor)pPlayerNumber);

		Info.PhotonPlayer = pPhotonPlayer;

		//SetName($"{PhotonNetwork.LocalPlayer.NickName}_{pPlayerNumber}");

		if(pPhotonPlayer == null)
			SetReady(false);

		SetType(pPlayerType);

		if(IsItMe)
			SetName(PlayerNameManager.GetPlayerName(pPlayerNumber));

		//refresh keyset (needed when element is removed and added again)
		OnKeySetChanged();

		//if photon player is set then element is being
		//initialized at client
		bool elementCreated = pPhotonPlayer == null;
		//OnElementChanged(elementCreated);

		if(pHero == EHero.None)
			pHero = (EHero)UnityEngine.Random.Range(1, Enum.GetValues(typeof(EHero)).Length);
		heroSwapper.SetValue((int)pHero);

		//apply debug hero only in SP
		if(pPhotonPlayer == null && debug.Hero != EHero.None)
		{
			Debug.Log("Setting debug hero");
			heroSwapper.SetValue((int)debug.Hero);
		}

		brainiacs.GameInitInfo.UpdatePlayer(Info);
	}

	bool isClientInitializing;

	/// <summary>
	/// Init only for remote players 
	/// (called on both sides after any update)
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
		Init(pPlayer.Number, pPlayer.PlayerType, pPlayer.PhotonPlayer, pPlayer.Hero);
		//IsItMe = Info.PlayerType == EPlayerType.RemotePlayer &&
		//	pPlayer.PhotonPlayer == PhotonNetwork.LocalPlayer;
		SetReady(pPlayer.IsReady); //client has to confirm that he is ready

		portraitAnimator.enabled = false;

		//if LocalPlayer (master) then at first is active for all clients, but disabled if it is not me
		keySetSwapper.gameObject.SetActive(IsItMe && !PlatformManager.IsMobile());
		//OnKeySetChanged();

		if(Info.Color != pPlayer.Color)
		{
			Debug.LogError("Assigned color doesnt match");
		}
		AssignColor(pPlayer.Color);

		heroSwapper.SetValue((int)pPlayer.Hero);

		SetName(pPlayer.GetName());

		//if(IsItMe)
		//{
		//	Debug.Log("this is me");
		//	//this is only for remote player => always use the 1st player name 
		//	SetName(PlayerNameManager.GetPlayerName(1), false);
		//	OnRemoteConnected(Info.PhotonPlayer);
		//}
		heroSwapper.SetInteractable(IsItMe);
		//only master can kick out player
		//btnRemove.gameObject.SetActive(PhotonNetwork.IsMasterClient);

		isClientInitializing = false;

		if(IsItMe)
		{
			Debug.Log("this is me");
			//this is only for remote player => always use the 1st player name 
			SetName(PlayerNameManager.GetPlayerName(1));
			OnRemoteConnected(Info.PhotonPlayer);
		}
	}

	public void UpdateInfo(PlayerInitInfo pPlayerInfo)
	{
		//Debug.Log("UpdateInfo name " + Info.Name);
		Info = pPlayerInfo;
		Init(pPlayerInfo);
	}

	private void SetName(string pName)
	{
		Info.Name = pName;
		playerNameInput.text = pName;
	}

	private void SetType(EPlayerType pPlayerType)
	{
		//btnRemove.gameObject.SetActive(true);
		Info.PlayerType = pPlayerType;
		bool isRemote = pPlayerType == EPlayerType.RemotePlayer;
		if(isRemote)
		{
			if(Info.PhotonPlayer == null)
			{
				playerNameInput.text = "WAITING...";
				//SetName("WAITING...", false);
			}
		}
		//todo: even AI needs to have assigned photon player?
		else if(pPlayerType == EPlayerType.LocalPlayer ||
			pPlayerType == EPlayerType.AI)
		{
			//local player = master
			if(PhotonNetwork.InRoom && !IsMaster())
			{
				//client doesnt change local player
			}
			else
			{
				Info.PhotonPlayer = PhotonNetwork.LocalPlayer;
			}
		}

		keySetSwapper.gameObject.SetActive(IsItMe && !PlatformManager.IsMobile());
		playerNameInput.interactable = IsItMe;

		playerTypeText.text = pPlayerType.ToString();

		//start animation if player is remote and not connected yet
		portraitAnimator.enabled = isRemote && Info.PhotonPlayer == null;
		heroSwapper.SetInteractable(!isRemote);

		//master is always considered ready
		//client ready state is set in Init
		if(!isRemote)
			SetReady(true);
	}

	/// <summary>
	/// Is menu opened in master mode?
	/// </summary>
	private bool IsMaster()
	{
		return mainMenu.SetupMain.IsMaster;
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
		color.color = brainiacs.PlayerColorManager.GetColor(Info.Color);
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


	/// <summary>
	/// Called only on master
	/// </summary>
	public void OnRemoteConnected(PhotonPlayer pPlayer)
	{
		Info.PhotonPlayer = pPlayer;
		//remoteConnected = true;
		//remoteReady = true;
		//Debug.Log($"Remote player {pPlayer.UserId} connected");
		//SetName(pPlayer.NickName);
		portraitAnimator.enabled = false;
		portrait.rectTransform.rotation = Quaternion.identity;

		//OnElementChanged(true);
		SyncInfo();
	}

	/// <summary>
	/// Called only on master
	/// </summary>
	public void OnRemoteDisconnected(PhotonPlayer pPlayer)
	{
		Info.PhotonPlayer = null;
		Debug.Log($"Remote player {pPlayer.UserId} disconnected");

		//sets text and icon animation
		SetType(EPlayerType.RemotePlayer);

		SetReady(false);
	}

	private void OnHeroChanged()
	{
		Info.Hero = (EHero)(heroSwapper.CurrentIndex);
		//UnityEngine.Debug.Log($"OnHeroChanged {heroSwapper.CurrentIndex} => {Info.Hero}");

		portrait.sprite = brainiacs.HeroManager.GetHeroConfig(Info.Hero).Portrait;
		SyncInfo();
	}

	private void OnKeySetChanged()
	{
		//only local players can have keyset
		if(!IsLocalPlayer())
		{
			//Info.Keyset = EKeyset.None;
			return;
		}

		Info.Keyset = (EKeyset)keySetSwapper.CurrentIndex;
		var allPlayers = mainMenu.SetupMain.GetActivatedPlayers();

		//check if selected keyset is not used by another player
		foreach(var p in allPlayers)
		{
			if(p.Info.Number == Info.Number)
				continue;

			//if it is used => set another
			//note: use p.Info.PhotonPlayer.IsLocal, not p.Info.PlayerType == LocalPlayer
			//		or this doesnt work correctly on remote side
			//PhotonPlayer == null => not connected yet
			if((p.Info.PhotonPlayer == null || p.Info.PhotonPlayer.IsLocal)
				&& p.Info.Keyset == Info.Keyset)
			{
				keySetSwapper.SetNextValue();
				return;
			}
		}

		//SyncInfo();
	}

	/// <summary>
	/// On master local = LocalPlayer.
	/// On client local = RemotePlayer.
	/// </summary>
	public bool IsLocalPlayer()
	{
		bool isMaster = IsMaster();
		return isMaster && Info.PlayerType == EPlayerType.LocalPlayer ||
			!isMaster && Info.PlayerType == EPlayerType.RemotePlayer;
	}

	private void SyncInfo()
	{
		if(isClientInitializing)
			return;

		//trigger update info
		brainiacs.GameInitInfo.UpdatePlayer(Info);

		//no reason to sync
		//should happen only on master
		if(!PhotonNetwork.InRoom)
			return;

		byte[] infoS = Info.Serialize();
		mainMenu.Photon.Send(EPhotonMsg.MainMenu_SyncPlayerInfo, infoS);
		//Debug.Log("sync hero" + Info.Hero);
		//Debug.Log("sync name " + Info.GetName());
	}

}


public enum EPlayerType
{
	None = 0,
	LocalPlayer = 1,
	RemotePlayer = 2,
	AI = 3
}
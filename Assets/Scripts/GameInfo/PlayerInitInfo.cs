using System;
using System.Collections.Generic;
using FlatBuffers;
using Photon.Pun;
using PhotonPlayer = Photon.Realtime.Player;

public class PlayerInitInfo
{
	public string Name;
	public int Number;
	public EHero Hero;
	//public PlayerKeys PlayerKeys;
	public EPlayerColor Color;
	public EPlayerType PlayerType;
	public bool IsReady;
	public PhotonPlayer PhotonPlayer;
	//NOTE: if new attribute added -> update Update() and 
	//PlayerInitInfo(UIGameSetupPlayerEl pElement)

	public List<EWeaponId> debug_StartupWeapon = new List<EWeaponId>();

	//TODO: add PlayerInitInfo to UIGameSetupPlayerEl as parameter to hold data
	public PlayerInitInfo() { }

	//DEBUG INIT
	public PlayerInitInfo(int pNumber, EHero pHero, string pName, EPlayerColor pColor, EPlayerType pPlayerType, PhotonPlayer pPhotonPlayer = null)
	{
		Number = pNumber;
		Hero = pHero;
		Name = pName;
		Color = pColor;
		PlayerType = pPlayerType;
		PhotonPlayer = pPhotonPlayer;
	}

	public PlayerInitInfo Clone()
	{
		return new PlayerInitInfo(Number, Hero, Name, Color, PlayerType, PhotonPlayer);
	}

	internal Offset<PlayerInitInfoS> Create(ref FlatBufferBuilder fbb)
	{
		StringOffset nameOff = fbb.CreateString(Name);

		Offset<PlayerInitInfoS> result = PlayerInitInfoS.CreatePlayerInitInfoS(fbb,
			nameOff, Number, (int)Hero, (int)Color, (int)PlayerType,
			PhotonPlayer != null ? PhotonPlayer.ActorNumber : -1, 
			IsReady);
		return result;
	}

	internal byte[] Serizalize()
	{
		FlatBufferBuilder fbb = new FlatBufferBuilder(1);
		StringOffset nameOff = fbb.CreateString(Name);

		PlayerInitInfoS.StartPlayerInitInfoS(fbb);
		PlayerInitInfoS.AddName(fbb, nameOff);
		PlayerInitInfoS.AddNumber(fbb, Number);
		PlayerInitInfoS.AddHero(fbb, (int)Hero);
		PlayerInitInfoS.AddColor(fbb, (int)Color);
		PlayerInitInfoS.AddPlayerType(fbb, (int)PlayerType);
		PlayerInitInfoS.AddIsReady(fbb, IsReady);
		PlayerInitInfoS.AddPhotonPlayerNumber(fbb, PhotonPlayer.ActorNumber);
		Offset<PlayerInitInfoS> playerInfoOffset = PlayerInitInfoS.EndPlayerInitInfoS(fbb);
		fbb.Finish(playerInfoOffset.Value);
		byte[] result = fbb.SizedByteArray();
		return result;
	}

	internal static PlayerInitInfo Deserialize(PlayerInitInfoS pPlayerS)
	{
		PlayerInitInfo playerInfo = new PlayerInitInfo();
		playerInfo.Number = pPlayerS.Number;
		playerInfo.Hero = (EHero)pPlayerS.Hero;
		playerInfo.Name = pPlayerS.Name;
		playerInfo.Color = (EPlayerColor)pPlayerS.Color;
		playerInfo.PlayerType = (EPlayerType)pPlayerS.PlayerType;
		playerInfo.IsReady = pPlayerS.IsReady;

		if(PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null)
			playerInfo.PhotonPlayer = PhotonNetwork.CurrentRoom.GetPlayer(pPlayerS.PhotonPlayerNumber);

		return playerInfo;
	}

	internal void Update(PlayerInitInfo pNewInfo)
	{
		Number = pNewInfo.Number;
		Hero = pNewInfo.Hero;
		Name = pNewInfo.Name;
		Color = pNewInfo.Color;
		PlayerType = pNewInfo.PlayerType;
		PhotonPlayer = pNewInfo.PhotonPlayer;
		IsReady = pNewInfo.IsReady;
	}

	/// <summary>
	/// Checks if assigned photon player is local player.
	/// If single player => always true.
	/// </summary>
	public bool IsItMe()
	{
		//single player => all players are ME
		if(!Brainiacs.Instance.GameInitInfo.IsMultiplayer())
		{
			return true;
		}

		if(PhotonPlayer == null || !PhotonNetwork.IsConnected)
			return false;

		return PhotonPlayer == PhotonNetwork.LocalPlayer;
	}
}

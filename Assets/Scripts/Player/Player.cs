using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class Player : BrainiacsBehaviour
{
	[SerializeField]
	private PlayerInput input;

	[SerializeField]
	public PlayerWeaponController WeaponController;

	[SerializeField]
	public PlayerVisual Visual;

	[SerializeField]
	public PlayerMovement Movement;

	[SerializeField]
	public PlayerHealth Health;

	public BoxCollider2D Collider => boxCollider2D;

	[SerializeField]
	public PlayerItemController ItemController;

	[SerializeField]
	public PlayerStats Stats;

	[SerializeField] PhotonView view;
	public PhotonPlayer photonPlayer;

	PlayerInitInfo initInfo;

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			Health.DebugDie();
		}
	}

	internal void SetInfo(PlayerInitInfo pPlayerInfo, Vector3 pSpawnPosition)
	{
		initInfo = pPlayerInfo;

		photonPlayer = pPlayerInfo.PhotonPlayer;
		if(photonPlayer != null)
		{
			view.TransferOwnership(photonPlayer);
			Debug.Log("Transfer ownership to " + photonPlayer.NickName);
		}		
		const int player_photon_id_start = 10;
		view.ViewID = player_photon_id_start + pPlayerInfo.Number;
		Debug.Log(this + " IsMine: " + view.IsMine);

		//Debug.Log($"{this} SetInfo");
		Stats.Init(pPlayerInfo);

		input.Init(pPlayerInfo);


		HeroSpecialWeaponConfig heroSpecialConfig =
			brainiacs.ItemManager.GetHeroSpecialWeaponConfig(pPlayerInfo.Hero);
		ItemController.AddHeroSpecialWeapon(pPlayerInfo.Hero);


		ItemController.AddMapWeapon(EWeaponId.MP40);
		ItemController.AddMapWeapon(EWeaponId.Lasergun);
		ItemController.AddMapWeapon(EWeaponId.Biogun);

		ItemController.AddHeroBasicWeapon(pPlayerInfo.Hero);

		Visual.Init(pPlayerInfo);

		Movement.SpawnAt(pSpawnPosition);
	}

	public override string ToString()
	{
		return $"{initInfo.Number}_{gameObject.name} [{view.ViewID}]";
	}
}

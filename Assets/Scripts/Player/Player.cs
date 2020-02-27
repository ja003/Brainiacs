using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GameBehaviour
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

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			Health.DebugDie();
		}
	}

	internal void SetInfo(PlayerInitInfo pPlayerInfo, Vector3 pSpawnPosition)
	{
		//Debug.Log($"{this} SetInfo");
		Stats.Init(pPlayerInfo);

		input.Keys = pPlayerInfo.PlayerKeys;


		HeroSpecialWeaponConfig heroSpecialConfig =
			brainiacs.ItemManager.GetHeroSpecialWeaponConfig(pPlayerInfo.Hero);
		ItemController.AddHeroSpecialWeapon(pPlayerInfo.Hero);


		ItemController.AddMapWeapon(EWeaponId.MP40);

		ItemController.AddHeroBasicWeapon(pPlayerInfo.Hero);

		Visual.Init(pPlayerInfo);

		Movement.SpawnAt(pSpawnPosition);
	}
}

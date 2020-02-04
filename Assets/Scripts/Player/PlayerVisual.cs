using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : GameBehaviour
{
	SpriteRenderer playerRenderer;
	HeroConfig heroConfig;

	[SerializeField]
	private SpriteRenderer weaponUp;
	[SerializeField]
	private SpriteRenderer weaponRight;
	[SerializeField]
	private SpriteRenderer weaponDown;
	[SerializeField]
	private SpriteRenderer weaponLeft;



	[SerializeField]
	private SpriteRenderer handsUp;
	[SerializeField]
	private SpriteRenderer handsRight;

	internal void OnDie()
	{
		animator.SetBool(AC_KEY_IS_DEAD, true);
		animator.SetTrigger(AC_KEY_DIE);

		DoInTime(() => SetVisible(false), 1);
	}

	public void SetVisible(bool pValue)
	{
		gameObject.SetActive(pValue);
		//playerRenderer.enabled = pValue;
	}

	public void OnSpawn()
	{
		//Debug.Log($"{this} Spawn");
		SetVisible(true);

		animator.SetBool(AC_KEY_IS_DEAD, false);
		OnDirectionChange(currentDirection); //reset animator value
	}

	[SerializeField]
	private SpriteRenderer handsDown;
	[SerializeField]
	private SpriteRenderer handsLeft;

	public void Init(SpriteRenderer pPlayerRenderer, HeroConfig pHeroConfig)
	{
		playerRenderer = pPlayerRenderer;
		heroConfig = pHeroConfig;
	}

	[SerializeField]
	private int currentSortOrder = 0;
	const int player_sort_index_start = 10;

	private const string AC_KEY_DIRECTION = "direction";
	private const string AC_KEY_IS_WALKING = "isWalking";
	private const string AC_KEY_IS_DEAD = "isDead";
	private const string AC_KEY_DIE = "die";

	internal void SetSortOrder(int pOrder, bool pForceRefresh = false)
	{
		if(currentSortOrder == pOrder && !pForceRefresh)
			return;

		currentSortOrder = pOrder;


		UpdatePlayerSortOrder();
		UpdateWeaponSortOrder();
		UpdateHandsSortOrder();


	}

	public void Move()
	{
		animator.SetBool(AC_KEY_IS_WALKING, true);
	}

	public void Idle()
	{
		animator.SetBool(AC_KEY_IS_WALKING, false);
	}

	private EDirection currentDirection;
	public void OnDirectionChange(EDirection pDirection)
	{
		currentDirection = pDirection;

		handsDown.enabled = false;
		handsRight.enabled = false;
		handsUp.enabled = false;
		handsLeft.enabled = false;

		weaponRight.enabled = false;
		weaponDown.enabled = false;
		weaponLeft.enabled = false;
		weaponUp.enabled = false;

		switch(pDirection)
		{
			case EDirection.Up:
				handsUp.enabled = true;
				weaponUp.enabled = true;
				break;
			case EDirection.Right:
				weaponRight.enabled = true;
				handsRight.enabled = true;
				break;
			case EDirection.Down:
				weaponDown.enabled = true;
				handsDown.enabled = true;
				break;
			case EDirection.Left:
				handsLeft.enabled = true;
				weaponLeft.enabled = true;
				break;
		}

		SetSortOrder(currentSortOrder, true);

		animator.SetInteger(AC_KEY_DIRECTION, (int)pDirection);
	}

	public void SetActiveWeapon(PlayerWeapon pWeapon)
	{
		weaponUp.sprite = pWeapon.Config.PlayerSpriteUp;
		weaponRight.sprite = pWeapon.Config.PlayerSpriteRight;
		weaponDown.sprite = pWeapon.Config.playerSpriteDown;
		weaponLeft.sprite = pWeapon.Config.PlayerSpriteLeft;
	}

	private void UpdatePlayerSortOrder()
	{
		int order = GetCurrentOrderIndexStart();
		if(currentDirection == EDirection.Up)
			order += 2;

		playerRenderer.sortingOrder = order;
	}

	private void UpdateHandsSortOrder()
	{
		int order = GetCurrentOrderIndexStart() + 2;

		handsUp.sortingOrder = order - 1;
		handsRight.sortingOrder = order;
		handsDown.sortingOrder = order;
		handsLeft.sortingOrder = order;
	}
	
	/// <summary>
	/// Standart order:
	/// - Player, Weapon, Hands
	/// Direction up:
	/// - Weapon, Hands, Player
	/// 
	/// Players are sorted based on their Y position.
	/// Each player has reserved layer indexes based on their order.
	/// </summary>
	private int GetCurrentOrderIndexStart()
	{
		return player_sort_index_start + currentSortOrder * player_sort_index_start;
	}

	private void UpdateWeaponSortOrder()
	{
		int order = GetCurrentOrderIndexStart() + 1;

		weaponUp.sortingOrder = order - 1;
		weaponRight.sortingOrder = order;
		weaponDown.sortingOrder = order;
		weaponLeft.sortingOrder = order;
	}
}

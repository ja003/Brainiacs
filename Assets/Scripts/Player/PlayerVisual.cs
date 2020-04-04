using ActionCode.SpriteEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : PlayerBehaviour
{
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
	private SpriteRenderer handsDown;
	[SerializeField]
	private SpriteRenderer handsLeft;
	[SerializeField]
	private SpriteRenderer handsUp;
	[SerializeField]
	private SpriteRenderer handsRight;


	[SerializeField]
	private PaletteSwapController paletteSwap;
	[SerializeField]
	private PlayerMovement movement;

	[SerializeField] PlayerNetworkController network;


	internal void OnDie()
	{
		SetAnimBool(AC_KEY_IS_DEAD, true);
		SetAnimTrigger(AC_KEY_DIE);

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

		SetAnimBool(AC_KEY_IS_DEAD, false);
		OnDirectionChange(currentDirection); //reset animator value
	}



	public void Init(PlayerInitInfo pPlayerInfo)
	{
		heroConfig = brainiacs.HeroManager.GetHeroConfig(pPlayerInfo.Hero);

		if(heroConfig.Animator)
			animator.runtimeAnimatorController = heroConfig.Animator;
		else
			Debug.LogError($"{heroConfig.Hero} doesnt have animator configured");

		int colorIndex = GetColorPaletteIndex(pPlayerInfo.Color);
		paletteSwap.SetPalette(colorIndex);
	}

	private static int GetColorPaletteIndex(EPlayerColor pColor)
	{
		return (int)pColor - 1;
	}

	[SerializeField]
	public int CurrentSortOrder { get; private set; } = 0;
	const int player_sort_index_start = 10;

	/// <summary>
	/// Returns sort order for projectile which equals
	/// the weapon sort order + 1
	/// </summary>
	public int GetProjectileSortOrder()
	{
		switch(currentDirection)
		{
			case EDirection.Up:
				return weaponUp.sortingOrder + 1;
			//right, down and left should be the same
			case EDirection.Right:
				return weaponRight.sortingOrder + 1;
			case EDirection.Down:
				return weaponDown.sortingOrder + 1;
			case EDirection.Left:
				return weaponLeft.sortingOrder + 1;
		}
		Debug.LogError("Error GetProjectileSortOrder");
		return -1;
	}

	/// <summary>
	/// Returns current player's sort order + 2 (to display over hands)
	/// </summary>
	public int GetPlayerOverlaySortOrder()
	{
		return spriteRend.sortingOrder + 2;
	}


	private const string AC_KEY_DIRECTION = "direction";
	//private const string AC_KEY_IS_WALKING = "isWalking";
	private const string AC_KEY_IS_DEAD = "isDead";
	private const string AC_KEY_DIE = "die";
	private const string AC_KEY_WALK_SPEED = "walkSpeed";

	//idle is just slowed walk animation
	private const float WALK_ANIM_SPEED = 1;
	//maybe idle speed shoudl be 0 - TEST
	private const float IDLE_ANIM_SPEED = 0.1f;

	internal void SetSortOrder(int pOrder, bool pForceRefresh = false)
	{
		if(CurrentSortOrder == pOrder && !pForceRefresh)
			return;

		CurrentSortOrder = pOrder;

		UpdatePlayerSortOrder();
		UpdateWeaponSortOrder();
		UpdateHandsSortOrder();

		OnSortOrderChanged?.Invoke();
	}
	public Action OnSortOrderChanged;

	public void Move()
	{
		SetAnimFloat(AC_KEY_WALK_SPEED, WALK_ANIM_SPEED);
	}

	public void Idle()
	{
		SetAnimFloat(AC_KEY_WALK_SPEED, IDLE_ANIM_SPEED);
	}

	private void SetAnimFloat(string pKey, float pValue)
	{
		animator.SetFloat(pKey, pValue);
		player.LocalRemote?.Visual.SetAnimFloat(pKey, pValue);
	}

	private void SetAnimBool(string pKey, bool pValue)
	{
		animator.SetBool(pKey, pValue);
		player.LocalRemote?.Visual.SetAnimBool(pKey, pValue);
	}

	private void SetAnimTrigger(string pKey)
	{
		animator.SetTrigger(pKey);
		player.LocalRemote?.Visual.SetAnimTrigger(pKey);
	}

	private EDirection currentDirection => movement.CurrentDirection;

	public UIPlayerInfoElement PlayerInfo;
	public UIScoreboardElement Scoreboard;

	public void OnDirectionChange(EDirection pDirection)
	{
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

		SetSortOrder(CurrentSortOrder, true);

		//direction has to be float to be used in blend tree
		SetAnimFloat(AC_KEY_DIRECTION, (int)pDirection);

		//activeWeapon.OnDirectionChange(pDirection);

		network.Send(EPhotonMsg.Player_ChangeDirection, pDirection);
	}

	//todo: PlayerWeapon ref shouldnt be stored in Visual
	PlayerWeapon activeWeapon;
	public void SetActiveWeapon(PlayerWeapon pWeapon)
	{
		activeWeapon = pWeapon;
		ShowWeapon(pWeapon.Id);
		network.Send(EPhotonMsg.Player_ShowWeapon, pWeapon.Id);
	}
	public void ShowWeapon(EWeaponId pWeapon)
	{
		WeaponConfig config = 
			brainiacs.ItemManager.GetWeaponConfig(pWeapon);
		weaponUp.sprite = config.VisualInfo.PlayerSpriteUp;
		weaponRight.sprite = config.VisualInfo.PlayerSpriteRight;
		weaponDown.sprite = config.VisualInfo.playerSpriteDown;
		weaponLeft.sprite = config.VisualInfo.PlayerSpriteLeft;
	}

	private void UpdatePlayerSortOrder()
	{
		int order = GetCurrentOrderIndexStart();
		if(currentDirection == EDirection.Up)
			order += 2;

		spriteRend.sortingOrder = order;
		//Debug.Log("UpdatePlayerSortOrder " + order);
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
		return player_sort_index_start + CurrentSortOrder * player_sort_index_start;
	}

	private void UpdateWeaponSortOrder()
	{
		int order = GetCurrentOrderIndexStart() + 1;

		//todo: merge into 1 sprite
		weaponUp.sortingOrder = order - 1;
		weaponRight.sortingOrder = order;
		weaponDown.sortingOrder = order;
		weaponLeft.sortingOrder = order;
	}
}

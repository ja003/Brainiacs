using ActionCode.SpriteEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : PlayerBehaviour
{
	HeroConfig heroConfig;

	[SerializeField] private SpriteRenderer weaponUp = null;
	[SerializeField] private SpriteRenderer weaponRight = null;
	[SerializeField] private SpriteRenderer weaponDown = null;
	[SerializeField] private SpriteRenderer weaponLeft = null;

	[SerializeField] private SpriteRenderer handsDown = null;
	[SerializeField] private SpriteRenderer handsLeft = null;
	[SerializeField] private SpriteRenderer handsUp = null;
	[SerializeField] private SpriteRenderer handsRight = null;


	[SerializeField] private PaletteSwapController paletteSwap = null;

	[SerializeField] public int CurrentSortOrder { get; private set; } = 0;

	//Cca player model size. Not precise!
	public static float PlayerBodySize { get; } = 1f;

	EPlayerColor playerColor; //has to be stored for color change on hit

	internal void OnSetActive(bool pValue)
	{
		spriteRend.enabled = pValue;

		//if(!pValue)
		//{
		//	handsDown.enabled = pValue;
		//	handsRight.enabled = pValue;
		//	handsUp.enabled = pValue;
		//	handsLeft.enabled = pValue;

		//	weaponRight.enabled = pValue;
		//	weaponDown.enabled = pValue;
		//	weaponLeft.enabled = pValue;
		//	weaponUp.enabled = pValue;
		//}
	}

	internal void OnDie()
	{
		SetAnimBool(AC_KEY_IS_DEAD, true);
		SetAnimTrigger(AC_KEY_DIE);

		DoInTime(() => SetVisible(false), 1);
	}

	public void SetVisible(bool pValue)
	{
		player.SetActive(pValue);
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

		playerColor = pPlayerInfo.Color;
		int paletteIndex = GetColorPaletteIndex(pPlayerInfo.Color);
		paletteSwap.SetPalette(paletteIndex);
	}

	/// <summary>
	/// Color flicker on any damage
	/// </summary>
	public void OnDamage()
	{
		StartCoroutine(FlickColor());
	}

	/// <summary>
	/// Swap player color palette for small amount of time then change it back
	/// </summary>
	private IEnumerator FlickColor()
	{
		const float flicker_length = 0.1f;
		paletteSwap.SetPalette(GetColorPaletteIndex(playerColor, 1));
		yield return new WaitForSeconds(flicker_length);
		paletteSwap.SetPalette(GetColorPaletteIndex(playerColor));
	}

	private static int GetColorPaletteIndex(EPlayerColor pColor, int pOffset = 0)
	{
		return (int)pColor - 1 + pOffset;
	}

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

	internal void UpdateSortOrder(int pOrder, bool pForceRefresh = false)
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
		player.LocalImage?.Visual.SetAnimFloat(pKey, pValue);
	}

	private void SetAnimBool(string pKey, bool pValue)
	{
		animator.SetBool(pKey, pValue);
		player.LocalImage?.Visual.SetAnimBool(pKey, pValue);
	}

	private void SetAnimTrigger(string pKey)
	{
		animator.SetTrigger(pKey);
		player.LocalImage?.Visual.SetAnimTrigger(pKey);
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

		UpdateSortOrder(CurrentSortOrder, true);

		//direction has to be float to be used in blend tree
		SetAnimFloat(AC_KEY_DIRECTION, (int)pDirection);

		//activeWeapon.OnDirectionChange(pDirection);

		player.Photon.Send(EPhotonMsg.Player_ChangeDirection, pDirection);
	}

	//todo: PlayerWeapon ref shouldnt be stored in Visual
	PlayerWeapon activeWeapon;
	public void SetActiveWeapon(PlayerWeapon pWeapon)
	{
		activeWeapon = pWeapon;
		ShowWeapon(pWeapon.Id);
		player.Photon.Send(EPhotonMsg.Player_ShowWeapon, pWeapon.Id);
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
		int order = SortLayerManager.GetPlayerSortIndexStart(ESortLayer.Player, CurrentSortOrder);
		if(currentDirection == EDirection.Up)
			order += 2;

		spriteRend.sortingOrder = order;
		//Debug.Log("UpdatePlayerSortOrder " + order);
	}

	private void UpdateHandsSortOrder()
	{
		int order = SortLayerManager.GetPlayerSortIndexStart(ESortLayer.Hands, CurrentSortOrder);

		handsUp.sortingOrder = order - 1;
		handsRight.sortingOrder = order;
		handsDown.sortingOrder = order;
		handsLeft.sortingOrder = order;
	}

	public enum ESortLayer
	{
		None,
		Player,
		Weapon,
		Hands
	}

	private void UpdateWeaponSortOrder()
	{
		int order = SortLayerManager.GetPlayerSortIndexStart(ESortLayer.Weapon, CurrentSortOrder);

		//todo: merge into 1 sprite
		weaponUp.sortingOrder = order - 1;
		weaponRight.sortingOrder = order;
		weaponDown.sortingOrder = order;
		weaponLeft.sortingOrder = order;
	}
}

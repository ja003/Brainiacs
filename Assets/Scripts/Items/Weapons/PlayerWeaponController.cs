using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : PlayerBehaviour
{
	[SerializeField] private Transform projectileStartUp = null;
	[SerializeField] private Transform projectileStartRight = null;
	[SerializeField] private Transform projectileStartDown = null;
	[SerializeField] private Transform projectileStartLeft = null;

	private List<PlayerWeapon> weapons = new List<PlayerWeapon>();
	public PlayerWeapon ActiveWeapon { get; private set; }
	[SerializeField] private int activeWeaponIndex;

	private Action<PlayerWeapon> onWeaponInfoChanged;


	public void InvokeWeaponChange(PlayerWeapon pWeapon)
	{
		onWeaponInfoChanged(pWeapon);
	}

	public void OnPowerUpAmmo()
	{
		foreach(var weapon in weapons)
		{
			weapon.OnPowerUpAmmo();
		}
	}

	internal void OnDirectionChange(EDirection pDirection)
	{
		if(ActiveWeapon == null && !player.IsLocalImage)
		{
			Debug.LogError("Active weapon is null");
		}

		ActiveWeapon?.OnDirectionChange(pDirection);
	}

	private void RemoveWeapon(PlayerWeapon pWeapon)
	{
		if(ActiveWeapon == pWeapon)
		{
			//we can count on player always having at least 2 weapons (basic + special)
			SwapWeapon();
		}
		weapons.Remove(pWeapon);
	}

	public void SetOnWeaponInfoChanged(Action<PlayerWeapon> pAction)
	{
		onWeaponInfoChanged += pAction;
		if(ActiveWeapon != null)
			onWeaponInfoChanged.Invoke(ActiveWeapon);
	}

	//public void AddWeapon(NewWeaponConfig pConfig)
	public void AddWeapon(PlayerWeapon pWeapon)
	{
		PlayerWeapon weaponInInventory =
			weapons.Find(a => a.Id.Equals(pWeapon.Id));
		if(weaponInInventory != null)
		{
			weaponInInventory.OnAddSameWeapon(pWeapon);
		}
		else
		{
			weapons.Add(pWeapon);
			weaponInInventory = pWeapon;
		}

		SetActiveWeapon(weapons.IndexOf(weaponInInventory));
	}

	public bool CanSwapWeapon = true;
	public void SwapWeapon()
	{
		if(!CanSwapWeapon)
		{
			Debug.Log("Cant swap");
			return;
		}
		SetActiveWeapon(activeWeaponIndex + 1);
	}

	public void StopUseWeapon()
	{
		ActiveWeapon.StopUse();
	}

	public void UseWeapon()
	{
		EWeaponUseResult useResult = ActiveWeapon.Use();
		if(useResult == EWeaponUseResult.CantUse)
		{
			//Debug.Log($"{activeWeapon} cant be used");
			//TODO: play CANT_USE sound
			return;
		}
		//Debug.Log($"{activeWeapon} USE");

		InvokeWeaponChange(ActiveWeapon);

		HandleUseResult(useResult);
	}

	public void ShootProjectile(ProjectileWeaponInfo pProjectile)
	{
		game.ProjectileManager.SpawnProjectile(
				GetProjectileStartPosition(movement.CurrentDirection),
				player, pProjectile.Projectile);

		player.LocalImage?.WeaponController.ShootProjectile(pProjectile);
	}

	private void HandleUseResult(EWeaponUseResult pUseResult)
	{
		switch(pUseResult)
		{
			case EWeaponUseResult.Reload:
				StopUseWeapon();
				StartReloadWeapon(ActiveWeapon);
				break;
			case EWeaponUseResult.Remove:
				StopUseWeapon();
				RemoveWeapon(ActiveWeapon);
				break;
		}
	}

	/// <summary>
	/// Starts realoding the weapon.
	/// There can be multiple weapons reloading at the same time.
	/// </summary>
	private void StartReloadWeapon(PlayerWeapon pWeapon)
	{
		pWeapon.IsRealoading = true;
		ActiveWeapon.OnStartReloadWeapon();

		Action onReloaded = pWeapon.Reload;
		Action<float> onReloadUpdate = pWeapon.ReportReloadProgress;

		DoInTime(onReloaded, pWeapon.Info.Cooldown, onReloadUpdate);
		InvokeWeaponChange(pWeapon);
	}

	/// <summary>
	/// Returns projectile start in current direction
	/// </summary>
	public Transform GetProjectileStart()
	{
		return GetProjectileStart(movement.CurrentDirection);
	}

	public Transform GetProjectileStart(EDirection pDirection)
	{
		switch(pDirection)
		{
			case EDirection.Up:
				return projectileStartUp;
			case EDirection.Right:
				return projectileStartRight;
			case EDirection.Down:
				return projectileStartDown;
			case EDirection.Left:
				return projectileStartLeft;
		}
		Debug.LogError("Cant GetProjectileStart");
		return projectileStartRight;
	}

	private Vector3 GetProjectileStartPosition(EDirection pDirection)
	{
		return GetProjectileStart(pDirection).position;
	}

	public void SetDefaultWeaponActive()
	{
		//todo: might not be valid
		//will use for debug for now
		SetActiveWeapon(0);
	}

	private void SetActiveWeapon(int pIndex)
	{
		pIndex = pIndex % weapons.Count;

		activeWeaponIndex = pIndex;

		if(ActiveWeapon != null)
			ActiveWeapon.IsActive = false;
		ActiveWeapon = weapons[pIndex];
		ActiveWeapon.IsActive = true;
		//Debug.Log("SetActiveWeapon " + ActiveWeapon);

		visual.SetActiveWeapon(ActiveWeapon);

		onWeaponInfoChanged?.Invoke(ActiveWeapon);
		ActiveWeapon.OnSetActive();
	}


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : GameBehaviour
{
	[SerializeField]
	private PlayerVisual visual;

	[SerializeField]
	public PlayerMovement movement;

	[SerializeField]
	private Player owner;


	[SerializeField] private Transform projectileStartUp;
	[SerializeField] private Transform projectileStartRight;
	[SerializeField] private Transform projectileStartDown;
	[SerializeField] private Transform projectileStartLeft;

	private List<PlayerWeapon> weapons = new List<PlayerWeapon>();
	private PlayerWeapon activeWeapon;
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

	private void RemoveWeapon(PlayerWeapon pWeapon)
	{
		if(activeWeapon == pWeapon)
		{
			//we can count on player always having at least 2 weapons (basic + special)
			SwapWeapon();
		}
		weapons.Remove(pWeapon);
	}

	public void SetOnWeaponInfoChanged(Action<PlayerWeapon> pAction)
	{
		onWeaponInfoChanged += pAction;
		onWeaponInfoChanged.Invoke(activeWeapon);
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

	public void SwapWeapon()
	{
		SetActiveWeapon(activeWeaponIndex + 1);
	}

	public void StopUseWeapon()
	{
		activeWeapon.StopUse();
	}

	public void UseWeapon()
	{
		EWeaponUseResult useResult = activeWeapon.Use();
		if(useResult == EWeaponUseResult.CantUse)
		{
			//Debug.Log($"{activeWeapon} cant be used");
			//TODO: play CANT_USE sound
			return;
		}
		//Debug.Log($"{activeWeapon} USE");

		//if(activeWeapon.Config.Projectile != null)
		//{
		//	game.ProjectileManager.SpawnProjectile(
		//		GetProjectileStartPosition(movement.CurrentDirection),
		//		movement,
		//		activeWeapon.Config.Projectile);
		//}
		onWeaponInfoChanged.Invoke(activeWeapon);

		HandleUseResult(useResult);
	}

	public void ShootProjectile(ProjectileWeaponInfo pProjectile)
	{
		game.ProjectileManager.SpawnProjectile(
				GetProjectileStartPosition(movement.CurrentDirection),
				owner, pProjectile.Projectile);
	}

	private void HandleUseResult(EWeaponUseResult pUseResult)
	{
		switch(pUseResult)
		{
			case EWeaponUseResult.Reload:
				ReloadWeapon(activeWeapon);
				break;
			case EWeaponUseResult.Remove:
				RemoveWeapon(activeWeapon);
				break;
		}
	}

	/// <summary>
	/// Starts realoding the weapon.
	/// There can be multiple weapons reloading at the same time.
	/// </summary>
	private void ReloadWeapon(PlayerWeapon pWeapon)
	{
		pWeapon.IsRealoading = true;

		Action onReloaded = pWeapon.Reload;
		Action<float> onReloadUpdate = pWeapon.ReportReloadProgress;

		DoInTime(onReloaded, pWeapon.Info.Cooldown, onReloadUpdate);
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

	private void SetActiveWeapon(int pIndex)
	{
		pIndex = pIndex % weapons.Count;

		activeWeaponIndex = pIndex;

		if(activeWeapon != null)
			activeWeapon.IsActive = false;
		activeWeapon = weapons[pIndex];
		activeWeapon.IsActive = true;

		visual.SetActiveWeapon(activeWeapon);
		onWeaponInfoChanged?.Invoke(activeWeapon);
		activeWeapon.OnSetActive();
	}


}

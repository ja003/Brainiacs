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

	public List<PlayerWeapon> weapons = new List<PlayerWeapon>();
	public PlayerWeapon ActiveWeapon { get; private set; } //set only on player side
	[SerializeField] private int activeWeaponIndex;

	private Action<PlayerWeapon> onWeaponInfoChanged;


	public void InvokeWeaponChange(PlayerWeapon pWeapon)
	{
		onWeaponInfoChanged?.Invoke(pWeapon);
	}

	public void OnDie()
	{
		StopUseWeapon(false);
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
		//Debug.Log("AddWeapon " + pWeapon);

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

	internal void PlayWeaponUseSound(EWeaponId pId)
	{
		brainiacs.AudioManager.PlayWeaponUseSound(pId, player.AudioSource, false);
		player.Photon.Send(EPhotonMsg.Player_PlayWeaponUseSound, (int)pId);
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

	public void StopUseWeapon(bool pIsUserInput)
	{
		if(IsLogEnabled())
			Debug.Log("StopUseWeapon");
		ActiveWeapon?.StopUse(pIsUserInput);
	}

	internal PlayerWeapon GetWeapon(EWeaponId pWeaponId)
	{
		return weapons.Find(a => a.Id == pWeaponId);
	}

	private bool IsLogEnabled()
	{
		return false;
		//return player.InitInfo.PlayerType == EPlayerType.AI;
	}

	public void UseWeapon()
	{
		if(IsLogEnabled())
			Debug.Log("UseWeapon");

		EWeaponUseResult useResult = ActiveWeapon.Use();
		if(useResult == EWeaponUseResult.CantUse)
		{
			//Debug.Log($"{activeWeapon} cant be used");
			//TODO: play CANT_USE sound
			StopUseWeapon(false);
			return;
		}
		//Debug.Log($"{activeWeapon} USE");

		InvokeWeaponChange(ActiveWeapon);

		HandleUseResult(useResult);
	}

	public void ShootProjectile(ProjectileWeaponInfo pProjectile)
	{
		game.ProjectileManager.SpawnProjectile(
				GetProjectileStartPosition(pProjectile),
				player, pProjectile.Projectile);

		player.LocalImage?.WeaponController.ShootProjectile(pProjectile);
	}

	private void HandleUseResult(EWeaponUseResult pUseResult)
	{
		switch(pUseResult)
		{
			case EWeaponUseResult.Reload:
				StopUseWeapon(true);
				StartReloadWeapon(ActiveWeapon);
				break;
			case EWeaponUseResult.Remove:
				StopUseWeapon(true);
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

		DoInTime(onReloaded, pWeapon.Info.Cooldown, false, onReloadUpdate);
		InvokeWeaponChange(pWeapon);
	}

	/// <summary>
	/// Returns projectile start in current direction
	/// </summary>
	public Transform GetProjectileStart()
	{
		return GetProjectileStart(movement.CurrentEDirection);
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

	private Vector2 GetProjectileStartPosition(ProjectileWeaponInfo pProjectile)
	{
		Vector3 projectileStart = GetProjectileStart(movement.CurrentEDirection).position;
		return Utils.GetVector2(projectileStart) + pProjectile.GetProjectileStartOffset(movement.CurrentEDirection);
	}

	public void SetDefaultWeaponActive()
	{
		//todo: might not be valid
		//will use for debug for now
		SetActiveWeapon(0);
	}

	public void SetActiveWeapon(EWeaponId pWeapon)
	{
		if(!HasWeapon(pWeapon))
		{
			Debug.LogError("Player doesnt have a weapon " + pWeapon);
			return;
		}
		int weaponIndex = GetWeaponIndex(pWeapon);
		SetActiveWeapon(weaponIndex);
	}

	private int GetWeaponIndex(EWeaponId pWeapon)
	{
		if(!HasWeapon(pWeapon))
		{
			Debug.LogError("Player doesnt have a weapon " + pWeapon);
			return -1;
		}
		for(int i = 0; i < weapons.Count; i++)
		{
			if(weapons[i].Id == pWeapon)
				return i;
		}
		Debug.LogError("Weapon not found " + pWeapon);
		return -1;
	}

	public bool HasWeapon(EWeaponId pWeapon)
	{
		return GetWeapon(pWeapon) != null;
	}

	private void SetActiveWeapon(int pIndex)
	{
		//stop using weapon before swap 
		//needed eg. when ai is swapping from DaVinci tank, flamethrower
		StopUseWeapon(false);

		pIndex = pIndex % weapons.Count;

		activeWeaponIndex = pIndex;

		if(ActiveWeapon != null)
			ActiveWeapon.IsActive = false;
		ActiveWeapon = weapons[pIndex];
		ActiveWeapon.IsActive = true;
		//Debug.Log(player + "SetActiveWeapon " + ActiveWeapon);

		visual.SetActiveWeapon(ActiveWeapon);

		InvokeWeaponChange(ActiveWeapon);
		//onWeaponInfoChanged?.Invoke(ActiveWeapon);
		ActiveWeapon.OnSetActive();
	}
}

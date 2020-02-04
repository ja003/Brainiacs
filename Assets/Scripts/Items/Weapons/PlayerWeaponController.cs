using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : GameBehaviour
{
	[SerializeField]
	private PlayerVisual visual;

	[SerializeField]
	private PlayerMovement movement;



	[SerializeField] private Transform projectileStartUp;
	[SerializeField] private Transform projectileStartRight;
	[SerializeField] private Transform projectileStartDown;
	[SerializeField] private Transform projectileStartLeft;

	private List<PlayerWeapon> weapons = new List<PlayerWeapon>();
	private PlayerWeapon activeWeapon;
	private int activeWeaponIndex;

	public void AddWeapon(PlayerWeaponConfig pConfig)
	{
		PlayerWeapon weaponInInventory =
			weapons.Find(a => a.Id.Equals(pConfig.Id));
		if(weaponInInventory != null)
		{
			weaponInInventory.Add(pConfig);
		}
		else
		{
			weaponInInventory = new PlayerWeapon(pConfig);
			weapons.Add(weaponInInventory);
		}

		SetActiveWeapon(weapons.IndexOf(weaponInInventory));
	}

	public void SwapWeapon()
	{
		SetActiveWeapon(activeWeaponIndex + 1);
	}

	public void UseWeapon()
	{
		//Debug.Log("USE");
		activeWeapon.Use();
		game.ProjectileManager.SpawnProjectile(
			GetProjectileStartPosition(movement.CurrentDirection),
			movement,
			activeWeapon.Config.Projectile);
	}

	private Vector3 GetProjectileStartPosition(EDirection pDirection)
	{
		switch(pDirection)
		{
			case EDirection.Up:
				return projectileStartUp.position;
			case EDirection.Right:
				return projectileStartRight.position;
			case EDirection.Down:
				return projectileStartDown.position;
			case EDirection.Left:
				return projectileStartLeft.position;
		}
		Debug.LogError("Cant GetProjectileStartPosition");
		return projectileStartRight.position;
	}

	private void SetActiveWeapon(int pIndex)
	{
		pIndex = pIndex % weapons.Count;

		activeWeaponIndex = pIndex;
		activeWeapon = weapons[pIndex];

		visual.SetActiveWeapon(activeWeapon);
	}


}

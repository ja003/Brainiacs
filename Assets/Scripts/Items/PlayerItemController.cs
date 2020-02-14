using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : GameController
{
	[SerializeField]
	private PlayerStats stats;

	[SerializeField]
	private PlayerWeaponController weaponController;

	public void OnEnterWeapon(PlayerWeaponConfig pPlayerWeaponConfig)
	{
		weaponController.AddWeapon(pPlayerWeaponConfig);
	}

	protected override void OnGameActivated()
	{
	}

	protected override void OnGameAwaken()
	{
	}
}

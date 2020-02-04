using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : GameBehaviour
{
	[SerializeField]
	private PlayerVisual visual;

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
		Debug.Log("USE");
		activeWeapon.Use();
	}

	private void SetActiveWeapon(int pIndex)
	{
		pIndex = pIndex % weapons.Count;

		activeWeaponIndex = pIndex;
		activeWeapon = weapons[pIndex];

		visual.SetActiveWeapon(activeWeapon);
	}

	
}

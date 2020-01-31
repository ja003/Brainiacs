using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : GameBehaviour
{
	[SerializeField]
	private SpriteRenderer itemUp;
	[SerializeField]
	private SpriteRenderer itemRight;
	[SerializeField]
	private SpriteRenderer itemDown;
	[SerializeField]
	private SpriteRenderer itemLeft;



	[SerializeField]
	private SpriteRenderer handsUp;
	[SerializeField]
	private SpriteRenderer handsRight;
	[SerializeField]
	private SpriteRenderer handsDown;
	[SerializeField]
	private SpriteRenderer handsLeft;


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

		itemUp.sprite = activeWeapon.Config.PlayerSpriteUp;
		itemLeft.sprite = activeWeapon.Config.PlayerSpriteLeft;
		itemRight.sprite = activeWeapon.Config.PlayerSpriteRight;
		itemDown.sprite = activeWeapon.Config.playerSpriteDown;
	}

	public void OnChangeDirection(EDirection pDirection)
	{
		handsDown.enabled = false;
		handsRight.enabled = false;
		handsUp.enabled = false;
		handsLeft.enabled = false;

		itemRight.enabled = false;
		itemDown.enabled = false;
		itemLeft.enabled = false;
		itemUp.enabled = false;

		switch(pDirection)
		{
			case EDirection.Up:
				handsUp.enabled = true;
				itemUp.enabled = true;
				break;
			case EDirection.Right:
				itemRight.enabled = true;
				handsRight.enabled = true;
				break;
			case EDirection.Down:
				itemDown.enabled = true;
				handsDown.enabled = true;
				break;
			case EDirection.Left:
				handsLeft.enabled = true;
				itemLeft.enabled = true;
				break;
		}
	}
}

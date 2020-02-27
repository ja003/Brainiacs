using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSpecial : PlayerWeapon
{
	PlayerWeaponSpecialController specialController;

	public PlayerWeaponSpecial(Player pOwner, HeroSpecialWeaponConfig pConfig) : 
		base(pOwner, pConfig.Id, pConfig.InHandInfo, pConfig.InHandVisualInfo)
	{
		//controller = pController;

		PlayerWeaponSpecialController instance =
			Game.Instantiate(
				pConfig.SpecialWeaponInfo.ControllerPrefab, pOwner.WeaponController.transform);

		specialController = instance;
		specialController.Init(pOwner);
	}

	public override EWeaponUseResult Use()
	{
		EWeaponUseResult useResult = base.Use();
		if(useResult == EWeaponUseResult.OK)
			specialController.Use();

		return useResult;
	}

	public override void StopUse()
	{
		specialController.StopUse();
		base.StopUse();
	}

	public override void OnDirectionChange(EDirection pDirection)
	{
		base.OnDirectionChange(pDirection);
		specialController.OnDirectionChange(pDirection);
	}

	public override void OnSetActive()
	{
		base.OnSetActive();
		specialController.OnSetActive();
	}
}

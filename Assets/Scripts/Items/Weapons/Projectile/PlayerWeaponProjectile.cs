using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponProjectile : PlayerWeapon
{
	public ProjectileWeaponInfo ProjectileInfo;

	public PlayerWeaponProjectile(
		Player pOwner,
		MapWeaponConfig pConfig) : 
		base(pOwner, pConfig.Id, pConfig.InHandWeaponInfo,
			pConfig.VisualInfo)
	{
		ProjectileInfo = pConfig.ProjectileInfo;
	}

	public PlayerWeaponProjectile(
		Player pOwner,
		HeroBasicWeaponConfig pConfig) :
		base(pOwner, pConfig.Id,
			pConfig.InHandWeaponInfo,
			pConfig.VisualInfo)
	{
		ProjectileInfo = pConfig.ProjectileWeaponInfo;
	}

	public override EWeaponUseResult Use()
	{
		EWeaponUseResult useResult = base.Use();
		if(useResult != EWeaponUseResult.CantUse)
			Owner.WeaponController.ShootProjectile(ProjectileInfo);
		return useResult;
	}

}

using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialFlamethrowerNew : PlayerWeaponSpecial
{
	SpecialFlamethrowerFlame flame => 
		prefabInstance.GetComponent<SpecialFlamethrowerFlame>();

	public SpecialFlamethrowerNew(Player pOwner, MapSpecialWeaponConfig pConfig) : base(pOwner, pConfig)
	{
		//Debug.Log("SpecialFlamethrowerNew()");
		//flame = InstanceFactory.Instantiate(prefab.gameObject).GetComponent<SpecialFlamethrowerFlame>();
	}


	protected override void OnUse()
	{
		flame.Use();
	}

	public override void StopUse()
	{
		flame.StopUse();
	}

	public override void OnDirectionChange(EDirection pDirection)
	{
		flame.OnDirectionChange(pDirection);
	}
}

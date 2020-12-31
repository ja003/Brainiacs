using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialFlamethrower : PlayerWeaponSpecial
{
	SpecialFlamethrowerFlame flame => 
		prefabInstance.GetComponent<SpecialFlamethrowerFlame>();

	public SpecialFlamethrower(Player pOwner, MapSpecialWeaponConfig pConfig) : base(pOwner, pConfig)
	{
		//Debug.Log("SpecialFlamethrowerNew()");
		//flame = InstanceFactory.Instantiate(prefab.gameObject).GetComponent<SpecialFlamethrowerFlame>();
	}


	protected override void OnUse()
	{
		flame.Use();
		base.OnUse();
	}

	public override void StopUse()
	{
		flame.StopUse();
		base.StopUse();
	}

	float lastTimeChangeDirection;

	public override void OnDirectionChange(EDirection pDirection)
	{
		lastTimeChangeDirection = Time.time;
		flame.OnDirectionChange(pDirection);
	}

	public override void OnSetActive()
	{
		base.OnSetActive();
		//refresh direction
		//reason: when flamethrower reloads while holding another weapon and player switches
		//back to it, flame remembers only the last direction
		OnDirectionChange(Owner.Movement.CurrentEDirection);
	}

	/// <summary>
	/// When direction is changed, dont use flamethrower for short period.
	/// Looks weird.
	/// </summary>
	public override bool CanUse()
	{
		const float min_use_after_dir_change_delay = 0.1f;
		bool isEnoughTimeAfterDirectionChange;
		isEnoughTimeAfterDirectionChange = Time.time - min_use_after_dir_change_delay > lastTimeChangeDirection;

		return base.CanUse() && isEnoughTimeAfterDirectionChange;
	}
}

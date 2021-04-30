using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Picks weapon for AI based on weapon priorities and their usage.
/// </summary>
public class AiWeaponPicker 
{
	public EWeaponId PickedWeapon;

	private PlayerAiBrain brain;
	Player player;
	protected CDebug debug => CDebug.Instance;
	protected Brainiacs brainiacs => Brainiacs.Instance;

	EWeaponId myBasicWeapon;
	EWeaponId mySpecialWeapon;


	public AiWeaponPicker(PlayerAiBrain pBrain, Player pPlayer)
	{
		brain = pBrain;
		player = pPlayer;
		myBasicWeapon = brainiacs.ItemManager.GetHeroBasicWeaponConfig(player.InitInfo.Hero).Id;
		mySpecialWeapon = brainiacs.ItemManager.GetHeroSpecialWeaponConfig(player.InitInfo.Hero).Id;
		PickedWeapon = myBasicWeapon;
	}

	internal void Evaluate()
	{
		PickedWeapon = PickWeapon();

		if(brain.IsTmp && PickedWeapon == EWeaponId.Special_Tesla)
		{
			Debug.LogError("Tesla clone cant clone itself");
			return;
		}

		//swap to it
		if(PickedWeapon != player.WeaponController.ActiveWeapon.Id)
		{
			player.WeaponController.SetActiveWeapon(PickedWeapon);
			//isUseWeaponRequested = false;
			LastTimeSwapWeapon = Time.time;
		}
	}

	private EWeaponId PickWeapon()
	{
		//todo: limit weapon swap frequency

		//prefer using special weapon (always present in inventory)
		//if(player.WeaponController.GetWeapon(mySpecialWeapon).CanUse())
		//{
		//	return mySpecialWeapon;
		//}
		if(debug.AiWeapon != EWeaponId.None)
			return debug.AiWeapon;

		if(!CanSwapWeapon())
		{
			return player.WeaponController.ActiveWeapon.Id;
		}

		List<Tuple<EWeaponId, int>> weaponsPriority = GetWeaponsPriority();
		if(weaponsPriority.Count < 2)
		{
			//Debug.LogError("Cant have less than 2 weapons");
			//actually can, weapon might be reloading => pick basic
			return myBasicWeapon;
		}
		weaponsPriority.Sort((b, a) => a.Item2.CompareTo(b.Item2)); //sort descending

		foreach(var weaponPrio in weaponsPriority)
		{
			EWeaponId weapon = weaponPrio.Item1;
			const int minTimeToPickAnotherWeapon = 3;
			bool wasPickedRecently = GetTimeSinceWeaponPicked(weapon) < minTimeToPickAnotherWeapon;
			bool wasPickedInLongTime = GetTimeSinceWeaponPicked(weapon) > 2 * minTimeToPickAnotherWeapon;
			if(wasPickedRecently || wasPickedInLongTime)
			{
				if(PickedWeapon != weapon)
					UpdateLastTimeWeaponPicked(weapon);
				return weapon;
			}
			//Debug.Log("Skip weapon " + weapon);
		}
		//Debug.Log("All weapons have been used by AI recently => pick basic weapon");
		return weaponsPriority[0].Item1;
	}


	/// <summary>
	/// Calculates priority of each weapon (0 = cant use, 10 = max prio)
	/// </summary>
	internal List<Tuple<EWeaponId, int>> GetWeaponsPriority()
	{
		List<Tuple<EWeaponId, int>> weaponsPriority = new List<Tuple<EWeaponId, int>>();
		foreach(var weapon in player.WeaponController.weapons)
		{
			EWeaponId weaponId = weapon.Id;
			int priority = 0;
			if(weapon.CanUse())
			{
				//todo: implement special cases for individual weapons
				switch(brainiacs.ItemManager.GetWeaponCathegory(weaponId))
				{
					case EWeaponCathegory.HeroBasic:
						priority = 3;
						break;
					case EWeaponCathegory.HeroSpecial:
						//todo: da vinci => based on target distance + if is under fire
						priority = 10;
						break;
					case EWeaponCathegory.MapBasic:
						priority = 5;
						break;
					case EWeaponCathegory.MapSpecial:
						priority = 7;
						break;
				}
			}

			//prevent Tesla clone from cloning itself
			if(weaponId == EWeaponId.Special_Tesla && brain.IsTmp)
				continue;

			weaponsPriority.Add(new Tuple<EWeaponId, int>(weaponId, priority));
		}
		return weaponsPriority;
	}



	public float LastTimeSwapWeapon;

	private bool CanSwapWeapon()
	{
		const float min_weapon_swap_frequency = 0.5f;
		return Time.time - min_weapon_swap_frequency > LastTimeSwapWeapon;
	}


	private Dictionary<EWeaponId, float> lastTimeWeaponPicked = new Dictionary<EWeaponId, float>();


	private void UpdateLastTimeWeaponPicked(EWeaponId pWeapon)
	{
		//Debug.Log($"UpdateLastTimeWeaponPicked {pWeapon} | {Time.time}");

		if(lastTimeWeaponPicked.ContainsKey(pWeapon))
			lastTimeWeaponPicked[pWeapon] = Time.time;
		else
			lastTimeWeaponPicked.Add(pWeapon, Time.time);
	}

	private float GetTimeSinceWeaponPicked(EWeaponId pWeapon)
	{
		float lastTimePicked;
		bool wasPicked = lastTimeWeaponPicked.TryGetValue(pWeapon, out lastTimePicked);
		return wasPicked ? Time.time - lastTimePicked : int.MaxValue;
	}
}

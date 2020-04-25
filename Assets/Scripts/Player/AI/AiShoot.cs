using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiShoot : AiGoalController
{
	public Player targetedPlayer;

	Vector3 moveTarget;
	Vector3 idealMoveTarget;
	bool moveTargetReached;

	EWeaponId myBasicWeapon;
	EWeaponId mySpecialWeapon;

	public AiShoot(PlayerAiBrain pBrain, Player pPlayer) : base(pBrain, pPlayer)
	{
		myBasicWeapon = Brainiacs.Instance.ItemManager.GetHeroBasicWeaponConfig(player.InitInfo.Hero).Id;
		mySpecialWeapon = Brainiacs.Instance.ItemManager.GetHeroSpecialWeaponConfig(player.InitInfo.Hero).Id;
	}

	public override int GetPriority()
	{
		return targetedPlayer == null || DebugData.TestNonAggressiveAi ? 0 : 5;
	}

	bool isUseWeaponRequested;
	internal void Update()
	{
		if(isUseWeaponRequested)
			player.WeaponController.UseWeapon();
		else
			player.WeaponController.StopUseWeapon();
	}

	private Player GetPlayerToTarget(EWeaponId pPickeWeapon)
	{
		if(DebugData.TestNonAggressiveAi)
			return null;

		foreach(var otherPlayer in game.PlayerManager.Players)
		{
			if(otherPlayer.Equals(player) || otherPlayer.Stats.IsDead)
				continue;
			return otherPlayer;
		}

		//Debug.Log("No player to shoot");
		return null;
	}

	public void OnReachedTarget()
	{
		//Debug.Log("OnReachedTarget - UseWeapon");

		isUseWeaponRequested = targetedPlayer;

		//if(targetedPlayer)
		//{
		//	isUseWeaponRequested = true;
		//	//player.WeaponController.UseWeapon();
		//}

		moveTargetReached = true;
	}

	internal void OnDirectionChange(EDirection pDirection)
	{
		isUseWeaponRequested = false;
		player.WeaponController.StopUseWeapon();
	}

	private Vector3 GetMoveTarget()
	{
		return moveTargetReached ? idealMoveTarget : moveTarget;
	}

	public override Vector3 GetTarget()
	{
		return GetMoveTarget();
		////Player targetPlayer = GetPlayerToTarget();
		////if(targetPlayer == null)
		////    return player.transform.position;

		////Vector3 pos = GetShootPositionTo(targetPlayer.transform.position);
		////return pos;
	}

	//private Tuple<Player, Vector3> GetPlayerTarget()
	//{
	//    Player targetPlayer = GetPlayerToShoot();
	//    if(targetPlayer == null)
	//        return null;
	//    Vector3 pos = GetShootPositionTo(targetPlayer.transform.position);
	//    return new Tuple<Player, Vector3>(targetPlayer, pos);
	//}

	public override void Evaluate()
	{
		//pick weapon
		EWeaponId pickedWeapon = PickWeapon();

		//swap to it
		if(pickedWeapon != player.WeaponController.ActiveWeapon.Id)
		{
			player.WeaponController.SetActiveWeapon(pickedWeapon);
			isUseWeaponRequested = false;
		}


		//target a player
		targetedPlayer = GetPlayerToTarget(pickedWeapon);

		//set move target
		Tuple<Vector3, Vector3> usePositions = GetUseWeaponPosition(pickedWeapon);
		SetMoveTarget(usePositions);

		//moveTarget = targetedPlayer == null ? 
		//    playerPosition : //no targeted player => stay
		//    GetShootPositionTo(targetedPlayer.transform.position);
	}

	

	private void SetMoveTarget(Tuple<Vector3, Vector3> pTargetPositions)
	{
		if(Vector3.Distance(moveTarget, pTargetPositions.Item1) > 0.1f &&
			Vector3.Distance(idealMoveTarget, pTargetPositions.Item2) > 0.1f)
			moveTargetReached = false;

		moveTarget = pTargetPositions.Item1;
		idealMoveTarget = pTargetPositions.Item2;
	}

	/// <summary>
	/// Returns position from which the picked weapon will be used.
	/// For projectile weapons => distant position vertical/horizontal to targeted player
	/// Mine => power up or some interesting place
	/// ...
	/// </summary>
	private Tuple<Vector3, Vector3> GetUseWeaponPosition(EWeaponId pPickedWeapon)
	{
		switch(pPickedWeapon)
		{
			//can be used from anywhere
			case EWeaponId.Special_Einstein:
				return new Tuple<Vector3, Vector3>(playerPosition, playerPosition);


		}
		if(targetedPlayer == null)
			return new Tuple<Vector3, Vector3>(playerPosition, playerPosition);


		Tuple<Vector3, Vector3> shootPositions = GetShootPositionTo(targetedPlayer.transform.position, pPickedWeapon);

		//Vector3 shootPosition = targetedPlayer != null ?
		//	GetShootPositionTo(targetedPlayer.transform.position, pPickedWeapon) : playerPosition;

		Utils.DebugDrawCross(shootPositions.Item1, Color.red);
		Utils.DebugDrawCross(shootPositions.Item2, Color.blue);

		return shootPositions;
	}

	/// <summary>
	/// Calculate the best position from which I can shoot to the target using the weapon
	/// </summary>
	private Tuple<Vector3, Vector3> GetShootPositionTo(Vector3 pShootTarget, EWeaponId pWeapon)
	{
		float maxDist = GetWeaponMaxUseDistance(pWeapon);
		float idealDist = GetWeaponIdealUseDistance(pWeapon);

		Tuple<Vector3, Vector3> posHorizontal = GetShootPositions(pShootTarget, maxDist, idealDist, true);
		Tuple<Vector3, Vector3> posVertical = GetShootPositions(pShootTarget, maxDist, idealDist, false);

		if(Vector3.Distance(playerPosition, posHorizontal.Item1) < Vector3.Distance(playerPosition, posVertical.Item1))
		{
			return posHorizontal;
		}
		else
		{
			return posVertical;
		}
	}

	private float GetWeaponIdealUseDistance(EWeaponId pWeapon)
	{
		if(brainiacs.ItemManager.GetWeaponCathegory(pWeapon) == EWeaponCathegory.HeroBasic)
		{
			return 5;
		}

		switch(pWeapon)
		{
			case EWeaponId.Special_DaVinci:
				return 0;

			case EWeaponId.MP40:
				return 4;
			case EWeaponId.Flamethrower:
				return 2;
		}
		return 666;
	}

	private float GetWeaponMaxUseDistance(EWeaponId pWeapon)
	{
		if(brainiacs.ItemManager.GetWeaponCathegory(pWeapon) == EWeaponCathegory.HeroBasic)
		{
			return 10;
		}

		switch(pWeapon)
		{
			case EWeaponId.Special_DaVinci:
				return 1;

			case EWeaponId.MP40:
				return 8;
			case EWeaponId.Flamethrower:
				return 4;
		}
		return 666;
	}

	private Tuple<Vector3, Vector3> GetShootPositions(Vector3 pShootTarget, float pMaxDistance, float pIdealDistance, bool pHorizontal)
	{
		Vector3 closestPos = pHorizontal ?
			new Vector3(playerPosition.x, pShootTarget.y, 0) :
			new Vector3(pShootTarget.x, playerPosition.y, 0) ;
		//float targetDistance = Vector3.Distance(idealPos, pShootTarget);
		int iter = 0;
		while(Vector3.Distance(closestPos, pShootTarget) > pMaxDistance)
		{
			closestPos += (pShootTarget - closestPos).normalized * 0.1f;
			iter++;
			if(iter > 100)
			{
				Debug.LogError("Too many closestPos iterations");
				break;
			}
		}
		//todo: test collsisions, move positions



		Vector3 idealPos = closestPos;
		iter = 0;
		while(Vector3.Distance(idealPos, pShootTarget) > pIdealDistance)
		{
			idealPos += (pShootTarget - idealPos).normalized * 0.1f;
			iter++;
			if(iter > 100)
			{
				Debug.LogError("Too many idealPos iterations");
				break;
			}
		}

		return new Tuple<Vector3, Vector3>(closestPos, idealPos);
	}

	private EWeaponId PickWeapon()
	{
		//prefer using special weapon (always present in inventory)
		//if(player.WeaponController.GetWeapon(mySpecialWeapon).CanUse())
		//{
		//	return mySpecialWeapon;
		//}

		List<Tuple<EWeaponId, int>> weaponsPriority = GetWeaponsPriority();
		if(weaponsPriority.Count < 2)
		{
			Debug.LogError("Cant have less than 2 weapons");
			return myBasicWeapon;
		}
		weaponsPriority.Sort((b, a) => a.Item2.CompareTo(b.Item2)); //sort descending
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
						//da vinci => based on target distance + if is under fire
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

			weaponsPriority.Add(new Tuple<EWeaponId, int>(weaponId, priority));
		}
		return weaponsPriority;
	}

	

	
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiShoot : AiGoalController
{
	//if a target is further than this it is considered invalid 
	//(happens when player dies or is teleported)
	private const int INVALID_TARGET_DISTANCE = 100;

	public Player targetedPlayer;

	Vector2 moveTarget;
	Vector2 idealMoveTarget;
	bool moveTargetReached;

	EWeaponId myBasicWeapon;
	EWeaponId mySpecialWeapon;

	private Tuple<Vector2, Vector2> playerPositions => new Tuple<Vector2, Vector2>(playerPosition, playerPosition);

	public AiShoot(PlayerAiBrain pBrain, Player pPlayer) : base(pBrain, pPlayer, EAiGoal.Shoot)
	{
		myBasicWeapon = Brainiacs.Instance.ItemManager.GetHeroBasicWeaponConfig(player.InitInfo.Hero).Id;
		mySpecialWeapon = Brainiacs.Instance.ItemManager.GetHeroSpecialWeaponConfig(player.InitInfo.Hero).Id;
	}

	static bool debug_log = false;

	public override int GetPriority()
	{
		//picked weapon cant be used (is reloading, ...)
		if(!player.WeaponController.ActiveWeapon.CanUse()
			//no target
			|| targetedPlayer == null
			//target is shielded
			|| (targetedPlayer != null && targetedPlayer.Stats.IsShielded)
			|| debug.NonAggressiveAi)
		{
			return 0;
		}
		float distToTarget = Vector2.Distance(playerPosition, targetedPlayer.Position);
		const int max_prio = 10;
		const int min_measure_dist = 3;
		const int max_measure_dist = 10;
		const int dist_reduce_coef = 5;
		float coef = max_prio - dist_reduce_coef * (distToTarget - min_measure_dist) / max_measure_dist;
		int prio = (int)Mathf.Clamp(coef, 5, 10);

		if(debug_log)
			Debug.Log($"Shoot prio: dist = {distToTarget:0.0} => {prio}");

		return prio;
		//return targetedPlayer == null || debug.NonAggressiveAi ? 0 : 5;
	}

	public override Vector2 GetTarget()
	{
		return GetMoveTarget();
	}

	private float lastTimeSwapWeapon;

	EWeaponId pickedWeapon;

	public override void Evaluate()
	{
		//pick weapon
		pickedWeapon = PickWeapon();

		if(brain.IsTmp && pickedWeapon == EWeaponId.Special_Tesla)
		{
			Debug.LogError("Tesla clone cant clone itself");
			return;
		}

		//swap to it
		if(pickedWeapon != player.WeaponController.ActiveWeapon.Id)
		{
			player.WeaponController.SetActiveWeapon(pickedWeapon);
			isUseWeaponRequested = false;
			lastTimeSwapWeapon = Time.time;
		}


		//target a player
		targetedPlayer = GetPlayerToTarget(pickedWeapon);

		//set move target
		Tuple<Vector2, Vector2> usePositions = GetUseWeaponPosition(pickedWeapon);
		SetMoveTarget(usePositions);
	}



	//flag that weapon should be used
	// - handled in Update 
	bool isUseWeaponRequested;


	internal void Update()
	{
		if(brain.CurrentGoal != this)
		{
			//Debug.Log($"No shoot. {brain.CurrentGoal}");
			player.WeaponController.StopUseWeapon();
			return;
		}
		EvaluateWeaponUsage();

		if(isUseWeaponRequested)
			player.WeaponController.UseWeapon();
		else
			player.WeaponController.StopUseWeapon();
	}

	private Player GetPlayerToTarget(EWeaponId pPickeWeapon)
	{
		if(debug.NonAggressiveAi)
			return null;

		Player closestPlayer = game.PlayerManager.GetClosestPlayerTo(player);
		return closestPlayer;
		//return closestPlayer.Stats.IsShielded ? closestPlayer : null;
	}

	/// <summary>
	/// Called when player reaches set moveTarget
	/// </summary>
	public void OnReachedTarget()
	{
		//turn to targeted player (required to shoot)
		if(targetedPlayer)
			player.Movement.SetDirection(targetedPlayer.Position - player.Position);
		//Debug.Log("OnReachedTarget");
		EvaluateWeaponUsage();

		moveTargetReached = true;
	}

	/// <summary>
	/// Called when player reaches the path node that is at straight sub-path
	/// to the path goal.
	/// Idea: weapon might be used already here => recalculate
	/// </summary>
	public void OnReachedStraightPathToTargetNode()
	{
		//Debug.Log("OnReachedStraightPathToTargetNode");
		EvaluateWeaponUsage();
	}

	private void EvaluateWeaponUsage()
	{
		//if player is stil targeted, request weapon usage
		//but only if he is not shielded
		isUseWeaponRequested = targetedPlayer != null && !targetedPlayer.Stats.IsShielded;
		if(!isUseWeaponRequested)
			return;

		//dont use weapon if 
		// - targeted player is dead or too far
		float distanceToTarget = Vector2.Distance(playerPosition, targetedPlayer.Position);
		float maxUseDistance = GetWeaponMaxUseDistance(player.WeaponController.ActiveWeapon.Id);
		// - or is looking in wrong direction
		bool isLookingAtTarget = IsLookingAtTarget();
		if(targetedPlayer.Visual.IsDying
			|| distanceToTarget > INVALID_TARGET_DISTANCE
			|| distanceToTarget > maxUseDistance
			|| !isLookingAtTarget)
		{
			isUseWeaponRequested = false;
		}
		else //do Raycast only if other checks dont prevent weapon usage (performance)
		{
			//dont use weapon if - there is a map object between player and target
			Vector2 dirToTarget = targetedPlayer.Position - playerPosition;
			bool hitMapObject = Physics2D.Raycast(
				playerPosition, dirToTarget, dirToTarget.magnitude, 
				GetDontHitLayer(player.WeaponController.ActiveWeapon.Id));
			if(hitMapObject)
				isUseWeaponRequested = false;
		}
	}

	private LayerMask GetDontHitLayer(EWeaponId pWeapon)
	{
		LayerMask dontHitLayer = game.Layers.MapObject;
		if(pWeapon == EWeaponId.Special_Curie)
			dontHitLayer = dontHitLayer | game.Layers.Unwalkable;
		return dontHitLayer;
	}

	private bool IsLookingAtTarget()
	{
		switch(pickedWeapon)
		{
			//these weapons dont require player to be looking at the target
			case EWeaponId.Special_Einstein:
			case EWeaponId.Special_Nobel:
			case EWeaponId.Mine:
				return true;
		}

		Vector2 dirToTarget = targetedPlayer.Position - player.Position;
		EDirection dir = Utils.GetDirection(dirToTarget);
		return player.Movement.CurrentEDirection == dir;
	}


	internal void OnDirectionChange(EDirection pDirection)
	{
		//Debug.Log("OnDirectionChange  + pDirection");
		isUseWeaponRequested = false;
		player.WeaponController.StopUseWeapon();
	}

	private Vector2 GetMoveTarget()
	{
		if(!moveTargetReached)
		{
			moveTargetReached = brain.Movement.IsCloseTo(moveTarget);
		}

		return moveTargetReached ? idealMoveTarget : moveTarget;
	}




	private void SetMoveTarget(Tuple<Vector2, Vector2> pTargetPositions)
	{
		//Debug.Log("SetMoveTarget " + pTargetPositions);
		if(Vector2.Distance(moveTarget, pTargetPositions.Item1) > 0.1f &&
			Vector2.Distance(idealMoveTarget, pTargetPositions.Item2) > 0.1f)
			moveTargetReached = false;

		moveTarget = pTargetPositions.Item1;
		idealMoveTarget = pTargetPositions.Item2;
		//isUseWeaponRequested = false;
	}

	/// <summary>
	/// Returns position from which the picked weapon will be used.
	/// For projectile weapons => distant position vertical/horizontal to targeted player
	/// Mine => power up or some interesting place
	/// ...
	/// </summary>
	private Tuple<Vector2, Vector2> GetUseWeaponPosition(EWeaponId pPickedWeapon)
	{
		switch(pPickedWeapon)
		{
			//can be used from anywhere
			case EWeaponId.Special_Einstein:
				return playerPositions;
		}
		if(targetedPlayer == null)
			return playerPositions;

		if(Vector2.Distance(playerPosition, targetedPlayer.Position) > INVALID_TARGET_DISTANCE)
		{
			Debug.Log("Target is too far - either teleporting or dead");
			return playerPositions;
		}

		//todo: maybe dont recalculate when targetedPlayer.Position doesnt change but
		//that is expected to happen quite fast
		Tuple<Vector2, Vector2> shootPositions
			= GetShootPositionTo(targetedPlayer.Position, pPickedWeapon);
			//= new Tuple<Vector2, Vector2>(Vector2.zero, Vector2.zero);

		Utils.DebugDrawCross(shootPositions.Item1, Color.red);
		Utils.DebugDrawCross(shootPositions.Item2, Color.blue);

		//Debug.Log($"shootPositions = {shootPositions.Item1} | {shootPositions.Item1}");

		return shootPositions;
	}

	/// <summary>
	/// Calculate the best position from which I can shoot to the target using the weapon
	/// First = position closest to the player
	/// Second = position closer to the target
	/// </summary>
	private Tuple<Vector2, Vector2> GetShootPositionTo(Vector2 pShootTarget, EWeaponId pWeapon)
	{
		float maxDist = GetWeaponMaxUseDistance(pWeapon);
		float idealDist = GetWeaponIdealUseDistance(pWeapon);
		//ideal distance shouldnt be bigger. (probably not configured)
		if(idealDist > maxDist)
			idealDist = maxDist;

		Tuple<Vector2, Vector2> posHorizontal = 
			GetShootPositions(pShootTarget, maxDist, idealDist, true, 
			GetDontHitLayer(pWeapon));
		Tuple<Vector2, Vector2> posVertical = 
			GetShootPositions(pShootTarget, maxDist, idealDist, false,
			GetDontHitLayer(pWeapon));

		//TESTING - Select the closest shoot position 
		//- simple distance compare - select the closest
		bool useSimpleDistCompare = true;
		if(useSimpleDistCompare)
		{
			if(Vector2.Distance(playerPosition, posHorizontal.Item1) < Vector2.Distance(playerPosition, posVertical.Item1))
			{
				return posHorizontal;
			}
			else
			{
				return posVertical;
			}
		}

		//- calculate path and compare their lengths
		//-- seems very costly, player lags
		MovePath pathHorizontal = pathFinder.GetPath(playerPosition, posHorizontal.Item1);//, AiMovement.PATH_STEP);
		MovePath pathVertical = pathFinder.GetPath(playerPosition, posVertical.Item1);//, AiMovement.PATH_STEP);

		if(pathHorizontal.GetLength() < pathVertical.GetLength())
		{
			return posHorizontal;
		}
		else
		{
			return posVertical;
		}


	}

	/// <summary>
	/// Returns 2 positions from which a player should hit the target.
	/// First = position closest to the player.
	/// Second = position closer to the target.
	/// pDontHitLayer = layers that cant be hit during raycast from shoot target to the shoot position
	/// </summary>
	private Tuple<Vector2, Vector2> GetShootPositions(Vector2 pShootTarget, float pMaxDistance, float pIdealDistance, bool pHorizontal, LayerMask pDontHitLayer)
	{
		if(Vector2.Distance(playerPosition, pShootTarget) > INVALID_TARGET_DISTANCE)
		{
			Debug.Log("Target is too far - either teleporting or dead");
			return playerPositions;
		}

		const float pos_step = Player.COLLIDER_SIZE;

		Vector2 closestPos = pHorizontal ?
			new Vector2(playerPosition.x, pShootTarget.y) :
			new Vector2(pShootTarget.x, playerPosition.y);
		//float targetDistance = Vector2.Distance(idealPos, pShootTarget);

		Vector2 dir = closestPos - pShootTarget;
		float maxDist = Mathf.Min(dir.magnitude, pMaxDistance);
		RaycastHit2D hit = Physics2D.Raycast(pShootTarget, dir, maxDist, pDontHitLayer);

		Vector2 shootPos = closestPos;
		if(hit)
		{
			shootPos = hit.point;
			shootPos -= dir.normalized * pos_step;
			//Debug.Log("HIT " + shootPos);
			Utils.DebugDrawCross(shootPos, Color.magenta);
			Debug.DrawLine(pShootTarget, shootPos, Color.red, 1);
		}


		//int iter = 0;
		//while(Vector2.Distance(closestPos, pShootTarget) > pMaxDistance)
		//{
		//	closestPos += (pShootTarget - closestPos).normalized * pos_step;
		//	iter++;
		//	if(iter > 100)
		//	{
		//		Debug.LogError("Too many closestPos iterations");
		//		break;
		//	}
		//}

		Vector2 idealPos = shootPos;
		int iter = 0;
		while(Vector2.Distance(idealPos, pShootTarget) > pIdealDistance)
		{
			idealPos += (pShootTarget - idealPos).normalized * pos_step;
			iter++;
			if(iter > 100)
			{
				Debug.LogError("Too many idealPos iterations");
				break;
			}
		}
		//Debug.Log("idealPos = " + idealPos);

		return new Tuple<Vector2, Vector2>(shootPos, idealPos);
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
			//hero special
			case EWeaponId.Special_DaVinci:
				return 2;
			case EWeaponId.Special_Nobel:
			case EWeaponId.Mine:
				return 15;

			//special
			case EWeaponId.Flamethrower:
				return 3;

			//projectile
			case EWeaponId.MP40:
				return 8;
			
		}
		return 10;
	}

	private bool CanSwapWeapon()
	{
		const float min_weapon_swap_frequency = 0.5f;
		return Time.time - min_weapon_swap_frequency > lastTimeSwapWeapon;
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




}

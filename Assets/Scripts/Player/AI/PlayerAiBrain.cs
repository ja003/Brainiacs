using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAiBrain : PlayerBehaviour
{
	bool isInited;

	//Tesla clone is temporary
	public bool IsTmp { get; internal set; }

	//currently active goal
	public AiGoalController CurrentGoal { get; private set; }

	public AiMovement Movement;
	public AiShoot Shoot;
	public AiEvade Evade;
	public AiMapItem Item;
	public new AiDebug debug;

	//[SerializeField]
	//[Range(0.1f, 1)]
	float evaluation_frequency = .2f;

	public int CloneCounter;

	//owner of this AI (eg. of Tesla clone)
	public Player Owner;

	public void Init()
	{
		Movement = new AiMovement(this, player);
		Shoot = new AiShoot(this, player);
		Evade = new AiEvade(this, player);
		Item = new AiMapItem(this, player);
		debug = new AiDebug(this, player);

		game.PlayerManager.OnAllPlayersAdded.AddAction(() => DoInTime(InvokeEvaluateGoals, 1));
		isInited = true;
	}

	private void Update()
	{
		if(!isInited)
			return;

		if(base.debug.PassiveAi)
			return;

		Movement.Update();
		Shoot.Update();
		Item.Update();
		//debug.Update();
	}

	//no evaluation is done before this time
	float timeBrainEnabled;

	public void StopBrain(float pDuration)
	{
		timeBrainEnabled = Time.time + pDuration;
	}

	private void InvokeEvaluateGoals()
	{
		DoInTime(InvokeEvaluateGoals, evaluation_frequency);

		//repeat the invoke but evaluate only if clone is alive
		if(player.Stats.IsDead)
			return;

		//Debug.Log("InvokeEvaluateGoals");
		EvaluateGoals();

	}

	float lastEvaluateTime;


	public void EvaluateGoals()
	{
		//Debug.Log($"{player} EvaluateGoals");

		//EvaluateGoals can be called from other places.
		//prevent too frequent calls
		const float maxEvaluationFrequency = 0.1f;
		bool isEvaluationTooFrequent = Time.time < lastEvaluateTime + maxEvaluationFrequency;
		bool isBrainDisabled = Time.time < timeBrainEnabled;
		if(isEvaluationTooFrequent || isBrainDisabled)
		{
			//Debug.Log("Ignore evaluation");
			return;
		}

		lastEvaluateTime = Time.time;

		List<Tuple<int, EAiGoal>> priorityOfGoals = new List<Tuple<int, EAiGoal>>();

		//evaluate all controllers
		Shoot.Evaluate();
		Evade.Evaluate();
		Item.Evaluate();
		debug.Evaluate();

		//get priority of controllers
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(1, EAiGoal.None));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(Shoot.GetPriority(), EAiGoal.Shoot));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(Evade.GetPriority(), EAiGoal.Evade));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(Item.GetPriority(), EAiGoal.PickupItem));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(debug.GetPriority(), EAiGoal.Debug));

		//pick the highest priority controller
		priorityOfGoals.Sort((b, a) => a.Item1.CompareTo(b.Item1)); //sort descending

		Movement.TargetedPlayer = null;
		CurrentGoal = null;

		//Debug.Log("EvaluateGoals: " + priorityOfGoals[0].Item2);

		//apply
		switch(priorityOfGoals[0].Item2)
		{
			case EAiGoal.None:
				break;
			case EAiGoal.Shoot:

				CurrentGoal = Shoot;
				Movement.TargetedPlayer = Shoot.targetedPlayer;
				//aiMovement.SetTarget(shoot.moveTarget);
				Movement.SetTarget(Shoot.GetTarget());

				//var target = shoot.GetPlayerTarget();
				////Vector2 target = shoot.GetTarget();
				//if(target != null)
				//{
				//	aiMovement.TargetedPlayer = target.Item1;
				//	aiMovement.SetTarget(target.Item2);
				//}
				break;
			case EAiGoal.Evade:
				CurrentGoal = Evade;
				Movement.SetTarget(Evade.GetTarget());
				break;
			case EAiGoal.PickupItem:
				CurrentGoal = Item;
				Movement.SetTarget(Item.GetTarget());
				break;

			case EAiGoal.Debug:
				CurrentGoal = debug;
				Movement.SetTarget(debug.GetTarget());
				break;
		}
	}

	internal void OnDirectionChange(EDirection pDirection)
	{
		if(!isInited)
			return;

		Shoot.OnDirectionChange(pDirection);
	}

	//private void Update()
	//{
	//    if(!isInited)
	//        return;
	//}
}


public enum EAiGoal
{
	None,
	Shoot,
	Evade,
	PickupItem,
	Debug
}
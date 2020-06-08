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

	AiMovement aiMovement;
	public AiShoot shoot;
	public AiEvade evade;
	public AiMapItem item;
	public AiDebug debug;

	//[SerializeField]
	//[Range(0.1f, 1)]
	float evaluation_frequency = .2f;


	public void Init()
	{
		aiMovement = new AiMovement(this, player);
		shoot = new AiShoot(this, player);
		evade = new AiEvade(this, player);
		item = new AiMapItem(this, player);
		debug = new AiDebug(this, player);

		game.PlayerManager.OnAllPlayersAdded.AddAction(() => DoInTime(InvokeEvaluateGoals, 1));
		isInited = true;
	}

	private void Update()
	{
		if(!isInited)
			return;

		aiMovement.Update();
		shoot.Update();
		item.Update();
		//debug.Update();
	}

	private void InvokeEvaluateGoals()
	{
		EvaluateGoals();

		DoInTime(InvokeEvaluateGoals, evaluation_frequency);
	}

	float lastEvaluateTime;


	public void EvaluateGoals()
	{
		//EvaluateGoals can be called from other places.
		//prevent too frequent calls
		const float maxEvaluationFrequency = 0.1f;
		if(Time.time < lastEvaluateTime + maxEvaluationFrequency)
		{
			//Debug.Log("Ignore evaluation");
			return;
		}

		lastEvaluateTime = Time.time;

		List<Tuple<int, EAiGoal>> priorityOfGoals = new List<Tuple<int, EAiGoal>>();

		//evaluate all controllers
		shoot.Evaluate();
		evade.Evaluate();
		item.Evaluate();
		debug.Evaluate();

		//get priority of controllers
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(1, EAiGoal.None));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(shoot.GetPriority(), EAiGoal.Shoot));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(evade.GetPriority(), EAiGoal.Evade));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(item.GetPriority(), EAiGoal.PickupItem));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(debug.GetPriority(), EAiGoal.Debug));

		//pick the highest priority controller
		priorityOfGoals.Sort((b, a) => a.Item1.CompareTo(b.Item1)); //sort descending

		aiMovement.TargetedPlayer = null;
		CurrentGoal = null;

		//Debug.Log("EvaluateGoals: " + priorityOfGoals[0].Item2);

		//apply
		switch(priorityOfGoals[0].Item2)
		{
			case EAiGoal.None:
				break;
			case EAiGoal.Shoot:

				CurrentGoal = shoot;
				aiMovement.TargetedPlayer = shoot.targetedPlayer;
				//aiMovement.SetTarget(shoot.moveTarget);
				aiMovement.SetTarget(shoot.GetTarget());

				//var target = shoot.GetPlayerTarget();
				////Vector2 target = shoot.GetTarget();
				//if(target != null)
				//{
				//	aiMovement.TargetedPlayer = target.Item1;
				//	aiMovement.SetTarget(target.Item2);
				//}
				break;
			case EAiGoal.Evade:
				CurrentGoal = evade;
				aiMovement.SetTarget(evade.GetTarget());
				break;
			case EAiGoal.PickupItem:
				CurrentGoal = item;
				aiMovement.SetTarget(item.GetTarget());
				break;

			case EAiGoal.Debug:
				CurrentGoal = debug;
				aiMovement.SetTarget(debug.GetTarget());
				break;
		}
	}

	internal void OnDirectionChange(EDirection pDirection)
	{
		if(!isInited)
			return;

		shoot.OnDirectionChange(pDirection);
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
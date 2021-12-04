using System;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class PlayerAiBTController : PlayerBehaviour
{
	[SerializeField] BehaviourTree bt;

	public bool isInited;
	public bool IsActive => isInited && Time.time > updateAllowedTime;

	public AiMovement Movement;
	public AiShoot Shoot;
	public AiEvade Evade;
	public AiMapItem Item;
	public new AiDebug debug;

	//owner of this AI (eg. of Tesla clone - the player who summoned it)
	public Player Owner;
	public int debug_CloneCounter;


	internal void Init()
	{
		bt = bt.Clone();

		Movement = new AiMovement(player);
		Shoot = new AiShoot(player);
		Evade = new AiEvade(player);
		Item = new AiMapItem(player);
		debug = new AiDebug(player);

		//bt.Bind(Context.CreateFromGameObject(gameObject));
		Context context = Context.Create(this);
		bt.nodes.ForEach(a => a.context = context);
		bt.nodes.ForEach(a => a.blackboard = bt.blackboard);

		isInited = true;
	}

	private void Update()
	{
		if(!isInited || !player.IsSpawned || player.Stats.IsDead)
			return;

		//todo: IsInited for all controllers?
		if(!Movement.IsInited)
			return;

		if(!IsActive)
			return;


		bt.Update();
	}

	float updateAllowedTime;


	public void StopUpdateFor(float pSeconds)
	{
		updateAllowedTime = Time.time + pSeconds;
		Movement.Stop();
	}
}

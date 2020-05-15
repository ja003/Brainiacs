using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : PlayerBehaviour, ITeleportable
{
	[SerializeField] public Collider2D PlayerCollider = null;

	private const float MOVE_SPEED_BASE = 0.05f;

	public EDirection CurrentDirection { get; private set; } = EDirection.Right;
	//move is requested - move key is pressed
	public bool IsMoving { get; private set; }
	//did player physically moved since last update
	// - move might be requested, but there might be obstackle in the way
	private bool isActualyMoving;

	const float SYNC_POS_INTERVAL = 0.1f;
	float lastTimeSync;

	Vector3 lastPosition;

	private void FixedUpdate()
	{
		//dont update if it is not me,player is dead or is AI
		if(!player.IsInitedAndMe || player.Stats.IsDead)
		{
			if(player.InitInfo != null && player.InitInfo.PlayerType == EPlayerType.AI)
			{
				//Debug.Log("skip Move");igodBody
			}
			return;
		}


		//sync position
		if(lastTimeSync + SYNC_POS_INTERVAL / 2 < Time.time)
		{
			SyncPosition(false);
		}

		lastPosition = transform.position; //has to be called before ApplyMove

		//apply movement
		ApplyMove(CurrentDirection);

	}

	private void SyncPosition(bool pInstantly)
	{
		if(player.IsLocalImage)
			return;

		isActualyMoving = Vector3.Distance(lastPosition, transform.position) > MOVE_SPEED_BASE / 2;
		//Debug.Log(isActualyMoving);

		lastTimeSync = Time.time;
		Vector3 position = transform.position;
		if(player.LocalImage)
			position += Vector3.down;

		player.Photon.Send(EPhotonMsg.Player_SetSyncPosition, position,
			CurrentDirection, isActualyMoving, player.Stats.Speed, pInstantly);
	}

	internal void SpawnAt(Vector3 pPosition)
	{
		transform.position = pPosition;
		//Move(EDirection.Right); //set init direction
		player.Visual.OnSpawn();

		SetMove(CurrentDirection); //refresh visual
		SetMove(EDirection.None); //stop
	}

	public void Stop()
	{
		IsMoving = false;
		player.Visual.Idle();
	}

	/// <summary>
	/// Move player object and set direction
	/// </summary>
	public void SetMove(EDirection pDirection)
	{
		if(pDirection == EDirection.None)
		{
			if(player.InitInfo.PlayerType == EPlayerType.AI)
			{
				//Debug.Log("Idle");
			}

			Stop();
			return;
		}

		IsMoving = true;
		SetDirection(pDirection);

		player.LocalImage?.Movement.SetMove(pDirection);
	}

	/// <summary>
	/// Set direction and update visual
	/// </summary>
	public void SetDirection(EDirection pDirection)
	{
		//sdDebug.Log(gameObject.name + " SetDirection " + pDirection);
		bool isDirectionChanged = CurrentDirection != pDirection;
		CurrentDirection = pDirection; //has to be set before OnDirectionChange call
		if(isDirectionChanged)
		{
			visual.OnDirectionChange(pDirection);
			weapon.OnDirectionChange(pDirection);
			aiBrain.OnDirectionChange(pDirection);
		}
	}

	/// <summary>
	/// Moves player in given direction but only if move is requested
	/// </summary>
	private void ApplyMove(EDirection pDirection)
	{
		if(!IsMoving)
			return;

		if(pDirection == EDirection.None)
		{
			//player.Visual.Idle();
			return;
		}

		transform.position += Utils.GetVector3(pDirection) *
			MOVE_SPEED_BASE * player.Stats.Speed;

		player.Visual.Move();

		//player.LocalImage?.Movement.Move(pDirection);
	}

	//Debug try leanTween cancel
	//int id = -1;
	//[SerializeField] GameObject testGo;
	//private void Update()
	//{
	//	//debug_useEaseIn = Input.GetKey(KeyCode.Q);
	//	if(Input.GetKeyDown(KeyCode.Q))
	//	{
	//		//stats.Speed = 10;
	//		//DoInTime(() => stats.Speed = 1, 1);

	//		id = LeanTween.moveLocalX(testGo, testGo.transform.localPosition.x + 1, 1).id;
	//		Debug.Log("moveLocalX");
	//	}

	//	if(Input.GetKeyDown(KeyCode.R))
	//	{
	//		//LeanTween.cancel(stats.gameObject);
	//		LeanTween.cancel(id);
	//		//LeanTween.cancel(gameObject);
	//		Debug.Log("cancel");
	//	}
	//}


	float actualMovementDuration;
	float lastSyncPosTime;
	EDirection lastSyncedDirection; //for direction change detection

	public float debug_moveMulti = 1.3f;

	//the longer player moves the higher move multiply
	// - 0s - 1s => 1f - 1.5f
	// - 1s - 5s => 1.5f - 2f
	//idea: when player starts to move, image doesnt have to follow that tightly => smaller multiply.
	// if player moves for a long time in the same direction, image expects the direction to remain.
	// multiply is reset to 1 on direction change
	[SerializeField] AnimationCurve moveSyncMultiply = null;

	int moveFunctionId;

	/// <summary>
	/// Called only on image.
	/// Calculates target position based on the owner position and his current direction.
	/// If the owner is moving => target position will be moved by given direction.
	/// Smoothly moves player towards the calculated position.
	/// When pInstantly is passed, image is relocated to target position instantly (eg. Teleport)
	/// </summary>
	public void SetSyncPosition(Vector3 pPosition, EDirection pDirection, 
		bool pIsActualyMoving, float pSpeed, bool pInstantly)
	{
		if(player.IsItMe)
		{
			Debug.LogError("SetSyncPosition called on owner");
			return;
		}
		if(player.Health.IsDying)
			return;


		if(!gameObject.activeSelf)
		{
			//Debug.LogError("");
		}

		float moveCalls = SYNC_POS_INTERVAL / Time.fixedDeltaTime;
		Vector3 targetPos = pPosition;
		LeanTween.cancel(moveFunctionId);

		//when target position is way too far, assign position instantly
		// - this happens for example during respawn
		if(Vector3.Distance(targetPos, transform.position) > 10 || pInstantly)
		{
			Debug.Log(gameObject.name + " insta port " + pInstantly);
			transform.position = targetPos;
			return;
		}

		if(pIsActualyMoving)
		{
			actualMovementDuration += Time.time - lastSyncPosTime;

			float moveMultiply = moveSyncMultiply.Evaluate(actualMovementDuration);
			if(pDirection != lastSyncedDirection)
			{
				actualMovementDuration = 0;
				moveMultiply = 1;
			}

			if(moveMultiply < 0 || moveMultiply > 2)
			{
				Debug.LogError("Invalid move multiply");
				moveMultiply = 1;
			}
			//Debug.Log(moveMultiply);

			//Debug.Log(isMoveStarting);
			//todo: send speed info?
			targetPos += moveMultiply * moveCalls * Utils.GetVector3(pDirection) *
				MOVE_SPEED_BASE * pSpeed;
		}
		else
		{
			//SetDirection(pDirection); //should be redundant
			actualMovementDuration = 0;
		}
		lastSyncPosTime = Time.time;
		lastSyncedDirection = pDirection;

		//Debug.Log(pIsActualyMoving);

		float xDiffAbs = Mathf.Abs(Mathf.Abs(targetPos.x - transform.position.x));
		float yDiffAbs = Mathf.Abs(Mathf.Abs(targetPos.y - transform.position.y));
		float totalDiff = xDiffAbs + yDiffAbs;
		float xPercentage = xDiffAbs / totalDiff; //0-1
		float yPercentage = yDiffAbs / totalDiff; //0-1

		if(totalDiff < Mathf.Epsilon)
			return;

		//Utils.DebugDrawCross(targetPos, Color.green, SYNC_POS_INTERVAL);

		//LeanTween.cancel(moveFunctionId);
		const bool debug_useEaseIn = false;
		if(debug_useEaseIn)
		{
			//first move by X-axis in time proportional to x-diff from total dif		
			moveFunctionId = LeanTween.moveX(gameObject, targetPos.x, SYNC_POS_INTERVAL * xPercentage)
				.setEaseOutSine() //not much different than no ease
				.setOnComplete(() => //then by Y-axis
					LeanTween.moveY(gameObject, targetPos.y, SYNC_POS_INTERVAL * yPercentage)
						.setEaseOutSine()).id;
			//todo: image is either too far behind or jumps too much at small move
			// - find better ease function
			// - rework logic
			// - but for now its kinda OK
		}
		else //seems better without ease
		{
			moveFunctionId = LeanTween.moveX(gameObject, targetPos.x, SYNC_POS_INTERVAL * xPercentage)
				.setOnComplete(() => 
					LeanTween.moveY(gameObject, targetPos.y, SYNC_POS_INTERVAL * yPercentage)).id;
		}
	}

	/// TELEPORT

	public ITeleportable TeleportTo(Teleport pTeleport)
	{
		//simulated on owner side
		if(!player.IsItMe)
			return null;

		transform.position = Vector3.one * 666;
		player.SetActive(false);

		//small teleport delay
		DoInTime(() =>
		{
			transform.position = pTeleport.OutPosition.position;
			player.SetActive(true);
			SetDirection(pTeleport.OutDirection);
			SyncPosition(true);
		}, 0.5f);

		return this;
		//transform.position = pTeleport.OutPosition.position;
		//SetDirection(pTeleport.OutDirection);
		//SyncPosition(true);
	}

}

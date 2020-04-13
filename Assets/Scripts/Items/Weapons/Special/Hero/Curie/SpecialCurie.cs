using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class SpecialCurie : PlayerWeaponSpecialController
{
	EDirection direction;
	[SerializeField] ProjectileConfig projectile = null;
	[SerializeField] [Range(10, 100)]float speed = -1;
	[SerializeField] [Range(0.01f, 1)] float cadency = 0.1f;

	[SerializeField] BoxCollider2D colliderUpDown = null;
	[SerializeField] BoxCollider2D colliderRightLeft = null;

	[SerializeField] Transform projectileSpawnUp = null;
	[SerializeField] Transform projectileSpawnRight = null;
	[SerializeField] Transform projectileSpawnDown = null;
	[SerializeField] Transform projectileSpawnLeft = null;

	protected override void Awake()
	{
		//projectile has to be registered so we know its config.
		//it is the same as Curie basic (but can be changed in P_Projectile animator and config)
		brainiacs.ItemManager.AddProjectile(EWeaponId.Special_Curie, projectile);

		//if(!Network.IsMine)
		//{
		//	Player owner = game.PlayerManager.GetPlayer(Network.PhotonController);
		//	Init(owner);
		//}
		gameObject.SetActive(false);

		base.Awake();

	}
	protected override void OnInit()
	{
		gameObject.SetActive(false);

	}

	private void OnEnable()
	{
		if(!Owner)
			return;
		//Debug.Log("OnEnable ");
		
		Physics2D.IgnoreCollision(boxCollider2D, Owner.Collider);
	}

	bool canCollide;

	private const string AC_KEY_DIRECTION = "direction";
	private const string AC_KEY_IS_DEAD = "isDead";

	protected override void OnUse()
	{
		//remember player direction at the time of use
		StartTruck(playerDirection, weaponContoller.GetProjectileStart(playerDirection).position);
	}

	protected override void OnStopUse()
	{
	}

	public void StartTruck(EDirection pDirection, Vector3 pSpawnPosition)
	{
		direction = pDirection;
		rigidBody2D.bodyType = RigidbodyType2D.Dynamic;

		colliderUpDown.enabled = direction == EDirection.Down || direction == EDirection.Up;
		colliderRightLeft.enabled = direction == EDirection.Right || direction == EDirection.Left;

		Vector3 dir = Utils.GetVector3(direction);

		if(_RemoteOwner && !Owner)
		{
			debug_AssignOwner(_RemoteOwner.Owner.LocalImage);
		}

		if(Owner)
		{
			//BUG?
			//it seems that IgnoreCollision is reset after deactivation
			Physics2D.IgnoreCollision(boxCollider2D, Owner.Collider);
		}
		else
		{
			if(Photon.IsMine)
			{
				Debug.LogError(gameObject.name + "Owner not set");
			}
			colliderUpDown.enabled = false;
			colliderRightLeft.enabled = false;
		}

		transform.parent = game.ProjectileManager.transform;
		transform.position = pSpawnPosition;
		//Debug.Log("position " + transform.position);

		gameObject.SetActive(true);
		LeanTween.cancel(gameObject);
		LeanTween.move(gameObject, transform.position + dir * speed, 10);

		animator.SetBool(AC_KEY_IS_DEAD, false);
		animator.SetFloat(AC_KEY_DIRECTION, (int)direction);

		canCollide = false;
		DoInTime(Shoot, cadency);

		//Owner is not set for local image
		Vector3 sendSpawnPos = Owner && Owner.LocalImage ? 
			Owner.LocalImage.WeaponController.GetProjectileStart(direction).position : 
			pSpawnPosition;

		//todo no need to send?
		//Network.Send(EPhotonMsg.Special_Curie_StartTruck, pDirection, sendSpawnPos);
	}
	

	private void Shoot()
	{
		if(!gameObject.activeSelf)
			return;

		//the truck can collide after the first shot
		canCollide = true;

		EDirection dir1 = Utils.GetOrthogonalDirection(direction);
		EDirection dir2 = Utils.GetOppositeDirection(dir1);
		//Vector3 direction1 = Utils.GetVector3(dir1);
		//Debug.Log($"Shoot {direction}, {transform.position}");
		//Debug.Log($"dir1 {dir1}, dir2 {dir2}");

		//truck shoots from 2 sides
		//direction = UP/DOWN => shoots from left and right
		bool isDirectionUpOrDown = direction == EDirection.Up || direction == EDirection.Down;
		Vector3 spawnPos2 = isDirectionUpOrDown ?
			projectileSpawnLeft.position : projectileSpawnUp.position;
		Vector3 spawnPos1 = isDirectionUpOrDown ?
			projectileSpawnRight.position : projectileSpawnDown.position;

		game.ProjectileManager.SpawnProjectile(spawnPos1, Owner, projectile, dir1);
		game.ProjectileManager.SpawnProjectile(spawnPos2, Owner, projectile, dir2);

		DoInTime(Shoot, cadency);
	}

	/// <summary>
	/// Called from state machine SpecialCurieCrash
	/// </summary>
	internal void OnCrashStateExit()
	{
		//Debug.Log(gameObject.name + " OnCrashStateExit ");
		animator.SetBool(AC_KEY_IS_DEAD, false);
		gameObject.SetActive(false);
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if(canCollide)
			OnCollisionEnter2D(collision);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(!canCollide || collision.gameObject.layer == game.ProjectileManager.LayerProjectile)
		{
			//Debug.Log("just projectile");
			return;
		}

		//remote object has deactivated collider
		//if(!Network.IsMine)
		//	return;

		Collide();
	}

	public void Collide()
	{
		animator.SetBool(AC_KEY_IS_DEAD, true);

		//Debug.Log("BOOM " + collision.gameObject.name);
		LeanTween.cancel(gameObject);
		rigidBody2D.bodyType = RigidbodyType2D.Static;
		//gameObject.SetActive(false);
		Photon.Send(EPhotonMsg.Special_Curie_Collide);
	}
}

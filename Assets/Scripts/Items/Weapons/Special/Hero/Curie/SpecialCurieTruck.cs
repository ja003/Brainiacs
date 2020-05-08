using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class SpecialCurieTruck : PlayerWeaponSpecialPrefab
{
	EDirection direction;
	[SerializeField] [Range(10, 100)]float speed = -1;
	[SerializeField] [Range(0.01f, 1)] float cadency = 0.1f;

	[SerializeField] BoxCollider2D colliderUpDown = null;
	[SerializeField] BoxCollider2D colliderRightLeft = null;

	[SerializeField] Transform projectileSpawnUp = null;
	[SerializeField] Transform projectileSpawnRight = null;
	[SerializeField] Transform projectileSpawnDown = null;
	[SerializeField] Transform projectileSpawnLeft = null;

	ProjectileConfig projectile;

	//protected override void Awake()
	//{
	//	//projectile has to be registered so we know its config.
	//	//it is the same as Curie basic (but can be changed in P_Projectile animator and config)
	//	brainiacs.ItemManager.AddProjectile(EWeaponId.Special_Curie, projectile);

	//	base.Awake();

	//}

	protected override void OnInit()
	{
		projectile = brainiacs.ItemManager.GetProjectileConfig(EWeaponId.Special_Curie);
		if(projectile == null)
		{
			Debug.LogError("SpecialCurieTruck projectile not found");
		}
		Physics2D.IgnoreCollision(boxCollider2D, owner.Collider);
	}

	protected override void OnUse()
	{
		SetActive(true);
		StartTruck(playerDirection, owner.WeaponController.GetProjectileStart(playerDirection).position);
	}

	protected override void OnStopUse()
	{
	}

	protected override void OnReturnToPool3()
	{
		
	}

	protected override void OnSetActive2(bool pValue)
	{
		animator.enabled = pValue;
		spriteRend.enabled = pValue;
		EnableColliders(pValue);
	}

	private void EnableColliders(bool pValue)
	{
		bool colliderActive = owner && owner.IsItMe && direction != EDirection.None;
		colliderUpDown.enabled = pValue && colliderActive;
		colliderRightLeft.enabled = pValue && colliderActive;

		colliderUpDown.enabled = pValue && colliderActive && 
			(direction == EDirection.Down || direction == EDirection.Up);
		colliderRightLeft.enabled = pValue && colliderActive && 
			(direction == EDirection.Right || direction == EDirection.Left);
	}

	bool canCollide;

	private const string AC_KEY_DIRECTION = "direction";
	private const string AC_KEY_IS_DEAD = "isDead";


	public void StartTruck(EDirection pDirection, Vector3 pSpawnPosition)
	{
		direction = pDirection;
		rigidBody2D.bodyType = RigidbodyType2D.Dynamic;

		EnableColliders(true);
		//colliderUpDown.enabled = direction == EDirection.Down || direction == EDirection.Up;
		//colliderRightLeft.enabled = direction == EDirection.Right || direction == EDirection.Left;

		Vector3 dir = Utils.GetVector3(direction);

		//transform.parent = game.ProjectileManager.transform;
		transform.position = pSpawnPosition;
		//Debug.Log("position " + transform.position);

		//SetActive(true);
		LeanTween.cancel(gameObject);
		LeanTween.move(gameObject, transform.position + dir * speed, 10);

		animator.SetBool(AC_KEY_IS_DEAD, false);
		animator.SetFloat(AC_KEY_DIRECTION, (int)direction);

		canCollide = false;
		
		//only owner shoots, projectiles are handled separately
		if(Photon.IsMine)
			DoInTime(Shoot, cadency);

		//Owner is not set for local image
		//Vector3 sendSpawnPos = Owner && Owner.LocalImage ?
		//	Owner.LocalImage.WeaponController.GetProjectileStart(direction).position :
		//	pSpawnPosition;

		//todo no need to send?
		Photon.Send(EPhotonMsg.Special_Curie_StartTruck, pDirection, pSpawnPosition);
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

		game.ProjectileManager.SpawnProjectile(spawnPos1, owner, projectile, dir1);
		game.ProjectileManager.SpawnProjectile(spawnPos2, owner, projectile, dir2);

		DoInTime(Shoot, cadency);
	}

	/// <summary>
	/// Called from state machine SpecialCurieCrash
	/// </summary>
	internal void OnCrashStateExit()
	{
		//Debug.Log(gameObject.name + " OnCrashStateExit ");
		animator.SetBool(AC_KEY_IS_DEAD, false);
		SetActive(false);
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
		
		Photon.Send(EPhotonMsg.Special_Curie_Collide);
	}

}

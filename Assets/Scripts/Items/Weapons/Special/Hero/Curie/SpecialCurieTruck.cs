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
	[SerializeField] [Range(10, 100)] float speed = -1;
	[SerializeField] [Range(0.01f, 1)] float cadency = 0.1f;
	//damage done by collision
	[SerializeField] [Range(0, 100)] int damage = 50;
	[SerializeField] [Range(0.1f, 3)] float radius = 1;

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


	public void StartTruck(EDirection pDirection, Vector2 pSpawnPosition)
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

		SoundController.PlaySound(ESound.Curie_Truck_Ride, audioSource, true);

		spriteRend.sortingOrder = owner.Visual.GetProjectileSortOrder();

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
		//Vector2 direction1 = Utils.GetVector2(dir1);
		//Debug.Log($"Shoot {direction}, {transform.position}");
		//Debug.Log($"dir1 {dir1}, dir2 {dir2}");

		//truck shoots from 2 sides
		//direction = UP/DOWN => shoots from left and right
		bool isDirectionUpOrDown = direction == EDirection.Up || direction == EDirection.Down;
		Vector2 spawnPos2 = isDirectionUpOrDown ?
			projectileSpawnLeft.position : projectileSpawnUp.position;
		Vector2 spawnPos1 = isDirectionUpOrDown ?
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

	/// <summary>
	/// Eg. collision with MapItem
	/// </summary>
	private void OnTriggerEnter2D(Collider2D collision)
	{
		OnCollision(collision.gameObject.layer);
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		OnCollision(collision.gameObject.layer);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		OnCollision(collision.gameObject.layer);
	}

	/// <summary>
	/// Projectiles dont destroy the truck
	/// Called only on owner side, remote object has deactivated collider.
	/// </summary>
	private void OnCollision(int pLayer)
	{
		if(!canCollide
			|| pLayer == game.ProjectileManager.LayerProjectile
			|| pLayer == game.Layers.MapDecoration)
		{
			//Debug.Log("just projectile");
			return;
		}

		Collide();
	}

	public void Collide()
	{
		animator.SetBool(AC_KEY_IS_DEAD, true);

		//Debug.Log("BOOM " + collision.gameObject.name);
		LeanTween.cancel(gameObject);
		rigidBody2D.bodyType = RigidbodyType2D.Static;

		audioSource.Stop();
		SoundController.PlaySound(ESound.Curie_Truck_Explode, audioSource, false);

		Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, radius);

		ICollisionHandler ownerHandler = owner.GetComponent<ICollisionHandler>();
		foreach(var c in collisions)
		{
			ICollisionHandler handler = c.GetComponent<ICollisionHandler>();
			if(handler == null)
				continue;

			if(handler == ownerHandler)
			{
				Debug.Log("SpecialCurieTruck Collision with myself " + handler);
				continue;
			}

			handler.OnCollision(damage, owner, gameObject, GetPush(c.transform));
			Debug.Log("SpecialCurieTruck Collision with " + handler);
		}

		Photon.Send(EPhotonMsg.Special_Curie_Collide);
		canCollide = false;
	}

}

using Photon.Pun;
using UnityEngine;

public abstract class MapObject : PoolObjectNetwork, ICollisionHandler
{
	[SerializeField] protected int maxHealth;
	protected int health;
	[SerializeField] protected bool isHealthDiscrete;
	[SerializeField] protected bool isDamagable;
	[SerializeField] protected bool isPushable;

	[SerializeField] PhotonView photonView;

	protected override void Awake()
	{
		health = maxHealth;

		if(isPushable)
		{
			var rb = GetComponent<Rigidbody2D>();
			if(rb == null)
			{
				Debug.LogError("Pushable object doesnt have rigid body");
			}
		}

		base.Awake();
	}

	/// <summary>
	/// If the object is pushable we need to transfer ownership to the player who 
	/// collides with the object, otherwise there is a horrible delay.
	/// The position is synced using SmoothSync.
	/// </summary>
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(!isPushable)
			return;

		Player player = collision.gameObject.GetComponent<Player>();
		if(player)
		{
			photonView.TransferOwnership(player.InitInfo.PhotonPlayer);
		}
	}

	public bool OnCollision(int pDamage, Player pOwner, GameObject pOrigin, Vector2 pPush)
	{
		OnCollisionEffect(pDamage, pOrigin);

		//see: OnCollisionEnter2D
		if(isPushable && pOwner.InitInfo.PhotonPlayer != null)
		{
			photonView.TransferOwnership(pOwner.InitInfo.PhotonPlayer);
			//Debug.Log($"Push {pPush}");
			rigidBody2D.AddForce(pPush);
		}

		if(isDamagable && pDamage > 0)
		{
			health -= isHealthDiscrete ? 1 : pDamage;
			if(health <= 0)
			{
				OnDestroyed();
				return true;
			}
		}

		bool? result2 = OnCollision2(pDamage, pOwner, pOrigin, pPush);
		if(result2 != null)
			return (bool)result2;

		return true;
	}

	protected abstract void OnDestroyed();

	/// <summary>
	/// Possible override of collision outcome
	/// </summary>
	protected virtual bool? OnCollision2(int pDamage, Player pOwner, GameObject pOrigin, Vector2 pPush)
	{
		return null;
	}

	protected override void OnSetActive0(bool pValue)
	{
	}

	protected abstract void OnCollisionEffect(int pDamage, GameObject pOrigin);

	protected override void OnReturnToPool2()
	{
		health = maxHealth;
		//throw new NotImplementedException();
	}

}

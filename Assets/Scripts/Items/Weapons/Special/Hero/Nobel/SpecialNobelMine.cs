using FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialNobelMine : PoolObject
{
	[SerializeField] int damage = 100;
	//[SerializeField] Animator mineAnimator = null;
	[SerializeField] SpriteRenderer mineSprite = null;
	//[SerializeField] SpecialNobelMinePhoton photon = null;

	Player owner;
	bool isExploded;

	protected override void OnSetActive(bool pValue)
	{
		mineSprite.enabled = pValue;
		boxCollider2D.enabled = pValue;
		animator.enabled = pValue;
	}

	public void Spawn(Player pOwner)
    {
		Debug.Log(gameObject.name + " spawn");
		owner = pOwner;
		//spriteRend.enabled = false; //this is just holder, anmator is in child
		SetActive(true);
		mineSprite.sortingOrder = pOwner.Visual.GetProjectileSortOrder();

		Photon.Send(EPhotonMsg.Special_Nobel_Spawn, pOwner.InitInfo.Number);

		//mine is simulated only on its owner side
		boxCollider2D.enabled = pOwner.IsItMe;
	}

	private void OnTriggerEnter2D(Collider2D collision)
    {
		if(!owner.IsItMe)
		{
			Debug.LogError("Mine shouldnt be triggered on its owner image side");
			return;
		}

		if(isExploded)
			return;

		ICollisionHandler handler = collision.GetComponent<ICollisionHandler>();
		if(handler == null)
			return;

		Player player = collision.GetComponent<Player>();

		if(player == null)
		{
			//Debug.Log("Not player");
			return;
		}
		if(player.Equals(owner))
		{
			//Debug.Log("thats me ");
			return;
		}

		handler.OnCollision(damage, owner);
		Explode();
	}

	private void Explode()
	{
		//Debug.Log("Explode");
		isExploded = true;
		animator.SetBool("explode", true);
	}

	internal void OnExplosionStateEnter()
	{
		mineSprite.sortingOrder = SortLayerManager.GetSortIndex(ESortObject.MapObject);
	}

	internal void OnExplosionStateExit()
	{
		//Debug.Log("OnExplosionStateExit");
		if(Photon.IsMine)
			InstanceFactory.Destroy(gameObject);
	}

	//PHOTON

	//protected override bool CanSendMsg(EPhotonMsg pMsgType)
	//{
	//	if(pMsgType != EPhotonMsg.Special_Nobel_Spawn)
	//	{
	//		Debug.LogError("Cant send another message");
	//		return false;
	//	}

	//	return view.IsMine;
	//}

	//protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	//{
	//	switch(pReceivedMsg)
	//	{
	//		case EPhotonMsg.Special_Nobel_Spawn:
	//			int playerNumber = (int)pParams[0];
	//			Player player = Game.Instance.PlayerManager.GetPlayer(playerNumber);
	//			Spawn(player);
	//			break;
	//	}
	//}
}

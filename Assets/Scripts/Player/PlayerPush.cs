using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPush : PlayerBehaviour
{
	[SerializeField] Rigidbody2D playerRB;
	public void Push(Vector2 pForce)
	{
		if(!player.IsItMe)
		{
			Debug.LogError("TODO: sent MP msg");
			player.Photon.Send(EPhotonMsg.Player_Push, pForce);
			return;
		}

		if(pForce.magnitude < 0.001f)
			return;

		LeanTween.cancel(gameObject);
		playerRB.AddForce(pForce, ForceMode2D.Impulse);
	}

}
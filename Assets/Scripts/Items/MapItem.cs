using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : GameBehaviour
{
	PlayerItemConfig config;

	public void Spawn(Vector3 pPosition, PlayerItemConfig pConfig)
	{
		transform.position = pPosition;
		config = pConfig;
		spriteRend.sprite = config.mapSprite;
		//Debug.Log($"Item {config} spawned at {pPosition}");
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Player player = collision.collider.GetComponent<Player>();
		if(player)
		{
			OnEnter(player);
		}
	}

	private void OnEnter(Player pPlayer)
	{
		config.OnEnterPlayer(pPlayer);
		gameObject.SetActive(false);
	}

}

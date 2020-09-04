using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps sort order of this object always higher than objects it is colliding with
/// </summary>
public class TopLayerSorter : GameBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		SpriteRenderer r = collision.gameObject.GetComponent<SpriteRenderer>();
		if(!r)
			return;

		//Debug.Log($"OnTriggerEnter2D {collision.gameObject.name} - {r.sortingOrder}");

		//if(transform.position.y > collision.transform.position.y)
		//	return;

		//eg. player sortOrder is X, player hands are X + 2
		//todo: implement interface for getting actual SortOrder
		rend.sortingOrder = r.sortingOrder + 5;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		//Debug.Log("OnTriggerExit2D");
		rend.sortingOrder = 0;
	}
}

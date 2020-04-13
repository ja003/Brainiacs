using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
	IOnCollision onCollision;
	public void Init(IOnCollision pOnCollision)
	{
		onCollision = pOnCollision;
	}


	private void OnCollisionEnter2D(Collision2D collision)
	{
		//Debug.Log($"{gameObject.name} collission with {collision.gameObject.name}");
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Debug.Log($"{gameObject.name} triggers with {collision.gameObject.name}");
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		onCollision?.OnTriggerStay2D(collision);

		//Debug.Log($"{gameObject.name} triggerSTay with {collision.gameObject.name}");
	}

	private void OnTriggerExit(Collider other)
	{
		//Debug.Log($"{gameObject.name} trigger EXIT with {other.gameObject.name}");
	}
}

//todo: implement other methods if needed
public interface IOnCollision
{
	//void OnTriggerExit(Collider pOther);

	
	void OnTriggerStay2D(Collider2D pCollision);
}

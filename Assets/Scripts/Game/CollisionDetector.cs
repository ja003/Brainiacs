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

	private Collider2D _collider2D;
	public Collider2D Collider2D
	{
		get
		{
			if(_collider2D == null)
				_collider2D = GetComponent<Collider2D>();
			return _collider2D;
		}
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
		onCollision?.Event_OnTriggerStay2D(collision);

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

	
	void Event_OnTriggerStay2D(Collider2D pCollision);
}

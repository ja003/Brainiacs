using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder
{

	// The context is a shared object every node has access to.
	// Commonly used components and subsystems should be stored here
	// It will be somewhat specific to your game exactly what to add here.
	// Feel free to extend this class 
	public class Context
	{
		public PlayerAiBTController BTController;

		public AiMovement Movement;
		public AiShoot Shoot;
		public AiEvade Evade;
		public AiMapItem Item;
		public AiDebug debug;

		public static Context Create(PlayerAiBTController pBTController)
		{

			Context context = new Context();

			context.BTController = pBTController;
			context.Movement = pBTController.Movement;
			context.Shoot = pBTController.Shoot;
			context.Item = pBTController.Item;
			context.Evade = pBTController.Evade;
			context.debug = pBTController.debug;


			return context;
		}

		internal static Context CreateFromGameObject(GameObject gameObject)
		{
			return Create(gameObject.GetComponent<PlayerAiBTController>());
		}
	}
}
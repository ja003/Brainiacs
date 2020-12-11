using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : BrainiacsBehaviour
{
	[SerializeField] float pushForce = 5;
#if UNITY_EDITOR
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F1))
		{
			SetTimeScale(0);
		}
		else if(Input.GetKeyDown(KeyCode.F2))
		{
			SetTimeScale(0.5f);
		}
		else if(Input.GetKeyDown(KeyCode.F3))
		{
			SetTimeScale(1);
		}
		else if(Input.GetKeyDown(KeyCode.F4))
		{
			SetTimeScale(2);
		}
		else if(Input.GetKeyDown(KeyCode.F5))
		{
			SetTimeScale(5);
		}


		else if(Input.GetKeyDown(KeyCode.M))
		{
			Game.Instance.PlayerManager.GetPlayer(1).Health.ApplyDamage(50, null);
		}
		else if(Input.GetKeyDown(KeyCode.P))
		{
			Game.Instance.PlayerManager.GetPlayer(1).Push.Push(Vector2.up * pushForce);
		}
		else if(Input.GetKeyDown(KeyCode.O))
		{
			Game.Instance.PlayerManager.GetPlayer(1).Push.Push(Vector2.left * pushForce);
		}



		else if(Input.GetKeyDown(KeyCode.N))
		{
			Game.Instance.Lighting.SetMode(!Game.Instance.Lighting.IsNight);
		}
		else if(Input.GetKeyDown(KeyCode.B))
		{
			Game.Instance.Lighting.SetNight(3);
		}
	}
#endif

	private static void SetTimeScale(float pValue)
	{
		Time.timeScale = pValue;
		Debug.Log("SetTimeScale " + pValue);
	}
}

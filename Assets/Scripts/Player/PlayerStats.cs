using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : GameBehaviour
{
	private int lives;
	public int Deaths;
	public int LivesLeft => lives - Deaths;

	public EPlayerColor Color { get; internal set; }

	private const float DEFAULT_SPEED = 1;
	private const int MAX_HEALTH = 100;
	public float Speed { get; private set; } = DEFAULT_SPEED;
	public int Health { get; private set; } = 90;
	public int Kills;

	private bool isShielded;

	public EHero Hero;
	public string Name;

	private Action<PlayerStats> onStatsChange;

	[SerializeField] public Transform StatusUiPosition;

	public void SetOnStatsChange(Action<PlayerStats> pAction)
	{
		onStatsChange += pAction;
		onStatsChange.Invoke(this);
	}
	
	public void Init(PlayerInitInfo pPlayerInfo)
	{
		lives = pPlayerInfo.Lives;
		Hero = pPlayerInfo.Hero;
		Name = pPlayerInfo.Name;
		Color = pPlayerInfo.Color;
	}

	internal bool IsDead()
	{
		return Health <= 0;
	}

	//todo: implement effect management
	//if SetSpeed is called twice the first call will cancel effect
	//of the second one
	public void SetSpeed(float pSpeed, float pDuration)
	{
		Debug.Log($"{Name} set speed {pSpeed}");
		Speed = pSpeed; 
		DoInTime(() => Speed = DEFAULT_SPEED, pDuration);
	}

	public void OnRespawn()
	{
		Health = 100;
		onStatsChange.Invoke(this);
	}

	public void AddKill()
	{
		Kills++;
		onStatsChange.Invoke(this);
	}

	public void AddHealth(int pIncrement, bool pRespawn = false)
	{
		if(IsDead() && !pRespawn)
			return;

		if(pIncrement < 0 && isShielded)
		{
			Debug.Log($"{Name} is invurnelable");
			return;
		}

		Health = Mathf.Clamp(Health + pIncrement, 0, 100);
		onStatsChange.Invoke(this);
	}

	public void SetShield(float pDuration)
	{
		isShielded = true;
		DoInTime(() => isShielded = false, pDuration);
	}	
}

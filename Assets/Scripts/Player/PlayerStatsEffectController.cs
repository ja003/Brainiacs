using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsEffectController : PlayerBehaviour
{
	List<StatsEffect> appliedEffects = new List<StatsEffect>();

	[SerializeField] Transform uiEffectPosition = null;
	UIPlayerEffect uiEffect;

	protected override void Awake()
	{
		uiEffect = game.PlayerStatusManager.InitPlayerEffect();
		base.Awake();
	}

	private void Update()
	{
		uiEffect.SetPosition(uiEffectPosition.position);
	}

	/// <summary>
	/// Applies the affect and remove it after the pDuration if set to  > 0.
	/// Otherwise it has to be removed manually.
	/// </summary>
	public void ApplyEffect(EPlayerEffect pType, float pDuration)
	{
		StatsEffect appliedEffect = appliedEffects.Find(a => a.Type == pType);
		float duration = pDuration;

		//Debug.Log($"ApplyEffect {pType} for {duration}");

		if(appliedEffect != null)
		{
			LeanTween.cancel(appliedEffect.id);
			appliedEffects.Remove(appliedEffect);
			if(pDuration > 0 && appliedEffect.TimeLeft < 0)
				Debug.LogError("Time left cant be < 0");
			
			duration += appliedEffect.TimeLeft;
			//Debug.Log($"{pType}: add {appliedEffect.TimeLeft}s => {duration}s");
		}

		StatsEffect newEffect = new StatsEffect(pType, duration);//, pValue);
		appliedEffects.Add(newEffect);
		SetEffectActive(pType, true);
		if(pDuration > 0)
		{
			int id = DoInTime(() => RemoveEffect(pType), duration);
			newEffect.id = id;
		}
	}

	/// <summary>
	/// Removes the effect
	/// </summary>
	/// <param name="pValidateIfApplied">Checks if the effect was actually applied</param>
	public void RemoveEffect(EPlayerEffect pType, bool pValidateIfApplied = true)
	{
		int index = appliedEffects.FindIndex(a => a.Type == pType);
		if(index < 0)
		{
			if(pValidateIfApplied)
				Debug.LogError($"Effect {pType} not found");
			return;
		}
		appliedEffects.RemoveAt(index);
		SetEffectActive(pType, false);
	}

	public void SetEffectActive(EPlayerEffect pType, bool pState)
	{
		uiEffect.SetEffectActive(pType, pState);
		player.Photon.Send(EPhotonMsg.Player_UI_SetEffectActive, pType, pState);
	}

	public void OnDie()
	{
		for(int i = appliedEffects.Count - 1; i >= 0; i--)
		{
			StatsEffect effect = appliedEffects[i];
			RemoveEffect(effect.Type);
		}
	}

	internal void OnReturnToPool()
	{
		OnDie();
	}

	public float GetDamageMultiplier()
	{
		return GetEffectValue(EPlayerEffect.HalfDamage) * GetEffectValue(EPlayerEffect.DoubleDamage);
	}

	internal float GetEffectValue(EPlayerEffect pType)
	{
		//bool isEffectApplied = appliedEffects.Find(a => a.Type == pType) != null;

		//check if effect is applied based on UI.
		//only UI state is shared across network
		bool isEffectApplied = uiEffect.IsEffectActive(pType);

		switch(pType)
		{
			case EPlayerEffect.DoubleSpeed:
				return isEffectApplied ? 2 : 1;
			case EPlayerEffect.HalfSpeed:
				return isEffectApplied ? 0.5f : 1;
			case EPlayerEffect.Shield:
				return isEffectApplied ? 1 : 0; //1 => IsShielded
			case EPlayerEffect.DoubleDamage:
				return isEffectApplied ? 2 : 1;
			case EPlayerEffect.HalfDamage:
				return isEffectApplied ? 0.5f : 1;
			case EPlayerEffect.DaVinciTank:
				return isEffectApplied ? 1 : 0;
		}
		Debug.LogError($"Effect value {pType} undefined");
		return 0;
	}

	//internal bool IsEffectApplied(EPlayerEffect pType)
	//{
	//    return appliedEffects.Find(a => a.Type == pType) != null;
	//}

	private class StatsEffect
	{
		public EPlayerEffect Type;
		public int id;
		//public float Value;
		float timeStarted;
		float duration;
		public float TimeLeft => timeStarted + duration - Time.time;

		public StatsEffect(EPlayerEffect pType, float pDuration)//, float pValue)
		{
			Type = pType;
			duration = pDuration;
			//Value = pValue;
			timeStarted = Time.time;
		}
	}
}

public enum EPlayerEffect
{
	None,
	DoubleSpeed,
	HalfSpeed,
	Shield,
	DoubleDamage,
	HalfDamage,
	DaVinciTank
}

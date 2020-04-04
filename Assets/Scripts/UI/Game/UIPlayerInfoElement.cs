using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfoElement : BrainiacsBehaviour
{
	[SerializeField] private Image portrait;
	[SerializeField] private Image weapon;
	[SerializeField] private Text health;
	[SerializeField] private Text ammo;

	private void Update()
	{
		if(activeWeapon == EWeaponId.None)
			return;

		// Reloading
		weapon.color = isRealoading ? Color.black : Color.white;
		if(isRealoading)
		{
			float reloadTimeLeft = (reloadTimeStarted + reloadTimeTotal) - Time.time;
			ammo.text = reloadTimeLeft.ToString("0.0");
			return;
		}

		// Cadency
		if(activeWeaponCadency < 0.1f)
			return;

		//set weapon alpha based on cadency ready state
		float rdyPercentage = GetCadencyReadyPercentage();
		//Debug.Log($"{activeWeapon} rdy: {rdyPercentage*100}%");
		weapon.color = new Color(
			weapon.color.r,
			weapon.color.g,
			weapon.color.b,
			rdyPercentage
			);
	}

	private float GetCadencyReadyPercentage()
	{
		float remains = activeWeaponLastUsedTime + activeWeaponCadency - Time.time;
		float percentage = 1 - remains / activeWeaponCadency;
		return Mathf.Clamp(percentage, 0, 1);
	}

	Player player;
	internal void Init(Player pPlayer)
	{
		player = pPlayer;
		gameObject.SetActive(true);

		image.color = UIColorDB.GetColor(pPlayer.InitInfo.Color);

		portrait.sprite = brainiacs.HeroManager.GetHeroConfig(pPlayer.InitInfo.Hero).Portrait;

		if(pPlayer.IsItMe)
		{
			pPlayer.WeaponController.SetOnWeaponInfoChanged(SetWeaponInfo);
			pPlayer.Stats.SetOnStatsChange(OnPlayerStatsChange);
		}
		pPlayer.Visual.PlayerInfo = this;
	}

	private void OnPlayerStatsChange(PlayerStats pStats)
	{
		SetHealth(pStats.Health);		
	}

	public void SetHealth(int pHealth)
	{
		if(health.text == pHealth.ToString())
			return;
		health.text = pHealth.ToString();

		player.Network.Send(
			EPhotonMsg.Player_UI_PlayerInfo_SetHealth, pHealth);
	}

	/// <summary>
	/// Called only on local player.
	/// Only visual data is shared across network.
	/// </summary>
	private void SetWeaponInfo(PlayerWeapon pWeapon)
	{
		SetActiveWeapon(pWeapon.Id, pWeapon.Info.Cadency);

		SetReloading(pWeapon.IsRealoading, pWeapon.RealoadTimeLeft);

		if(!pWeapon.IsRealoading)
		{
			SetAmmo(pWeapon.AmmoLeft);
		}
	}


	//-- NETWORK METHODS --//

	EWeaponId activeWeapon;
	float activeWeaponCadency;
	float activeWeaponLastUsedTime;
	public void SetActiveWeapon(EWeaponId pId, float pCadency)
	{
		if(activeWeapon == pId)
			return;
		activeWeapon = pId;
		activeWeaponCadency = pCadency;

		//weapon sprite
		InHandWeaponVisualInfo info = brainiacs.ItemManager.GetWeaponConfig(pId).VisualInfo;
		weapon.sprite = info.InfoSprite;

		player.Network.Send(EPhotonMsg.Player_UI_PlayerInfo_SetActiveWeapon, pId, pCadency);
	}

	public void SetAmmo(int pAmmoLeft)
	{
		if(ammo.text == pAmmoLeft.ToString())
			return;
		ammo.text = pAmmoLeft.ToString();
		activeWeaponLastUsedTime = Time.time;
		player.Network.Send(EPhotonMsg.Player_UI_PlayerInfo_SetAmmo, pAmmoLeft);
	}

	bool isRealoading;
	float reloadTimeTotal;
	float reloadTimeStarted;
	public void SetReloading(bool pIsReloading, float pRealoadTimeTotal)
	{
		if(isRealoading != pIsReloading)
		{
			player.Network.Send(EPhotonMsg.Player_UI_PlayerInfo_SetReloading,
				pIsReloading, pRealoadTimeTotal);
		}

		isRealoading = pIsReloading;
		reloadTimeTotal = pRealoadTimeTotal;
		if(isRealoading)
			reloadTimeStarted = Time.time;
	}
}

using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfoElement : UiBehaviour
{
	[SerializeField] private Image portrait = null;
	[SerializeField] public Image weapon = null;
	[SerializeField] private Image frame = null;
	[SerializeField] public Text health = null;
	[SerializeField] public Text ammo = null;

	private void Update()
	{
		if(Time.time < 0.1f)
			return;

		if(activeWeapon == EWeaponId.None)
			return;

		// Reloading
		weapon.color = isRealoading ? Color.black : Color.white;
		weapon.color *= game.Lighting.GlobalLight.intensity;
		Utils.SetAlpha(weapon, 1);

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
		Utils.SetAlpha(weapon, rdyPercentage);
	}

	private float GetCadencyReadyPercentage()
	{
		float remains = activeWeaponLastUsedTime + activeWeaponCadency - Time.time;
		float percentage = 1 - remains / activeWeaponCadency;
		return Mathf.Clamp(percentage, 0, 1);
	}

	Player player;

	/// <summary>
	/// Called after all players are added
	/// </summary>
	public void Init(Player pPlayer)
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

		game.Lighting.RegisterForLighting(image);
		game.Lighting.RegisterForLighting(portrait);
		game.Lighting.RegisterForLighting(frame);
		//game.Lighting.RegisterForLighting(weapon); //controlled in Update
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

		player.Photon.Send(EPhotonMsg.Player_UI_PlayerInfo_SetHealth, pHealth);
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
			//we cant simply set the time when weapon was used, because 
			//Time.time is different on all clients
			//todo: use PhotonNetwork.Time ? !!! returns 0 in single player
			bool weaponUsed = Time.time - pWeapon.LastUseTime < 0.1f;
			SetAmmo(pWeapon.AmmoLeft, weaponUsed);
		}
	}


	//-- NETWORK METHODS --//

	EWeaponId activeWeapon;
	float activeWeaponCadency;
	float activeWeaponLastUsedTime = int.MinValue;
	public void SetActiveWeapon(EWeaponId pId, float pCadency)
	{
		if(activeWeapon == pId)
			return;
		activeWeapon = pId;
		activeWeaponCadency = pCadency;

		//weapon sprite
		InHandWeaponVisualInfo info = brainiacs.ItemManager.GetWeaponConfig(pId).VisualInfo;
		weapon.sprite = info.InfoSprite;

		player.Photon.Send(EPhotonMsg.Player_UI_PlayerInfo_SetActiveWeapon, pId, pCadency);
	}

	public void SetAmmo(int pAmmoLeft, bool pWeaponUsed)
	{
		if(ammo.text == pAmmoLeft.ToString())
			return;
		ammo.text = pAmmoLeft.ToString();

		if(pWeaponUsed)
			activeWeaponLastUsedTime = Time.time;

		player.Photon.Send(EPhotonMsg.Player_UI_PlayerInfo_SetAmmo, pAmmoLeft, pWeaponUsed);
	}

	bool isRealoading;
	float reloadTimeTotal;
	float reloadTimeStarted;
	public void SetReloading(bool pIsReloading, float pRealoadTimeTotal)
	{
		if(isRealoading != pIsReloading)
		{
			player.Photon.Send(EPhotonMsg.Player_UI_PlayerInfo_SetReloading,
				pIsReloading, pRealoadTimeTotal);
		}

		isRealoading = pIsReloading;
		reloadTimeTotal = pRealoadTimeTotal;
		if(isRealoading)
			reloadTimeStarted = Time.time;
	}
}

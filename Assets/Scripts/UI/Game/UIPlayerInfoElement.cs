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
		if(activeWeapon == null)
			return;
		if(activeWeapon.IsRealoading)
			return;		
		if(activeWeapon.Info.Cadency < 0.1f)
			return;

		//set weapon alpha based on cadency ready state
		float rdyPercentage = activeWeapon.GetCadencyReadyPercentage();
		//Debug.Log($"{activeWeapon} rdy: {rdyPercentage*100}%");
		weapon.color = new Color(
			weapon.color.r,
			weapon.color.g,
			weapon.color.b,
			rdyPercentage
			);
	}

	internal void Init(Player pPlayer)
	{
		gameObject.SetActive(true);

		image.color = UIColorDB.GetColor(pPlayer.Stats.Color);

		portrait.sprite = brainiacs.HeroManager.GetHeroConfig(pPlayer.Stats.Hero).Portrait;

		pPlayer.WeaponController.SetOnWeaponInfoChanged(SetWeaponInfo);
		pPlayer.Stats.SetOnStatsChange(SetHealth);
	}

	private void SetHealth(PlayerStats pStats)
	{
		health.text = pStats.Health.ToString();
	}

	PlayerWeapon activeWeapon;
	private void SetWeaponInfo(PlayerWeapon pWeapon)
	{
		activeWeapon = pWeapon;
		weapon.sprite = pWeapon.VisualInfo.InfoSprite;
		weapon.color = pWeapon.IsRealoading ? Color.black : Color.white;
		ammo.text = pWeapon.GetAmmoText();
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : GameBehaviour
{
	[SerializeField] private Image portrait;
	[SerializeField] private Image weapon;
	[SerializeField] private Text health;
	[SerializeField] private Text ammo;

	internal void Init(Player pPlayer)
	{
		gameObject.SetActive(true);

		image.color = pPlayer.Stats.Color;

		portrait.sprite = brainiacs.HeroManager.GetHeroConfig(pPlayer.Stats.Hero).Portrait;

		pPlayer.WeaponController.SetOnWeaponInfoChanged(SetWeaponInfo);
		pPlayer.Stats.SetOnStatsChange(SetHealth);
	}

	private void SetHealth(PlayerStats pStats)
	{
		health.text = pStats.Health.ToString();
	}

	private void SetWeaponInfo(PlayerWeapon pWeapon)
	{
		weapon.sprite = pWeapon.Config.InfoSprite;
		ammo.text = pWeapon.Ammo.ToString();
	}
}

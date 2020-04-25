using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemManager : BrainiacsController
{
	[SerializeField]
	public List<MapWeaponConfig> MapWeapons;

	[SerializeField]
	public List<MapSpecialWeaponConfig> MapWeaponsSpecial;


	[SerializeField]
	public List<PowerUpConfig> PowerUps;

	private Dictionary<EWeaponId, WeaponConfig> allWeapons = 
		new Dictionary<EWeaponId, WeaponConfig>();

	private Dictionary<EWeaponId, ProjectileConfig> allProjectiles 
		= new Dictionary<EWeaponId, ProjectileConfig>();



	public void AddProjectile(EWeaponId pWeapon, ProjectileConfig pConfig)
	{
		if(allProjectiles.ContainsKey(pWeapon))
			return;
		allProjectiles.Add(pWeapon, pConfig);
	}

	private void Init()
	{
		//Map weapons
		foreach(var w in MapWeapons)
		{
			allWeapons.Add(w.Id, w);
			allProjectiles.Add(w.Id, w.ProjectileInfo.Projectile);
		}

		//Map weapons special
		foreach(var w in MapWeaponsSpecial)
		{
			allWeapons.Add(w.Id, w);
		}

		//hero basic + special weapons
		foreach(var h in brainiacs.HeroManager.heroConfigs)
		{
			WeaponConfig heroBasic = h.BasicWeapon;
			WeaponConfig heroSpecial = h.SpecialWeapon;

			allWeapons.Add(heroBasic.Id, heroBasic);
			allProjectiles.Add(heroBasic.Id, h.BasicWeapon.Projectile);

			//todo: not all special weapons are configured
			if(allWeapons.ContainsKey(heroSpecial.Id) || heroSpecial == null)
				continue;
			allWeapons.Add(heroSpecial.Id, heroSpecial);
		}
	}


	protected override void OnMainControllerAwaken()
	{
		//hero configs need to be inited first
		brainiacs.HeroManager.SetOnAwaken(Init);
	}

	internal PowerUpConfig GetPowerupConfig(EPowerUp pPowerUp)
	{
		return PowerUps.Find(a => a.Type == pPowerUp);
	}

	public MapSpecialWeaponConfig GetMapSpecialWeaponConfig(EWeaponId pId)
	{
		return MapWeaponsSpecial.Find(a => a.Id == pId);
	}

	public MapWeaponConfig GetMapWeaponConfig(EWeaponId pId)
	{
		MapWeaponConfig config = MapWeapons.Find(a => a.Id == pId);
		//if(config == null) //not error?ge
		//	Debug.LogError($"Config for weapon {pId} not found");

		return config;
	}

	public HeroSpecialWeaponConfig GetHeroSpecialWeaponConfig(EHero pHero)
	{
		HeroConfig heroConfig = brainiacs.HeroManager.GetHeroConfig(pHero);
		return heroConfig.SpecialWeapon;
	}

	public HeroBasicWeaponConfig GetHeroBasicWeaponConfig(EHero pHero)
	{
		HeroConfig heroConfig = brainiacs.HeroManager.GetHeroConfig(pHero);
		return heroConfig.BasicWeapon;
	}

	public WeaponConfig GetWeaponConfig(EWeaponId pWeapon)
	{
		WeaponConfig weapon;
		allWeapons.TryGetValue(pWeapon, out weapon);
		if(weapon == null)
			Debug.LogError("Missing config for weapon: " + pWeapon);

		return weapon;
	}

	public ProjectileConfig GetProjectileConfig(EWeaponId pWeapon)
	{
		ProjectileConfig projectile;
		allProjectiles.TryGetValue(pWeapon, out projectile);
		if(projectile == null)
			Debug.LogError("Missing config for projectile: " + pWeapon);

		return projectile;
	}

	/// <summary>
	/// Not sure if neccessary
	/// </summary>
	public EWeaponCathegory GetWeaponCathegory(EWeaponId pWeapon)
	{
		if(pWeapon.ToString().Contains("Basic_"))
			return EWeaponCathegory.HeroBasic;
		if(pWeapon.ToString().Contains("Special_"))
			return EWeaponCathegory.HeroSpecial;

		if(GetMapWeaponConfig(pWeapon) != null)
			return EWeaponCathegory.MapBasic;
		if(GetMapSpecialWeaponConfig(pWeapon) != null)
			return EWeaponCathegory.MapSpecial;

		Debug.LogError("No cathegory for weapon " + pWeapon);
		return EWeaponCathegory.None;
	}

}

public enum EWeaponCathegory
{
	None,
	HeroBasic,
	HeroSpecial,
	MapBasic,
	MapSpecial
}
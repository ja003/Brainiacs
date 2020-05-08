using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls special weapons.
/// - instancing prefabs
/// - using
/// - sending other info to prefabs
/// </summary>
public class PlayerWeaponSpecial : PlayerWeapon
{
	//PlayerWeaponSpecialController specialController;
	private PlayerWeaponSpecialPrefab prefab;
	protected PlayerWeaponSpecialPrefab prefabInstance;
	private EHero hero;

	public PlayerWeaponSpecial(Player pOwner, HeroSpecialWeaponConfig pConfig, EHero pHero) :
		base(pOwner, pConfig.Id, pConfig.SpecialWeaponInfo.InHandInfo, pConfig.VisualInfo)
	{
		hero = pHero;
		prefab = pConfig.SpecialWeaponInfo.Prefab;

		//Game.Instance.PlayerManager.OnAllPlayersAdded.AddAction(InstantiatePrefab);
		InstantiatePrefab();

	}

	

	public PlayerWeaponSpecial(Player pOwner, MapSpecialWeaponConfig pConfig) :
		base(pOwner, pConfig.Id, pConfig.InHandInfo, pConfig.VisualInfo)
	{
		prefab = pConfig.SpecialWeaponInfo.Prefab;
		//Game.Instance.PlayerManager.OnAllPlayersAdded.AddAction(InstantiatePrefab);
		InstantiatePrefab();
	}

	protected void InstantiatePrefab()
	{
		if(prefab == null)
		{
			Debug.LogError("Prefab not defined");
			return;
		}
		Vector3 spawnPos = Owner.WeaponController.GetProjectileStart().position;
		prefabInstance = InstanceFactory.Instantiate(prefab.gameObject, spawnPos)
			.GetComponent<PlayerWeaponSpecialPrefab>();

		if(!prefabInstance.Photon.IsMine)
		{
			Debug.LogError("Instance is not mine " + prefabInstance.name);
			prefabInstance.Photon.view.TransferOwnership(PhotonNetwork.LocalPlayer);
		}

		prefabInstance.Init(Owner);
	}

	public override EWeaponUseResult Use()
	{
		EWeaponUseResult useResult = base.Use();
		if(useResult != EWeaponUseResult.CantUse)
		{		
			if(prefab.InstanceOnEveryUse)
				InstantiatePrefab();

			prefabInstance.Use();
			//OnUse();
			//specialController.Use();
		}

		return useResult;
	}

	protected virtual void OnUse() { }

	public override void StopUse()
	{
		prefabInstance.StopUse();
		//specialController.StopUse();
		base.StopUse();
	}

	internal override void OnStartReloadWeapon()
	{
		prefabInstance.OnStartReloadWeapon();
		base.OnStartReloadWeapon();
		//specialController.OnStartReloadWeapon();
	}

	public override void OnDirectionChange(EDirection pDirection)
	{
		base.OnDirectionChange(pDirection);
		//specialController.OnDirectionChange(pDirection);
	}

	public override void OnSetActive()
	{
		base.OnSetActive();
		//specialController.OnSetActive();
	}
}

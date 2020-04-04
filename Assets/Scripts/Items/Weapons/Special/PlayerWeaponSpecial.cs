using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSpecial : PlayerWeapon
{
	PlayerWeaponSpecialController specialController;

	public PlayerWeaponSpecial(Player pOwner, HeroSpecialWeaponConfig pConfig) :
		base(pOwner, pConfig.Id, pConfig.SpecialWeaponInfo.InHandInfo, pConfig.VisualInfo)
	{
		InstantiateWeaponController(pOwner, pConfig.SpecialWeaponInfo.ControllerPrefab.name);

	}

	public PlayerWeaponSpecial(Player pOwner, MapSpecialWeaponConfig pConfig) :
		base(pOwner, pConfig.Id, pConfig.InHandInfo, pConfig.VisualInfo)
	{
		InstantiateWeaponController(pOwner, pConfig.SpecialWeaponInfo.ControllerPrefab.name);
	}

	private void InstantiateWeaponController(Player pOwner,string pPrefabName)
	{
		var instance = PhotonNetwork.Instantiate(pPrefabName, Vector3.zero, Quaternion.identity)
					.GetComponent<PlayerWeaponSpecialController>();
		//Debug.Log("Instantiate: " + instance.name);

		//instance.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
		instance.GetComponent<PhotonView>().TransferOwnership(pOwner.InitInfo.PhotonPlayer);

		if(DebugData.LocalRemote)
		{
			var localRemote = PhotonNetwork.Instantiate(pPrefabName, Vector3.zero, Quaternion.identity)
			   .GetComponent<PlayerWeaponSpecialController>();
			instance._LocalRemote = localRemote;

			localRemote._RemoteOwner = instance;
			//instance.IsLocalRemote = true;
			//instance.debug_AssignOwner(pOwner.LocalRemote);
			localRemote.gameObject.SetActive(false);
			localRemote.name += "_LR";
			//Debug.Log("Instantiate: " + localRemote.name);
		}

		specialController = instance;
		specialController.Init(pOwner);
	}

	protected override bool CanUse()
	{
		return base.CanUse() && specialController.CanUse();
	}


	public override EWeaponUseResult Use()
	{
		EWeaponUseResult useResult = base.Use();
		if(useResult != EWeaponUseResult.CantUse)
			specialController.Use();

		return useResult;
	}

	public override void StopUse()
	{
		specialController.StopUse();
		base.StopUse();
	}

	internal override void OnStartReloadWeapon()
	{
		base.OnStartReloadWeapon();
		specialController.OnStartReloadWeapon();
	}

	public override void OnDirectionChange(EDirection pDirection)
	{
		base.OnDirectionChange(pDirection);
		specialController.OnDirectionChange(pDirection);
	}

	public override void OnSetActive()
	{
		base.OnSetActive();
		specialController.OnSetActive();
	}
}

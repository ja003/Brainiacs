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
		InstantiateWeaponController(pOwner, pConfig.SpecialWeaponInfo.ControllerPrefab.gameObject);

	}

	public PlayerWeaponSpecial(Player pOwner, MapSpecialWeaponConfig pConfig) :
		base(pOwner, pConfig.Id, pConfig.InHandInfo, pConfig.VisualInfo)
	{
		InstantiateWeaponController(pOwner, pConfig.SpecialWeaponInfo.ControllerPrefab.gameObject);
	}

	private void InstantiateWeaponController(Player pOwner,GameObject pPrefab)
	{
		var instance = InstanceFactory.Instantiate(pPrefab)
					.GetComponent<PlayerWeaponSpecialController>();
		//Debug.Log("Instantiate: " + instance.name);

		if(PhotonNetwork.IsConnected)
			instance.GetComponent<PhotonView>().TransferOwnership(pOwner.InitInfo.PhotonPlayer);

		if(DebugData.LocalImage)
		{
			var localImage = InstanceFactory.Instantiate(pPrefab)
			   .GetComponent<PlayerWeaponSpecialController>();
			instance._LocalImage = localImage;

			localImage._RemoteOwner = instance;
			//instance.IsLocalImage = true;
			//instance.debug_AssignOwner(pOwner.LocalImage);
			localImage.gameObject.SetActive(false);
			localImage.name += "_LR";
			//Debug.Log("Instantiate: " + localImage.name);
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

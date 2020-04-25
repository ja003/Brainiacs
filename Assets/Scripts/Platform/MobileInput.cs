using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInput : GameBehaviour
{
	[SerializeField] public ExtendedButton btnShoot;
	[SerializeField] public ExtendedButton btnSwap;

	[SerializeField] public MoveJoystick moveJoystick;

	protected override void Awake()
	{
		base.Awake();
		bool isMobile = PlatformManager.GetPlatform() == EPlatform.Mobile;
		btnShoot.gameObject.SetActive(isMobile);
		btnSwap.gameObject.SetActive(isMobile);

		moveJoystick.gameObject.SetActive(isMobile || DebugData.TestMobileInput);
	}
}

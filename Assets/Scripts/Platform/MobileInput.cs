using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInput : GameBehaviour
{
	[SerializeField] public ExtendedButton btnShoot;
	[SerializeField] public ExtendedButton btnSwap;

	[SerializeField] public MoveJoystick moveJoystick;

	[SerializeField] private GridLayoutGroup moveButtonsGroup;
	[SerializeField] public ExtendedButton btnMoveUp;
	[SerializeField] public ExtendedButton btnMoveRight;
	[SerializeField] public ExtendedButton btnMoveDown;
	[SerializeField] public ExtendedButton btnMoveLeft;

	protected override void Awake()
	{
		base.Awake();
		
		
		SetActive(PlatformManager.IsMobile(), brainiacs.PlayerPrefs.MobileInputJoystick);

		SetMoveInputScale(brainiacs.PlayerPrefs.MoveInputScale);
	}

	public void SetActive(bool pValue, bool pUseJoystick)
	{
		btnShoot.gameObject.SetActive(pValue);
		btnSwap.gameObject.SetActive(pValue);

		moveJoystick.gameObject.SetActive(pValue && pUseJoystick);

		moveButtonsGroup.gameObject.SetActive(pValue && !pUseJoystick);
		btnMoveUp.gameObject.SetActive(pValue && !pUseJoystick);
		btnMoveRight.gameObject.SetActive(pValue && !pUseJoystick);
		btnMoveDown.gameObject.SetActive(pValue && !pUseJoystick);
		btnMoveLeft.gameObject.SetActive(pValue && !pUseJoystick);
	}

	public void SetMoveInputScale(float pValue)
	{
		moveButtonsGroup.transform.localScale = Vector3.one * pValue;
		moveJoystick.transform.localScale = Vector3.one * pValue;
	}
}

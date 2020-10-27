using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputKeyset : BrainiacsBehaviour
{
	[SerializeField] Text header;

	[SerializeField] UiInputKey keyUp;
	[SerializeField] UiInputKey keyRight;
	[SerializeField] UiInputKey keyDown;
	[SerializeField] UiInputKey keyLeft;

	[SerializeField] UiInputKey keySwap;
	[SerializeField] UiInputKey keyUse;

	EKeyset Keyset;

	public void Init(EKeyset pKeyset, PlayerKeys pPlayerKeys)
	{
		Keyset = pKeyset;
		header.text = Keyset.ToString();

		keyUp.OnKeySet += OnAnyKeySet;
		keyRight.OnKeySet += OnAnyKeySet;
		keyDown.OnKeySet += OnAnyKeySet;
		keyLeft.OnKeySet += OnAnyKeySet;
		keySwap.OnKeySet += OnAnyKeySet;
		keyUse.OnKeySet += OnAnyKeySet;

		keyUp.SetKey(pPlayerKeys.moveUp);
		keyRight.SetKey(pPlayerKeys.moveRight);
		keyDown.SetKey(pPlayerKeys.moveDown);
		keyLeft.SetKey(pPlayerKeys.moveLeft);

		keySwap.SetKey(pPlayerKeys.swapWeapon);
		keyUse.SetKey(pPlayerKeys.useWeapon);
	}

	private void OnAnyKeySet()
	{
		brainiacs.PlayerKeysManager.UpdateKeySet(Keyset, GetKeys());
	}

	private PlayerKeys GetKeys()
	{
		return new PlayerKeys(
			keyUp.Key,
			keyRight.Key,
			keyDown.Key,
			keyLeft.Key,
			keyUse.Key,
			keySwap.Key
			);
	}
}

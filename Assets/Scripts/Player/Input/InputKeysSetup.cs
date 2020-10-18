using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputKeysSetup : MainMenuBehaviour
{
	[SerializeField] private Button btnSettingsBack = null;

    [SerializeField] InputKeyset keySetA;
	[SerializeField] InputKeyset keySetB;
    [SerializeField] InputKeyset keySetC;
    [SerializeField] InputKeyset keySetD;

    private new void Awake()
    {
		btnSettingsBack.onClick.AddListener(mainMenu.OnBtnBack);

		PlayerKeys keysA = brainiacs.PlayerPrefs.GetPlayerKeys(EKeyset.KeysetA);
		PlayerKeys keysB = brainiacs.PlayerPrefs.GetPlayerKeys(EKeyset.KeysetB);
		PlayerKeys keysC = brainiacs.PlayerPrefs.GetPlayerKeys(EKeyset.KeysetC);
		PlayerKeys keysD = brainiacs.PlayerPrefs.GetPlayerKeys(EKeyset.KeysetD);

		keySetA.Init(EKeyset.KeysetA, keysA);
		keySetB.Init(EKeyset.KeysetB, keysB);
		keySetC.Init(EKeyset.KeysetC, keysC);
		keySetD.Init(EKeyset.KeysetD, keysD);
	}
}

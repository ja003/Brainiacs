using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputKeysSetup : MainMenuBehaviour
{
	[SerializeField] private Button btnSettingsBack = null;

    [SerializeField] InputKeys keySetA;
	[SerializeField] InputKeys keySetB;
    [SerializeField] InputKeys keySetC;
    [SerializeField] InputKeys keySetD;

    private new void Awake()
    {
		btnSettingsBack.onClick.AddListener(mainMenu.OnBtnBack);

		PlayerKeys keysA = brainiacs.PlayerPrefs.GetPlayerKeys(EKeyset.A);
		PlayerKeys keysB = brainiacs.PlayerPrefs.GetPlayerKeys(EKeyset.B);
		PlayerKeys keysC = brainiacs.PlayerPrefs.GetPlayerKeys(EKeyset.C);
		PlayerKeys keysD = brainiacs.PlayerPrefs.GetPlayerKeys(EKeyset.D);

		keySetA.Init(EKeyset.A, keysA);
		keySetB.Init(EKeyset.B, keysB);
		keySetC.Init(EKeyset.C, keysC);
		keySetD.Init(EKeyset.D, keysD);
	}
}

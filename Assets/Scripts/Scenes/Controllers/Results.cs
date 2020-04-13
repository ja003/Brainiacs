using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main controller of the result scene
/// </summary>
public class Results : CSingleton<Results>
{
	[SerializeField] Button btnClose = null;

	protected override void Awake()
	{
		btnClose.onClick.AddListener(Close);

		base.Awake();
	}

	public void Close()
	{
		brainiacs.Scenes.LoadScene(EScene.MainMenu);
	}
}

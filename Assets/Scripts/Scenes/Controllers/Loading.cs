using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
	[SerializeField]
	private Slider slider;

	private void Awake()
	{
		Brainiacs.InstantiateSingleton();

		Brainiacs.SelfInitGame = false;

	}

	public void TestLoad()
	{
		StartCoroutine(Brainiacs.Instance.Scenes.LoadSceneAsync(EScene.Game, slider));
	}

	public void TestStart()
	{
		Game.Instance.Activate();
		Brainiacs.Instance.Scenes.UnloadScene(EScene.Loading);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scenes : MonoBehaviour
{
	private int GetSceneIndex(EScene pScene)
	{
		return (int)pScene;
	}

	public void LoadScene(EScene pScene)
	{
		SceneManager.LoadScene(GetSceneIndex(pScene));
	}

	public IEnumerator LoadSceneAsync(EScene pScene, Slider pSlider, Action pOnLoaded)
	{
		AsyncOperation loading = SceneManager.LoadSceneAsync(
			GetSceneIndex(pScene), LoadSceneMode.Additive);

		while(!loading.isDone)
		{
			float progress = Mathf.Clamp01(loading.progress / 0.9f);
			pSlider.value = progress;
			Debug.Log(progress);
			yield return null;
		}

		pOnLoaded?.Invoke();
	}

	public void UnloadScene(EScene pScene)
	{
		SceneManager.UnloadSceneAsync(GetSceneIndex(pScene));
	}
}

public enum EScene
{
	Init,
	MainMenu,
	Loading,
	Game,
	Results
}
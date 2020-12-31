using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scenes : BrainiacsBehaviour
{
	private int GetSceneIndex(EScene pScene)
	{
		return (int)pScene;
	}

	public static EScene GetCurrentScene()
	{
		return (EScene)SceneManager.GetActiveScene().buildIndex;
	}

	public void LoadScene(EScene pScene)
	{
		brainiacs.PhotonManager.OnSceneChange();

		if(IsPhotonLoad(pScene))
		{
			//only master loads levels
			if(PhotonNetwork.IsMasterClient)
				PhotonNetwork.LoadLevel(GetSceneIndex(pScene));
		}
		else
			SceneManager.LoadScene(GetSceneIndex(pScene));
	}

	private bool IsPhotonLoad(EScene pScene)
	{
		if(!Brainiacs.Instance.GameInitInfo.IsMultiplayer())
			return false;

		switch(pScene)
		{
			case EScene.Init:
				return false;
			case EScene.MainMenu:
				return false;
			case EScene.Loading:
				return true;
			case EScene.Game:
				return true;
			case EScene.Results:
				return false;
		}
		return true;
	}

	//public IEnumerator LoadSceneAsync(EScene pScene, Slider pSlider, Action pOnLoaded)
	//{
	//	AsyncOperation loading = SceneManager.LoadSceneAsync(
	//		GetSceneIndex(pScene), LoadSceneMode.Additive);

	//	while(!loading.isDone)
	//	{
	//		float progress = Mathf.Clamp01(loading.progress / 0.9f);
	//		pSlider.value = progress;
	//		Debug.Log("LoadSceneAsync: " + progress);
	//		yield return null;
	//	}

	//	pOnLoaded?.Invoke();
	//}

	//IEnumerator LoadLevelAsync()
	//{
	//	PhotonNetwork.LoadLevel("Network Test");

	//	while(PhotonNetwork.LevelLoadingProgress < 1)
	//	{
	//		loadAmountText.text = "Loading: %" + (int)(PhotonNetwork.LevelLoadingProgress * 100);
	//		//loadAmount = async.progress;
	//		progressBar.fillAmount = PhotonNetwork.LevelLoadingProgress;
	//		yield return new WaitForEndOfFrame();
	//	}
	//}

	//public void UnloadScene(EScene pScene)
	//{
	//	SceneManager.UnloadSceneAsync(GetSceneIndex(pScene));
	//}
}

public enum EScene
{
	Init,
	MainMenu,
	Loading,
	Game,
	Results
}
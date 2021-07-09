using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerInfoController : GameController
{
	[SerializeField] private UIPlayerInfoElement prefab = null;

	protected override void OnMainControllerAwaken()
	{
		//client has to init after all players are added
		game.PlayerManager.OnAllPlayersAdded.AddAction(InitPlayersInfo);
	}

	protected override void OnMainControllerActivated()
	{
	}

	/// <summary>
	/// Called after all players are added
	/// </summary>
	private void InitPlayersInfo()
	{
		List<UIPlayerInfoElement> elements = new List<UIPlayerInfoElement>();
		foreach(var player in game.PlayerManager.Players)
		{
			elements.Add(AddPlayerInfo(player));
		}
		prefab.gameObject.SetActive(false);

		//easiest way to position tutorial on the first element
		//todo: or use UiCopyPosition
		tutorialPlayerInfo.transform.SetParent(elements[0].transform);
		tutorialPlayerInfo.transform.localPosition = Vector3.zero;
		DoInTime(() => tutorialPlayerInfo.transform.SetParent(game.Tutorial.transform), 0.1f);

	}

	[SerializeField] TutorialGame tutorialPlayerInfo;
	bool isFirstPlayerAdded;

	UIPlayerInfoElement AddPlayerInfo(Player pPlayer)
	{
		UIPlayerInfoElement instance = Instantiate(prefab, transform);
		instance.Init(pPlayer);

		if(!isFirstPlayerAdded)
		{
			tutorialPlayerInfo.SetFocusedObject(instance.weapon.gameObject);
			tutorialPlayerInfo.SetFocusedObject(instance.ammo.gameObject);
			tutorialPlayerInfo.SetFocusedObject(instance.health.gameObject);
		}

		isFirstPlayerAdded = true;
		return instance;
	}

}

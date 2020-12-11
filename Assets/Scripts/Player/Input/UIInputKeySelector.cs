using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Menu awaiting key press
/// </summary>
public class UIInputKeySelector : MonoBehaviour
{
	KeyCode selectedKey;

	private void Awake()
	{
		if(Time.time < 1)
			SetActive(false);
	}

	void Update()
	{
		foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
		{
			if(Input.GetKeyDown(key))
			{
				selectedKey = key;
			}
		}
	}

	private void SetActive(bool pActive)
	{
		selectedKey = KeyCode.None;
		gameObject.SetActive(pActive);

		//this menu has to be last item so it shows over other items
		transform.SetAsLastSibling();
	}

	public IEnumerator GetKeyPressed(Action<KeyCode> pSetFunction)
	{
		SetActive(true);

		while(selectedKey == KeyCode.None)
			yield return new WaitForEndOfFrame();

		pSetFunction.Invoke(selectedKey);
		SetActive(false);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugBtnClearPlayerPrefs : MonoBehaviour
{
	[SerializeField] Button button;
	// Start is called before the first frame update
	void Start()
	{
		button.onClick.AddListener(() =>
		{
			PlayerPrefs.DeleteAll();
			Debug.Log("Delete all");
		});
	}

	// Update is called once per frame
	void Update()
	{

	}
}

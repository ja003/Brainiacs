using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTest : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F7))
		{
			Debug.Log("F7 ");
			//--Do something here
		}
	}

	void OnGUI()
	{
		Event e = Event.current;
		if(e.isKey)
			Debug.Log("Detected key code: " + e.keyCode);
	}
}

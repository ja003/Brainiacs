using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Assigns position of selected object to this object
/// </summary>
public class UiCopyPosition : UiBehaviour
{
	[SerializeField] public RectTransform copyFrom;
	[SerializeField] public Vector2 offset;

	[SerializeField] bool useGlobalPosition;

	public void Update()
	{
		if(useGlobalPosition)
			rectTransform.position = copyFrom.position + Utils.GetVector3(offset);
		else
			rectTransform.anchoredPosition = copyFrom.anchoredPosition + offset;
		//rectTransform.position = copyFrom.position + Utils.GetVector3(offset);
			//* new Vector3(Screen.width, Screen.height, 0);
		//Debug.Log($"{gameObject.name} = {rectTransform.position}");
	}
}

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

	private void Update()
	{
		rectTransform.position = copyFrom.position + Utils.GetVector3(offset);
		//Debug.Log($"{gameObject.name} = {rectTransform.position}");
	}
}

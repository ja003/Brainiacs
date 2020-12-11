using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DragableElement : UiBehaviour, IBeginDragHandler, IDragHandler
{
	[SerializeField] RectTransform holder;
	[SerializeField] float minDragTime = 0.05f;

	float beginDragTime;

	public void OnBeginDrag(PointerEventData eventData)
	{
		beginDragTime = Time.time;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if(Time.time - beginDragTime < minDragTime)
			return;

		if(!Utils.IsWithinScreeen(Input.mousePosition))
			return;

		holder.position = Input.mousePosition - (rectTransform.position - holder.position);
	}
}

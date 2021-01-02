using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScalableElement : UiBehaviour, IBeginDragHandler, IDragHandler
{
	[SerializeField] RectTransform holder;
	[SerializeField] float minDragTime = 0.05f;

	float beginDragTime;
	float handleDistFromCenter;
	float scaleAtDragBegin;

	public void OnBeginDrag(PointerEventData eventData)
	{
		beginDragTime = Time.unscaledTime;
		handleDistFromCenter = Vector3.Distance(rectTransform.position, holder.position);
		scaleAtDragBegin = holder.localScale.x;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if(Time.unscaledTime - beginDragTime < minDragTime)
			return;

		if(!Utils.IsWithinScreeen(Input.mousePosition))
			return;

		float distFromCenter = Vector3.Distance(Input.mousePosition, holder.position);
		float scale = scaleAtDragBegin * distFromCenter / handleDistFromCenter;
		holder.localScale = scale * Vector2.one;
		return;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI element with layout:
/// |<| TEXT |>|
/// </summary>
public class UiTextSwapper : MonoBehaviour
{
	[SerializeField] Text text;
	[SerializeField] Button btnNext;
	[SerializeField] Button btnPrevious;

	List<string> values;
	public int CurrentIndex { get; private set; }
	public Action OnValueChanged { get; private set; }

	private void Awake()
	{
		btnNext.onClick.AddListener(OnBtnNext);
		btnPrevious.onClick.AddListener(OnBtnPrevious);
	}

	/// <summary>
	/// Geneartes text values from number sequence
	/// </summary>
	public void InitNumberSwapper(int pMin, int pMax, Action pOnValueChanged = null)
	{
		values = new List<string>();
		for(int i = pMin; i <= pMax; i++)
		{
			values.Add(i.ToString());
		}
		Init(values, pOnValueChanged);
	}

	public void Init(List<string> pValues, Action pOnValueChanged = null, int pSkippedIndex = -1)
	{
		skippedIndex = pSkippedIndex;
		values = pValues;
		OnValueChanged = pOnValueChanged;
		SetValue(0);
		SetEnabled(true);
	}


	internal void SetEnabled(bool pValue)
	{
		btnNext.interactable = pValue;
		btnPrevious.interactable = pValue;
	}

	int skippedIndex = -1;

	private void SetValue(int pIndex)
	{
		if(pIndex >= values.Count)
		{
			pIndex %= values.Count;
		}

		if(pIndex == skippedIndex)
		{
			//Debug.Log("Skipping index " + pIndex);
			SetValue(pIndex + 1);
			return;
		}

		CurrentIndex = pIndex;
		text.text = values[pIndex];
		OnValueChanged?.Invoke();
	}

	private void OnBtnPrevious()
	{
		SetValue(CurrentIndex - 1);
	}

	private void OnBtnNext()
	{
		SetValue(CurrentIndex + 1);
	}

}

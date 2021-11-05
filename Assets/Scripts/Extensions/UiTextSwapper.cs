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
	[SerializeField] Text text = null;
	[SerializeField] public Button btnNext = null;
	[SerializeField] public Button btnPrevious = null;
	[SerializeField] public Animator animator;
	[SerializeField] int initialValueIndex;

	List<string> values;
	public int CurrentIndex { get; private set; }
	public Action OnValueChanged { get; private set; }
	public object SetIntValuebrainiacs { get; internal set; }

	private void Awake()
	{
		btnNext?.onClick.AddListener(OnBtnNext);
		btnPrevious?.onClick.AddListener(OnBtnPrevious);
	}

	int min;
	int max;
	bool isNumber; //todo: implement UiNumberSwapper

	/// <summary>
	/// Geneartes text values from number sequence
	/// </summary>
	public void InitNumberSwapper(int pMin, int pMax, Action pOnValueChanged = null)
	{
		isNumber = true;
		min = pMin;
		max = pMax;

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
		SetValue(initialValueIndex);
		SetInteractable(true);
	}


	internal void SetInteractable(bool pValue)
	{
		if(btnNext != null)
			btnNext.interactable = pValue;
		if(btnPrevious != null)
			btnPrevious.interactable = pValue;
	}

	int skippedIndex = -1;

	public bool SetNumberValue(int pValue)
	{
		if(!isNumber)
		{
			Debug.LogError("Setting number value to non-number swapper");
			return false;
		}
		pValue = Mathf.Clamp(pValue, min, max);

		int index = pValue - min;
		return SetValue(index);
	}


	public bool SetValue(int pIndex, bool pIsIncrement = true)
	{
		pIndex = Mathf.Clamp(pIndex, 0, pIndex %= values.Count);

		if(pIndex == skippedIndex)
		{
			//Debug.Log("Skipping index " + pIndex);
			int newIndex = pIndex + (pIsIncrement ? 1 : -1);
			newIndex = Utils.PositiveModulo(newIndex, values.Count);
			return SetValue(newIndex);
		}

		bool isChange = CurrentIndex != pIndex;
		CurrentIndex = pIndex;
		text.text = values[pIndex];
		if(isChange)
			OnValueChanged?.Invoke();

		return isChange;
	}

	public void SetNextValue()
	{
		SetValue(CurrentIndex + 1);
	}

	private void OnBtnPrevious()
	{
		SetValue(CurrentIndex - 1, false);
		PlayAnimation(false);
	}


	private void OnBtnNext()
	{
		SetNextValue();
		PlayAnimation(true);
	}

	public void PlayAnimation(bool pNext)
	{
		if(animator == null)
			return;

		animator?.SetTrigger(pNext ? "next" : "previous");
	}
}

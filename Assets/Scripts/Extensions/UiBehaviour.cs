using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiBehaviour : MonoBehaviour
{
	protected Brainiacs brainiacs => Brainiacs.Instance;
	protected Game game => Game.Instance;
	
	private RectTransform _rectTransform;
	protected RectTransform rectTransform
	{
		get
		{
			if(_rectTransform == null)
				_rectTransform = GetComponent<RectTransform>();
			return _rectTransform;
		}
	}

	private Animator _animator;
	protected Animator animator
	{
		get
		{
			if(_animator == null)
				_animator = GetComponent<Animator>();
			return _animator;
		}
	}

	private Image _image;
	protected Image image
	{
		get
		{
			if(_image == null)
				_image = GetComponent<Image>();
			return _image;
		}
	}

	private bool awaken;
	private List<Action> onAwaken = new List<Action>();

	private bool activated;
	private List<Action> onActivated = new List<Action>();

	protected virtual void Awake()
	{
		awaken = true;
		SetOnAwaken();
	}

	protected void Activate()
	{
		activated = true;
		SetOnActivated(); //invoke OnMainControllerActivated actions
	}

	public void SetOnAwaken(Action pAction = null)
	{
		if(pAction != null)
			onAwaken.Add(pAction);
		//onAwake.Add(pAction);

		if(awaken)
		{
			for(int i = onAwaken.Count - 1; i >= 0; i--)
			{
				if(i >= onAwaken.Count)
					continue;
				Action action = onAwaken[i];
				onAwaken.RemoveAt(i);
				action.Invoke();
			}
			onAwaken.Clear();
		}
	}

	public void SetOnActivated(Action pAction = null)
	{
		//Debug.Log($"{gameObject.name} SetOnActivated {activated}");
		if(pAction != null)
			onActivated.Add(pAction);
		//OnMainControllerActivated.Add(pAction);

		if(activated)
		{
			for(int i = onActivated.Count - 1; i >= 0; i--)
			{
				//NOTE: onActivated count can be changed within invoked action
				if(i >= onActivated.Count)
					continue;

				Action action = onActivated[i];
				onActivated.RemoveAt(i);
				action.Invoke();
			}
			onActivated.Clear();
		}
	}
}

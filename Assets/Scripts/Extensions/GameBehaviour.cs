using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameBehaviour : MonoBehaviour
{
	protected Brainiacs brainiacs => Brainiacs.Instance;
	protected Game game => Game.Instance;

	protected Renderer rend;

	//TODO: implement other getters
	private SpriteRenderer _spriteRend;
	protected SpriteRenderer spriteRend
	{
		get
		{
			if(_spriteRend == null)
				_spriteRend = GetComponent<SpriteRenderer>();
			return _spriteRend;
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

	private BoxCollider2D _boxCollider2D;
	protected BoxCollider2D boxCollider2D
	{
		get
		{
			if(_boxCollider2D == null)
				_boxCollider2D = GetComponent<BoxCollider2D>();
			return _boxCollider2D;
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
	//private UnityAction onAwake;
	private List<Action> onAwaken = new List<Action>();

	private bool activated;
	//private UnityAction onActivated;
	//private Action onActivated;
	private List<Action> onActivated = new List<Action>();
	//private List<UnityAction> onGameActivated = new List<UnityAction>();

	protected virtual void Awake()
	{
		rend = GetComponent<Renderer>();
		awaken = true;
		SetOnAwaken();
	}

	protected void Activate()
	{
		activated = true;
		SetOnActivated(); //invoke onGameActivated actions
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
		//onGameActivated.Add(pAction);

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

	protected void DoInTime(Action pEvent, float pTime, Action<float> pOnUpdate = null)
	{
		if(pOnUpdate == null)
			LeanTween.value(0, 1, pTime).setOnComplete(pEvent);
		else
		{
			LeanTween.value(0, 1, pTime)
				  .setOnComplete(pEvent).setOnUpdate(pOnUpdate);
		}
	}
}

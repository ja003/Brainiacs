using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BrainiacsBehaviour : MonoBehaviour
{
	protected Brainiacs brainiacs => Brainiacs.Instance;

	private Renderer _rend;
	protected Renderer rend
	{
		get
		{
			if(_rend == null)
				_rend = GetComponent<Renderer>();
			return _rend;
		}
	}

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

	private Animation _anim;
	protected Animation anim
	{
		get
		{
			if(_anim == null)
				_anim = GetComponent<Animation>();
			return _anim;
		}
	}

	protected bool awaken;
	private List<Action> onAwaken = new List<Action>();

	private bool activated;
	private List<Action> onActivated = new List<Action>();

	protected virtual void Awake()
	{
		awaken = true;
		Brainiacs.InstantiateSingleton();
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

	protected void DoInTime(Action pEvent, float pTime, Action<float> pOnUpdate = null)
	{
		if(pOnUpdate == null)
			LeanTween.value(gameObject, 0, 1, pTime).setOnComplete(pEvent);
		else
		{
			LeanTween.value(gameObject, 0, 1, pTime)
				  .setOnComplete(pEvent).setOnUpdate(pOnUpdate);
		}
	}

	protected void UpdateValue(float pForm, float pTo, float pTime, Action<float> pOnUpdate)
	{
		LeanTween.value(gameObject, pForm, pTo, pTime).setOnUpdate(pOnUpdate);
	}

	private void OnDestroy()
	{
		LeanTween.cancel(gameObject);
	}
}

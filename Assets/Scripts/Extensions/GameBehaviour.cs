using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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


	private bool awaken;
	private UnityAction onAwake;
	//private List<UnityAction> onAwake = new List<UnityAction>();

	private bool activated;
	private UnityAction onGameActivated;
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

	public void SetOnAwaken(UnityAction pAction = null)
	{
		if(pAction != null)
			onAwake += pAction;
		//onAwake.Add(pAction);

		if(awaken)
		{
			onAwake?.Invoke();
			onAwake = null;
			//foreach(UnityAction action in onAwake)
			//{
			//	action.Invoke();
			//}
			//onAwake.Clear();
		}
	}

	public void SetOnActivated(UnityAction pAction = null)
	{
		//Debug.Log($"{gameObject.name} SetOnActivated {onGameActivated}, {activated}");
		if(pAction != null)
			onGameActivated += pAction;
		//onGameActivated.Add(pAction);

		if(activated)
		{
			onGameActivated?.Invoke();
			onGameActivated = null;
			//foreach(UnityAction action in onGameActivated)
			//{
			//	action.Invoke();
			//}
			//onGameActivated.Clear();
		}
	}

	protected void DoInTime(Action pEvent, float pTime)
	{
		LeanTween.value(0, 1, pTime).setOnComplete(pEvent);
	}


}

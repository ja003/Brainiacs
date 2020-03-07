/*
//=========================================//
AUTHOR:		Karel Brezina
DATE:		2018-08-21
FUNCTION:	
//=========================================//
*/

using System;
using UnityEngine;

public abstract class CSingleton<T> : BrainiacsBehaviour where T : CSingleton<T>
{
	//=========================================//
	// VARIABLES
	//=========================================//

	public static bool IsInstantiated { get; private set; }
	public static bool IsDestroyed { get; private set; }
	private static T _instance;

	//=========================================//
	// GET & SET
	//=========================================//

	public static T Instance
	{
		get
		{

			InstantiateSingleton();
			return _instance;
		}
	}

	public static bool IsNull => _instance == null;

	//=========================================//
	// UNITY METHODS
	//=========================================//

	protected override void Awake()
	{
		CSingletionAttribute attribute = Attribute.GetCustomAttribute(typeof(T), typeof(CSingletionAttribute)) as CSingletionAttribute;
		bool persistent = false;
		// Get set attribute
		if(attribute != null)
		{
			persistent = attribute.IsPersistent;
		}
		// Create instance
		if(_instance == null)
		{
			InstantiateSingleton();
			// Is this singleton marked to live forever?
			if(persistent)
			{
				DontDestroyOnLoad(this.gameObject);
			}

			base.Awake();
			return;
		}
		// Is this singleton marked to live forever?
		if(persistent)
		{
			DontDestroyOnLoad(this.gameObject);
		}
		// Isn't this singleton first selected?
		if(this.GetInstanceID() != _instance.GetInstanceID())
		{
			Destroy(this.gameObject);
		}
		base.Awake();
	}

	protected virtual void OnDestroy()
	{
		IsInstantiated = false;
		_instance = null;
	}

	protected virtual void OnApplicationQuit()
	{
		IsDestroyed = true;
	}

	//=========================================//
	// PUBLIC METHODS
	//=========================================//

	/// <summary>
	/// Instantiate singleton if not yet
	/// </summary>
	public static void InstantiateSingleton()
	{
		// Is inited?
		if(IsInstantiated || IsDestroyed)
		{
			return;
		}
		// Check if singleton exists in scene yet
		Type type = typeof(T);
		T[] singletons = FindObjectsOfType<T>();

		int singletonsMax = singletons.Length;
		if(singletonsMax > 0)
		{
			if(singletonsMax > 1)
			{
				Debug.LogWarning("In scene is more than one singleton of type " + type + ". First will stay, others die!");
				for(int i = 1; i < singletonsMax; i++)
				{
					Destroy(singletons[i].gameObject);
				}
			}
			// Set first as live singleton
			_instance = singletons[0];
			_instance.gameObject.SetActive(true);
			_instance.Init();

			IsInstantiated = true;
			IsDestroyed = false;

			return;
		}
		// Set as inited
		IsInstantiated = true;
		IsDestroyed = false;
		// Create singleton gameobject into scene
		GameObject go;
		CSingletionAttribute attribute = Attribute.GetCustomAttribute(type, typeof(CSingletionAttribute)) as CSingletionAttribute;
		// If class doesn't have singleton attribute
		if(attribute == null || string.IsNullOrEmpty(attribute.Name))
		{
			go = new GameObject();
		}
		else
		{
			string prefabName = attribute.Name;
			go = Instantiate(Resources.Load<GameObject>(prefabName));
			if(go == null)
			{
				Debug.LogError("Couldn't find singleton prefab in Resources dir. Name: " + prefabName + " | Type: " + type);
			}
		}
		// Set name 
		if(attribute == null)
		{
			go.name = "-S-" + type.Name;
		}
		else
		{
			go.name = "-S" + (attribute.IsPersistent ? "P-" : "-") + type.Name;
		}
		// Set inherited script as component
		if(_instance == null)
		{
			_instance = go.GetComponent<T>() ?? go.AddComponent<T>();
		}

		_instance.Init();
	}

	//=========================================//
	// PROTECTED METHODS
	//=========================================//

	protected virtual void Init() { }
}

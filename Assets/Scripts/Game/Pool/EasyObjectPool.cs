/* 
 * Unless otherwise licensed, this file cannot be copied or redistributed in any format without the explicit consent of the author.
 * (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//namespace MarchingBytes {

[System.Serializable]
public class PoolInfo
{
	//public string poolName //change by me - no reason to specify name => use prefab name
	public string poolName => prefab.name;
	public GameObject prefab;
	public int poolSize;
	public bool fixedSize;

	//my extension
	public Transform holder;
}

class Pool
{
	//private Stack<PoolObject> availableObjStack = new Stack<PoolObject>();
	//MINE
	//we need to check more elements than just the top one - for mine photon
	private List<PoolObject> availableObjs = new List<PoolObject>();

	private bool fixedSize;
	private GameObject poolObjectPrefab;
	private int poolSize;
	private string poolName;

	public Transform holder;

	public Pool(string poolName, GameObject poolObjectPrefab, int initialCount, bool fixedSize, Transform pHolder)
	{
		this.poolName = poolName;
		this.poolObjectPrefab = poolObjectPrefab;
		this.poolSize = initialCount;
		this.fixedSize = fixedSize;
		this.holder = pHolder;
		//populate the pool
		for(int index = 0; index < initialCount; index++)
		{
			AddObjectToPool(NewObjectInstance());
		}
	}

	//o(1)
	private void AddObjectToPool(PoolObject po)
	{
		//Debug.Log("AddObjectToPool " + po.name);

		//add to pool
		po.SetActive(false);
		//availableObjStack.Push(po);
		availableObjs.Add(po);
		po.isPooled = true;
		if(holder)
			po.transform.parent = holder;
	}

	private PoolObject NewObjectInstance()
	{
		GameObject go = GameObject.Instantiate(poolObjectPrefab);
		//GameObject go = InstanceFactory.Instantiate(poolObjectPrefab);
		PoolObject po = go.GetComponent<PoolObject>();
		if(po == null)
		{
			po = go.AddComponent<PoolObject>();
		}
		if(po == null)
		{
			Debug.LogError(go.name + " doesnt have PoolObject");
		}
		po.OnInstantiated();

		//set name
		po.poolName = poolName;
		return po;
	}

	//o(1)
	public GameObject NextAvailableObject(Vector3 position, Quaternion rotation)
	{
		//Debug.Log("NextAvailableObject");
		PoolObject po = null;
		//if(availableObjStack.Count > 0)
		//TODO: changed by me
		//always pool object with photon mine.
		//todo: change structure from stack, not effective right now
		//if(availableObjStack.Count > 0 && availableObjStack.Peek().Photon.IsMine)
		//{
		//	po = availableObjStack.Pop();
		//}

		//find available obj that is mine
		for(int i = availableObjs.Count - 1; i >= 0 ; i--)
		{
			if(availableObjs[i].Photon.IsMine)
			{
				po = availableObjs[i];
				availableObjs.RemoveAt(i);
				break;
			}
		}

		//PoolObject availableObj = availableObjs.Find(a => a.Photon.IsMine);
		//if(availableObj)
		//{
		//	po = availableObj;
		//	availableObjs.Remove(po);
		//}
		if(po)
		{
			//available obj found
		}
		else if(fixedSize == false)
		{
			//increment size var, this is for info purpose only
			poolSize++;
			//Debug.Log(string.Format("Growing pool {0}. New size: {1}", poolName, poolSize));
			//create new object
			po = NewObjectInstance();
			AddObjectToPool(po);
			//bug: po is added to availableObjStack, but not popped
			//po = availableObjStack.Pop();
			availableObjs.RemoveAt(availableObjs.Count - 1);
		}
		else
		{
			Debug.LogWarning("No object available & cannot grow pool: " + poolName);
		}

		GameObject result = null;
		if(po != null)
		{
			//Debug.Log("NextAvailableObject " + po.name);
			po.isPooled = false;
			result = po.gameObject;
			//result.SetActive(true);

			result.transform.position = position;
			result.transform.rotation = rotation;
		}

		return result;
	}

	//o(1)
	public void ReturnObjectToPool(PoolObject po)
	{

		if(poolName.Equals(po.poolName))
		{

			/* we could have used availableObjStack.Contains(po) to check if this object is in pool.
			 * While that would have been more robust, it would have made this method O(n) 
			 */
			if(po.isPooled)
			{
				Debug.LogWarning(po.gameObject.name + " is already in pool. Why are you trying to return it again? Check usage.");
			}
			else
			{
				AddObjectToPool(po);
			}

		}
		else
		{
			Debug.LogError(string.Format("Trying to add object to incorrect pool {0} {1}", po.poolName, poolName));
		}
	}
}

/// <summary>
/// Easy object pool.
/// </summary>
public class EasyObjectPool : MonoBehaviour
{

	public static EasyObjectPool instance;
	[Header("Editing Pool Info value at runtime has no effect")]
	public PoolInfo[] poolInfo;

	//mapping of pool name vs list
	private Dictionary<string, Pool> poolDictionary = new Dictionary<string, Pool>();

	// Use this for initialization
	//void Start () {
	//	//set instance
	//	instance = this;
	//	//check for duplicate names
	//	CheckForDuplicatePoolNames();
	//	//create pools
	//	CreatePools();
	//}

	public void Init()
	{
		//set instance
		instance = this;
		//check for duplicate names
		CheckForDuplicatePoolNames();
		//create pools
		CreatePools();
	}

	private void CheckForDuplicatePoolNames()
	{
		for(int index = 0; index < poolInfo.Length; index++)
		{
			string poolName = poolInfo[index].poolName;
			if(poolName.Length == 0)
			{
				Debug.LogError(string.Format("Pool {0} does not have a name!", index));
			}
			for(int internalIndex = index + 1; internalIndex < poolInfo.Length; internalIndex++)
			{
				if(poolName.Equals(poolInfo[internalIndex].poolName))
				{
					Debug.LogError(string.Format("Pool {0} & {1} have the same name. Assign different names.", index, internalIndex));
				}
			}
		}
	}

	private void CreatePools()
	{
		foreach(PoolInfo currentPoolInfo in poolInfo)
		{

			Pool pool = new Pool(currentPoolInfo.poolName, currentPoolInfo.prefab,
								 currentPoolInfo.poolSize, currentPoolInfo.fixedSize,
								 currentPoolInfo.holder);


			//Debug.Log("Creating pool: " + currentPoolInfo.poolName);
			//add to mapping dict
			poolDictionary[currentPoolInfo.poolName] = pool;
		}
	}


	/* Returns an available object from the pool 
	OR 
	null in case the pool does not have any object available & can grow size is false.
	*/
	public GameObject GetObjectFromPool(string poolName, Vector3 position, Quaternion rotation)
	{
		GameObject result = null;

		if(poolDictionary.ContainsKey(poolName))
		{
			Pool pool = poolDictionary[poolName];
			result = pool.NextAvailableObject(position, rotation);
			//scenario when no available object is found in pool
			if(result == null)
			{
				Debug.LogWarning("No object available in pool. Consider setting fixedSize to false.: " + poolName);
			}

		}
		else
		{
			Debug.LogError("Invalid pool name specified: " + poolName);
		}

		return result;
	}

	public void ReturnObjectToPool(GameObject go)
	{
		PoolObject po = go.GetComponent<PoolObject>();
		if(po == null)
		{
			Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
		}
		else
		{
			if(poolDictionary.ContainsKey(po.poolName))
			{
				Pool pool = poolDictionary[po.poolName];
				pool.ReturnObjectToPool(po);
				po.OnReturnToPool();
			}
			else
			{
				Debug.LogWarning("No pool available with name: " + po.poolName);
			}
		}
	}
}
//}

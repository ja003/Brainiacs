using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItemManager : GameBehaviour
{
    [SerializeField] MapItemGenerator generator = null;

    public List<MapItem> ActiveItems = new List<MapItem>();

	public void RegisterItem(MapItem pItem)
	{
		//Debug.Log("Add projectile " + pProjectile);
		ActiveItems.Add(pItem);
	}
	public void OnDestroyItem(MapItem pItem)
	{
		//Debug.Log("Remove projectile " + pProjectile);
		ActiveItems.Remove(pItem);
	}

	public void Init()
    {
        if(brainiacs.PhotonManager.IsMaster())
            generator.Init();
    }
}

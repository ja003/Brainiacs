//using MarchingBytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : GameController, IPunPrefabPool
{
    [SerializeField] EasyObjectPool pool = null;

    //[SerializeField] PoolManagerPhoton photon;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.PrefabPool = this;
    }

    protected override void OnMainControllerAwaken()
    {
        pool.Init();
    }

    //public GameObject Instantiate(GameObject pPrefab)
    //{
    //    return Instantiate(pPrefab, Vector3.zero);
    //}

    //public GameObject Instantiate(GameObject pPrefab, Vector3 pPosition)
    //{
    //    return Instantiate(pPrefab.name, pPosition);
    //}

    //public GameObject Instantiate(string prefabId, Vector3 position)
    //{
    //    return Instantiate(prefabId, position, Quaternion.identity);
    //}

    /// <summary>
    /// Always call PhotonNetwork.Instantiate, not this method directly
    /// </summary>
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        return CheckInstance(pool.GetObjectFromPool(prefabId, position, rotation));
    }

    private GameObject CheckInstance(GameObject pInstance)
    {
        //Debug.Log("CheckInstance " + pInstance.name);

        if(pInstance.activeSelf)
        {
            Debug.LogError("Prefab has to be disabled " + pInstance.name);
        }
        PoolObject poolObject = pInstance.GetComponent<PoolObject>();
        if(!poolObject)
        {
            Debug.LogError("Prefab is not type of PoolObject " + pInstance.name);
        }

        //if(poolObject.EnableOnInstanced)
        //    poolObject.SetActive(true);

        return pInstance;
    }

    //public void Return(PoolObject pObject){
    //    Destroy(pObject);
    //    //if(pObject.DisableOnDestroy)
    //    pObject.SetActive(false);
    //}

    /// <summary>
    /// Always call PhotonNetwork.Destroy, not this method directly
    /// </summary>
    public void Destroy(GameObject gameObject)
    {
        //todo: implement PoolObject Destroy/ReturnToPool photon call
        pool.ReturnObjectToPool(gameObject);
        //pObject.SetActive(false); //no need, it is deactivated once returned to pool

        //PhotonView view = gameObject.GetComponent<PhotonView>();
        //if(view)
        //    photon.Send(EPhotonMsg.Pool_Disable, view.ViewID);
    }


}

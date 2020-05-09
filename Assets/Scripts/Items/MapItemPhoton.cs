using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItemPhoton : PoolObjectPhoton
{
    [SerializeField] MapItem item = null;

    public override void OnReturnToPool()
    {
        //throw new System.NotImplementedException();
    }

    protected override bool CanSend2(EPhotonMsg pMsgType)
    {
        switch(pMsgType)
        {
            //master initiates
            case EPhotonMsg.MapItem_InitMapSpecial:
            case EPhotonMsg.MapItem_InitMapBasic:
            case EPhotonMsg.MapItem_InitPowerUp:
                return view.IsMine;

            //only master can return items to pool (master owns all map items)
            //case EPhotonMsg.MapItem_ReturnToPool:
            //    return !view.IsMine;
        }

        return false;
    }

    protected override void HandleMsg2(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
    {
        switch(pReceivedMsg)
        {
            case EPhotonMsg.MapItem_InitMapBasic:
                Vector3 pos = (Vector3)pParams[0];
                EWeaponId id = (EWeaponId)pParams[1];

                item.Init(pos, brainiacs.ItemManager.GetMapWeaponConfig(id));
                break;

            case EPhotonMsg.MapItem_InitMapSpecial:
                pos = (Vector3)pParams[0];
                id = (EWeaponId)pParams[1];

                item.Init(pos, brainiacs.ItemManager.GetMapSpecialWeaponConfig(id));
                break;

            case EPhotonMsg.MapItem_InitPowerUp:
                pos = (Vector3)pParams[0];
                EPowerUp type = (EPowerUp)pParams[1];

                item.Init(pos, brainiacs.ItemManager.GetPowerupConfig(type));
                break;

            //case EPhotonMsg.MapItem_ReturnToPool:
            //    item.ReturnToPool();
            //    break;

            default:
                OnMsgUnhandled(pReceivedMsg);
                break;
        }
    }

    protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
    {
    }
}

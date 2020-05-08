using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialDaVinciPhoton : PlayerWeaponSpecialPrefabPhoton
{
    protected override bool CanSend3(EPhotonMsg pMsgType)
    {
        return true;
    }

    protected override void HandleMsg3(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
    {
        
    }
}

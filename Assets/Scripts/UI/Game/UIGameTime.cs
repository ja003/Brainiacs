using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameTime : GameBehaviour
{
    [SerializeField] private Text textTime = null;

    EGameMode mode => brainiacs.GameInitInfo.Mode;

    public void UpdateTime(int pTimePassed, int pTimeLeft)
    {
        int showTimeValue;
        if(mode == EGameMode.Time)
        {
            showTimeValue = pTimeLeft;
        }
        else
        {
            showTimeValue = pTimePassed;
        }

        ShowTimeValue(showTimeValue);
    }

    public void ShowTimeValue(int pTime)
    {
        TimeSpan time = TimeSpan.FromSeconds(pTime);
        string timeText = time.ToString(@"m\:ss");
        textTime.text = timeText;

        game.Photon.Send(EPhotonMsg.Game_Ui_ShowTimeValue, pTime);
    }
}

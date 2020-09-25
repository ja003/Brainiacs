using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Only UI indication of a connection status
/// </summary>
public class UIConnectionIndicator : UiBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Text status;

    private new void Awake()
    {
        SetState(EConnectState.Offline);
    }

    private void Update()
    {
        //we focus only on some connection states
        switch(PhotonNetwork.NetworkClientState)
        {
            case Photon.Realtime.ClientState.JoiningLobby:
                SetState(EConnectState.Connecting);
                break;
            case Photon.Realtime.ClientState.JoinedLobby:
                SetState(EConnectState.Online);
                break;
            case Photon.Realtime.ClientState.Disconnecting:
            case Photon.Realtime.ClientState.Disconnected:
                SetState(EConnectState.Offline);
                break;
		}
	}

    EConnectState lastState;
    float lastChangeStateTime;

    private void SetState(EConnectState pState)
    {
        if(lastState == pState)
            return;

        //set delay so that state is not changed too quickly
        float delay = lastState == EConnectState.None ? 0 : lastChangeStateTime - Time.time + 1;
        delay = Mathf.Max(0, delay);

        lastState = pState;
        //Debug.Log($"Set state {pState} in {delay} | {Time.time}");
        DoInTime(() => UpdateUI(pState), delay);
        lastChangeStateTime = Time.time + delay;

        void UpdateUI(EConnectState pNewState)
        {
            switch(pNewState)
            {
                case EConnectState.Offline:
                    background.color = Color.red;
                    status.text = "Offline";
                    break;
                case EConnectState.Connecting:
                    background.color = Color.magenta;
                    status.text = "Connecting...";
                    break;
                case EConnectState.Online:
                    background.color = Color.green;
                    status.text = "Online";
                    break;
            }
        }
    }

    private enum EConnectState
    {
        None,
        Offline,
        Connecting,
        Online
    }
}
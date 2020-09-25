using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles showing of info messages in mid-bottom of the screen.
/// Eg.: Game started, Player has left, Player eliminated, ...
/// </summary>
public class UIInfoMessenger : GameBehaviour
{
    [SerializeField] UIInfoMessage infoMessagePrefab;

    //protected override void Awake()
    //{
    //    DoInTime(() => Show("TEST test"), 1);
    //    base.Awake();
    //}

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            Show("HHH " + Time.frameCount);
        }
    }

    public void Show(string pText, float pDuration = 3)
    {
        if(game.GameEnd.GameEnded)
        {
            //Debug.Log("Dont show message after game ended");
            return;
        }

        //Debug.Log("Show " + pText);
        UIInfoMessage msgInstance = InstanceFactory
            .Instantiate(infoMessagePrefab.gameObject).GetComponent<UIInfoMessage>();
        msgInstance.Show(pText, pDuration);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : BrainiacsBehaviour
{

#if UNITY_EDITOR
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            SetTimeScale(0);
        }
        else if(Input.GetKeyDown(KeyCode.F2))
        {
            SetTimeScale(0.5f);
        }
        else if(Input.GetKeyDown(KeyCode.F3))
        {
            SetTimeScale(1);
        }
        else if(Input.GetKeyDown(KeyCode.F4))
        {
            SetTimeScale(2);
        }
        else if(Input.GetKeyDown(KeyCode.F5))
        {
            SetTimeScale(5);
        }
    }
#endif

    private static void SetTimeScale(float pValue)
    {
        Time.timeScale = pValue;
        Debug.Log("SetTimeScale " + pValue);
    }
}

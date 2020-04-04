using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Holder of the actions.
/// After Invoke is called all actions are invoked.
/// If any action is added after Invoke, it is invoked instantly.
/// </summary>
public class ActionControl
{
    private Action action;
    bool invoked;

    public void AddAction(Action pAction)
    {
        action += pAction;
        if(invoked)
            Invoke();
    }

    public void Invoke()
    {
        action?.Invoke();
        action = null;
        invoked = true;
    }
}

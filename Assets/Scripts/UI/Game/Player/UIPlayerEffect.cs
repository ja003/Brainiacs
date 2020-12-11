using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerEffect : UiBehaviour
{
    [SerializeField] GameObject doubleSpeed = null;
    [SerializeField] GameObject halfSpeed = null;
    [SerializeField] GameObject doubleDamage = null;
    [SerializeField] GameObject halfDamage = null;
    [SerializeField] GameObject shield = null;

    internal void SetEffectActive(EPlayerEffect pType, bool pState)
    {
        GetIconObject(pType).SetActive(pState);
    }

    public bool IsEffectActive(EPlayerEffect pType)
    {
        return GetIconObject(pType).activeSelf;
    }

    private GameObject GetIconObject(EPlayerEffect pType)
    {
        switch(pType)
        {
            case EPlayerEffect.None:
                break;
            case EPlayerEffect.DoubleSpeed:
                return doubleSpeed;
            case EPlayerEffect.HalfSpeed:
                return halfSpeed;
            case EPlayerEffect.Shield:
                return shield;
            case EPlayerEffect.DoubleDamage:
                return doubleDamage;
            case EPlayerEffect.HalfDamage:
                return halfDamage;
            default:
                break;
        }
        Debug.LogError($"Effect {pType} not found");
        return null;
    }
}

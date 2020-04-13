using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICurtain : GameBehaviour
{
    protected override void Awake()
    {
        if(!Brainiacs.SelfInitGame)
        {
            SetAlpha(1);
        }

        base.Awake();
    }

    public void SetFade(bool pIn, Action pOnComplete)
    {

        int startAlpha = pIn ? 0 : 1;
        SetAlpha(startAlpha);

        LeanTween.alpha(image.rectTransform, pIn ? 1 : 0, 1).setOnComplete(pOnComplete);
    }

    private void SetAlpha(int pAlpha)
    {
        image.enabled = true;
        image.color = new Color(image.color.r, image.color.g, image.color.b, pAlpha);
    }
}

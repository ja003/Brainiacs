using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonObject : UiBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected override void Awake()
    {
        button.onClick.AddListener(OnClick);
        base.Awake();
    }

    private void OnClick()
    {
        brainiacs.AudioManager.PlaySound(ESound.Ui_Button_Click, null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        brainiacs.AudioManager.PlaySound(ESound.Ui_Button_Hover, null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        brainiacs.AudioManager.PlaySound(ESound.Ui_Button_Hover, null);
    }
}

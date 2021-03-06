﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager : GameController
{
    [SerializeField] Button btnPause;
    [SerializeField] PauseMenu pause;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pause.SetActive(!pause.isActiveAndEnabled);
        }
    }

    protected override void OnMainControllerAwaken()
    {
        pause.SetActive(false);
        btnPause.onClick.AddListener(() => pause.SetActive(true));

        pause.Init();
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameVersion : MonoBehaviour
{
	[SerializeField] Text txtGameVersion;

	private void Start()
	{
		txtGameVersion.text = Application.version;
	}
}

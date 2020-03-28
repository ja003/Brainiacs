﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BrainiacsController : ControllerBehaviour
{

	protected Brainiacs brainiacs => Brainiacs.Instance;

	protected override BrainiacsBehaviour GetMainController()
	{
		return brainiacs;
	}
}
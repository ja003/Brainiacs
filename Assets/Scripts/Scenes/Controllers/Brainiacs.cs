using System.Collections;
using System.Collections.Generic;
using UnityEngine;











[CSingletion("Singletons/P_Brainiacs", true)]
public class Brainiacs : CSingleton<Brainiacs>
{
	[SerializeField]
	public Scenes Scenes;

	public static bool SelfInitGame = true;
}

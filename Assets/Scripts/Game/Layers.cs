using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layers : MonoBehaviour
{
    [SerializeField] public LayerMask Player;
    //cant be walked on, projectiles collide
    [SerializeField] public LayerMask MapObject;
    //cant be walked on, projectiles dont collide
    [SerializeField] public LayerMask Unwalkable;

    public static LayerMask UnwalkableObject => Game.Instance.Layers.MapObject | Game.Instance.Layers.Unwalkable;

}

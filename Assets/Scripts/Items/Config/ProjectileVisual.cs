using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
//todo: Animator
public class ProjectileVisual : GameBehaviour
{
	public BoxCollider2D GetCollider() { return boxCollider2D; }
	public Sprite GetSprite() { return spriteRend.sprite; }
}

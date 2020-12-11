using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Scripts attached to P_Player object.
/// If not attached dirrectly to the player object, _player ref has to be setup.
/// Gives access to player related scripts.
/// </summary>
public abstract class PlayerBehaviour : GameBehaviour
{
	[SerializeField]
	private Player _player;
	[DebuggerHidden]
	protected Player player
	{
		get
		{
			if(_player == null)
				_player = GetComponent<Player>();
			return _player;
		}
	}

	protected PlayerMovement movement => player.Movement;
	protected PlayerWeaponController weapon => player.WeaponController;
	protected PlayerStats stats => player.Stats;
	protected PlayerVisual visual => player.Visual;
	protected PlayerAiBrain aiBrain => player.ai;
}

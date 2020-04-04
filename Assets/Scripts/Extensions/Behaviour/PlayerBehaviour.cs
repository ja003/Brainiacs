using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBehaviour : GameBehaviour
{
	private Player _player;
	protected Player player
	{
		get
		{
			if(_player == null)
				_player = GetComponent<Player>();
			return _player;
		}
	}
}

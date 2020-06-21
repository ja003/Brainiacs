using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PathFinderController : IPathFinder
{
	static Vector2 playerSize;

	Map activeMap => Game.Instance.Map.ActiveMap;

	public PathFinderController(float pStepSize, Vector2 pTopLeft, Vector2 pBotRight)
    {
		playerSize = Game.Instance.PlayerManager.PLAYER_SIZE;
        AstarAdapter.Init(pStepSize, pTopLeft, pBotRight);
	}

	//public void Init(float pStepSize, Vector2 pTopLeft, Vector2 pBotRight)
	//{
	//    AstarAdapter.Init(pStepSize, pTopLeft, pBotRight);
	//}


	//  public MovePath GetPath(Vector2 pFrom, Vector2 pTo)
	//  {
	////Debug.Log($"GetPath {pFrom}-{pTo}");

	//if(!activeMap.IsWithinMap(pFrom) || !activeMap.IsWithinMap(pTo))
	//{
	//	return new MovePath();
	//}

	//return AstarAdapter.GetPath(pFrom, pTo);
	//  }

	


	public async Task<MovePath> GetPathAsync(Vector2 pFrom, Vector2 pTo)
	{
		//Debug.Log($"GetPath {pFrom}-{pTo}");

		if(!activeMap.IsWithinMap(pFrom) || !activeMap.IsWithinMap(pTo))
		{
			return new MovePath();
		}

		return await AstarAdapter.GetPath(pFrom, pTo);
	}


	/// <summary>
	/// The path finder is tailored to the Player -> use its collider
	/// </summary>
	public static bool OverlapsWithMapObject(Vector2 pPoint)
	{
		//const float player_size = 0.2f; //todo: connect to real value?
		Collider2D overlaps = Physics2D.OverlapBox(pPoint, playerSize, 0, Layers.UnwalkableObject);
		if(overlaps)
		{
			//Debug.Log("OverlapsWithMapObject " + overlaps.gameObject.name);
			Utils.DebugDrawBox(pPoint, playerSize, Color.yellow, 1);
		}

		return overlaps;
		//return Physics2D.OverlapCircle(pPoint, Player.COLLIDER_SIZE, mapObject); //not enough
		//return Physics2D.OverlapPoint(pPoint, mapObject);
	}

	/// <summary>
	/// Returns point connecting pFrom and pTo in the simplest way.
	/// X		X - P
	/// |			|
	/// P - Y		Y
	/// The point cant overlap with any map object.
	/// </summary>
	public static Vector2? GetJoinPoint(Vector2 pFrom, Vector2 pTo)
	{
		Vector2 p1 = new Vector2(pFrom.x, pTo.y);
		Vector2 p2 = new Vector2(pTo.x, pFrom.y);
		if(!OverlapsWithMapObject(p1))
			return p1;
		else if(!OverlapsWithMapObject(p2))
			return p2;

		return null;
	}

	public MovePath GetPath(Vector2 pFrom, Vector2 pTo)
	{
		throw new System.NotImplementedException();
	}
}

public interface IPathFinder
{
   // void Init(float pStepSize, Vector2 pTopLeft, Vector2 pBotRight);

    MovePath GetPath(Vector2 pFrom, Vector2 pTo);

	Task<MovePath> GetPathAsync(Vector2 pFrom, Vector2 pTo);

}
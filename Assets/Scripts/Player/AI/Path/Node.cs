using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;


namespace AStarSharp
{
	public class Node
	{
		// Change this depending on what the desired size is for each element in the grid
		public static int NODE_SIZE = 1;//32;
		public Node Parent;
		public Vector2 Position;
		public Vector2 Center
		{
			get
			{
				return new Vector2(Position.X + NODE_SIZE / 2, Position.Y + NODE_SIZE / 2);
			}
		}
		public float DistanceToTarget;
		public float Cost;
		public float Weight;
		public float F
		{
			get
			{
				if(DistanceToTarget != -1 && Cost != -1)
					return DistanceToTarget + Cost;
				else
					return -1;
			}
		}
		public bool Walkable;

		public struct SNeighbors
		{
			public Node Up;
			public Node Right;
			public Node Down;
			public Node Left;
		}
		public SNeighbors Neighbors;

		public int Column => (int)Position.X;
		public int Row => (int)Position.Y;

		public Node(Vector2 pos, bool walkable, float weight = 1)
		{
			Parent = null;
			Position = pos;
			DistanceToTarget = -1;
			Cost = 1;
			Weight = weight;
			Walkable = walkable;
		}

		public override string ToString()
		{
			return $"{Position} ({Walkable})";
		}

		/// <summary>
		/// Optimization for finding the closest walkable node in one direction
		/// </summary>
		public struct SDistanceToWalkable
		{
			public int Up;
			public int Right;
			public int Down;
			public int Left;

			internal int Get(EDirection pDirection)
			{
				switch(pDirection)
				{
					case EDirection.Up:
						return Up;
					case EDirection.Right:
						return Right;
					case EDirection.Down:
						return Down;
					case EDirection.Left:
						return Left;
				}
				Debug.LogError("invalid direction");
				return -1;
			}
		}

		SDistanceToWalkable DistanceToWalkable = new SDistanceToWalkable();

		internal void CalculateDistance()
		{
			if(Walkable)
				return;

			if(Neighbors.Up != null)
				DistanceToWalkable.Up = Neighbors.Up.Walkable ? 1 : 1 + Neighbors.Up.DistanceToWalkable.Up;
			else
				DistanceToWalkable.Up = int.MaxValue;
			if(Neighbors.Right != null)
				DistanceToWalkable.Right = Neighbors.Right.Walkable ? 1 : 1 + Neighbors.Right.DistanceToWalkable.Right;
			else
				DistanceToWalkable.Right = int.MaxValue;
			if(Neighbors.Down != null)
				DistanceToWalkable.Down = Neighbors.Down.Walkable ? 1 : 1 + Neighbors.Down.DistanceToWalkable.Down;
			else
				DistanceToWalkable.Down = int.MaxValue;
			if(Neighbors.Left != null)
				DistanceToWalkable.Left = Neighbors.Left.Walkable ? 1 : 1 + Neighbors.Left.DistanceToWalkable.Left;
			else
				DistanceToWalkable.Left = int.MaxValue;
		}

		internal int GetDistanceToWalkableInDirection(EDirection pDirection)
		{
			if(Walkable)
				return 0;
			return DistanceToWalkable.Get(pDirection);
		}
	}
}
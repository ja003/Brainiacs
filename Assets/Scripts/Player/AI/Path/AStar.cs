﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

//https://raw.githubusercontent.com/davecusatis/A-Star-Sharp/master/Astar.cs
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
	}

	public class Astar
	{
		List<List<Node>> Grid;
		private bool stopCalculation;
		public bool IsSearching;

		int GridRows
		{
			get
			{
				return Grid[0].Count;
			}
		}
		int GridCols
		{
			get
			{
				return Grid.Count;
			}
		}
		public Stack<Node> Path = new Stack<Node>();

		public Astar(List<List<Node>> grid)
		{
			Grid = grid;
		}

		public IEnumerator FindPathAsync(Vector2 Start, Vector2 End)
		{
			stopCalculation = false;
			IsSearching = true;

			Node start = GetNode(Start);
			Node end = GetNode(End);

			//UnityEngine.Debug.Log("start = " + start);
			//UnityEngine.Debug.Log("end = " + end);


			Path = new Stack<Node>();
			List<Node> OpenList = new List<Node>();
			List<Node> ClosedList = new List<Node>();
			List<Node> adjacencies;
			Node current = start;

			// add start node to Open List
			OpenList.Add(start);

			int iterCounter = 0;
			//do short delay every X-th iteration to avoid lag 
			const int max_iter_per_frame = 50;

			while(OpenList.Count != 0 && !ClosedList.Exists(x => x.Position == end.Position))
			{
				if(stopCalculation)
					break;
					//return Path;

				current = OpenList[0];
				OpenList.Remove(current);
				ClosedList.Add(current);
				adjacencies = GetAdjacentNodes(current);

				iterCounter++;
				if(iterCounter % max_iter_per_frame == 0)
				{
					//UnityEngine.Debug.Log($"Delay on iteration: {iterCounter} ({OpenList.Count}/{ClosedList.Count})");
					yield return new WaitForEndOfFrame();
				}

				//this probably means that path is not correct, there shouldnt be that many nodes
				// => return those found so far
				if(ClosedList.Count > 300)
				{
					//UnityEngine.Debug.LogError("Too many nodes!");
					break;
				}

				foreach(Node n in adjacencies)
				{
					if(!ClosedList.Contains(n) && n.Walkable)
					{
						if(!OpenList.Contains(n))
						{
							if(stopCalculation)
								break;
								//return Path;

							n.Parent = current;
							n.DistanceToTarget = Math.Abs(n.Position.X - end.Position.X) + Math.Abs(n.Position.Y - end.Position.Y);
							n.Cost = n.Weight + n.Parent.Cost;
							OpenList.Add(n);
							OpenList = OpenList.OrderBy(node => node.F).ToList<Node>();
						}
					}
				}
			}
			//UnityEngine.Debug.Log($"FindPath took {iterCounter} iterations");

			// construct path, if end was not closed return null
			//if(!ClosedList.Exists(x => x.Position == end.Position))
			//{
			//	return null;
			//}

			// if all good, return path
			Node temp = ClosedList[ClosedList.IndexOf(current)];
			if(temp == null) yield return null;
			do
			{
				if(Path.Count > 200)
				{
					UnityEngine.Debug.LogError("Path too long");
					break;
				}

				try
				{
					Path.Push(temp);
					//UnityEngine.Debug.Log("add: " + temp);
				}
				catch(OutOfMemoryException e)
				{
					UnityEngine.Debug.LogError("OutOfMemoryException!");
				}
				temp = temp.Parent;
			} while(temp != start && temp != null);
			IsSearching = false;
			//return Path;
		}

		internal void StopCalculation()
		{
			//UnityEngine.Debug.Log("StopCalculation");
			stopCalculation = true;
		}

		private static Node GetNode(Vector2 pPoint)
		{
			return new Node(new Vector2((int)(pPoint.X / Node.NODE_SIZE), (int)(pPoint.Y / Node.NODE_SIZE)), true);
		}

		//This is not correct!
		//private Node GetNode(Vector2 pPoint) 
		//{
		//	int row = (int)pPoint.X;
		//	int col = (int)pPoint.Y;
		//	return Grid[col][row];
		//}

		public bool IsWalkable(Vector2 pPoint)
		{
			Node node = GetNode(pPoint);
			int row = (int)node.Position.Y;
			int col = (int)node.Position.X;
			if(!IsWithinGrid(row, col))
				return false;

			return Grid[col][row].Walkable;
		}

		/// <summary>
		/// Returns the first walkable adjacent node that is closest to pRefPoint.
		/// </summary>
		public Vector2? GetClosestWalkable(Vector2 pPoint, Vector2 pRefPoint)
		{
			if(IsWalkable(pPoint))
				return pPoint;

			List<Node> neighbours = new List<Node>();
			const int max_steps = 3; //todo: make dependent on input parameter?
			for(int dist = 1; dist < max_steps; dist++)
			{
				neighbours = GetNodesInDistance(GetNode(pPoint), dist, true);
				if(IsAnyWalkable(neighbours))
					break;
				else
					neighbours.Clear();
			}

			//sort ascending based on distance to pRefPoint
			neighbours.Sort((a, b) => Vector2.Distance(a.Center, pRefPoint)
				.CompareTo(Vector2.Distance(b.Center, pRefPoint)));

			foreach(var n in neighbours)
			{
				if(n.Walkable)
					return n.Center;
			}
			return null;
		}

		private static bool IsAnyWalkable(List<Node> pNodes)
		{
			return pNodes.Any(x => x.Walkable);
		}

		private List<Node> GetNodesInDistance(Node pOrigin, int pSteps, bool pOnlyWalkable)
		{
			List<Node> nodes = new List<Node>();
			for(int x = -pSteps; x <= pSteps; x++)
			{
				for(int y = -pSteps; y <= pSteps; y++)
				{
					int row = (int)pOrigin.Position.Y + y;
					int col = (int)pOrigin.Position.X + x;
					if(!IsWithinGrid(row, col))
						continue;

					Node node = Grid[col][row];
					if(pOnlyWalkable && !node.Walkable)
						continue;

					nodes.Add(node);
				}
			}

			return nodes;
		}

		private List<Node> GetAdjacentNodes(Node n)
		{
			List<Node> temp = new List<Node>();

			int row = (int)n.Position.Y;
			int col = (int)n.Position.X;

			if(!IsWithinGrid(row, col))
			{
				UnityEngine.Debug.LogError($"Invalid input coord value {row},{col}");
				row = UnityEngine.Mathf.Clamp(row, 0, GridRows - 1);
				col = UnityEngine.Mathf.Clamp(col, 0, GridCols - 1);
			}

			if(row + 1 < GridRows)
			{
				temp.Add(Grid[col][row + 1]);
			}
			if(row - 1 >= 0)
			{
				temp.Add(Grid[col][row - 1]);
			}
			if(col - 1 >= 0)
			{
				temp.Add(Grid[col - 1][row]);
			}
			if(col + 1 < GridCols)
			{
				temp.Add(Grid[col + 1][row]);
			}

			return temp;
		}

		private bool IsWithinGrid(int pRow, int pCol)
		{
			if(pRow < 0 || pRow >= GridRows)
			{
				return false;
			}
			if(pCol < 0 || pCol >= GridCols)
			{
				return false;
			}
			return true;
		}
	}

}
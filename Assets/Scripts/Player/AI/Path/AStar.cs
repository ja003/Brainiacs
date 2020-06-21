using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;
using System.Threading.Tasks;

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

		public Astar(List<List<Node>> grid)
		{
			Grid = grid;
		}

		public async Task<Stack<Node>> FindPathAsync(Vector2 Start, Vector2 End)
		{
			Node start = GetNode(Start);
			Node end = GetNode(End);

			Stack<Node> Path = new Stack<Node>();
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
				current = OpenList[0];
				OpenList.Remove(current);
				ClosedList.Add(current);
				adjacencies = GetAdjacentNodes(current);

				iterCounter++;
				if(iterCounter % max_iter_per_frame == 0)
				{
					//UnityEngine.Debug.Log($"Delay on iteration: {iterCounter} ");
					await Task.Delay(1);
				}

				foreach(Node n in adjacencies)
				{
					if(!ClosedList.Contains(n) && n.Walkable)
					{
						if(!OpenList.Contains(n))
						{
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
			if(!ClosedList.Exists(x => x.Position == end.Position))
			{
				return null;
			}

			// if all good, return path
			Node temp = ClosedList[ClosedList.IndexOf(current)];
			if(temp == null) return null;
			do
			{
				Path.Push(temp);
				temp = temp.Parent;
			} while(temp != start && temp != null);
			return Path;
		}

		private static Node GetNode(Vector2 pPoint)
		{
			return new Node(new Vector2((int)(pPoint.X / Node.NODE_SIZE), (int)(pPoint.Y / Node.NODE_SIZE)), true);
		}

		public bool IsWalkable(Vector2 pPoint)
		{
			int row = (int)(pPoint.X / Node.NODE_SIZE);
			int col = (int)(pPoint.Y / Node.NODE_SIZE);
			if(!IsWithinGrid(row, col))
				return false;

			return Grid[row][col].Walkable;
		}

		/// <summary>
		/// Returns the first walkable adjacent node that is closest to pRefPoint.
		/// </summary>
		public Vector2? GetClosestWalkable(Vector2 pPoint, Vector2 pRefPoint)
		{
			if(IsWalkable(pPoint))
				return pPoint;

			var neighbours = GetAdjacentNodes(GetNode(pPoint));

			//and other neighbours 
			//todo: make dependent on input parameter?
			for(int i = neighbours.Count - 1; i >= 0; i--)
			{
				neighbours.AddRange(GetAdjacentNodes(neighbours[i]));
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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrowingTree : MazeGenerator
{
	#region Enums
	public enum EGTVariant
	{
		NEW,
		RANDOM
	}
	#endregion
	
	#region Static Members/Methods
	public static Dictionary<GridPosition, MazeNode> Create(GridPosition Origin, int Width, int Height)
	{
		return Create(EGTVariant.NEW, Origin, Width, Height);
	}
	
	public static Dictionary<GridPosition, MazeNode> Create(EGTVariant Variant, GridPosition Origin, int Width, int Height)
	{
		GrowingTree Generator = new GrowingTree(Variant, Origin, Width, Height);
		Generator.BuildMaze();
		
		return Generator.m_NodeList;
	}
	#endregion
	
	#region Members
	private EGTVariant m_Variant;

    int m_LeftEdge;
    int m_RightEdge;
    int m_TopEdge;
    int m_BottomEdge;
	
	private Dictionary<GridPosition, MazeNode> m_NodeList;
	#endregion
	
	private GrowingTree(EGTVariant Variant)
	{
		m_Variant = Variant;

        m_LeftEdge = 0;
        m_RightEdge = 0;
        m_TopEdge = 0;
        m_BottomEdge = 0;
		
		m_NodeList = new Dictionary<GridPosition, MazeNode>();
	}
	
	private GrowingTree(EGTVariant Variant, GridPosition Origin, int Width, int Height):this(Variant)
	{
        int WidthOffset = Mathf.FloorToInt(Width / 2);
        m_LeftEdge = Origin.x - WidthOffset;
        m_RightEdge = Origin.x + WidthOffset;

        int HeightOffset = Mathf.FloorToInt(Height / 2);
        m_TopEdge = Origin.y + HeightOffset;
        m_BottomEdge = Origin.y - HeightOffset;
	}
	
	private List<MazeNode> ChooseFirstNode()
	{
		int x = Random.Range(m_LeftEdge, m_RightEdge);
		int y = Random.Range(m_BottomEdge, m_TopEdge);
	
		MazeNode FirstNode = CreateNode(x, y);
		
		List<MazeNode> NodeList = new List<MazeNode>();
		NodeList.Add(FirstNode);
		return NodeList;
	}
	
	private void BuildMaze()
	{
		List<MazeNode> OpenNodeList = ChooseFirstNode();
		
		if (OpenNodeList.Count == 0)
		{
			throw new MazeGenerationException(EMazeAlgorithm.GROWING_TREE);
		}
		else
		{
			MazeNode CurrentNode = OpenNodeList[0];
			
			while (CurrentNode != null)
			{
				List<GridPosition> NeighborList = FindUnvisitedNeighbors(CurrentNode);
				if (NeighborList.Count == 0)
				{					
					OpenNodeList.Remove(CurrentNode);
				}
				else
				{
					MazeNode Neighbor = CreateNode(NeighborList[Random.Range(0, NeighborList.Count)]);
					ConnectNodes(CurrentNode, Neighbor);
					OpenNodeList.Add(Neighbor);
				}
				
				CurrentNode = ChooseNextNode(OpenNodeList);
			}
		}
	}
	
	private MazeNode ChooseNextNode(List<MazeNode> Nodes)
	{
		if ((Nodes == null) || (Nodes.Count == 0))
		{
			return null;
		}
		
		int NewestNode = Nodes.Count - 1;
		int Index = -1;
		
		switch (m_Variant)
		{
			case EGTVariant.NEW:
				Index = NewestNode; // Note this results in Count - 1 (for zero indexing)
				break;
			case EGTVariant.RANDOM:
				Index = Random.Range(0, NewestNode);
				break;
		}
		
		return Nodes[Index];
	}
	
	private List<GridPosition> FindUnvisitedNeighbors(MazeNode Node)
	{
		List<GridPosition> UnvisitedNeighbors = new List<GridPosition>();
		
		// Left 
		int x = Node.Position.x - 1;
		int y = Node.Position.y;
		if ((x >= m_LeftEdge) && CheckIfNeighborUnvisited(x, y))
		{
			UnvisitedNeighbors.Add(new GridPosition(x, y, 0));
		}
		
		// Right
		x = Node.Position.x + 1;
		if ((x <= m_RightEdge) && CheckIfNeighborUnvisited(x, y))
		{
			UnvisitedNeighbors.Add(new GridPosition(x, y, 0));
		}
		
		// Top
		x = Node.Position.x;
		y = Node.Position.y + 1;
		if ((y <= m_TopEdge) && CheckIfNeighborUnvisited(x, y))
		{
			UnvisitedNeighbors.Add(new GridPosition(x, y, 0));
		}
		
		// Bottom
		y = Node.Position.y - 1;
		if ((y >= m_BottomEdge) && CheckIfNeighborUnvisited(x, y))
		{
			UnvisitedNeighbors.Add(new GridPosition(x, y, 0));
		}
		
		return UnvisitedNeighbors;
	}
		
	private bool CheckIfNeighborUnvisited(int x, int y)
	{
		GridPosition Neighbor = new GridPosition(x, y, 0);
		if (m_NodeList.ContainsKey(Neighbor))
		{
			return false;
		}
		else
		{		
			return true;
		}
	}
	
	private MazeNode CreateNode(GridPosition Position)
	{
		MazeNode Node = new MazeNode(Position);
		
		m_NodeList.Add(Position, Node);
		return Node;
	}
	
	private MazeNode CreateNode(int x, int y)
	{
		GridPosition Position = new GridPosition(x, y, 0);
		return CreateNode(Position);
	}
	
	private void ConnectNodes(MazeNode Node1, MazeNode Node2)
	{
		if (Node1.Position.x > Node2.Position.x)
		{
			Node1.AddExitPoint(new GridPosition(-1, 0, 0));
			Node2.AddExitPoint(new GridPosition(1, 0, 0));
		}
		else if (Node1.Position.x < Node2.Position.x)
		{			
			Node1.AddExitPoint(new GridPosition(1, 0, 0));
			Node2.AddExitPoint(new GridPosition(-1, 0, 0));
		}
		
		if (Node1.Position.y > Node2.Position.y)
		{
			Node1.AddExitPoint(new GridPosition(0, -1, 0));
			Node2.AddExitPoint(new GridPosition(0, 1, 0));
		}
		else if (Node1.Position.y < Node2.Position.y)
		{			
			Node1.AddExitPoint(new GridPosition(0, 1, 0));
			Node2.AddExitPoint(new GridPosition(0, -1, 0));
		}
	}
}

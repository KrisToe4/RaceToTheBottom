using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellularAutomata : MazeGenerator
{
    #region Enums
    public enum ECAVariant
    {
        SEQUENTIAL,
        INSTANTANEOUS,
        RIPPLE
    }
    #endregion

    #region Static Members/Methods
    public static Dictionary<GridPosition, MazeNode> Create(GridPosition Origin, int Width, int Height)
	{
		return Create(ECAVariant.SEQUENTIAL, Origin, Width, Height);
	}
	
	public static Dictionary<GridPosition, MazeNode> Create(ECAVariant Variant, GridPosition Origin, int Width, int Height)
	{
        CellularAutomata Generator = new CellularAutomata(Variant, Origin, Width, Height);
		Generator.BuildMaze();
		
		return Generator.m_NodeList;
	}
    #endregion

    #region Members
    private ECAVariant m_Variant;

    int m_LeftEdge;
    int m_RightEdge;
    int m_TopEdge;
    int m_BottomEdge;

    private int m_InitialBlockChance;
    private int m_MutationBlockThreshhold;

    private int m_MutationCount;
	
	private Dictionary<GridPosition, MazeNode> m_NodeList;
	#endregion
	
	private CellularAutomata(ECAVariant Variant)
	{
        m_Variant = Variant;

        m_InitialBlockChance = 40;
        m_MutationBlockThreshhold = 4;

        m_MutationCount = 0;
		
		m_NodeList = new Dictionary<GridPosition, MazeNode>();
	}
	
	private CellularAutomata(ECAVariant Variant, GridPosition Origin, int Width, int Height) :this(Variant)
	{
        int WidthOffset = Mathf.FloorToInt(Width / 2);
        m_LeftEdge = Origin.x - WidthOffset;
        m_RightEdge = Origin.x + WidthOffset;

        int HeightOffset = Mathf.FloorToInt(Height / 2);
        m_TopEdge = Origin.y + HeightOffset;
        m_BottomEdge = Origin.y - HeightOffset;
    }
	
	private void BuildMaze()
    {
        GenerateRandomState();

        switch(m_Variant)
        {
            case ECAVariant.SEQUENTIAL:
                SequentialMutation();
                break;
            case ECAVariant.INSTANTANEOUS:
                InstantaneousMutation();
                break;
        }

        FinalizeNodeList();
    }

    private void GenerateRandomState()
    {
        for (int x = m_LeftEdge; x <= m_RightEdge; x++)
        {
            for (int y = m_TopEdge; y >= m_BottomEdge; y--)
            {
                MazeNode NewNode = CreateNode(x, y);

                if (Random.Range(0, 100) > m_InitialBlockChance)
                {
                    NewNode.AddExitList(GridPosition.AllExits);
                }
            }
        }
    }

    private void SequentialMutation()
    {
        m_MutationCount++;
        bool NodesStable = true;

        for (int x = m_LeftEdge; x <= m_RightEdge; x++)
        {
            for (int y = m_TopEdge; y >= m_BottomEdge; y--)
            {
                MazeNode Node = GetNode(x, y);

                int BlockedNeighbors = CountBlockedNeighbors(Node);

                if (Node.ExitCount == 0)
                {
                    if (BlockedNeighbors < m_MutationBlockThreshhold)
                    {
                        Node.AddExitList(GridPosition.AllExits);
                        NodesStable = false;
                    }
                }
                else
                {
                    if (BlockedNeighbors > m_MutationBlockThreshhold)
                    {
                        Node.ClearExits();
                        NodesStable = false;
                    }
                }
            }
        }

        // Mutation cycles are capped at 100 for sanity
        if (!NodesStable && (m_MutationCount < 100))
        {
            SequentialMutation();
        }
    }

    private void InstantaneousMutation()
    {
        m_MutationCount++;
        bool NodesStable = true;

        Dictionary<GridPosition, MazeNode> MutatedNodes = new Dictionary<GridPosition, MazeNode>();

        for (int x = m_LeftEdge; x <= m_RightEdge; x++)
        {
            for (int y = m_TopEdge; y >= m_BottomEdge; y--)
            {
                MazeNode Node = GetNode(x, y);
                MazeNode NewNode = new MazeNode(Node.Position);

                int BlockedNeighbors = CountBlockedNeighbors(Node);

                if (Node.ExitCount == 0)
                {
                    if (BlockedNeighbors < m_MutationBlockThreshhold)
                    {
                        NewNode.AddExitList(GridPosition.AllExits);
                        NodesStable = false;
                    }
                }
                else
                {
                    if (BlockedNeighbors <= m_MutationBlockThreshhold)
                    {
                        NewNode.AddExitList(GridPosition.AllExits);
                        NodesStable = false;
                    }
                }

                MutatedNodes.Add(Node.Position, NewNode);
            }
        }

        m_NodeList = MutatedNodes;

        // Mutation cycles are capped at 100 for sanity
        if (!NodesStable && (m_MutationCount < 100))
        {
            InstantaneousMutation();
        }
    }

    private void FinalizeNodeList()
    {
        for (int x = m_LeftEdge; x <= m_RightEdge; x++)
        {
            for (int y = m_TopEdge; y >= m_BottomEdge; y--)
            {
                MazeNode Node = GetNode(x, y);

                if (Node.ExitCount > 0)
                {
                    // Check the nodes in all 4 directions and if they're blocked remove the exit 
                    if (CheckIfNodeBlocked(x, y + 1))
                    {
                        Node.RemoveExitPoint(Vector3.up);
                    }
                    if (CheckIfNodeBlocked(x - 1, y))
                    {
                        Node.RemoveExitPoint(Vector3.left);

                    }
                    if (CheckIfNodeBlocked(x + 1, y))
                    {
                        Node.RemoveExitPoint(Vector3.right);
                    }
                    if (CheckIfNodeBlocked(x, y - 1))
                    {
                        Node.RemoveExitPoint(Vector3.down);
                    }
                }
                else
                {
                    // Wondering if we should remove the node from the list here or leave it for merging
                }
            }
        }
    }

    private MazeNode CreateNode(int x, int y)
    {
        GridPosition Position = new GridPosition(x, y, 0);
        return CreateNode(Position);
    }

    private MazeNode CreateNode(GridPosition Position)
	{
		MazeNode Node = new MazeNode(Position);
		
		m_NodeList.Add(Position, Node);
		return Node;
	}

    private MazeNode GetNode(int x, int y)
    {
        GridPosition Position = new GridPosition(x, y, 0);
        return GetNode(Position);
    }

    private MazeNode GetNode(GridPosition Position)
    {
        MazeNode Node = null;
        if (m_NodeList.ContainsKey(Position))
        {
            Node = m_NodeList[Position];
        }

        return Node;
    }


    private int CountBlockedNeighbors(MazeNode Node)
    {
        int BlockedCount = 0;

        // Top Row 
        if (CheckIfNodeBlocked(Node.Position.x - 1, Node.Position.y + 1))
        {
            BlockedCount++;
        }
        if (CheckIfNodeBlocked(Node.Position.x, Node.Position.y + 1))
        {
            BlockedCount++;
        }
        if (CheckIfNodeBlocked(Node.Position.x + 1, Node.Position.y + 1))
        {
            BlockedCount++;
        }

        // Sides
        if (CheckIfNodeBlocked(Node.Position.x - 1, Node.Position.y))
        {
            BlockedCount++;
        }
        if (CheckIfNodeBlocked(Node.Position.x + 1, Node.Position.y))
        {
            BlockedCount++;
        }

        // Bottom Row
        if (CheckIfNodeBlocked(Node.Position.x - 1, Node.Position.y - 1))
        {
            BlockedCount++;
        }
        if (CheckIfNodeBlocked(Node.Position.x, Node.Position.y - 1))
        {
            BlockedCount++;
        }
        if (CheckIfNodeBlocked(Node.Position.x + 1, Node.Position.y - 1))
        {
            BlockedCount++;
        }

        return BlockedCount;
    }

    private bool CheckIfNodeBlocked(int x, int y)
    {
        bool Blocked = true;

        GridPosition Position = new GridPosition(x, y, 0);
        if (m_NodeList.ContainsKey(Position))
        {
            MazeNode Neighbor = m_NodeList[Position];

            if (Neighbor.ExitCount > 0)
            {
                Blocked = false;
            }
        }

        return Blocked;
    }
}



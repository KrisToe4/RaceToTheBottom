using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EMazeAlgorithm
{
    CELLULAR_AUTOMATA,
    GROWING_TREE,
    SIMPLE
}

public class MazeSector
{
	public static MazeSector Generate(GridPosition Origin, int Width, int Height, EMazeAlgorithm Algorithm)
	{
        MazeSector Sector = new MazeSector(Origin, Width, Height, Algorithm);
        Sector.Generate();
		return Sector;
	}

    #region Members
    private GridPosition m_Origin;
    private int m_Width;
    private int m_Height;

	private EMazeAlgorithm m_Algorithm;
	
	private Dictionary<GridPosition,MazeNode> m_NodeMap;
	#endregion
		
	#region Properties
	public MazeNode this[GridPosition Key]
	{
		get
		{			
			if (m_NodeMap.ContainsKey(Key))
			{
				return m_NodeMap[Key];
			}
			else
			{
				return null;
			}
		}
		set
		{
			m_NodeMap[Key] = value;
		}
	}
	

    public GridPosition Origin
    {
        get
        {
            return m_Origin;
        }
    }


	public int Width
	{
		get
		{
			return m_Width;
		}
	}	
    public int Height
    {
        get
        {
            return m_Height;
        }
    }
	#endregion
	
	private MazeSector(GridPosition Origin, int Width, int Height, EMazeAlgorithm Algorithm)
	{
        m_Origin = Origin;
        m_Width = Width;
        m_Height = Height;
		
		m_Algorithm = Algorithm;
		
		m_NodeMap = new Dictionary<GridPosition, MazeNode>();
	}
	
	private void Generate()
	{
        Debug.Log(m_Origin.ToString());

		switch (m_Algorithm)
		{
            case EMazeAlgorithm.CELLULAR_AUTOMATA:
                m_NodeMap = CellularAutomata.Create(m_Origin, m_Width, m_Height);
                break;
            case EMazeAlgorithm.GROWING_TREE:
                m_NodeMap = GrowingTree.Create(m_Origin, m_Width, m_Height);
                break;
            case EMazeAlgorithm.SIMPLE:
                m_NodeMap = CreateSimpleSector();
                break;
        }
	}
	
	public IEnumerable<MazeNode> Nodes()
	{
		foreach (MazeNode Node in m_NodeMap.Values)
		{
			yield return Node;
		}
	}
	
    // This method returns a square Sector edged in walls but otherwise open and doesn't include default exits between Sectors
	public Dictionary<GridPosition,MazeNode> CreateSimpleSector()
	{
        int WidthOffset = Mathf.FloorToInt(m_Width / 2);
        int LeftEdge = m_Origin.x - WidthOffset;
        int RightEdge = m_Origin.x + WidthOffset;

        int HeightOffset = Mathf.FloorToInt(m_Height / 2);
        int TopEdge = m_Origin.y + HeightOffset;
        int BottomEdge = m_Origin.y - HeightOffset;

        Dictionary<GridPosition,MazeNode> NodeList = new Dictionary<GridPosition, MazeNode>();
		
		Vector3 ExitPoint = Vector3.zero;
		for (int x = LeftEdge; x <= RightEdge; x++)
		{
			for (int y = TopEdge; y >= BottomEdge; y--)
			{
				MazeNode NewNode = new MazeNode(new GridPosition(x, y, m_Origin.z));
				
				if (x > LeftEdge)
				{
					ExitPoint = new Vector3(-1, 0, 0);
					NewNode.AddExitPoint(ExitPoint);
				}	
				if (x < RightEdge)
				{
					ExitPoint = new Vector3(1, 0, 0);
					NewNode.AddExitPoint(ExitPoint);
				}
					
				if (y < TopEdge)
				{
					ExitPoint = new Vector3(0, 1, 0);
					NewNode.AddExitPoint(ExitPoint);
				}
				if (y > BottomEdge)
				{
					ExitPoint = new Vector3(0, -1, 0);
					NewNode.AddExitPoint(ExitPoint);
				}
						
				GridPosition Key = new GridPosition(x, y, 0);
				NodeList.Add(Key, NewNode);
			}
		}
		
		return NodeList;
	}
}
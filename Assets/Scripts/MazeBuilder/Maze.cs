using UnityEngine;
using System.Collections.Generic;

public class Maze
{
    #region Members	
    private MazeNode m_EntryNode;
    private List<MazeFloor> m_Floors;

    private Dictionary<GridPosition, MazeNode> m_NodeMap;
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

    public MazeNode EntryNode
    {
        get
        {
            return m_EntryNode;
        }
    }
    #endregion

    public Maze()
    {
        m_EntryNode = null;
        m_Floors = new List<MazeFloor>();

        m_NodeMap = new Dictionary<GridPosition, MazeNode>();
    }

    public IEnumerable<MazeNode> Nodes()
    {
        foreach (MazeNode Node in m_NodeMap.Values)
        {
            yield return Node;
        }
    }

    public void UpdateNodeMap()
    {
        m_NodeMap.Clear();

        // Then merge in the rest of the Regions
        foreach (MazeFloor Floor in m_Floors)
        {
            MergeFloor(Floor);
        }

        // Assign the core if needed
        if (m_EntryNode != null)
        {
            m_EntryNode = m_NodeMap[m_EntryNode.Position];
        }
    }

    private void MergeFloor(MazeFloor Floor)
    {
        foreach (MazeNode Node in Floor.Nodes())
        {
            if (Node.ExitCount > 0)
            {
                // If one doesn't already exist, create a new node so that we can force it to merge (for more 
                // uniform control). Assign the new nodes up vector based on current rotation (from ZPos = Up)
                if (!m_NodeMap.ContainsKey(Node.Position))
                {
                    m_NodeMap.Add(Node.Position, Node);
                }
                else
                {
                    m_NodeMap[Node.Position] = m_NodeMap[Node.Position].MergeNode(Node);
                }

                // Here we need to see if there's an exit down deeper into the maze. If so, generate
                // a mapnode manually to represent the exit out in that lower Region. This will cause 
                // it to be merged in when loading the next Floor down			
                if (m_NodeMap[Node.Position].ContainsExitPoint(GridPosition.DownExit))
                {
                    GridPosition TempPosition = new GridPosition(Node.Position.GridVector + GridPosition.DownExit.GridVector);
                    if (!m_NodeMap.ContainsKey(TempPosition))
                    {
                        m_NodeMap.Add(TempPosition, new MazeNode(TempPosition));
                    }

                    m_NodeMap[TempPosition].AddExitPoint(GridPosition.UpExit);
                }
            }
        }
    }

    #region Built-in Generator Methods
    public void GenerateSimpleFloor(int Size, EMazeAlgorithm Algorithm)
    {
        m_Floors.Clear();

        MazeFloor Floor = new MazeFloor();
        Floor.GenerateSimpleFloor(Size, Algorithm);
        m_Floors.Add(Floor);

        m_EntryNode = new MazeNode();

        UpdateNodeMap();
    }

    public void GenerateComplexFloor(int SectorCount, int SectorSpacing)
    {
        m_Floors.Clear();

        MazeFloor Floor = new MazeFloor();
        Floor.GenerateComplexFloor(SectorCount, SectorSpacing);
        m_Floors.Add(Floor);

        m_EntryNode = new MazeNode();

        UpdateNodeMap();
    }
    
	#endregion
}



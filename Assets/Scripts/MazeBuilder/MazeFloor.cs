using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeFloor
{
	#region Members
	private List<MazeSector> m_Sectors;
	
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
	
	public int SectorCount
	{
		get
		{
			return m_Sectors.Count;
		}
	}
	#endregion
	
	public MazeFloor()
	{			
		m_Sectors = new List<MazeSector>();
		
		m_NodeMap = new Dictionary<GridPosition, MazeNode>();
	}
	
	public IEnumerable<MazeNode> Nodes()
	{
		foreach (MazeNode Node in m_NodeMap.Values)
		{
			yield return Node;
		}
	}
	
	public void AddSector(MazeSector Sector)
	{
        m_Sectors.Add(Sector);
		UpdateNodeMap();
	}
	
	public void AddSectors(List<MazeSector> SectorList)
	{
        m_Sectors.AddRange(SectorList);
		UpdateNodeMap();
	}
		
	private void UpdateNodeMap()
	{
		m_NodeMap.Clear();
		
		foreach (MazeSector Sector in m_Sectors)
		{
			MergeSector(Sector);
		}
	}
	
	private void MergeSector(MazeSector Sector)
	{
        Debug.Log("Merging Sector");

		foreach (MazeNode Node in Sector.Nodes())
		{
            // If one doesn't already exist, create a new node so that we can force it to merge (for more 
            // uniform control). 
            if (!m_NodeMap.ContainsKey(Node.Position))
            {
                m_NodeMap.Add(Node.Position, Node);
            }
            else
            {
                m_NodeMap[Node.Position] = m_NodeMap[Node.Position].MergeNode(Node);
            }
		}
	}
	
	#region Built-in Generator Methods
    public void GenerateSimpleFloor(int Size, EMazeAlgorithm Algorithm)
    {
        m_Sectors.Clear();

        MazeSector Sector = MazeSector.Generate(new GridPosition(0, 0, 0), Size, Size, Algorithm);

        AddSector(Sector);
    }

    public void GenerateComplexFloor(int SectorCount, int SectorSpacing)
    {
        m_Sectors.Clear();

        List<MazeSector> SectorList = new List<MazeSector>();


        // The first sector defaults to a 5x5 room so we have a consistent starting point
        int SectorWidth = 5;
        int SectorHeight = 5;
        GridPosition SectorOrigin = GridPosition.zero;
        EMazeAlgorithm Algorithm = EMazeAlgorithm.SIMPLE;
        MazeSector LastSector = MazeSector.Generate(SectorOrigin, SectorWidth, SectorHeight, Algorithm);
        SectorList.Add(LastSector);

        // The rest of the sectors will be semi-randomly generated and placed towards the left or up
        // and have simple connectors for now
        GridPosition ConnectorOrigin = GridPosition.zero;
        int ConnectorWidth = 0;
        int ConnectorHeight = 0;

        for (int x = 1; x < SectorCount; x++)
        {
            // Set up our parms for the next sector
            SectorWidth = Random.Range(SectorSpacing - 10, SectorSpacing - 5);
            SectorHeight = Random.Range(SectorSpacing - 10, SectorSpacing - 5);

            if ((x % 2) == 1)
            {
                ConnectorOrigin = new GridPosition(SectorOrigin.x - Mathf.FloorToInt(SectorSpacing / 2), SectorOrigin.y, 0);
                ConnectorHeight = 3;
                ConnectorWidth = SectorSpacing;

                SectorOrigin = new GridPosition(SectorOrigin.x - SectorSpacing, SectorOrigin.y, 0);
            }
            else
            {
                ConnectorOrigin = new GridPosition(SectorOrigin.x, SectorOrigin.y + Mathf.FloorToInt(SectorSpacing / 2), 0);
                ConnectorHeight = SectorSpacing;
                ConnectorWidth = 3;

                SectorOrigin = new GridPosition(SectorOrigin.x, SectorOrigin.y + SectorSpacing, 0);
            }

            switch (x % 3)
            {
                case 0:
                    Algorithm = EMazeAlgorithm.SIMPLE;
                    break;
                case 1:
                    Algorithm = EMazeAlgorithm.CELLULAR_AUTOMATA;
                    break;
                case 2:
                    Algorithm = EMazeAlgorithm.GROWING_TREE;
                    break;
            }

            MazeSector NextSector = MazeSector.Generate(SectorOrigin, SectorWidth, SectorHeight, Algorithm);
            SectorList.Add(NextSector);

            //Add a connector between the two sectors and update our lastsector
            SectorList.Add(MazeSector.Generate(ConnectorOrigin, ConnectorWidth, ConnectorHeight, EMazeAlgorithm.SIMPLE));
            LastSector = NextSector;
        }

        AddSectors(SectorList);
    }

    #endregion
}
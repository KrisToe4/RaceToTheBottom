using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MazeNode
{
	#region Members
	private GridPosition m_Position;
	
	private HashSet<GridPosition> m_ExitPoints;
	
	private GameObject m_GameObject;
	#endregion
	
	#region Properties
	public GridPosition Position
	{
		get
		{
			return m_Position;
		}
	}
	
	public int ExitCount
	{
		get
		{
			return m_ExitPoints.Count;
		}
	}
	
	public HashSet<GridPosition> ExitList
	{
		get
		{
			return GetExitList();
		}
	}
	
	public GameObject AttachedObject
	{
		get
		{
			return m_GameObject;
		}
	}
	#endregion
		
	public MazeNode()
	{
		m_Position = new GridPosition();
		m_ExitPoints = new HashSet<GridPosition>();
	}
	
	public MazeNode(GridPosition NodePosition):this()
	{
		m_Position = NodePosition;
	}
		
	public void AddExitPoint(GridPosition ExitPoint)
	{
		m_ExitPoints.Add(ExitPoint);
	}
	
	public void AddExitPoint(Vector3 ExitPoint)
	{
		GridPosition Exit = new GridPosition(ExitPoint);
        AddExitPoint(Exit);
	}

    public void RemoveExitPoint(GridPosition ExitPoint)
    {
        m_ExitPoints.Remove(ExitPoint);
    }

    public void RemoveExitPoint(Vector3 ExitPoint)
    {
        GridPosition Exit = new GridPosition(ExitPoint);
        RemoveExitPoint(Exit);
    }

    public void AddExitList(HashSet<GridPosition> ExitList)
	{
		m_ExitPoints.UnionWith(ExitList);
	}

    public void ClearExits()
    {
        m_ExitPoints.Clear();
    }
		
	public bool ContainsExitPoint(GridPosition ExitPoint)
	{
		return m_ExitPoints.Contains(ExitPoint);
	}
	
	public MazeNode MergeNode(MazeNode NodeToMerge)
	{
		MazeNode Union = new MazeNode(m_Position);

		Union.AddExitList(m_ExitPoints);
		Union.AddExitList(NodeToMerge.GetExitList());
		
		return Union;
	}
	
	public void SetVerticalPosition(int ZOffset)
	{
        m_Position = new GridPosition(m_Position.x, m_Position.y, ZOffset);
	}
	
	private HashSet<GridPosition> GetExitList()
	{
		return m_ExitPoints;
	}
	
	public void AttachGameObject(GameObject ObjectToAttach)
	{
		m_GameObject = ObjectToAttach;
	}
	
}

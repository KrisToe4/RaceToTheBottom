using System;
using System.Collections.Generic;
using UnityEngine;

public class GridPosition
{
	#region Statics
	public static GridPosition zero
	{
		get
		{
			GridPosition ZeroPos = new GridPosition();
			return ZeroPos;
		}
	}

    public static GridPosition UpExit
    {
        get
        {
            return new GridPosition(0, 0, 1);
        }
    }

    public static GridPosition DownExit
    {
        get
        {
            return new GridPosition(0, 0, -1);
        }
    }

    private static HashSet<GridPosition> m_AllExits;
    public static HashSet<GridPosition> AllExits
    {
        get
        {
            if (m_AllExits == null)
            {
                m_AllExits = new HashSet<GridPosition> { new GridPosition(1, 0, 0), new GridPosition(-1, 0, 0), new GridPosition(0, 1, 0), new GridPosition(0, -1, 0) };
            }
            return m_AllExits;
        }
    }
    #endregion

    #region Members
    private int m_x = 0;
	private int m_y = 0;
	private int m_z = 0;
	#endregion
	
	#region Properties
	public Vector3 GridVector
	{
		get		
		{
			return new Vector3(m_x, m_y, m_z);
		}
		set
		{
			m_x = (int)Mathf.Round(value.x);
			m_y = (int)Mathf.Round(value.y);
			m_z = (int)Mathf.Round(value.z);
		}
	}
	
	public int x
	{
		get
		{
			return m_x;
		}
	}
	public int y
	{
		get
		{
			return m_y;
		}
	}
	public int z
	{
		get
		{
			return m_z;
		}
	}
	#endregion
	
	public GridPosition()
	{
		m_x = 0;
		m_y = 0;
		m_z = 0;
	}
	
	public GridPosition(int x, int y, int z)
	{
		m_x = x;
		m_y = y;
		m_z = z;
	}
	
	public GridPosition(Vector3 Position)
	{
		GridVector = Position;
	}
	
	public override string ToString ()
	{
		return "X: " + m_x.ToString() + ", Y: " + m_y.ToString() + ", Z: " + m_z.ToString();
	}
	
	#region Comparable Method
	public override bool Equals(object obj)
	{
		bool IsEqual = false;
		
		GridPosition Target = obj as GridPosition;
		if ((Target != null) && (x == Target.x) && (y == Target.y) && (z == Target.z))
		{
			IsEqual = true;
		}
		
		return IsEqual;
	}
	
	public override int GetHashCode()
	{
		int Hash = m_x.GetHashCode() + m_y.GetHashCode() + m_z.GetHashCode();
		return Hash.GetHashCode();
	}
	#endregion
}


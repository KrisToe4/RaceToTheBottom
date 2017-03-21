using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeManager : MonoBehaviour {
	
	#region Members	
	public int TileSpacing = 1;
	public int SectorSpacing = 20;
    public float MonsterFrequency = 0.01f;

    public GameObject[] FloorTiles;
    public GameObject[] WallTiles;
		
	private Maze m_MazeInstance;
    #endregion

	void Start ()  
	{	
		m_MazeInstance = new Maze();

        //m_MazeInstance.GenerateSimpleFloor(MazeSize, EMazeAlgorithm.CELLULAR_AUTOMATA);
        m_MazeInstance.GenerateComplexFloor(5, SectorSpacing);

        GenerateGameObjects();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//ProcessInput();
	}
	
    /*
	private void ProcessInput()
	{	
		GridPosition NextPosition = null;
		
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				RotateViewPoint(ViewPoint.transform.up, 90);
			}
			else
			{
				NextPosition = new GridPosition(ViewPoint.transform.right).Invert();
			}
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				RotateViewPoint(ViewPoint.transform.up, -90);
			}
			else
			{
				NextPosition = new GridPosition(ViewPoint.transform.right);
			}
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				RotateViewPoint(ViewPoint.transform.right, -90);
			}
			else
			{
				NextPosition = new GridPosition(ViewPoint.transform.up).Invert();	
			}
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				RotateViewPoint(ViewPoint.transform.right, 90);
			}
			else
			{
				NextPosition = new GridPosition(ViewPoint.transform.up);
			}
		}
		
		if (NextPosition != null)
		{
			MazeNode NextNode = m_MazeInstance.TryMove(m_CurrentNode, NextPosition);
			if (NextNode != null)
			{
				// Here we'll trigger entry stuff. For now just change colours
				switch (m_CurrentNode.ExitCount)
				{
					case 1:
						
						m_CurrentNode.AttachedObject.GetComponent<Renderer>().material.color = Color.blue;
						break;
					case 2:
						m_CurrentNode.AttachedObject.GetComponent<Renderer>().material.color = Color.cyan;
						break;
					case 3:
						m_CurrentNode.AttachedObject.GetComponent<Renderer>().material.color = Color.green;
						break;
					case 4:
						m_CurrentNode.AttachedObject.GetComponent<Renderer>().material.color = Color.yellow;
						break;
					case 5:
						m_CurrentNode.AttachedObject.GetComponent<Renderer>().material.color = Color.magenta;
						break;
					case 6:
						m_CurrentNode.AttachedObject.GetComponent<Renderer>().material.color = Color.red;
						break;
					default:
						m_CurrentNode.AttachedObject.GetComponent<Renderer>().material.color = Color.black;
						break;
				}
				NextNode.AttachedObject.GetComponent<Renderer>().material.color = Color.white;
				
				m_CurrentNode = NextNode;
			}
		}
		
		return;
	}
    */
		
	private void GenerateGameObjects()
	{				
		GameObject MazeContainer = new GameObject("Maze");
				
		foreach (MazeNode Node in m_MazeInstance.Nodes())
		{		
			Vector3 GridPosition = new Vector3(Node.Position.x * TileSpacing, Node.Position.y * TileSpacing, Node.Position.z * TileSpacing);

            // Each tile starts with a floor
            GameObject MazeTile;
            if (Random.Range(0f, 1f) > MonsterFrequency)
            {
                MazeTile = GameObject.Instantiate(FloorTiles[0], GridPosition, Quaternion.identity, MazeContainer.transform);
            }
            else
            {
                MazeTile = GameObject.Instantiate(FloorTiles[1], GridPosition, Quaternion.identity, MazeContainer.transform);
            }

            // Then we add walls (and eventually doors). This is an ugly set of if's for now out of simplicity
            GameObject Wall = null;
            if (!Node.ContainsExitPoint(new GridPosition(1, 0, 0)))
            {
                Wall = GameObject.Instantiate(WallTiles[0], MazeTile.transform);
                Wall.transform.localPosition = new Vector3(1.84f, 0, 0);
                Wall.transform.Rotate(Vector3.forward, 90f);
            }
            if (!Node.ContainsExitPoint(new GridPosition(-1, 0, 0)))
            {
                Wall = GameObject.Instantiate(WallTiles[0], MazeTile.transform);
                Wall.transform.localPosition = new Vector3(-1.84f, 0, 0);
                Wall.transform.Rotate(Vector3.forward, 90f);
            }
            if (!Node.ContainsExitPoint(new GridPosition(0, 1, 0)))
            {
                Wall = GameObject.Instantiate(WallTiles[0], MazeTile.transform);
                Wall.transform.localPosition = new Vector3(0, 1.84f, 0);
            }
            if (!Node.ContainsExitPoint(new GridPosition(0, -1, 0)))
            {
                Wall = GameObject.Instantiate(WallTiles[0], MazeTile.transform);
                Wall.transform.localPosition = new Vector3(0, -1.84f, 0);
            }

            MazeTile.name = Node.Position.ToString();
			Node.AttachGameObject(MazeTile);
					
		}
	}
}

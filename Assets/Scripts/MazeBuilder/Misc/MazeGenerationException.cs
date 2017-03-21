using UnityEngine;

public class MazeGenerationException: UnityException
{
    #region Members
    public EMazeAlgorithm GeneratorAlgorithm;
    #endregion

    public MazeGenerationException():base()
	{
	}
	
	public MazeGenerationException(EMazeAlgorithm Algorithm):base("Maze Generation failed.")	
	{
        GeneratorAlgorithm = Algorithm;
	}


}



using Sirenix.OdinInspector;
using Sirenix.Serialization;

/// <summary>
/// Represents the logical state of the game board.
/// Separate from visual representation for better testability.
/// </summary>
[System.Serializable]
public class BoardState
{
    [OdinSerialize, TableMatrix(HorizontalTitle = "Game Board", SquareCells = true)]
    public LetterData[,] tiles;

    public BoardState(int width, int height)
    {
        tiles = new LetterData[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tiles[i, j] = new LetterData(string.Empty, 0, null);
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                tiles[i, j] = new LetterData(string.Empty, 0, null);
            }
        }
    }
}
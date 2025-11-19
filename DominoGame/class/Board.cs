public interface IBoard
{
    List<IDominoTile> Tiles { get; }
}

public class Board : IBoard
{
    public List<IDominoTile> Tiles { get; } = new List<IDominoTile>();
}
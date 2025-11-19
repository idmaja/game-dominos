public interface IDeck
{
    List<IDominoTile> Tiles { get; }
}

public class Deck : IDeck
{
    public List<IDominoTile> Tiles { get; } = new List<IDominoTile>();
}
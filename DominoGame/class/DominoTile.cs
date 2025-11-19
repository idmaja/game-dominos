public interface IDominoTile
{
    int Left { get; }
    int Right { get; }
}

public class DominoTile : IDominoTile
{
    public int Left { get; }
    public int Right { get; }

    public DominoTile(int left, int right)
    {
        Left = left;
        Right = right;
    }

    public override string ToString()
    {
        return $"[{Left}|{Right}]";
    }
}
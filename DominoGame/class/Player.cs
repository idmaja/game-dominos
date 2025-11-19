public interface IPlayer
{
    string Name { get; }
}

public class Player : IPlayer
{
    public string Name { get; }

    public Player(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}
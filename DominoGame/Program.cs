
Console.Write("Masukkan jumlah pemain (2-4): ");
int playerCount;
while (!int.TryParse(Console.ReadLine(), out playerCount) || playerCount < 2 || playerCount > 4)
{
    Console.Write("Input tidak valid. Masukkan jumlah pemain (2-4): ");
}

var players = new List<IPlayer>();
for (int i = 1; i <= playerCount; i++)
{
    players.Add(new Player($"Pemain {i}"));
}

IDeck deck = new Deck();
IBoard board = new Board();
int tilesPerPlayer = 7;

var game = new DominoGame(
    players,
    deck,
    board,
    tilesPerPlayer,
    msg => Console.WriteLine(msg)
);

game.StartGame();

using System;
using System.Collections.Generic;
using System.Linq;

public class DominoGame
{
    private readonly List<IPlayer> _players;
    private readonly Dictionary<IPlayer, List<IDominoTile>> _playerTiles;
    private readonly IDeck _deck;
    private readonly IBoard _board;
    private readonly Random _random;
    private int _currentPlayerIndex;
    private bool _isGameDone;
    private readonly int _tilesPerPlayer;

    public event Action<string>? GameLogging;

    public DominoGame(List<IPlayer> players, IDeck deck, IBoard board, int tilesPerPlayer, Action<string> gameLogging)
    {
        _players = players;
        _deck = deck;
        _board = board;
        _tilesPerPlayer = tilesPerPlayer;
        _playerTiles = new Dictionary<IPlayer, List<IDominoTile>>();
        _random = new Random();
        _currentPlayerIndex = 0;
        _isGameDone = false;

        if (gameLogging != null)
        {
            GameLogging += gameLogging;
        }

        foreach (var p in _players)
        {
            _playerTiles[p] = new List<IDominoTile>();
        }
    }

    // Game
    public void StartGame()
    {
        Log("Game dimulai.");
        InitializeTiles();
        ShuffleTiles();
        DealTilesToPlayer(_tilesPerPlayer);
        Log($"Kartu dibagikan, {_tilesPerPlayer} per pemain kalau stok cukup.");
    
        while (!_isGameDone)
        {
            Console.Clear();
            PrintGameState();
    
            var currentPlayer = GetCurrentPlayer();
    
            // BAGIAN INI DIUBAH
            if (!HasAnyPlayableTile(currentPlayer))
            {
                if (_deck.Tiles.Count > 0)
                {
                    Log($"Pemain {currentPlayer.Name} tidak punya tile yang bisa dimainkan, menarik dari deck.");
                    var drawn = DrawTileForPlayer();
                    if (drawn != null)
                    {
                        _playerTiles[currentPlayer].Add(drawn);
                    }
    
                    // Tampilkan state setelah draw
                    Console.Clear();
                    PrintGameState();
    
                    // Cek ulang, setelah draw apakah sekarang sudah bisa main
                    if (!HasAnyPlayableTile(currentPlayer))
                    {
                        // Kalau deck sudah habis dan tidak ada yang bisa main, cek buntu
                        if (_deck.Tiles.Count == 0 && IsGameStuck())
                        {
                            Log("Game buntu. Tidak ada pemain yang bisa main.");
                            _isGameDone = true;
                            break;
                        }
    
                        // Tetap tidak bisa main, lewati giliran
                        Log("Tetap tidak ada tile yang bisa dimainkan. Giliran dilewati.");
                        Console.WriteLine();
                        Console.WriteLine("Tekan Enter untuk lanjut ke giliran berikutnya...");
                        Console.ReadLine();
    
                        NextPlayer();
                        continue;
                    }
                }
                else
                {
                    Log($"Pemain {currentPlayer.Name} tidak punya tile yang bisa dimainkan dan deck habis.");
                    if (IsGameStuck())
                    {
                        Log("Game buntu. Tidak ada pemain yang bisa main.");
                        _isGameDone = true;
                        break;
                    }
    
                    NextPlayer();
                    continue;
                }
            }
    
            // Mulai dari sini, kita yakin currentPlayer punya setidaknya satu tile yang bisa dimainkan
            var tileToPlay = AskPlayerTileChoice(currentPlayer);
            if (tileToPlay == null)
            {
                _isGameDone = true;
                break;
            }
    
            var hand = _playerTiles[currentPlayer];
    
            if (_board.Tiles.Count == 0)
            {
                PlaceTileToBoard(tileToPlay);
            }
            else
            {
                bool canLeft = CanPlaceTileOnLeft(tileToPlay);
                bool canRight = CanPlaceTileOnRight(tileToPlay);
    
                if (!canLeft && !canRight)
                {
                    Log("Tile tidak bisa dimainkan. Melewati giliran.");
                    NextPlayer();
                    continue;
                }
    
                if (canLeft && canRight)
                {
                    Console.Write("Tile bisa di kiri atau kanan. Pilih (L/R): ");
                    string sideInput;
                    do
                    {
                        sideInput = Console.ReadLine()!.Trim().ToUpper();
                    } while (sideInput != "L" && sideInput != "R");
    
                    PlaceTileToBoard(tileToPlay, sideInput);
                }
                else if (canLeft)
                {
                    PlaceTileToLeft(tileToPlay, GetLeftEnd());
                }
                else
                {
                    PlaceTileToRight(tileToPlay, GetRightEnd());
                }
            }
    
            Log($"Pemain {currentPlayer.Name} memainkan {tileToPlay}.");
    
            hand.Remove(tileToPlay);
    
            if (IsPlayerWin(currentPlayer))
            {
                Log($"Pemain {currentPlayer.Name} menang. Semua tile sudah habis.");
                _isGameDone = true;
            }
            else if (IsGameStuck())
            {
                Log("Game buntu. Tidak ada pemain yang bisa main.");
                _isGameDone = true;
            }
    
            Console.WriteLine();
            Console.WriteLine("Tekan Enter untuk lanjut ke giliran berikutnya...");
            Console.ReadLine();
    
            if (!_isGameDone)
            {
                NextPlayer();
            }
        }
    
        Console.WriteLine("Game selesai.");
    }


    public bool IsGameStuck()
    {
        if (_deck.Tiles.Count > 0)
        {
            return false;
        }

        foreach (var p in _players)
        {
            if (HasAnyPlayableTile(p))
            {
                return false;
            }
        }

        return true;
    }

    public List<IPlayer> GetPlayerList()
    {
        return _players;
    }

    public Dictionary<IPlayer, List<IDominoTile>> GetPlayerTiles()
    {
        return _playerTiles;
    }

    public IPlayer GetCurrentPlayer()
    {
        return _players[_currentPlayerIndex];
    }

    private void NextPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count();
    }

    // Deck
    public void InitializeTiles()
    {
        _deck.Tiles.Clear();
        for (int left = 0; left <= 6; left++)
        {
            for (int right = left; right <= 6; right++)
            {
                _deck.Tiles.Add(new DominoTile(left, right));
            }
        }
    }

    public void ShuffleTiles()
    {
        int n = _deck.Tiles.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            var temp = _deck.Tiles[i];
            _deck.Tiles[i] = _deck.Tiles[j];
            _deck.Tiles[j] = temp;
        }
    }

    public void DealTilesToPlayer(int tilesPerPlayer)
    {
        for (int t = 0; t < tilesPerPlayer; t++)
        {
            foreach (var p in _players)
            {
                if (_deck.Tiles.Count == 0)
                {
                    return;
                }

                var tile = _deck.Tiles[_deck.Tiles.Count - 1];
                _deck.Tiles.RemoveAt(_deck.Tiles.Count - 1);
                _playerTiles[p].Add(tile);
            }
        }
    }

    public IDominoTile DrawTileForPlayer()
    {
        if (_deck.Tiles.Count == 0)
        {
            return null!;
        }

        var tile = _deck.Tiles[_deck.Tiles.Count - 1];
        _deck.Tiles.RemoveAt(_deck.Tiles.Count - 1);
        return tile;
    }

    // Player placement logic
    public bool CanPlaceTile(IDominoTile tile)
    {
        if (_board.Tiles.Count == 0)
        {
            return true;
        }

        return CanPlaceTileOnLeft(tile) || CanPlaceTileOnRight(tile);
    }

    public bool CanPlaceTileOnLeft(IDominoTile tile)
    {
        if (_board.Tiles.Count == 0)
        {
            return true;
        }

        int leftEnd = GetLeftEnd();
        return IsTileMatches(leftEnd, tile);
    }

    public bool CanPlaceTileOnRight(IDominoTile tile)
    {
        if (_board.Tiles.Count == 0)
        {
            return true;
        }

        int rightEnd = GetRightEnd();
        return IsTileMatches(rightEnd, tile);
    }

    public void PlaceTileToBoard(IDominoTile tile)
    {
        if (_board.Tiles.Count == 0)
        {
            _board.Tiles.Add(tile);
            return;
        }

        bool canLeft = CanPlaceTileOnLeft(tile);
        bool canRight = CanPlaceTileOnRight(tile);

        if (canRight)
        {
            PlaceTileToRight(tile, GetRightEnd());
        }
        else if (canLeft)
        {
            PlaceTileToLeft(tile, GetLeftEnd());
        }
    }

    public void PlaceTileToBoard(IDominoTile tile, string side)
    {
        side = side.ToUpper();
        if (side == "L")
        {
            PlaceTileToLeft(tile, GetLeftEnd());
        }
        else
        {
            PlaceTileToRight(tile, GetRightEnd());
        }
    }

    public void PlaceTileToLeft(IDominoTile tile, int leftEnd)
    {
        var tileToPlace = tile;

        if (tileToPlace.Right == leftEnd)
        {
            // ok
        }
        else if (tileToPlace.Left == leftEnd)
        {
            tileToPlace = Flip(tileToPlace);
        }
        else
        {
            return;
        }

        _board.Tiles.Insert(0, tileToPlace);
    }

    public void PlaceTileToRight(IDominoTile tile, int rightEnd)
    {
        var tileToPlace = tile;

        if (tileToPlace.Left == rightEnd)
        {
            // ok
        }
        else if (tileToPlace.Right == rightEnd)
        {
            tileToPlace = Flip(tileToPlace);
        }
        else
        {
            return;
        }

        _board.Tiles.Add(tileToPlace);
    }

    private bool HasAnyPlayableTile(IPlayer player)
    {
        var hand = _playerTiles[player];
        foreach (var tile in hand)
        {
            if (CanPlaceTile(tile))
            {
                return true;
            }
        }

        return false;
    }

    public IDominoTile Flip(IDominoTile tile)
    {
        return new DominoTile(tile.Right, tile.Left);
    }

    // Validation
    public bool CanPlayRule(IPlayer player, IDominoTile tile)
    {
        if (!_playerTiles[player].Contains(tile))
        {
            return false;
        }

        return CanPlaceTile(tile);
    }

    public bool IsTileMatches(int value, IDominoTile tile)
    {
        return tile.Left == value || tile.Right == value;
    }

    public bool IsPlayerWin(IPlayer player)
    {
        return _playerTiles[player].Count == 0;
    }

    // Event
    protected virtual void OnGameLogging(string message)
    {
        GameLogging?.Invoke(message);
    }

    private void Log(string message)
    {
        OnGameLogging($"[INFO] {message}");
    }

    // Helpers
    private int GetLeftEnd()
    {
        if (_board.Tiles.Count == 0)
        {
            return -1;
        }

        return _board.Tiles.First().Left;
    }

    private int GetRightEnd()
    {
        if (_board.Tiles.Count == 0)
        {
            return -1;
        }

        return _board.Tiles.Last().Right;
    }

    private void PrintGameState()
    {
        Console.WriteLine("===== DOMINO GAME =====");
        Console.WriteLine();

        if (_board.Tiles.Count == 0)
        {
            Console.WriteLine("Board: (kosong)");
        }
        else
        {
            Console.Write("Board: ");
            foreach (var tile in _board.Tiles)
            {
                Console.Write(tile + " ");
            }
            Console.WriteLine();
        }

        Console.WriteLine($"Sisa tile di deck: {_deck.Tiles.Count}");
        Console.WriteLine();

        int idx = 1;
        foreach (var p in _players)
        {
            var hand = _playerTiles[p];
            Console.Write($"Pemain {idx} hand ({hand.Count}): ");
            foreach (var tile in hand)
            {
                Console.Write(tile + " ");
            }
            Console.WriteLine();
            idx++;
        }

        Console.WriteLine();
        var currentPlayer = GetCurrentPlayer();
        Console.WriteLine($"Giliran: {currentPlayer.Name}");
        Console.WriteLine("Kartu kamu:");
        var currentHand = _playerTiles[currentPlayer];
        for (int i = 0; i < currentHand.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {currentHand[i]}");
        }

        Console.WriteLine();
    }

    private IDominoTile AskPlayerTileChoice(IPlayer player)
    {
        var hand = _playerTiles[player];

        while (true)
        {
            Console.Write("Pilih nomor tile yang akan dimainkan: ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Input tidak valid.");
                continue;
            }

            if (choice < 1 || choice > hand.Count)
            {
                Console.WriteLine("Nomor di luar jangkauan.");
                continue;
            }

            var tile = hand[choice - 1];

            if (!CanPlayRule(player, tile))
            {
                Console.WriteLine("Tile tidak bisa dimainkan. Pilih tile lain.");
                continue;
            }

            return tile;
        }
    }
}
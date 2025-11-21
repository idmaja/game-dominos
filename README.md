# Domino Game

A console-based implementation of the classic Domino game written in C# and .NET 8.0. This application allows 2 to 4 players to play a standard game of dominoes (double-six set) directly from the terminal.

## Features

- **Multiplayer Support**: Supports 2 to 4 players.
- **Standard Rules**: Uses a standard double-six set (28 tiles).
- **Turn-Based System**: Handles player turns, validates moves, and manages the board state.
- **Deck Management**: Automatic shuffling and dealing of tiles (7 tiles per player initially).
- **Draw Mechanics**: Players automatically draw from the deck if they have no playable tiles.
- **Game End Detection**: Detects when a player wins (empties their hand) or when the game is stuck (no moves possible for anyone).
- **Interactive Console UI**: Displays the current board state, player hands, and prompts for user input.

## Prerequisites

- .NET 8.0 SDK or later
- A compatible IDE (Visual Studio 2022, JetBrains Rider, or VS Code) or a terminal interface.

## How to Run

1. Open your terminal or command prompt.
2. Navigate to the project directory containing the solution file.
3. Run the application using the dotnet CLI:

```bash
   dotnet run --project DominoGame
```

Alternatively, you can open the `DominoGame.sln` file in Visual Studio and click the Start/Run button.

## How to Play

1. **Start**: Upon running the application, enter the number of players (between 2 and 4).
2. **Turns**: The game will display the current state of the board and the tiles in your hand.
3. **Action**:
   - The game will ask you to select a tile by its number index (e.g., 1 for the first tile).
   - If the selected tile matches both ends of the board, you will be prompted to choose which side to play on: Left (L) or Right (R).
   - If you do not have a matching tile, the game will automatically attempt to draw a tile from the deck for you.
4. **Winning**: The game ends when a player plays all their tiles or when the game becomes blocked.

## Project Structure

- **DominoGame/Program.cs**: The entry point of the application. Sets up the players, deck, board, and starts the game loop.
- **DominoGame/controller/DominoGame.cs**: Contains the core game logic, including turn management, rule validation, and state printing.
- **DominoGame/class/**:
  - `DominoTile.cs`: Represents a single domino tile with Left and Right values.
  - `Deck.cs`: Represents the collection of tiles (draw pile).
  - `Board.cs`: Represents the playing area where tiles are placed.
  - `Player.cs`: Represents a player entity.

## License

This project is licensed under the Creative Commons Attribution-NonCommercial 4.0 International Public License (CC BY-NC 4.0). See the LICENSE file for details.

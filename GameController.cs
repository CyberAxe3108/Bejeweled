using System;
using System.Collections.Generic;

namespace Bejeweled;

/// <summary>
/// Drives the main game loop: displays the board, reads player input,
/// validates and executes swaps, resolves chain matches, and refills the grid.
/// </summary>
public class GameController
{
    private readonly Grid _grid;
    private readonly MoveValidator _validator;
    private readonly HintSystem _hint;

    public GameController()
    {
        _grid = new Grid();
        _validator = new MoveValidator();
        _hint = new HintSystem(_grid);
        _grid.FillBoard();
        GameLoop();
    }

    private void GameLoop()
    {
        while (true)
        {
            PrintBoard();
            Console.WriteLine("Choose gem to move:");
            (int r1, int c1) = ReadPosition();

            Console.WriteLine("Swap gem with:");
            (int r2, int c2) = ReadPosition();

            var gem1 = _grid.gems[r1, c1];
            var gem2 = _grid.gems[r2, c2];

            if (!_validator.AreAdjacent(gem1, gem2))
            {
                Console.WriteLine("Gems are not adjacent. Try again.");
                continue;
            }

            if (!_validator.IsValidSwap(gem1, gem2, _grid))
            {
                Console.WriteLine("That swap produces no match. Try again.");
                continue;
            }

            // Execute the swap
            _grid.SwapGems((r1, c1), (r2, c2));

            // Resolve all cascading matches
            List<Match> matches;
            bool isChain = false;
            while ((matches = _grid.FindMatches()).Count > 0)
            {
                // Mark chain matches (cascades after the first resolution)
                if (isChain)
                    foreach (var m in matches)
                        m.isChain = true;

                _grid.RemoveMatches(matches);
                PrintBoard();
                Console.WriteLine();

                _grid.DropGems();
                PrintBoard();
                Console.WriteLine();

                _grid.FillBoard();
                PrintBoard();
                Console.WriteLine();

                isChain = true;
            }

            // Check for game-over (no valid moves left)
            if (!_grid.HasValidMove())
            {
                PrintBoard();
                Console.WriteLine("No valid moves remain. Game over!");
                break;
            }
        }
    }

    private (int row, int col) ReadPosition()
    {
        int r, c;
        while (true)
        {
            if (!TryReadInt("Enter Row (0-7):", out r) || !TryReadInt("Enter Column (0-7):", out c))
            {
                Console.WriteLine("Invalid input. Please enter numbers only.");
                continue;
            }
            if (r < 0 || r > 7 || c < 0 || c > 7)
            {
                Console.WriteLine("Out of range. Row and Column must be between 0 and 7.");
                continue;
            }
            return (r, c);
        }
    }

    private bool TryReadInt(string prompt, out int value)
    {
        Console.WriteLine(prompt);
        return int.TryParse(Console.ReadLine(), out value);
    }

    private void PrintBoard()
    {
        // Column header
        Console.Write("    ");
        for (int c = 0; c < 8; c++)
            Console.Write($"{c}  ");
        Console.WriteLine();

        for (int r = 0; r < 8; r++)
        {
            Console.Write($"{r}  ");
            for (int c = 0; c < 8; c++)
            {
                var gem = _grid.gems[r, c];
                string emoji = gem == null ? "⬛" : GetEmoji(gem.Type);
                Console.Write($"{emoji} ");
            }
            Console.WriteLine();
        }
    }

    private static string GetEmoji(GemType type) => type switch
    {
        GemType.Ruby      => "🟥",
        GemType.Sapphire  => "🟦",
        GemType.Emerald   => "🟩",
        GemType.Topaz     => "🟨",
        GemType.Amethyst  => "🟪",
        GemType.Diamond   => "🔷",
        _                 => "❓"
    };
}
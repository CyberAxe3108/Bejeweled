using System;
using System.Collections.Generic;

namespace Bejeweled;

public class MoveValidator
{
    private const int GridSize = 8;

    private static readonly (int row, int col)[] Directions =
    {
        (-1,  0),   // up
        ( 1,  0),   // down
        ( 0, -1),   // left
        ( 0,  1)    // right
    };

    // Checks if two gems are orthogonally adjacent
    public bool AreAdjacent(Gem gem1, Gem gem2)
    {
        if (gem1 == null || gem2 == null)
            return false;

        int rowDiff = Math.Abs(gem1.Row - gem2.Row);
        int colDiff = Math.Abs(gem1.Column - gem2.Column);

        return (rowDiff == 1 && colDiff == 0) ||
               (rowDiff == 0 && colDiff == 1);
    }

    // A swap is legal only if the gems are adjacent AND the swap produces a match
    public bool IsValidSwap(Gem gem1, Gem gem2, Grid grid)
    {
        if (gem1 == null || gem2 == null)
            return false;

        if (!AreAdjacent(gem1, gem2))
            return false;

        if (!IsInBounds(gem1.Row, gem1.Column) || !IsInBounds(gem2.Row, gem2.Column))
            return false;

        // Use WouldMatch directly (it does its own swap and restore)
        return grid.WouldMatch((gem1.Row, gem1.Column), (gem2.Row, gem2.Column));
    }

    // Returns adjacent gems, skipping null cells
    public List<Gem> GetAdjacentGems(Gem[,] grid, Gem gem)
    {
        if (grid == null || gem == null)
            return new List<Gem>();

        var adjacent = new List<Gem>();
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        foreach (var dir in Directions)
        {
            int newRow = gem.Row + dir.row;
            int newCol = gem.Column + dir.col;

            if (newRow >= 0 && newRow < rows &&
                newCol >= 0 && newCol < cols &&
                grid[newRow, newCol] != null)          // skip empty cells
            {
                adjacent.Add(grid[newRow, newCol]);
            }
        }

        return adjacent;
    }

    // Returns all legal swaps on the board — used for hints and game-over detection
    public List<(Gem, Gem)> GetAllValidMoves(Grid grid)
    {
        var moves = new List<(Gem, Gem)>();

        for (int r = 0; r < GridSize; r++)
        {
            for (int c = 0; c < GridSize; c++)
            {
                var gem = grid.gems[r, c];
                if (gem == null) continue;

                // Only check right and down to avoid duplicate pairs
                if (c + 1 < GridSize)
                {
                    var neighbor = grid.gems[r, c + 1];
                    if (neighbor != null && IsValidSwap(gem, neighbor, grid))
                        moves.Add((gem, neighbor));
                }

                if (r + 1 < GridSize)
                {
                    var neighbor = grid.gems[r + 1, c];
                    if (neighbor != null && IsValidSwap(gem, neighbor, grid))
                        moves.Add((gem, neighbor));
                }
            }
        }

        return moves;
    }

    // Convenience method for game-over detection
    public bool HasAnyValidMove(Grid grid) => GetAllValidMoves(grid).Count > 0;

    // Returns a single hint move, or null if no moves exist
    public (Gem, Gem)? GetHint(Grid grid)
    {
        var moves = GetAllValidMoves(grid);
        if (moves.Count == 0) return null;

        return moves[new Random().Next(moves.Count)];
    }

    private bool IsInBounds(int row, int col) =>
        row >= 0 && row < GridSize && col >= 0 && col < GridSize;
}
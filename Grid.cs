using System;
using System.Collections.Generic;

namespace Bejeweled;
    public class Grid
    {
        private const int Size = 8;
        private static readonly string[] GemTypes = { "Ruby", "Sapphire", "Emerald", "Topaz", "Amethyst", "Diamond" };
        private static readonly Random Rng = new Random();
        private readonly MoveValidator _validator = new MoveValidator();

        public Gem[,] gems { get; set; } = new Gem[Size, Size];

        public void FillBoard()
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (gems[r, c] == null)
                        gems[r, c] = SpawnGemWithoutMatch(r, c);
                }
            }
        }

        // Picks a random type that won't immediately form a match at (r, c)
        private Gem SpawnGemWithoutMatch(int r, int c)
        {
            var available = new List<string>(GemTypes);

            // Exclude types that would complete a horizontal run of 3
            if (c >= 2
                && gems[r, c - 1] != null
                && gems[r, c - 2] != null
                && gems[r, c - 1].Type == gems[r, c - 2].Type)
            {
                available.Remove(gems[r, c - 1].Type.ToString()); // Convert to string
            }

            // Exclude types that would complete a vertical run of 3
            if (r >= 2
                && gems[r - 1, c] != null
                && gems[r - 2, c] != null
                && gems[r - 1, c].Type == gems[r - 2, c].Type)
            {
                available.Remove(gems[r - 1, c].Type.ToString()); //  Convert to string
            }

            if (available.Count == 0)
                available = new List<string>(GemTypes);

            string typeName = available[Rng.Next(available.Count)];
            GemType gemType = Enum.Parse<GemType>(typeName); // Parse to enum
            var gem = new Gem(gemType, r, c);
            return gem;
        }

        public void SwapGems((int r, int c) pos1, (int r, int c) pos2)
        {
            var temp = gems[pos1.r, pos1.c];
            gems[pos1.r, pos1.c] = gems[pos2.r, pos2.c];
            gems[pos2.r, pos2.c] = temp;

            // Keep gem position properties in sync
            if (gems[pos1.r, pos1.c] != null)
            {
                gems[pos1.r, pos1.c].Row = pos1.r;
                gems[pos1.r, pos1.c].Column = pos1.c;
            }
            if (gems[pos2.r, pos2.c] != null)
            {
                gems[pos2.r, pos2.c].Row = pos2.r;
                gems[pos2.r, pos2.c].Column = pos2.c;
            }
        }

        public bool WouldMatch((int r, int c) pos1, (int r, int c) pos2)
        {
            SwapGems(pos1, pos2);
            var matches = FindMatches();
            SwapGems(pos1, pos2);   // always restore — no side effects
            return matches.Count > 0;
        }

        public List<Match> FindMatches() => Match.FindMatches(gems);

        public void RemoveMatches(List<Match> matches)
        {
            foreach (var match in matches)
                foreach (var gem in match.gems)
                    gems[gem.Row, gem.Column] = null;
        }

        public void DropGems()
{
    for (int c = 0; c < Size; c++)
    {
        for (int r = Size - 1; r >= 0; r--)
        {
            if (gems[r, c] == null)
            {
                for (int k = r - 1; k >= 0; k--)
                {
                    if (gems[k, c] != null)
                    {
                        gems[r, c] = gems[k, c];
                        gems[k, c] = null;

                        // Add null check before updating position
                        if (gems[r, c] != null)
                        {
                            gems[r, c].Row = r;
                            gems[r, c].Column = c;
                        }
                        break;
                    }
                }
            }
        }
    }
}
        // Returns true if at least one valid swap exists on the board
        public bool HasValidMove()
    {
        return _validator.HasAnyValidMove(this); // Delegate to validator
    }
    }

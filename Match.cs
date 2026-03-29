using System.Collections.Generic;
using System;

namespace Bejeweled;

    public class Match
    {
        public bool isChain { get; set; }       // Set externally by GameManager on cascade
        public bool isPowerGem => gems.Count == 4;
        public bool isHyperCube => gems.Count >= 5;
        public List<Gem> gems { get; set; }
        public MatchDirection direction { get; set; }

        public enum MatchDirection { Horizontal, Vertical }

        public Match(List<Gem> matchedGems, MatchDirection dir)
        {
            gems = matchedGems;
            direction = dir;
            isChain = false;    // GameManager sets this to true during cascade
        }

        public static List<Match> FindMatches(Gem[,] board)
        {
            var matches = new List<Match>();
            if (board == null) return matches;

            int rows = board.GetLength(0);
            int cols = board.GetLength(1);

            // Horizontal runs
            for (int r = 0; r < rows; r++)
            {
                int c = 0;
                while (c < cols)
                {
                    if (board[r, c] == null) { c++; continue; }

                    var startGem = board[r, c];
                    var run = new List<Gem> { startGem };
                    int scan = c + 1;

                    while (scan < cols
                        && board[r, scan] != null
                        && board[r, scan].Type == startGem.Type)
                    {
                        run.Add(board[r, scan]);
                        scan++;
                    }

                    if (run.Count >= 3)
                        matches.Add(new Match(run, Match.MatchDirection.Horizontal));

                    c = scan;
                }
            }

            // Vertical runs
            for (int c = 0; c < cols; c++)
            {
                int r = 0;
                while (r < rows)
                {
                    if (board[r, c] == null) { r++; continue; }

                    var startGem = board[r, c];
                    var run = new List<Gem> { startGem };
                    int scan = r + 1;

                    while (scan < rows
                        && board[scan, c] != null
                        && board[scan, c].Type == startGem.Type)
                    {
                        run.Add(board[scan, c]);
                        scan++;
                    }

                    if (run.Count >= 3)
                        matches.Add(new Match(run, Match.MatchDirection.Vertical));

                    r = scan;
                }
            }

            // Merge overlapping matches (L-shapes, T-shapes, crosses)
            MergeOverlappingMatches(matches);

            return matches;
        }

        private static void MergeOverlappingMatches(List<Match> matches)
        {
            bool merged = true;
            while (merged)
            {
                merged = false;
                for (int i = 0; i < matches.Count; i++)
                {
                    for (int j = i + 1; j < matches.Count; j++)
                    {
                        if (SharesGem(matches[i], matches[j]))
                        {
                            // Combine into one match, remove duplicates
                            var combined = new HashSet<Gem>(matches[i].gems);
                            combined.UnionWith(matches[j].gems);

                            matches[i] = new Match(
                                new List<Gem>(combined),
                                matches[i].direction   // keep the first direction as dominant
                            );
                            matches.RemoveAt(j);
                            merged = true;
                            break;
                        }
                    }
                    if (merged) break;
                }
            }
        }

        private static bool SharesGem(Match a, Match b)
        {
            foreach (var gem in a.gems)
                if (b.gems.Contains(gem))
                    return true;
            return false;
        }
    }

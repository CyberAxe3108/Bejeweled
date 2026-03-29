
using System;

namespace Bejeweled;

    public class HintSystem
    {
        private readonly Grid _grid;
        private readonly MoveValidator _validator;

        public HintSystem(Grid gameGrid)
        {
            _grid = gameGrid ?? throw new ArgumentNullException(nameof(gameGrid));
            _validator = new MoveValidator();
        }
        public (int Row, int Column)? GetHint()
        {
            var hint = _validator.GetHint(_grid);
            if (hint.HasValue)
            {
                var (gem1, _) = hint.Value;
                return (gem1.Row, gem1.Column);
            }
            return null;
        }
    }


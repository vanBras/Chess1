using System.Collections.Generic;
using System.Drawing;

namespace Chess_4_with_Framework.Pieces
{
    public class PowerPiece : Piece
    {
        public PowerPiece(Point pos, EColor ecolor) : base(pos, ecolor)
        {
            Type = EPieceType.PowerPiece;
            Text = ecolor == EColor.White ? "\u26C1" : "\u26C3";
            Value = 0;
        }

        public override Piece Clone()
        {
            Piece clone = new PowerPiece(Pos, Color);
            clone.Taken = Taken;
            clone.HasMoved = HasMoved;
            return clone;
        }

        public override List<Point> GenerateMoves(Board board)
        {
            List<Point> moves = new List<Point>();
            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    moves.Add(new Point(x, y));
                }
            }

            return moves;
        }
    }
}
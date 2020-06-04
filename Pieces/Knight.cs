using System.Collections.Generic;
using System.Drawing;

namespace Chess_4_with_Framework.Pieces
{
    public class Knight : Piece
    {
        public Knight(Point pos, EColor ecolor) : base(pos, ecolor)
        {
            Type = EPieceType.Knight;
            Text = ecolor == EColor.White ? "\u2658" : "\u265E";
            Value = 3;
        }

        public override Piece Clone()
        {
            Piece clone = new Knight(Pos, Color);
            clone.Taken = Taken;
            clone.HasMoved = HasMoved;
            return clone;
        }

        public override List<Point> GenerateMoves(Board board)
        {
            List<Point> moves = new List<Point>();
            
            moves.Add(Pos);

            for (int x = -2; x < 3; x += 4)
            {
                for (int y = -1; y < 2; y += 2)
                {
                    Point point = new Point(x + Pos.X, y + Pos.Y);
                    if (WithinBounds(point) && point != Pos && !AttackingAllies(point, board))
                    {
                        moves.Add(point);
                    }
                }
            }
            
            for (int x = -1; x < 2; x += 2)
            {
                for (int y = -2; y < 3; y += 4)
                {
                    Point point = new Point(x + Pos.X, y + Pos.Y);
                    if (WithinBounds(point) && point != Pos && !AttackingAllies(point, board))
                    {
                        moves.Add(point);
                    }
                }
            }
            
            return moves;
        }
    }
}
using System.Collections.Generic;
using System.Drawing;

namespace Chess_4_with_Framework.Pieces
{
    public class King : Piece
    {
        public King(Point pos, EColor ecolor) : base(pos, ecolor)
        {
            Type = EPieceType.King;
            Text = ecolor == EColor.White ? "\u2654" : "\u265A";
            Value = 99;
        }

        public override Piece Clone()
        {
            Piece clone = new King(Pos, Color);
            clone.Taken = Taken;
            clone.HasMoved = HasMoved;
            return clone;
        }
        
        public override List<Point> GenerateMoves(Board board)
        {
            List<Point> moves = new List<Point>();
            moves.Add(Pos);
            
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    Point pos = new Point(Pos.X + x,Pos.Y + y);
                    if (WithinBounds(pos) && pos != Pos && !AttackingAllies(pos, board))
                    {
                        moves.Add(pos);
                    }
                }
            }  
            return moves;
        }
    }
}
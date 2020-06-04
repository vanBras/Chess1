using System.Collections.Generic;
using System.Drawing;
using Chess_4_with_Framework.Pieces;

namespace Chess_4_with_Framework.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(Point pos, EColor ecolor) : base(pos, ecolor)
        {
            Type = EPieceType.Bishop;
            Text = ecolor == EColor.White ? "\u2657" : "\u265D";
            Value = 3;
        }

        public override Piece Clone()
        {
            Piece clone = new Bishop(Pos, Color);
            clone.Taken = Taken;
            clone.HasMoved = HasMoved;
            return clone;
        }

        public override List<Point> GenerateMoves(Board board)
        {
            List<Point> moves = new List<Point>();
            Point point;
            moves.Add(Pos);
            for (int i = 0; i < 12; i++)
            {
                point = new Point(i, Pos.Y - (Pos.X - i));
                if (point.X != Pos.X && CheckMove(point, board))
                    moves.Add(point);

                point = new Point(Pos.X + (Pos.Y - i), i);
                if (point.Y != Pos.Y && CheckMove(point, board))
                    moves.Add(point);
            }
            
            return moves;
        }
    }
}
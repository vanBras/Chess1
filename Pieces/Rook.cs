using System.Collections.Generic;
using System.Drawing;

namespace Chess_4_with_Framework.Pieces
{
    public class Rook : Piece
    {
        public Rook(Point pos, EColor ecolor) : base(pos, ecolor)
        {
            Type = EPieceType.Rook;
            Text = ecolor == EColor.White ? "\u2656" : "\u265C";
            Value = 5;
        }

        public override Piece Clone()
        {
            Piece clone = new Rook(Pos, Color);
            clone.Taken = Taken;
            clone.HasMoved = HasMoved;
            return clone;
        }

        public override List<Point> GenerateMoves(Board board)
        {
            List<Point> moves = new List<Point>();
            moves.Add(Pos);
            Point point;
            
            for (int i = 0; i < 12; i++)
            {
                point = new Point(i, Pos.Y);
                if (point.X != Pos.X && CheckMove(point, board))
                    moves.Add(point);

                point = new Point(Pos.X, i);
                if (point.Y != Pos.Y && CheckMove(point, board))
                    moves.Add(point);
            }

            return moves;
        }
    }
}
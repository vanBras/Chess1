using System.Collections.Generic;
using System.Drawing;

namespace Chess_4_with_Framework.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(Point pos, EColor ecolor) : base(pos, ecolor)
        {
            Type = EPieceType.Pawn;
            Text = ecolor == EColor.White ? "\u2659" : "\u265F";
            Value = 1;
        }

        public override Piece Clone()
        {
            Piece clone = new Pawn(Pos, Color);
            clone.Taken = Taken;
            clone.HasMoved = HasMoved;
            return clone;
        }

        public override List<Point> GenerateMoves(Board board)
        {
            List<Point> moves = new List<Point>();
            Point point;
            moves.Add(Pos);

            for (int y = -1; y < 2; y += 2)
            {
                point = new Point(Color == EColor.White ? Pos.X + 1 : Pos.X - 1, Pos.Y + y);
                if (WithinBounds(point) && !AttackingAllies(point, board) && board.IsPieceAt(point))
                    moves.Add(point);
            }

            point = new Point(Color == EColor.White ? Pos.X + 1 : Pos.X - 1, Pos.Y);
            if (CheckMove(point, board) && !board.IsPieceAt(point))
                moves.Add(point);
            
            if (!HasMoved)
            {
                point = new Point(Color == EColor.White ? Pos.X + 2 : Pos.X - 2, Pos.Y);
                if (CheckMove(point, board) && !board.IsPieceAt(point))
                    moves.Add(point);
            }
            
            return moves;
        }
    }
}
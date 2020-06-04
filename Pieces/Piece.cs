using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Point = System.Drawing.Point;


namespace Chess_4_with_Framework.Pieces
{
    public abstract class Piece
    {
        public Point Pos { get; set; }
        public EColor Color { get; set; }
        public bool Taken { get; set; }
        public string Text { get; set; }
        public int Value { get; set; }
        public EPieceType Type { get; set; }
        public bool HasMoved { get; set; }

        protected Piece(Point pos, EColor ecolor)
        {
            Pos = pos;
            Color = ecolor;
            Taken = false;
            Text = "";
            HasMoved = false;
        }

        public abstract Piece Clone();
        public abstract List<Point> GenerateMoves(Board board);
        public static Label Show(Piece piece)
        {
            Label label = new Label
            {
                Content = piece.Text,
                FontSize = 50,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = MainWindow.TileSize,
                Width = MainWindow.TileSize,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(MainWindow.TileSize * piece.Pos.X, MainWindow.TileSize * piece.Pos.Y, 0, 0)
            };
            return label;
        }

        public List<Board> GenerateNewBoards(Board currentBoard)
        {
            List<Board> boards = new List<Board>();
            List<Point> moves = GenerateMoves(currentBoard);
            foreach (var point in moves)
            {
                Board board = currentBoard.Clone();
                boards.Add(board);
            }
            return boards;
        }

        public bool WithinBounds(Point pos)
        {
            return pos.X >= 2 && pos.X <= 9 && pos.Y >= 0 && pos.Y <= 7;
        }

        public void Move(Point pos, Board board)
        {
            var attacking = board.GetPieceAt(pos);
            if (attacking != null)
            {
                attacking.Taken = true;
                attacking.Move(ReturnToStorage(attacking, board, false), board);
            }
            Pos = pos;
            HasMoved = true;
            //Add something more.
        }
        
        public Point ReturnToStorage(Piece piece, Board board, bool robot)
        {
            var enumerable = from st in board.StorageList where st.Type == piece.Type && st.Color == piece.Color && !st.Taken select st;
            var storage = enumerable.First();
            if (!robot)
                storage.Taken = true; 
            
            return storage.Point;
        }

        public bool AttackingAllies(Point pos, Board board)
        {
            var attacking = board.GetPieceAt(pos);
            return attacking != null && attacking.Color == Color;
        }

        private bool MoveThroughPieces(Point pos, Board board)
        {
            var stepDirectionX = pos.X - Pos.X;
            if (stepDirectionX > 0)
                stepDirectionX = 1;
            else if (stepDirectionX < 0)
                stepDirectionX = -1;

            var stepDirectionY = pos.Y - Pos.Y;
            if (stepDirectionY > 0)
                stepDirectionY = 1;
            else if (stepDirectionY < 0)
                stepDirectionY = -1;

            Point tempPos = new Point(Pos.X, Pos.Y);
            tempPos.X += stepDirectionX;
            tempPos.Y += stepDirectionY;
            while (tempPos.X != pos.X || tempPos.Y != pos.Y)
            {
                if (board.GetPieceAt(tempPos) != null)
                {
                    return true;
                }

                tempPos.X += stepDirectionX;
                tempPos.Y += stepDirectionY;
            }

            return false;
        }

        public bool CheckMove(Point pos, Board board)
        {
            return WithinBounds(pos) && (!AttackingAllies(pos, board) && !MoveThroughPieces(pos, board));
        }
    }
}
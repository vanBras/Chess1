using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Chess_4_with_Framework.Pieces;
using Point = System.Drawing.Point;

namespace Chess_4_with_Framework
{
    public class Board
    {
        public List<Piece> WhitePieces;
        public List<Piece> BlackPieces;
        public bool Filled;
        public List<Storage> StorageList;
        public bool Test = false;

        public Board()
        {
            WhitePieces = new List<Piece>();
            BlackPieces = new List<Piece>();
            StorageList = GetStorageList();
            Filled = false;
            FillStorage();
        }

        public void FillBoard()
        {
            foreach (var piece in WhitePieces)
                piece.Pos = new Point(piece.Pos.X + 2, piece.Pos.Y);
            foreach (var piece in BlackPieces)
                piece.Pos = new Point(piece.Pos.X - 2, piece.Pos.Y);
            foreach (var storage in StorageList)
                storage.Taken = false;
            Filled = true;
        }

        public void Move(Point from, Point to)
        {
            Piece pieceToMove = GetPieceAt(from);
            pieceToMove?.Move(to, this);
        }

        public List<Label> Show()
        {
            List<Label> labels = new List<Label>();
            labels.AddRange(WhitePieces.Union(BlackPieces).ToList().Select(Piece.Show));
            return labels;
        }

        public bool IsPieceAt(Point pos)
        {
            if (WhitePieces.Any(piece => piece.Pos.X == pos.X && piece.Pos.Y == pos.Y && !piece.Taken))
                return true;

            if (BlackPieces.Any(piece => piece.Pos.X == pos.X && piece.Pos.Y == pos.Y && !piece.Taken))
                return true;

            return false;
        }

        public Piece GetPieceAt(Point pos)
        {
            foreach (var piece in WhitePieces.Where(piece => !piece.Taken && piece.Pos == pos))
                return piece;

            foreach (var piece in BlackPieces.Where(piece => !piece.Taken && piece.Pos == pos))
                return piece;

            return null;
        }

        public Board Clone()
        {
            Board clone = new Board();

            foreach (var piece in WhitePieces)
                clone.WhitePieces.Add(piece);

            foreach (var piece in BlackPieces)
                clone.BlackPieces.Add(piece);

            foreach (var storage in StorageList)
                clone.StorageList.Add(storage);

            clone.Filled = Filled;
            return clone;
        }

        public List<Storage> GetStorageList()
        {
            if (Test)
            {
                List<Storage> storageList = new List<Storage>()
                {
                    new Storage(new Point(0, 0), EPieceType.PowerPiece, EColor.White),
                    new Storage(new Point(11, 3), EPieceType.PowerPiece, EColor.Black),
                };
                return storageList;
            }
            else
            {
                List<Storage> storageList = new List<Storage>()
                {
                    new Storage(new Point(0, 0), EPieceType.Rook, EColor.White),
                    new Storage(new Point(0, 1), EPieceType.Knight, EColor.White),
                    new Storage(new Point(0, 2), EPieceType.Bishop, EColor.White),
                    new Storage(new Point(0, 3), EPieceType.Queen, EColor.White),
                    new Storage(new Point(0, 4), EPieceType.King, EColor.White),
                    new Storage(new Point(0, 5), EPieceType.Bishop, EColor.White),
                    new Storage(new Point(0, 6), EPieceType.Knight, EColor.White),
                    new Storage(new Point(0, 7), EPieceType.Rook, EColor.White),

                    new Storage(new Point(1, 0), EPieceType.Pawn, EColor.White),
                    new Storage(new Point(1, 1), EPieceType.Pawn, EColor.White),
                    new Storage(new Point(1, 2), EPieceType.Pawn, EColor.White),
                    new Storage(new Point(1, 3), EPieceType.Pawn, EColor.White),
                    new Storage(new Point(1, 4), EPieceType.Pawn, EColor.White),
                    new Storage(new Point(1, 5), EPieceType.Pawn, EColor.White),
                    new Storage(new Point(1, 6), EPieceType.Pawn, EColor.White),
                    new Storage(new Point(1, 7), EPieceType.Pawn, EColor.White),

                    new Storage(new Point(11, 0), EPieceType.Rook, EColor.Black),
                    new Storage(new Point(11, 1), EPieceType.Knight, EColor.Black),
                    new Storage(new Point(11, 2), EPieceType.Bishop, EColor.Black),
                    new Storage(new Point(11, 3), EPieceType.Queen, EColor.Black),
                    new Storage(new Point(11, 4), EPieceType.King, EColor.Black),
                    new Storage(new Point(11, 5), EPieceType.Bishop, EColor.Black),
                    new Storage(new Point(11, 6), EPieceType.Knight, EColor.Black),
                    new Storage(new Point(11, 7), EPieceType.Rook, EColor.Black),

                    new Storage(new Point(10, 0), EPieceType.Pawn, EColor.Black),
                    new Storage(new Point(10, 1), EPieceType.Pawn, EColor.Black),
                    new Storage(new Point(10, 2), EPieceType.Pawn, EColor.Black),
                    new Storage(new Point(10, 3), EPieceType.Pawn, EColor.Black),
                    new Storage(new Point(10, 4), EPieceType.Pawn, EColor.Black),
                    new Storage(new Point(10, 5), EPieceType.Pawn, EColor.Black),
                    new Storage(new Point(10, 6), EPieceType.Pawn, EColor.Black),
                    new Storage(new Point(10, 7), EPieceType.Pawn, EColor.Black),
                };
                return storageList;
            }
        }

        private void FillStorage()
        {
            foreach (var storage in StorageList)
            {
                switch (storage.Type)
                {
                    case EPieceType.Pawn:
                        if (storage.Color == EColor.White)
                            WhitePieces.Add(new Pawn(storage.Point, storage.Color));
                        else
                            BlackPieces.Add(new Pawn(storage.Point, storage.Color));
                        break;
                    case EPieceType.Rook:
                        if (storage.Color == EColor.White)
                            WhitePieces.Add(new Rook(storage.Point, storage.Color));
                        else
                            BlackPieces.Add(new Rook(storage.Point, storage.Color));
                        break;
                    case EPieceType.Knight:
                        if (storage.Color == EColor.White)
                            WhitePieces.Add(new Knight(storage.Point, storage.Color));
                        else
                            BlackPieces.Add(new Knight(storage.Point, storage.Color));
                        break;
                    case EPieceType.Bishop:
                        if (storage.Color == EColor.White)
                            WhitePieces.Add(new Bishop(storage.Point, storage.Color));
                        else
                            BlackPieces.Add(new Bishop(storage.Point, storage.Color));
                        break;
                    case EPieceType.Queen:
                        if (storage.Color == EColor.White)
                            WhitePieces.Add(new Queen(storage.Point, storage.Color));
                        else
                            BlackPieces.Add(new Queen(storage.Point, storage.Color));
                        break;
                    case EPieceType.King:
                        if (storage.Color == EColor.White)
                            WhitePieces.Add(new King(storage.Point, storage.Color));
                        else
                            BlackPieces.Add(new King(storage.Point, storage.Color));
                        break;
                    case EPieceType.PowerPiece:
                        if (storage.Color == EColor.White)
                            WhitePieces.Add(new PowerPiece(storage.Point, storage.Color));
                        else
                            BlackPieces.Add(new PowerPiece(storage.Point, storage.Color));
                        break;
                }

                storage.Taken = true;
            }
        }

        public List<Board> GenerateNewBoardTurn(EColor color)
        {
            return (color == EColor.White ? WhitePieces : BlackPieces).Where(piece => !piece.Taken).SelectMany(piece => piece.GenerateNewBoards(this)).ToList();
        }

        public bool IsDone()
        {
            return WhitePieces.Exists(piece => piece.Type == EPieceType.King && piece.Taken) || BlackPieces.Exists(piece => piece.Type == EPieceType.King && piece.Taken);
        }

        public List<Piece> PiecesOnTheBoard()
        {
            List<Piece> list = new List<Piece>();
            list.AddRange(WhitePieces.Where(piece => !piece.Taken));
            list.AddRange(BlackPieces.Where(piece => !piece.Taken));
            return list;
        }
    }
}
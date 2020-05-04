using ChessConsole.Pieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChessConsole
{
    public class ChessBoard
    {
        public class Cell
        {
            public ChessBoard Parent
            {
                private set;
                get;
            }

            public int X; // 0-7 -> A-H mapping on an actual chessboard 

            public int Y; // 0-7 -> 1-8 mapping on an actual chessboard

            public Piece Piece;

            public List<Piece> HitBy;

            public Cell(ChessBoard parent, int x, int y)
            {
                Parent = parent;
                HitBy = new List<Piece>();
                X = x;
                Y = y;
            }

            /// <param name="desiredCount">The ammount of consecutive cells to return (until outside of the board)</param>
            public IEnumerable<Cell> OpenLineOfSight(int directionX, int directionY, int desiredCount = 1)
            {
                for (int i = 1; i <= desiredCount; i++)
                {
                    Cell cell = Parent.GetCell(X + directionX * i, Y + directionY * i);
                    if (cell == null) yield break; //the cell is out of the board

                    yield return cell;

                    //Stop anyway as line of sight is blocked
                    if (cell.Piece != null)
                        yield break;
                }
            }

            /// <summary>
            /// Returns a cell on the board relative to this one.
            /// </summary>
            /// <param name="x">Relative X-coordinate of the cell</param>
            /// <param name="y">Relative X-coordinate of the cell</param>
            /// <returns>Node at (x, y) position or null if index is out of bounds</returns>
            public Cell Open(int x, int y)
            {
                Cell cell = Parent.GetCell(X + x, Y + y);
                return cell ?? null;
            }
        }

        private Cell[,] cells;

        public Cell EnPassant
        {
            private set;
            get;
        }

        /// <summary>
        /// The cell where the pawn will be captured after en passant is performed
        /// </summary>
        public Cell EnPassantCapture

        {
            private set;
            get;
        }

        private List<Piece> pieces = new List<Piece>();

        private Piece blackKing;
        private Piece whiteKing;
        private bool inCheck;

        public ChessBoard()
        {
            ResetBoardState();
        }

        #region Getters

        public Cell GetCell(int x, int y)
        {
            if (x < 0 || cells.GetLength(0) <= x || y < 0 || cells.GetLength(1) <= y) return null;

            return cells[x, y];
        }

        #endregion

        #region HelperMethods

        private void AddPiece(Cell cell, Piece piece)
        {
            cell.Piece = piece;
            pieces.Add(piece);
            piece.OnPlace(cell);
        }

        #endregion

        #region InterfaceMethods

        public void ResetBoardState()
        {
            int boardSize = 8;
            cells = new Cell[boardSize, boardSize];
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    cells[i, j] = new Cell(this, i, j);
                }
            }

            pieces.Clear();

            EnPassant = null;
            EnPassantCapture = null;

            OrderChessPiecesByColor(PlayerColor.White);

            OrderChessPiecesByColor(PlayerColor.Black);

            foreach (Piece piece in pieces)
            {
                piece.Recalculate();
            }
        }

        private void OrderChessPiecesByColor(PlayerColor color)
        {
            if (color == PlayerColor.White)
            {
                AddPiece(cells[0, 0], new Rook(PlayerColor.White));
                AddPiece(cells[1, 0], new Knight(PlayerColor.White));
                AddPiece(cells[2, 0], new Bishop(PlayerColor.White));
                AddPiece(cells[3, 0], new Queen(PlayerColor.White));
                AddPiece(cells[4, 0], (whiteKing = new King(PlayerColor.White)));
                AddPiece(cells[5, 0], new Bishop(PlayerColor.White));
                AddPiece(cells[6, 0], new Knight(PlayerColor.White));
                AddPiece(cells[7, 0], new Rook(PlayerColor.White));

                for (int i = 0; i <= 7; i++)
                {
                    AddPiece(cells[i, 1], new Pawn(PlayerColor.White));
                }
            }
            else if (color == PlayerColor.Black)
            {
                AddPiece(cells[0, 7], new Rook(PlayerColor.Black));
                AddPiece(cells[1, 7], new Knight(PlayerColor.Black));
                AddPiece(cells[2, 7], new Bishop(PlayerColor.Black));
                AddPiece(cells[3, 7], new Queen(PlayerColor.Black));
                AddPiece(cells[4, 7], (blackKing = new King(PlayerColor.Black)));
                AddPiece(cells[5, 7], new Bishop(PlayerColor.Black));
                AddPiece(cells[6, 7], new Knight(PlayerColor.Black));
                AddPiece(cells[7, 7], new Rook(PlayerColor.Black));

                for (int i = 0; i <= 7; i++)
                {
                    AddPiece(cells[i, 6], new Pawn(PlayerColor.Black));
                }
            }

        }

        public bool TurnStart(PlayerColor currentPlayer)
        {
            foreach (Cell cell in cells)
            {
                cell.HitBy.Clear();
            }

            foreach (Piece piece in pieces)
            {
                piece.Recalculate();
            }

            bool anyLegalMove = false;

            foreach (Piece piece in pieces)
            {
                piece.LegalMoves.Clear();
                foreach (Cell move in piece.PossibleMoves)
                {
                    if (piece.Color == currentPlayer && IsMoveLegal(piece, move))
                    {
                        piece.LegalMoves.Add(move);
                        anyLegalMove = true;
                    }
                }
            }

            return anyLegalMove;
        }

        private bool IsMoveLegal(Piece piece, Cell move)
        {
            //The strategy is to try everything that can fail and return true only if nothing fails

            if (piece is King)
            {
                //If some enemy hits where we move we can't move with the king
                foreach (Piece hitter in move.HitBy)
                {
                    if (hitter.Parent != move && hitter.Color != piece.Color)
                        return false;
                }

                //Validate castling
                if (Math.Abs(move.X - piece.Parent.X) == 2)
                {
                    //You can't castle in check
                    if (inCheck)
                        return false;

                    //Check if some enemy hits the middle castling
                    foreach (Piece hitter in GetCell(move.X > piece.Parent.X ? move.X - 1 : move.X + 1, move.Y).HitBy)
                    {
                        if (hitter.Color != piece.Color)
                            return false;
                    }
                }
            }
            else
            {
                Piece currentKing = piece.Color == PlayerColor.White ? whiteKing : blackKing;

                if (inCheck)
                {
                    //Let's try capturing or blocking the attacker, keep in mind that we can't unblock another attacker
                    foreach (Piece hitter in currentKing.Parent.HitBy)
                    {
                        if (hitter.Color == currentKing.Color) continue; //Same color don't care
                        if (hitter.Parent == move) continue; //Was captured
                        if (hitter.IsBlockedIfMove(piece.Parent, move, currentKing.Parent)) continue;

                        return false;
                    }
                }

                foreach (Piece hitter in piece.Parent.HitBy)
                {
                    if (hitter.Color == currentKing.Color) continue;
                    if (hitter.Parent == move) continue;

                    if (!hitter.IsBlockedIfMove(piece.Parent, move, currentKing.Parent))
                        return false;
                }
            }


            return true;
        }

        public bool IsInCheck(PlayerColor playerColor, bool useCache = true)
        {
            if (useCache)
                return inCheck;

            if (playerColor == PlayerColor.White)
                return whiteKing.Parent.HitBy.Any(hitter => hitter.Color != playerColor);
            else
                return blackKing.Parent.HitBy.Any(hitter => hitter.Color != playerColor);
        }

        public void Move(Cell from, Cell to, PromoteOptions promoteOption)
        {
            if (to.Piece != null)
                pieces.Remove(to.Piece);

            to.Piece = from.Piece;
            from.Piece = null;

            if (to == EnPassant && to.Piece is Pawn)
            {
                pieces.Remove(EnPassantCapture.Piece);
                EnPassantCapture.Piece = null;
            }

            //Castling to the right
            if (to.Piece is King && to.X - from.X == 2)
            {
                Move(GetCell(7, to.Y), GetCell(to.X - 1, to.Y), promoteOption); //Move the rook as well
            }

            //Castling to the left
            if (to.Piece is King && to.X - from.X == -2)
            {
                Move(GetCell(0, to.Y), GetCell(to.X + 1, to.Y), promoteOption);
            }

            //Handles promotion
            if (to.Piece is Pawn && to.Y == (to.Piece.Color == PlayerColor.White ? 7 : 0))
            {
                Piece promoted = null; //we have to set it to null cuz C# complains
                switch (promoteOption)
                {
                    case PromoteOptions.Queen:
                        promoted = new Queen(to.Piece);
                        break;
                    case PromoteOptions.Rook:
                        promoted = new Rook(to.Piece);
                        break;
                    case PromoteOptions.Bishop:
                        promoted = new Bishop(to.Piece);
                        break;
                    case PromoteOptions.Knight:
                        promoted = new Knight(to.Piece);
                        break;
                }

                //Update the list with the new promoted piece
                pieces.Remove(to.Piece);
                to.Piece = promoted;
                promoted.OnPlace(to); //Place it otherwise weird bugs occur
                pieces.Add(promoted);
            }

            //The code has to be in this exact order to prevent from listeners firing when we move into our own listened cells.
            //Recalculate possible moves
            to.Piece.OnMove(to);
            to.Piece.Recalculate();

            //Resets en passant
            EnPassant = null;
            EnPassantCapture = null;

            //Handles en passant detection
            if (to.Piece is Pawn && Math.Abs(to.Y - from.Y) == 2)
            {
                EnPassant = GetCell(to.X, (from.Y > to.Y) ? from.Y - 1 : from.Y + 1);
                EnPassantCapture = to;
            }
        }

        public bool IsPromotable(Cell from, Cell to)
        {
            return from.Piece is Pawn && to.Y == (from.Piece.Color == PlayerColor.White ? 7 : 0);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessConsole
{
    public class Direction
    {
        public Piece Piece
        {
            private set;
            get;
        }

        public int X
        {
            private set;
            get;
        }

        public int Y
        {
            private set;
            get;
        }

        private List<ChessBoard.Cell> possibleMoves;

        public IEnumerable<ChessBoard.Cell> GetPossibleMoves(bool enemyHittable = true)
        {
            if (possibleMoves.Count == 0)
                yield break;

            for (int i = 0; i < possibleMoves.Count - 1; i++)
            {
                yield return possibleMoves[i];
            }

            if (possibleMoves.Last().Piece == null)
            {
                yield return possibleMoves.Last();
            }
            else if (enemyHittable && possibleMoves.Last().Piece.Color != Piece.Color)
            {
                yield return possibleMoves.Last();
            }
        }

        public int GetPossibleMovesCount(bool enemyHittable = true)
        {
            if (possibleMoves.Count == 0)
                return 0;

            if (possibleMoves.Last().Piece == null)
            {
                return possibleMoves.Count;
            }
            else if (!enemyHittable || possibleMoves.Last().Piece.Color == Piece.Color)
            {
                return possibleMoves.Count - 1;
            }
            else
                return possibleMoves.Count;
        }

        /// The number of moves that we could take, considering no blocking or out of board.
        public int DesiredCount
        {
            private set;
            get;
        }

        private bool updateHitGraph = false;

        public Direction(Piece piece, int x, int y, int desiredCount = 8, bool updateHitGraph = true)
        {
            Piece = piece;
            X = x;
            Y = y;
            DesiredCount = desiredCount;
            this.updateHitGraph = updateHitGraph;

            possibleMoves = new List<ChessBoard.Cell>();
            possibleMoves.AddRange(piece.Parent.OpenLineOfSight(x, y, desiredCount));

            foreach (ChessBoard.Cell move in possibleMoves)
            {
                if (updateHitGraph)
                    move.HitBy.Add(Piece);
            }
        }

        public bool IsBlockedIfMove(ChessBoard.Cell from, ChessBoard.Cell to, ChessBoard.Cell blocked)
        {
            if (possibleMoves.Contains(blocked) && !possibleMoves.Contains(to))
            {
                return false;
            }
            else if (possibleMoves.Contains(from))
            {
                int toIndex = possibleMoves.IndexOf(to);
                if (0 <= toIndex && toIndex < possibleMoves.Count - 1)
                    return true; //The blocker closer to the piece
                else
                {
                    //If we moved further
                    foreach (ChessBoard.Cell move in from.OpenLineOfSight(X, Y, DesiredCount - possibleMoves.Count))
                    {
                        if (move == to) //The blocker moved into the new path
                            return true;
                        if (move == blocked) //The blocked is hittable
                            return false;
                    }
                }
            }
            return true;
        }
    }
}

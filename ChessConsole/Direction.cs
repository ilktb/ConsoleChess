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
        public List<ChessBoard.Cell> PossibleMoves { get => possibleMoves; set => possibleMoves = value; }

        public IEnumerable<ChessBoard.Cell> GetPossibleMoves(bool enemyHittable = true)
        {
            if (PossibleMoves.Count == 0)
                yield break;

            for (int i = 0; i < PossibleMoves.Count - 1; i++)
            {
                yield return PossibleMoves[i];
            }

            if (PossibleMoves.Last().Piece == null)
            {
                yield return PossibleMoves.Last();
            }
            else if (enemyHittable && PossibleMoves.Last().Piece.Color != Piece.Color)
            {
                yield return PossibleMoves.Last();
            }
        }

        public int GetPossibleMovesCount(bool enemyHittable = true)
        {
            if (PossibleMoves.Count == 0)
                return 0;

            if (PossibleMoves.Last().Piece == null)
            {
                return PossibleMoves.Count;
            }
            else if (!enemyHittable || PossibleMoves.Last().Piece.Color == Piece.Color)
            {
                return PossibleMoves.Count - 1;
            }
            else
                return PossibleMoves.Count;
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

            PossibleMoves = new List<ChessBoard.Cell>();
            PossibleMoves.AddRange(piece.Parent.OpenLineOfSight(x, y, desiredCount));

            foreach (ChessBoard.Cell move in PossibleMoves)
            {
                if (updateHitGraph)
                    move.HitBy.Add(Piece);
            }
        }

        public bool IsBlockedIfMove(ChessBoard.Cell from, ChessBoard.Cell to, ChessBoard.Cell blocked)
        {
            if (PossibleMoves.Contains(blocked) && !PossibleMoves.Contains(to))
            {
                return false;
            }
            else if (PossibleMoves.Contains(from))
            {
                int toIndex = PossibleMoves.IndexOf(to);
                if (0 <= toIndex && toIndex < PossibleMoves.Count - 1)
                    return true;
                else
                {
                    //If we moved further
                    foreach (ChessBoard.Cell move in from.OpenLineOfSight(X, Y, DesiredCount - PossibleMoves.Count))
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

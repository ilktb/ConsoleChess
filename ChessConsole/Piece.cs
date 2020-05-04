using System.Collections.Generic;

namespace ChessConsole
{
    public abstract class Piece
    {
        public PlayerColor Color
        {
            private set;
            get;
        }

        public bool IsMoved
        {
            protected set;
            get;
        }

        public abstract IEnumerable<ChessBoard.Cell> PossibleMoves
        {
            get;
        }

        public List<ChessBoard.Cell> LegalMoves
        {
            private set;
            get;
        }

        public ChessBoard.Cell Parent
        {
            private set;
            get;
        }

        public Piece(PlayerColor color)
        {
            Color = color;
            IsMoved = false;
            LegalMoves = new List<ChessBoard.Cell>();
        }

        public void OnPlace(ChessBoard.Cell cell)
        {
            Parent = cell;
        }

        public void OnMove(ChessBoard.Cell cell)
        {
            Parent = cell;
            IsMoved = true;
        }

        public abstract void Recalculate();

        public abstract bool IsBlockedIfMove(ChessBoard.Cell from, ChessBoard.Cell to, ChessBoard.Cell blocked);

        public abstract char Char { get; }

        protected virtual bool CanHit(ChessBoard.Cell cell)
        {
            return cell != null && cell.Piece != null && cell.Piece.Color != Color;
        }
    }
}

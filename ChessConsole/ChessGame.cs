using System;
using System.Linq;

namespace ChessConsole
{
    public enum PlayerColor
    {
        White, Black
    }

    public enum PlayerState
    {
        Idle, Holding, AwaitPromote, GameOver
    }

    public enum PromoteOptions
    {
        Queen = 0, Rook = 1, Bishop = 2, Knight = 3
    }

    public class ChessGame
    {
        public bool Running
        {
            private set;
            get;
        }

        private PlayerState playerState;
        
        private PromoteOptions promoteOption;

        private PlayerColor currentPlayer;
        
        private int cursorX, cursorY;

        private ChessBoard board;

        private ChessBoard.Cell holdedParentNode = null;

        private ChessBoard.Cell moveTo = null;

        public ChessGame()
        {
            Running = true;
            board = new ChessBoard();
            currentPlayer = PlayerColor.White;
            TurnStart();
        }

        #region PublicInterfaceCommands
        public void Update()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (CanCursorBeMovedLeft(keyInfo))
                    cursorX--;
                else if (CanCursorBeMovedRight(keyInfo))
                    cursorX++;
                else if (CanCursorBeMovedUp(keyInfo))
                {
                    if (IsDownBorder())
                        cursorY++;
                    else if ((int)promoteOption > 0)
                        promoteOption--;
                }
                else if (CanCursorBeMovedDown(keyInfo))
                {
                    if (IsUpBorder())
                        cursorY--;
                    else if ((int)promoteOption < 3)
                        promoteOption++;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                    Interact();
                else if (keyInfo.Key == ConsoleKey.D)
                    DebugInteract();
                else if (keyInfo.Key == ConsoleKey.Escape)
                    Cancel();
            }
        }
        private bool CanCursorBeMovedLeft(ConsoleKeyInfo keyInfo)
        {
            return keyInfo.Key == ConsoleKey.LeftArrow && cursorX > 0 && playerState != PlayerState.AwaitPromote;
        }
        private bool CanCursorBeMovedRight(ConsoleKeyInfo keyInfo)
        {
            return keyInfo.Key == ConsoleKey.RightArrow && cursorX < 7 && playerState != PlayerState.AwaitPromote;
        }
        private bool CanCursorBeMovedUp(ConsoleKeyInfo keyInfo)
        {
            return keyInfo.Key == ConsoleKey.UpArrow;
        }
        private bool CanCursorBeMovedDown(ConsoleKeyInfo keyInfo)
        {
            return keyInfo.Key == ConsoleKey.DownArrow;
        }
        private bool IsDownBorder()
        {
            return playerState != PlayerState.AwaitPromote && cursorY < 7;
        }
        private bool IsUpBorder()
        {
            return playerState != PlayerState.AwaitPromote && cursorY > 0;
        }

        /// <summary>
        /// Draws the game
        /// </summary>
        /// <param name="g">ConsoleGraphics object to draw with/to</param>
        public void Draw(ConsoleGraphics g)
        {
            const int xCoordinateArea = 10;
            const int yCoordinateArea = 5;
            const int boardSize = 8;

            g.FillAreaColoredCharacter(new CChar(' ', ConsoleColor.Black, ConsoleColor.DarkGray), xCoordinateArea, yCoordinateArea, boardSize, boardSize);

            //boardSize-1 - j (7-j) everywhere cuz it's reversed in chess
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    //Draw the symbol
                    ChessBoard.Cell cell = board.GetCell(i, j);
                    if (cell.Piece != null)
                    {
                        g.DrawTransparentBackground(cell.Piece.Char, (cell.Piece.Color == PlayerColor.White) ? ConsoleColor.White : ConsoleColor.Black, xCoordinateArea + i, yCoordinateArea + (boardSize - 1 - j));
                        if (cell.Piece.LegalMoves.Count == 0)
                        {
                            g.SetBackgroundColor(ConsoleColor.DarkRed, xCoordinateArea + i, yCoordinateArea + (boardSize - 1 - j));
                        }
                    }

                    if (cell.HitBy.Contains(debugPiece))
                        g.SetBackgroundColor(ConsoleColor.DarkMagenta, xCoordinateArea + i, yCoordinateArea + (boardSize - 1 - j));
                }
            }

            if (holdedParentNode != null && playerState == PlayerState.Holding)
            {
                //Highlight legal moves
                foreach (ChessBoard.Cell move in holdedParentNode.Piece.LegalMoves)
                {
                    g.SetBackgroundColor(ConsoleColor.DarkGreen, xCoordinateArea + move.X, yCoordinateArea + (boardSize - 1 - move.Y));
                }
            }

            //Sets the cursor color -> yellow
            g.SetBackgroundColor(ConsoleColor.DarkYellow, xCoordinateArea + cursorX, yCoordinateArea + (boardSize - 1 - cursorY));


            //Lighten for checkerboard pattern
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if ((i + j) % 2 == 1) g.LightenBackgroundColor(xCoordinateArea + i, yCoordinateArea + j);
                }
            }

            //Promotion option menu

            if (playerState == PlayerState.AwaitPromote)
            {
                g.DrawTextTrasparentBackground("Queen", promoteOption == PromoteOptions.Queen ? ConsoleColor.Yellow : ConsoleColor.White, 22, 7);
                g.DrawTextTrasparentBackground("Rook", promoteOption == PromoteOptions.Rook ? ConsoleColor.Yellow : ConsoleColor.White, 22, 9);
                g.DrawTextTrasparentBackground("Bishop", promoteOption == PromoteOptions.Bishop ? ConsoleColor.Yellow : ConsoleColor.White, 22, 11);
                g.DrawTextTrasparentBackground("Knight", promoteOption == PromoteOptions.Knight ? ConsoleColor.Yellow : ConsoleColor.White, 22, 13);
            }
            else
            {
                g.ClearArea(22, 7, 6, 7);
            }
        }

        #endregion

        #region EventHandlerLikeMethods

        private void Interact()
        {
            switch (playerState)
            {
                case PlayerState.Idle:
                    holdedParentNode = board.GetCell(cursorX, cursorY);

                    if (holdedParentNode.Piece == null || holdedParentNode.Piece.Color != currentPlayer || holdedParentNode.Piece.LegalMoves.Count == 0)
                    {
                        holdedParentNode = null;
                        return;
                    }
                    else playerState = PlayerState.Holding;


                    break;
                case PlayerState.Holding:
                    playerState = PlayerState.Holding;

                    moveTo = board.GetCell(cursorX, cursorY);

                    if (!holdedParentNode.Piece.LegalMoves.Contains(moveTo))
                    {
                        moveTo = null;
                        return;
                    }

                    if (board.IsPromotable(holdedParentNode, moveTo))
                        ShowPromote();
                    else
                        TurnOver();
                    
                    break;
                case PlayerState.AwaitPromote:
                    TurnOver();
                    break;
                case PlayerState.GameOver:
                    Running = false;
                    break;
            }
        }


        private Piece debugPiece;
        private void DebugInteract()
        {
            debugPiece = board.GetCell(cursorX, cursorY).Piece;
        }

        private void Cancel()
        {
            playerState = PlayerState.Idle;
            holdedParentNode = null;
        }

        #endregion

        #region EventLikeMethods

        private void TurnStart()
        {
            board.TurnStart(currentPlayer);
        }

        private void ShowPromote()
        {
            playerState = PlayerState.AwaitPromote;
            promoteOption = PromoteOptions.Queen; //reset the menu
        }

        private void TurnOver()
        {
            board.Move(holdedParentNode, moveTo, promoteOption);
            holdedParentNode = null;
            moveTo = null;
            playerState = PlayerState.Idle;
            currentPlayer = currentPlayer == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
            TurnStart();
        }
        #endregion
    }
}

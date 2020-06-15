using System;
using System.Collections.Generic;
using ChessConsole;
using ChessConsole.Pieces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ChessConsole.ChessBoard;

namespace UnitTestChessConsole
{
    [TestClass]
    public class ChessBoardTests
    {
        [TestMethod]
        public void TestTurnStart_CurrentPlayer()
        {
            ChessBoard chessBoard = new ChessBoard();
            PlayerColor currentPlayer = new PlayerColor();
            bool expected = true;

            bool result = chessBoard.TurnStart(currentPlayer);

            Assert.AreEqual(expected, result, "Expected to be curPlayer but it is not");
        }

        [TestMethod]
        public void TestIsInCheck_IfUseCacheIsFalse()
        {
            ChessBoard chessBoard = new ChessBoard();
            PlayerColor playerColor = PlayerColor.White;
            bool useCache = false;
            bool expected = true;

            bool result = chessBoard.IsInCheck(playerColor, useCache);

            Assert.AreEqual(expected, result, "Had to return true but it return false");
        }

        [TestMethod]
        public void TestMove_LegalMove()
        {

        }

        [TestMethod]
        public void TestIsPromotable()
        {
            ChessBoard parentChessBoard = new ChessBoard();
            ChessBoard chessBoard = new ChessBoard();
            Cell from = new Cell(parentChessBoard,4,5);
            Cell to = new Cell(parentChessBoard,3,4);
            from.Piece = new Pawn(PlayerColor.White);
            to.Piece = new Pawn(PlayerColor.White);

            bool result = chessBoard.IsPromotable(from, to);

            Assert.IsTrue(result, "doesnt work well");
        }
    }

    
}

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
        public void TestTurnStart_whenCurrentPlayerIsWhite_AreThereAnyLegalMoves()
        {
            ChessBoard chessBoard = new ChessBoard();
            PlayerColor currentPlayer = new PlayerColor();
            currentPlayer = PlayerColor.White;
            bool expected = true;

            bool result = chessBoard.TurnStart(currentPlayer);

            Assert.AreEqual(expected, result, "CurrentPlayer has to have any legal moves");
        }

        [TestMethod]
        public void TestTurnStart_whenCurrentPlayerIsBlack_AreThereAnyLegalMoves()
        {
            ChessBoard chessBoard = new ChessBoard();
            PlayerColor currentPlayer = new PlayerColor();
            currentPlayer = PlayerColor.Black;
            bool expected = true;

            bool result = chessBoard.TurnStart(currentPlayer);

            Assert.AreEqual(expected, result, "CurrentPlayer has to have any legal moves");
        }

        [TestMethod]
        public void TestIsInCheck_whenWhiteKingIsHittingByPawn_ItMustSignal()
        {
            ChessBoard chessBoard = new ChessBoard();
            PlayerColor playerColor = new PlayerColor();
            playerColor = PlayerColor.White;
            Piece hitterPiece = new Pawn(PlayerColor.Black);
            chessBoard.WhiteKing.Parent.HitBy.Add(hitterPiece);
            bool useCache = false;
            bool expected = true;

            bool result = chessBoard.IsInCheck(playerColor, useCache);

            Assert.AreEqual(expected, result, "The white king was hit by black pawn");
        }

        [TestMethod]
        public void TestIsInCheck_whenBlackKingIsHittingByPawn_ItMustSignal()
        {
            ChessBoard chessBoard = new ChessBoard();
            PlayerColor playerColor = new PlayerColor();
            playerColor = PlayerColor.Black;
            Piece hitterPiece = new Pawn(PlayerColor.White);
            chessBoard.BlackKing.Parent.HitBy.Add(hitterPiece);
            bool useCache = false;
            bool expected = true;

            bool result = chessBoard.IsInCheck(playerColor, useCache);

            Assert.AreEqual(expected, result, "The black king was hit by white pawn");
        }

        [TestMethod]
        public void TestIsInCheck_whenWhiteKingIsNotHitByAnithing()
        {
            ChessBoard chessBoard = new ChessBoard();
            PlayerColor playerColor = new PlayerColor();
            playerColor = PlayerColor.White;
            bool useCache = false;
            bool expected = false;

            bool result = chessBoard.IsInCheck(playerColor, useCache);

            Assert.AreEqual(expected, result, "The white king wasn't hit by anithing");
        }


        [TestMethod]
        public void TestIsPromotable_whenThePawnIsOnOppositeBorder()
        {
            ChessBoard parentChessBoard = new ChessBoard();
            ChessBoard chessBoard = new ChessBoard();
            Cell from = new Cell(parentChessBoard,4,5);
            Cell to = new Cell(parentChessBoard,3,4);
            from.Piece = new Pawn(PlayerColor.White);
            to.Piece = new Pawn(PlayerColor.White);
            to.Y = 7; // in the border
            bool expected = true;

            bool result = chessBoard.IsPromotable(from, to);

            Assert.AreEqual(expected, result, "doesnt work well");
        }
    }

    
}

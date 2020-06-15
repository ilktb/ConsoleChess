using System;
using System.Collections.Generic;
using ChessConsole;
using ChessConsole.Pieces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestChessConsole
{
    [TestClass]
    public class UnitTestDirection
    {
        [TestMethod]
        public void TestGetPossibleMoves()
        {
            List<ChessBoard.Cell> possibleMoves = new List<ChessBoard.Cell>();
            bool enemyHittable = true;
            ChessBoard.Cell firstCell= new ChessBoard.Cell(new ChessBoard(), 1, 3);
            ChessBoard.Cell secondCell = new ChessBoard.Cell(new ChessBoard(), 5, 4);
            possibleMoves.Add(firstCell);
            possibleMoves.Add(secondCell);

            
        }

        [TestMethod]
        public void TestGetPossibleMovesCount_WithTwoMoves()
        {
            Direction direction = new Direction(new Pawn(PlayerColor.Black), 2, 3, 8, true);

            List<ChessBoard.Cell> possibleMoves = new List<ChessBoard.Cell>();
            bool enemyHittable = true;
            ChessBoard.Cell firstCell = new ChessBoard.Cell(new ChessBoard(), 1, 3);
            ChessBoard.Cell secondCell = new ChessBoard.Cell(new ChessBoard(), 5, 4);
            possibleMoves.Add(firstCell);
            possibleMoves.Add(secondCell);
            int expected = 2;

            int result = direction.GetPossibleMovesCount(enemyHittable);

            Assert.AreEqual(expected, result, "In the list have two cells");
        }

        [TestMethod]
        public void TestIsBlockedIfMove()
        {

        }
    }
}

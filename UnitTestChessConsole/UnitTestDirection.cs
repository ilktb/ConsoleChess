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
            Direction direction = new Direction(new Pawn(PlayerColor.Black), 2, 3, 8, true);

            ChessBoard.Cell firstCell= new ChessBoard.Cell(new ChessBoard(), 1, 3);
            ChessBoard.Cell secondCell = new ChessBoard.Cell(new ChessBoard(), 5, 4);
            direction.PossibleMoves.Add(firstCell);
            direction.PossibleMoves.Add(secondCell);

            List<ChessBoard.Cell> expected = new List<ChessBoard.Cell>();
            expected.Add(firstCell);
            expected.Add(secondCell);

            Assert.AreEqual(expected, direction.PossibleMoves, "There is more ot less object in possibleMoves");
        }

        [TestMethod]
        public void TestGetPossibleMovesCount_WithTwoMoves()
        {
            Direction direction = new Direction(new Pawn(PlayerColor.Black), 2, 3, 7, true);

            bool enemyHittable = true;
            ChessBoard.Cell firstCell = new ChessBoard.Cell(new ChessBoard(), 1, 3);
            ChessBoard.Cell secondCell = new ChessBoard.Cell(new ChessBoard(), 5, 4);
            direction.PossibleMoves.Add(firstCell);
            direction.PossibleMoves.Add(secondCell);
            int expected = 2;

            int result = direction.GetPossibleMovesCount(enemyHittable);

            Assert.AreEqual(expected, result, "In the list have more or less than two cells");
        }

        [TestMethod]
        public void TestIsBlockedIfMove_possibleMovesContainsBlocked()
        {
            Direction direction = new Direction(new Pawn(PlayerColor.Black), 2, 3, 8, true);

            ChessBoard.Cell from = new ChessBoard.Cell(new ChessBoard(), 1, 3);
            ChessBoard.Cell to = new ChessBoard.Cell(new ChessBoard(), 5, 4);
            ChessBoard.Cell blocked = new ChessBoard.Cell(new ChessBoard(), 2, 6);
            ChessBoard.Cell randomCell = new ChessBoard.Cell(new ChessBoard(), 4, 2);
            direction.PossibleMoves.Add(blocked);
            direction.PossibleMoves.Add(randomCell);
            bool expected = false;

            bool result = direction.IsBlockedIfMove(from, to, blocked);

            Assert.AreEqual(expected, result, "Does not block move even possibleMoves contains 'blocked' cell");
        }

        [TestMethod]
        public void TestIsBlockedIfMove_possibleMovesContainsTo()
        {
            Direction direction = new Direction(new Pawn(PlayerColor.Black), 2, 3, 8, true);

            ChessBoard.Cell from = new ChessBoard.Cell(new ChessBoard(), 1, 3);
            ChessBoard.Cell to = new ChessBoard.Cell(new ChessBoard(), 5, 4);
            ChessBoard.Cell blocked = new ChessBoard.Cell(new ChessBoard(), 2, 6);
            ChessBoard.Cell randomCell = new ChessBoard.Cell(new ChessBoard(), 4, 2);
            direction.PossibleMoves.Add(to);
            direction.PossibleMoves.Add(randomCell);
            bool expected = false;

            bool result = direction.IsBlockedIfMove(from, to, blocked);

            Assert.AreEqual(expected, result, "Does not block move even possibleMoves contains 'To' cell");
        }

        [TestMethod]
        public void TestIsBlockedIfMove_possibleMovesContainsFrom()
        {
            Direction direction = new Direction(new Pawn(PlayerColor.Black), 2, 3, 8, true);

            ChessBoard.Cell from = new ChessBoard.Cell(new ChessBoard(), 1, 3);
            ChessBoard.Cell to = new ChessBoard.Cell(new ChessBoard(), 5, 4);
            ChessBoard.Cell blocked = new ChessBoard.Cell(new ChessBoard(), 2, 6);
            ChessBoard.Cell randomCell = new ChessBoard.Cell(new ChessBoard(), 4, 2);
            direction.PossibleMoves.Add(from);
            direction.PossibleMoves.Add(to);
            direction.PossibleMoves.Add(randomCell);
            bool expected = true;

            bool result = direction.IsBlockedIfMove(from, to, blocked);

            Assert.AreEqual(expected, result, "Blocked move even possibleMoves contains 'to' cell in the middle");
        }

        [TestMethod]
        public void TestIsBlockedIfMove_possibleMovesContainsNothing()
        {
            Direction direction = new Direction(new Pawn(PlayerColor.Black), 2, 3, 8, true);

            ChessBoard.Cell from = new ChessBoard.Cell(new ChessBoard(), 1, 3);
            ChessBoard.Cell to = new ChessBoard.Cell(new ChessBoard(), 5, 4);
            ChessBoard.Cell blocked = new ChessBoard.Cell(new ChessBoard(), 2, 6);
            
            bool expected = true;

            bool result = direction.IsBlockedIfMove(from, to, blocked);

            Assert.AreEqual(expected, result, "Block move even possibleMoves was empty");
        }
    }
}

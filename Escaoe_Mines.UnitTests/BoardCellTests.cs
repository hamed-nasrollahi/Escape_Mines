using Escape_Mines.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace Escaoe_Mines.UnitTests
{
    [TestFixture]
    public class BoardCellTests
    {
        BoardCell _boardCell;

        [SetUp]
        public void Setup()
        {
            _boardCell = new BoardCell();
        }

        [Test]
        [TestCase(CellType.Occupied)]
        [TestCase(CellType.Exit)]
        public void Init_WhenCalled_ChangeCellType(CellType celltype)
        {
            _boardCell.Init(celltype);
            Assert.That(_boardCell.Type, Is.EqualTo(celltype));
        }

        [Test]
        [TestCase(Direction.N)]
        [TestCase(Direction.W)]
        public void Occupie_WhenCalled_ChangeCellDir(Direction celldir)
        {
            _boardCell.Occupie(celldir);
            Assert.That(_boardCell.Dir, Is.EqualTo(celldir));
            Assert.That(_boardCell.Type, Is.EqualTo(CellType.Occupied));
        }

        [Test]
        public void Free_WhenCalled_ChangeCellType()
        {
            _boardCell.Occupie(Direction.W);
            _boardCell.Free();
            Assert.That(_boardCell.Dir, Is.EqualTo(Direction.W));
            Assert.That(_boardCell.Type, Is.EqualTo(CellType.Empty));
        }

        [Test]
        [TestCaseSource(nameof(position))]
        public void SetPosition_WhenCalled_ChangeCellPosition((int X, int Y) newpos)
        {
            _boardCell.SetPosition(newpos);
            Assert.That(_boardCell.Position, Is.EqualTo(newpos));
        }

        public static IEnumerable<(int, int)[]> position
        {
            get
            {
                yield return new[] { (1, 2) };
            }
        }
    }
}
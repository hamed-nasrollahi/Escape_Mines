using Moq;
using Escape_Mines.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Escaoe_Mines.UnitTests
{
    [TestFixture]
    public class BoardTests
    {
        Board _board;
        FileReader _fileReader;

        [SetUp]
        public void Setup()
        {
            _fileReader = new FileReader("settings.txt");
            _board = new Board(_fileReader, false);
        }

        [Test]
        [TestCase()]
        public void LoadSettings_WhenCalled_returnTrueandchangeBoard()
        {
            _fileReader.Set_FileStream_for_Tests(SettingStream());

            var tsk = _board.LoadSettings();
            tsk.Wait();

            Assert.That(tsk.Result, Is.True);
            Assert.That(_board.BoardSize, Is.EqualTo((5,4)));
        }

        [Test]
        [TestCase()]
        public void ReadMovesLine_WhenCalled_returnTrueandchangeMovesSeries()
        {
            _fileReader.Set_FileStream_for_Tests(MovesStream());

            var tsk = _board.ReadMovesLine();
            tsk.Wait();

            Assert.That(tsk.Result, Is.True);
            Assert.That(_board.MovesSeries.Length, Is.EqualTo(4));
        }

        [Test]
        [TestCase(Moves.R)]
        [TestCase(Moves.L)]
        [TestCase(Moves.M)]
        public void MoveNext_WhenCalled_ChangeCurrentCell(Moves mv)
        {
            _fileReader.Set_FileStream_for_Tests(SettingStream());
            var tsk = _board.LoadSettings();
            tsk.Wait();

            var result = _board.MoveNext(mv);
            Assert.That(result.error,Is.EqualTo(MoveErrors.None));
            Assert.That(result.error_index, Is.EqualTo(1));
        }

        private StreamReader SettingStream()
        {
            string set_string = "5 4\r\n3,3\r\n4 2\r\n1 1 N\r\n" ;
            return new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(set_string)));

        }

        private StreamReader MovesStream()
        {
            string mv_string = "R M M M\r\n";
            return new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(mv_string)));

        }
    }
}

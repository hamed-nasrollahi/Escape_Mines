using System;
using System.Collections.Generic;
using System.Text;

namespace Escape_Mines.Models
{
    public class BoardCell:IBoardCell
    {
        private CellType _type = new CellType();
        private Direction _dir = new Direction();
        private (int X, int Y) _Position;

        public CellType Type 
        {
            get
            {
                return _type;
            }
        }

        public Direction Dir 
        {
            get
            {
                return _dir;
            }
        }

        public (int X, int Y) Position
        {
            get
            {
                return (_Position.X, _Position.Y);
            }
        }
        //class constructor
        public BoardCell()
        {

        }

        public void Free()
        {
            _type = CellType.Empty;
        }

        public void Occupie(Direction direct)
        {
            _type = CellType.Occupied;
            _dir = direct;
        }

        public void Init(CellType typ)
        {
            _type = typ;
        }

        /// <summary>
        /// function to set cell position
        /// </summary>
        public void SetPosition((int X, int Y) pos)
        {
            _Position = pos;
        }
    }

}

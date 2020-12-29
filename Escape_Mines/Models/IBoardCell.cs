using System;
using System.Collections.Generic;
using System.Text;

namespace Escape_Mines.Models
{
    public interface IBoardCell
    {
        /// <summary>
        /// cell status properties
        /// </summary>
        CellType Type { get;  }

        /// <summary>
        /// cell Direction properties
        /// </summary>
        Direction Dir { get;  }

        /// <summary>
        /// cell position properties
        /// </summary>
        (int X, int Y) Position { get; }

        /// <summary>
        /// function to free the cell after turtle move out
        /// </summary>
        void Free();

        /// <summary>
        /// function to freez cell when turte move in
        /// </summary>
        void Occupie(Direction direct);

        /// <summary>
        /// function used to initial cell status
        /// </summary>
        void Init(CellType typ);

        /// <summary>
        /// function to set cell position
        /// </summary>
        void SetPosition((int X, int Y) pos);

    }
    public enum CellType
    {
        /// <summary>
        /// the cell is empty and can be occupied
        /// </summary>
        Empty,

        /// <summary>
        /// the cell contain mine
        /// </summary>
        Mine,

        /// <summary>
        /// the cell is final point and finish the game
        /// </summary>
        Exit,

        /// <summary>
        /// the cell is occupied by turtle
        /// </summary>
        Occupied,

        /// <summary>
        /// the cell is out of board
        /// </summary>
        Out
    }
    public enum Direction
    {
        /// <summary>
        /// the turtle directed to south
        /// </summary>
        S,

        /// <summary>
        /// the turtle directed to north
        /// </summary>
        N,

        /// <summary>
        /// the turtle directed to east
        /// </summary>
        E,

        /// <summary>
        /// the turtle directed to west
        /// </summary>
        W
    }
}

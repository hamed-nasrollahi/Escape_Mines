using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Escape_Mines.Models
{
    public interface IBoard
    {
        /// <summary>
        /// retrive the board size as properties
        /// </summary>
        (ushort X, ushort Y) BoardSize { get; }

        /// <summary>
        /// get current cell properties 
        /// </summary>
        IBoardCell CurrentCell { get; }

        /// <summary>
        /// get Moves taken from the first of line 
        /// </summary>
        ushort MovesCount { get; }

        /// <summary>
        /// indicate the moves ended by condition 
        /// </summary>
        (bool end, MoveErrors cause) MovesEnd { get; }

        /// <summary>
        /// get next cell properties 
        /// </summary>
        IBoardCell NextCell { get; }

        /// <summary>
        /// array of moves to act 
        /// </summary>
        Moves[] MovesSeries { get; }

        /// <summary>
        /// init board cells as 2d array 
        /// also load & set board settings
        /// </summary>
        Task<bool> LoadSettings();

        /// <summary>
        /// read moves from one line
        /// </summary>
        Task<bool> ReadMovesLine();

        /// <summary>
        /// function to do next move
        /// if the next move is not possible return false
        /// </summary>
        (MoveErrors error, ushort error_index) MoveNext(Moves mv);

        /// <summary>
        /// function to retrun board as string for print
        /// </summary>
        string PrintBoard();

    }
    public enum MoveErrors
    {
        /// <summary>
        /// move successful
        /// </summary>
        None,

        /// <summary>
        /// move to mine block
        /// </summary>
        Mine,

        /// <summary>
        /// reach the board edge
        /// </summary>
        Edge,

        /// <summary>
        /// next block is final block
        /// </summary>
        Exit
    }
    public enum Moves
    {
        /// <summary>
        /// Rotate 90 degrees to the right
        /// </summary>
        R,

        /// <summary>
        /// Rotate 90 degrees to the left
        /// </summary>
        L,

        /// <summary>
        /// Move Forward
        /// </summary>
        M
    }
}

using System;
using System.Threading.Tasks;
using System.Linq;
using Escape_Mines.Models;

namespace Escape_Mines.Models
{
    public class Board : IBoard,IDisposable
    {
        //private local variable for public properties
        //curent cell variable
        private IBoardCell _currentcell;
        //next cell acording to current cell direction
        private IBoardCell _nextcell;
        //series of move in a line
        private Moves[] _movesseries;
        //main board size variable
        private (ushort X, ushort Y)  _boardsize;
        //Debug message out Flag
        private bool _debug;
        //main board memory
        private BoardCell[,] _boardCells;
        //count moves from the lines first move
        private ushort _movescount;
        //flag for ending the jub and cause the end
        private (bool end, MoveErrors cause) _movesend;

        //setting file path
        //private string _filepath;

        //show the object is disposed or not
        private bool disposed = false;

        //setting file read stream
        private IFileReader _file;
    
        public IBoardCell CurrentCell
        {
            get
            {
                return _currentcell;
            }
        }

        public IBoardCell NextCell
        {
            get
            {
                return _nextcell;
            }
        }

        public Moves[] MovesSeries
        {
            get
            {
                return _movesseries;
            }
        }

        public (ushort X, ushort Y) BoardSize 
        {
            get
            {
                return _boardsize;
            }
        }
        public ushort MovesCount 
        { 
            get
            {
                return _movescount;
            }
        }

        public (bool end, MoveErrors cause) MovesEnd 
        {
            get
            {
                return _movesend;
            }
        }

        public Board(IFileReader SettingFile,bool Enable_debug=true)
        {
            //initial the object and set setting file path
            _file = SettingFile;
            _currentcell = new BoardCell();
            _nextcell = new BoardCell();
            _debug = Enable_debug;
            _movesend = (false,MoveErrors.None);
        }

        public async Task<bool> LoadSettings()
        {
            try
            {
                //first 4 line is board setting
                for (int i = 0; i < 4; i++)
                {
                    //read a line async from the file stream
                    var ReadedLine = await _file.Read();
                    //check the line contain atleast 2 char (no linne smaller than 2)
                    if (ReadedLine.Length >= 2)
                    {
                        //seprate the line contents by 'space' to be ready to use
                        string[] line_inputs = ReadedLine.Split(' ');

                        //in which line set some setting
                        switch (i)
                        {
                            //first line of setting file
                            case 0:
                                //contain the width & height of board
                                if(!ResizeBoard(line_inputs[0], line_inputs[1]))
                                    return false;
                                break;
                            //second line of setting file
                            case 1:
                                //contain a list of mines
                                if (!SetMines(line_inputs))
                                    return false;
                                break;
                            //line 3 of setting file
                            case 2:
                                //contain a list of mines
                                if (!SetExit(line_inputs[0], line_inputs[1]))
                                    return false;
                                break;
                            //line 4 of setting file
                            case 3:
                                //contain the starting position of the turtle.
                                if (!SetStart(line_inputs[0], line_inputs[1], line_inputs[2]))
                                    return false;
                                break;
                        }
                    }
                    else
                    {
                        PrintLog("Read Setting Fail on Line:" + (i+1).ToString());
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                PrintLog("LoadSettings = " + ex.Message);
                return false;
            }
            return true;
        }

        //this function only use once to initial board
        private bool ResizeBoard(string width,string height)
        {
            //validate input to be Digits
            if(ValidateInput(width) && ValidateInput(height))
            {
                try
                {
                    //set the board size variable
                    _boardsize.X = ushort.Parse(width);
                    _boardsize.Y = ushort.Parse(height);
                    //define the main board cells
                    _boardCells = new BoardCell[_boardsize.X, _boardsize.Y];
                    //alocate the main board cells
                    //because the definition does not instantiate them
                    for (int i=0; i< _boardsize.X; i++)
                    {
                        for (int j = 0; j < _boardsize.Y; j++)
                        {
                            _boardCells[i, j] = new BoardCell();
                            _boardCells[i, j].SetPosition((i,j));
                            _boardCells[i, j].Free();
                        }
                    }

                }
                catch (Exception ex)
                {
                    PrintLog("ResizeBoard = " + ex.Message);
                    return false;
                }
            }
            else
            {
                PrintLog("ResizeBoard = The Board size characters must be Digit");
                return false;
            }
            return true;
        }

        //this function only use once to initial mines location
        private bool SetMines(string[] MinesString)
        {
            try
            {
                //fetch each mine from the string
                foreach(string mineStr in MinesString)
                {
                    //validate each input to be Digits
                    if (ValidateInput(mineStr.Split(',')[0]) && ValidateInput(mineStr.Split(',')[1]))
                    {
                        //in sample file it is (y,x) so parameter[0] is height and Y
                        int mine_y = int.Parse(mineStr.Split(',')[0]);
                        int mine_x = int.Parse(mineStr.Split(',')[1]);
                        _boardCells[mine_x, mine_y].Init(CellType.Mine);
                    }
                    else
                    {
                        //if validation go wrong
                        PrintLog("SetMines = The Mines Location characters must be Digit");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                PrintLog("SetMines = " + ex.Message);
                return false;
            }
            return true;
        }

        //this function only use once to initial exit point
        private bool SetExit(string x, string y)
        {
            try
            {
                //check the input to be valid and digit
                if (ValidateInput(x) && ValidateInput(y))
                {
                    _boardCells[int.Parse(x), int.Parse(y)].Init(CellType.Exit);
                }
                else
                {
                    PrintLog("SetExit = The Exit Location characters must be Digit");
                    return false;
                }
            }
            catch (Exception ex)
            {
                PrintLog("SetExit = " + ex.Message);
                return false;
            }
            return true;
        }

        //this function only use once to initial start point
        private bool SetStart(string x, string y, string direct)
        {
            try
            {
                //check the input to be valid
                if (ValidateInput(x) && ValidateInput(y) && ValidateInput(direct, false))
                {
                    //convert direction character to enum type
                    Direction dir = (Direction)Enum.Parse(typeof(Direction), direct);
                    _boardCells[int.Parse(x), int.Parse(y)].Occupie(dir);

                    //store current turtle position to _currentcell
                    _currentcell.SetPosition(_boardCells[int.Parse(x), int.Parse(y)].Position);
                    _currentcell.Occupie(_boardCells[int.Parse(x), int.Parse(y)].Dir);
                    //calculate the next cell
                    CalcNext();
                }
                else
                {
                    PrintLog("SetStart = The Start Location characters must be Digit & Direction must be Valid");
                    return false;
                }
                    
            }
            catch (Exception ex)
            {
                PrintLog("SetStart = " + ex.Message);
                return false;
            }
            return true;
        }

        //this function validate the input string to ensure unexpected chars
        private bool ValidateInput(string input,bool Digit=true)
        {
            try
            {
                //check the string to have only Digits
                if (Digit)
                {
                    //check the string to not be null & be digit
                    if (!input.All(char.IsDigit) || (String.IsNullOrEmpty(input)))
                        return false;
                }
                //only Chars "M W E S N R L" are allowed
                else
                {
                    string allowableChars = "MWESNRL";
                    foreach (char chr in input)
                    {
                        //check the string to not be null and be allowable
                        if (!allowableChars.Contains(chr.ToString()) || (String.IsNullOrEmpty(input)))
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                PrintLog("ValidateInput = " + ex.Message);
                return false;
            }
            return true;
        }
        //this function calculate the next cell from current cell
        private void CalcNext()
        {
            //calculate forward movement
            switch (_currentcell.Dir)
            {
                case (Direction.W):
                    _nextcell.SetPosition(((_currentcell.Position.X -1), _currentcell.Position.Y));
                    break;
                case (Direction.S):
                    _nextcell.SetPosition((_currentcell.Position.X, (_currentcell.Position.Y + 1)));
                    break;
                case (Direction.N):
                    _nextcell.SetPosition((_currentcell.Position.X, (_currentcell.Position.Y - 1)));
                    break;
                case (Direction.E):
                    _nextcell.SetPosition(((_currentcell.Position.X + 1), _currentcell.Position.Y));
                    break;
            }

            //the next cell out of the edge in width
            if (_nextcell.Position.X < 0 || _nextcell.Position.X >= _boardsize.X || _nextcell.Position.Y < 0 || _nextcell.Position.Y >= _boardsize.Y)
            {
                _nextcell.Init(CellType.Out);
            }
            //next cell inside board and get the type from the main board variable
            else
            {
                _nextcell.Init(_boardCells[_nextcell.Position.X, _nextcell.Position.Y].Type);
            }

        }
        public async Task<bool> ReadMovesLine()
        {
            try
            {
                //check to sure file not ended
                if (!_file.File_End)
                {
                    //read a line from file
                    var ReadedCmd = await _file.Read();
                    //line must contain chars
                    if (ReadedCmd.Length > 0)
                    {
                        //convert string line to moves array
                        string[] cmd_inputs = ReadedCmd.Split(' ');
                        _movesseries = new Moves[cmd_inputs.Length];

                        for (int i = 0; i < cmd_inputs.Length; i++)
                        {
                            if (!ValidateInput(cmd_inputs[i], false))
                            {
                                return false;
                            }
                            _movesseries[i] = (Moves)Enum.Parse(typeof(Moves), cmd_inputs[i]);
                        }
                        _movescount = 0;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                PrintLog("ReadMovesLine = " + ex.Message);
                return false;
            }
        }

        public (MoveErrors error,ushort error_index) MoveNext(Moves mv)
        {
            //act the moves from the first of line
            //on any error return the error code
            switch(mv)
            {
                //rotation to left
                case Moves.L:
                    {
                        switch(_currentcell.Dir)
                        {
                            case Direction.E:
                                {
                                    _currentcell.Occupie(Direction.N);
                                }
                                break;
                            case Direction.N:
                                {
                                    _currentcell.Occupie(Direction.W);
                                }
                                break;
                            case Direction.S:
                                {
                                    _currentcell.Occupie(Direction.E);
                                }
                                break;
                            case Direction.W:
                                {
                                    _currentcell.Occupie(Direction.S);
                                }
                                break;
                        }
                    }
                    break;
                //rotation to right
                case Moves.R:
                    {
                        switch (_currentcell.Dir)
                        {
                            case Direction.E:
                                {
                                    _currentcell.Occupie(Direction.S);
                                }
                                break;
                            case Direction.N:
                                {
                                    _currentcell.Occupie(Direction.E);
                                }
                                break;
                            case Direction.S:
                                {
                                    _currentcell.Occupie(Direction.W);
                                }
                                break;
                            case Direction.W:
                                {
                                    _currentcell.Occupie(Direction.N);
                                }
                                break;
                        }
                    }
                    break;
                //move forward
                case Moves.M:
                    { 
                        if (_nextcell.Type == CellType.Out)
                        {
                            _movesend = (true, MoveErrors.Edge);
                            return (MoveErrors.Edge, _movescount);
                        }
                        else
                        {
                            //next cell inside board so check for mine, exit or go forward
                            if (_nextcell.Type == CellType.Mine)
                            {
                                _movesend = (true, MoveErrors.Mine);
                                return (MoveErrors.Mine, _movescount);
                            }

                            else if (_nextcell.Type == CellType.Exit)
                            {
                                _movesend = (true, MoveErrors.Exit);
                                return (MoveErrors.Exit, _movescount);
                            }
                            else
                            {
                                //next cell is empty we go forward
                                int last_x = _currentcell.Position.X;
                                int last_y = _currentcell.Position.Y;

                                _currentcell.SetPosition((_nextcell.Position.X,_nextcell.Position.Y));
                                //free last turtle cell
                                _boardCells[last_x, last_y].Free();
                            }
                        }
                    }
                    break;
            }
            //update main board data cell
            _boardCells[_currentcell.Position.X, _currentcell.Position.Y].Occupie(_currentcell.Dir);
            CalcNext();
            _movescount++;
            _movesend = (false, MoveErrors.None);
            return (MoveErrors.None, _movescount);
        }

        /// <summary>
        /// function to print log into consol 
        /// </summary>
        private void PrintLog(string log)
        {
            if(_debug)
                Console.WriteLine("Internal Message:{0}",log);
        }

        //return the board as string to printable to console
        public string PrintBoard()
        {
            string BoardString = "Board Face\r\n";
            for(int j=0; j<_boardsize.Y; j++)
            {
                for (int i = 0; i < _boardsize.X; i++)
                {
                    switch (_boardCells[i,j].Type)
                    {
                        case CellType.Empty:
                            BoardString += "-\t";
                            break;
                        case CellType.Exit:
                            BoardString += "E\t";
                            break;
                        case CellType.Mine:
                            BoardString += "M\t";
                            break;
                        case CellType.Occupied:
                            BoardString += "S\t";
                            break;
                    }
                }
                BoardString += "\r\n";
            }
            return BoardString;
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
                _file.Close();
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        
    }
}

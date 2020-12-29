using Escape_Mines.Models;
using System;
using System.Threading.Tasks;

namespace Escape_Mines
{
    class Program
    {
        //flag to show debug message
        private static bool _debug_message_output = true;
        //flag to show debig message for each move in a line
        private static bool _debug_each_move_output = false;
        public static async Task Main(string[] args)
        {
            //print Copyright title
            Print_logo();
            //create the file read handler and instantiate board
            using (Board GameBoard = new Board(new FileReader("settings.txt"), _debug_message_output))
            {
                try
                {
                    Print_out("Loading Setting");
                    //load settings from file
                    if (await GameBoard.LoadSettings())
                    {
                        Print_out("Setting Loaded Successfully");
                        //print out the current setting
                        Print_out(GameBoard.PrintBoard());
                        Print_out("Read Movements from file:");
                        //variable to show current line to read. moves start from line 5
                        ushort lines_readed = 5;
                        //check that moves not ended(error occure) and still have moves in file
                        while (await GameBoard.ReadMovesLine() && !GameBoard.MovesEnd.end)
                        {
                            //act the moves for each moves in series
                            foreach(Moves mvs in GameBoard.MovesSeries)
                            {
                                //performe next move
                                var mvs_result = GameBoard.MoveNext(mvs);
                                //check for program end with cause
                                if (GameBoard.MovesEnd.end)
                                {
                                    string message="";
                                    switch(GameBoard.MovesEnd.cause)
                                    {
                                        //hit the mine
                                        case MoveErrors.Mine:
                                            message = "Mine Hit";
                                            break;
                                        //hit the edge
                                        case MoveErrors.Edge:
                                            message = "Edge Reach";
                                            break;
                                        //hit the exit point
                                        case MoveErrors.Exit:
                                            message = "Successful Exit";
                                            break;
                                    }
                                    Print_out(message + ". End in Move: " + GameBoard.MovesCount.ToString() +
                                        " Line: " + lines_readed.ToString(), true);
                                    break; 
                                }
                                //still no end. continue;
                                else
                                {
                                    if(_debug_each_move_output)
                                    {
                                        Print_out("Still in Danger. Move: " + GameBoard.MovesCount.ToString() +
                                            " Line: "+ lines_readed.ToString() + " done!", true);

                                        Print_out("Board Changed!");
                                        Print_out(GameBoard.PrintBoard());
                                    }
                                }
                                
                            }
                            
                            if (!GameBoard.MovesEnd.end)
                                Print_out("Line: " + lines_readed.ToString() + " Moves End: Still in Danger", true);

                            lines_readed++;
                        }

                        Print_out("Game End!");
                    }
                    else
                    {
                        Print_out("Setting Loading Error");
                    }

                    
                }
                catch (Exception ex)
                {
                    //print error by force
                    Print_out("Unhandled Error Catch:"+ ex.Message,true);
                }
                
            }
                
        }
        //function to print logo
        private static void Print_logo()
        {
            string logo = "Escape Mines Test Program!\r\n\r\n";
            logo += "        .,-;-;-,. /'_\\\r\n";
            logo += "       _/_/_/_|_\\_\\) /\r\n";
            logo += "     '-<_><_><_><_>=/\\\r\n";
            logo += "       `/_/====/_/-'\\_\\\r\n";
            logo += "        \"\"     \"\"    \"\"\r\n";
            logo += "By Hamed Nasrollahi";

            Print_out(logo, true);
        }
        //Write to console function if debug flag set true or force flag seted
        private static void Print_out(string msg, bool force_print=false)
        {
            if (_debug_message_output || force_print)
                Console.WriteLine("Message:{0}", msg);
        }
    }
}

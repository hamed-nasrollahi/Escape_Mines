using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Escape_Mines.Models
{
    public interface IFileReader
    {
        /// <summary>
        /// setting file patch
        /// </summary>
        string FilePatch { get; }

        /// <summary>
        /// setting file End Flag
        /// </summary>
        bool File_End { get; }

        /// <summary>
        /// Close the file reader and free resurces
        /// </summary>
        void Close();

        /// <summary>
        /// read a line from the file handler Async
        /// </summary>
        /// <returns></returns>
        Task<string> Read();
    }
}

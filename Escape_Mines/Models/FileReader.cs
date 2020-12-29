using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Escape_Mines.Models
{
    //this class is to fetch data from file
    public class FileReader : IFileReader
    {
        //private file handler
        private StreamReader _fileHandler;
        private string _filePatch="";

        public string FilePatch
        {
            get => _filePatch;

        }


        public bool File_End
        {
            get => _fileHandler.EndOfStream;
        }

        public FileReader(string FilePatch)
        {
            //init the new file stream 
            _filePatch = FilePatch;
            _fileHandler = new System.IO.StreamReader(_filePatch);
        }

        public void Close()
        {

            _fileHandler.Close();
        }

        #nullable enable
        public async Task<string?> Read()
        {
            return (await _fileHandler.ReadLineAsync());

        }
        #nullable disable

        public void Set_FileStream_for_Tests(StreamReader stream)
        {
            _fileHandler = stream;
        }
    }
}

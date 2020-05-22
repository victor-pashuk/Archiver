using System;
using System.Threading;

namespace GZipTestApplication
{
    public class Archiver
    {

        private readonly IReader _reader;
        private readonly IWriter _writer;
        private readonly IZipper _zipper;
        private readonly Information _information;
        public int FinishFlag => _information.FinishFlag;
     
        public Archiver(IReader reader, IWriter writer, IZipper zipper, Information information)
        {
            _information = new Information();
            _reader = reader;
            _writer = writer;
            _zipper = zipper;
            _information = information;

        }

        public int DoWork()
        {
            var readerThread = new Thread(_reader.Read);
            readerThread.Start();
            var compressThreads = new Thread[Environment.ProcessorCount];
            for (var i = 0; i < compressThreads.Length; i++)
            {
                compressThreads[i] = new Thread(_zipper.DoWork);
                compressThreads[i].Name = i.ToString();
                compressThreads[i].Start();
            }
            var writerThread = new Thread(_writer.Write);
            writerThread.IsBackground = false;
            writerThread.Start();
            writerThread.Join();
            return Convert.ToInt32(FinishFlag);

        }
    }
}

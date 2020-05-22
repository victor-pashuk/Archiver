using System.Collections.Generic;

namespace GZipTestApplication
{
    public class Launcher
    {
        private SyncQueue _writerQueue;
        private SyncQueue _readerQueue;
        private readonly string _resourcePath;
        private readonly string _destinationPath;
        private readonly Information _information;
        public List<ExceptionWithMessage> Exceptions => _information.Exceptions.GetList();
        public Launcher(string resourcePath, string destinationPath)
        {
            _resourcePath = resourcePath;
            _destinationPath = destinationPath;
            _writerQueue = new SyncQueue();
            _readerQueue = new SyncQueue();
            _information = new Information();
        }

        public int Launch(int action)
        {
            var result = 0;
            switch (action)
            {

                case (int)ActionEnum.Compress:
                    {
                        var reader = new CompressReader(_readerQueue, _information, _resourcePath);
                        var writer = new CompressWriter(_writerQueue, _information, _destinationPath);
                        var compressor = new Compressor(_readerQueue, _writerQueue, _information);
                        var archiver = new Archiver(reader, writer, compressor, _information);
                        result = archiver.DoWork();
                        break;
                    }
                case (int)ActionEnum.Decompress:
                    {
                        var reader = new DecompressReader(_readerQueue, _information, _resourcePath);
                        var writer = new DecompressWriter(_writerQueue, _information, _destinationPath);
                        var decompressor = new Decompressor(_readerQueue, _writerQueue, _information);
                        var archiver = new Archiver(reader, writer, decompressor, _information);
                        result = archiver.DoWork();
                        break;
                    }

            }

            return result;
        }

    }
}

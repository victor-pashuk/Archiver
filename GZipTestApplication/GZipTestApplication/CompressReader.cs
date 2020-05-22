using System;
using System.IO;

namespace GZipTestApplication
{
    public class CompressReader:IReader
    {
        private readonly int _bufferSize;
        private readonly  SyncQueue _readerQueue;
        public bool IsFinished { get; private set; }
        private readonly string _resourcePath;
        private readonly Information _information;

        public CompressReader(SyncQueue readerQueue, Information information, string resourcePath)
        {
            _bufferSize = 1* 1024 * 1024;
            _readerQueue = readerQueue;
            IsFinished = false;
            _resourcePath = resourcePath;
            _information = information;
        }

        public void Read()
        {
            try
            {
                using (var readStream = new FileStream(_resourcePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] streamBuffer;
                    int size;
                    while (readStream.Position < readStream.Length && _information.FinishFlag!=1)
                    {
                        if (readStream.Length - readStream.Position <= _bufferSize)
                            size = (int)(readStream.Length - readStream.Position);
                        else
                            size = _bufferSize;
                        streamBuffer = new byte[size];
                        var position = readStream.Position;
                        readStream.Read(streamBuffer, 0, size);
                        var byteBlock = new ByteBlock(streamBuffer, position);
                        _readerQueue.Enqueue(byteBlock);
                    }                    
                    IsFinished = true;
                    _readerQueue.IsFinished = true;
                }
            }
            catch (Exception ex)
            {
                var message = "Ошибка на этапе чтения файла:\n";
                _information.FinishFlag = 1;
                _readerQueue.IsFinished = true;
                _information.Exceptions.Add(new ExceptionWithMessage { Exception = ex, Message = message });
            }

        }

    }
}

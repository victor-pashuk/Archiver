using System;
using System.IO;

namespace GZipTestApplication
{
    public class DecompressReader : IReader
    {
        private readonly SyncQueue _readerQueue;
        public bool IsFinished { get; private set; }
        private readonly string _resourcePath;
        private readonly Information _information;

        public DecompressReader(SyncQueue readerQueue, Information information, string resourcePath)
        {
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
                    while (readStream.Position < readStream.Length && _information.FinishFlag != 1)
                    {
                        var lengthBuffer = new byte[4];
                        readStream.Read(lengthBuffer, 0, lengthBuffer.Length);
                        var blockLength = BitConverter.ToInt32(lengthBuffer, 0);
                        var offsetBytes = new byte[8];
                        readStream.Read(offsetBytes, 0, offsetBytes.Length);
                        var position = BitConverter.ToInt64(offsetBytes, 0);
                        var compressedData = new byte[blockLength];
                        var actualLength = readStream.Read(compressedData, 0, compressedData.Length);
                        var block = new ByteBlock(compressedData, position, actualLength);
                        _readerQueue.Enqueue(block);
                    }

                    IsFinished = true;
                    _readerQueue.IsFinished = true;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка на этапе чтения файла:\n " + ex);
                _information.FinishFlag = 1;
            }
        }
    }
}

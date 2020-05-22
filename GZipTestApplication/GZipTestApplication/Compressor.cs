using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZipTestApplication
{
    public class Compressor : IZipper
    {
        private readonly SyncQueue _readerQueue;
        private readonly SyncQueue _writerQueue;
        private object _locker = new object();
        private bool _isFinished;
        private readonly Information _information;
        private int _counter;

        public Compressor(SyncQueue readerQueue, SyncQueue writerQueue, Information information)
        {
            _readerQueue = readerQueue;
            _writerQueue = writerQueue;
            _isFinished = false;
            _information = information;
        }

        public void DoWork()
        {
            Compress();
        }

        private void Compress()
        {
            try
            {
                while (!_isFinished && _information.FinishFlag != 1)
                {
                    if (_readerQueue.IsFinished && _readerQueue.Count==0)
                    {
                        lock (_locker)
                        {
                            if (_writerQueue.IsFinished && _isFinished)
                                break;
                            _writerQueue.IsFinished = true;
                            _isFinished = true;
                        }
                    }
                    else
                    {
                        var block = _readerQueue.Dequeue();
                        if (block != null)
                        {
                            var resultBlock = CompressBlock(block);
                            _writerQueue.Enqueue(resultBlock);
                            Interlocked.Increment(ref _counter);
                            _information.ProccessedCount = _counter;
                        }
                    }
                }
                if (_information.FinishFlag == 1)
                {
                    _writerQueue.IsFinished = true;
                    _readerQueue.IsFinished = true;
                    _isFinished = true;

                }
            }
            catch (Exception ex)
            {
                var message = "Ошибка на этапе сжатия файла:\n";
                _information.FinishFlag = 1;
                _writerQueue.IsFinished = true;
                _readerQueue.IsFinished = true;
                _isFinished = true;
                _information.Exceptions.Add(new ExceptionWithMessage { Exception = ex, Message = message });

            }
        }

        private ByteBlock CompressBlock(ByteBlock block)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var compressStream = new GZipStream(memoryStream, CompressionLevel.Optimal, false))
                {
                    compressStream.Write(block.Buffer, 0, block.Buffer.Length);
                }
                var resultData = memoryStream.ToArray();
                var resultBlock = new ByteBlock(resultData, block.Position);
                memoryStream.Flush();
                return resultBlock;
            }
        }

    }
}

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace GZipTestApplication
{
    public class Decompressor : IZipper
    {
        private readonly SyncQueue _readerQueue;
        private readonly SyncQueue _writerQueue;
        private object _locker = new object();
        private bool _isFinished;
        private readonly Information _information;
        private int _counter;

        public Decompressor(SyncQueue readerQueue, SyncQueue writerQueue, Information information)
        {
            _readerQueue = readerQueue;
            _writerQueue = writerQueue;
            _isFinished = false;
            _information = information;
        }

        public void DoWork()
        {
            Decompress();
        }

        public void Decompress()
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
                    var block = _readerQueue.Dequeue();
                    if (block != null)
                    {
                        var resultBlock = Decompress(block);
                        _writerQueue.Enqueue(resultBlock);
                        Interlocked.Increment(ref _counter);
                        _information.ProccessedCount = _counter;
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
                var message = "Ошибка на этапе расжатия файла:\n";
                _information.FinishFlag = 1;
                _writerQueue.IsFinished = true;
                _readerQueue.IsFinished = true;
                _isFinished = true;
                _information.Exceptions.Add(new ExceptionWithMessage { Exception = ex, Message = message });
            }
        }

        public ByteBlock Decompress(ByteBlock block)
        {
            using (var inputStream = new MemoryStream(block.Buffer.Take(block.ActualLength).ToArray()))
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var decompressStream = new GZipStream(inputStream, CompressionMode.Decompress, false))
                    {
                        decompressStream.CopyTo(outputStream);
                    }
                    var resultBlock = new ByteBlock(outputStream.ToArray(), block.Position);
                    return resultBlock;
                }
            }
        }
    }
}

using System;
using System.IO;

namespace GZipTestApplication
{
    public class DecompressWriter : IWriter
    {
        private readonly SyncQueue _writerQueue;
        private readonly string _destinationPath;
        private bool _isFinished;
        private readonly Information _information;

        public DecompressWriter(SyncQueue writerQueue, Information information, string destinationPath)
        {
            _writerQueue = writerQueue;
            _destinationPath = destinationPath;
            _isFinished = false;
            _information = information;
        }

        public void Write()
        {
            try
            {
                using (var fileStream = new FileStream(_destinationPath, FileMode.Append))
                {
                    while (!_isFinished && _information.FinishFlag != 1)
                    {
                        if (_writerQueue.IsFinished && _information.WrittenCount == _information.ProccessedCount)
                        {
                            _isFinished = true;
                            _information.FinishFlag = 0;
                            break;
                        }

                        var block = _writerQueue.Dequeue();
                        if (block != null)
                        {

                            fileStream.Position = block.Position;
                            fileStream.Write(block.Buffer, 0, block.Buffer.Length);
                            _information.WrittenCount++;
                        }
                    }
                    if (_information.FinishFlag == 1)
                    {
                        _writerQueue.IsFinished = true;
                        _isFinished = true;
                    }
                }
            }
            catch (IOException ex)
            {
                var message = "Недостаточно свободного места на диске для записи ";
                _information.FinishFlag = 1;
                _isFinished = true;
                _writerQueue.IsFinished = true;
                _information.Exceptions.Add(new ExceptionWithMessage { Exception = ex, Message = message });
            }
            catch (Exception ex)
            {
                var message = "Ошибка на этапе записи файла:\n";
                _information.FinishFlag = 1;
                _isFinished = true;
                _writerQueue.IsFinished = true;
                _information.Exceptions.Add(new ExceptionWithMessage { Exception = ex, Message = message });
            }
        }
    }
}

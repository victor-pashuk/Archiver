using System.Collections.Generic;
using System.Threading;

namespace GZipTestApplication
{
    public class SyncQueue
    {
        private readonly Queue<ByteBlock> _queue;
        private object _locker = new object();
        private bool _isFinished;
        public int Count => _queue.Count;
        private readonly int _maxCount;

        public SyncQueue()
        {
            _queue = new Queue<ByteBlock>();
            _maxCount = 100;
        }

        public bool IsFinished
        {
            get
            {
                return _isFinished;
            }
            set
            {
                lock (_locker)
                {
                    _isFinished = value;
                    Monitor.PulseAll(_locker);
                }
            }
        }

        public void Enqueue(ByteBlock block)
        {
            lock (_locker)
            {
                while (Count == _maxCount)
                    Monitor.Wait(_locker);
                _queue.Enqueue(block);
                Monitor.PulseAll(_locker);
            }
        }

        public ByteBlock Dequeue()
        {
            lock (_locker)
            {
                while (_queue.Count == 0 && !IsFinished)
                    Monitor.Wait(_locker);
                if (_queue.Count == 0)
                    return null;
                Monitor.PulseAll(_locker);
                return _queue.Dequeue();
            }
        }
    }
}

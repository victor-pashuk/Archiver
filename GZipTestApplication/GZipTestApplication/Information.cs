namespace GZipTestApplication
{
    public class Information
    {
        private int _proccessedCount;
        private int _writtenCount;
        private int _finishFlag;
        private object _locker = new object();
        public MultithreadList<ExceptionWithMessage> Exceptions { get; }

        public Information()
        {
            _proccessedCount = 0;
            _writtenCount = 0;
            _finishFlag = 0;
            Exceptions = new MultithreadList<ExceptionWithMessage>();
        }

        public int FinishFlag
        {
            get { return _finishFlag; }
            set
            {
                lock (_locker)
                {
                    _finishFlag = value;
                }
            }
        }


        public int ProccessedCount
        {
            get { return _proccessedCount; }
            set
            {
                lock (_locker)
                {
                    _proccessedCount = value;
                }
            }
        }

        public int WrittenCount
        {
            get { return _writtenCount; }
            set
            {
                lock (_locker)
                {
                    _writtenCount = value;
                }
            }
        }

    }
}

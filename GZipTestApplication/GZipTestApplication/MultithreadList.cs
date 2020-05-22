using System.Collections.Generic;

namespace GZipTestApplication
{
    public class MultithreadList<T>
    {
        private readonly List<T> _list;
        private object _locker = new object();

        public MultithreadList()
        {
            _list = new List<T>();
        }

        public void Add(T value)
        {
            lock (_locker)
            {
                _list.Add(value);
            }
        }

        public List<T> GetList()
        {
            return _list;
        }




    }
}

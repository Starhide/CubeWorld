using System;
using System.Collections.Concurrent;
using System.Linq;

namespace BlockWorld.util
{
    internal class ObjectPool<T>
    {
        private ConcurrentBag<T> _objects;
        private readonly Func<T> _factory;
        private readonly int Capacity;

        public int Count { get => _objects.Count; }

        public ObjectPool(Func<T> factory, int capacity)
        {
            _objects = new ConcurrentBag<T>();
            _factory = factory ?? throw new ArgumentNullException("factory");
            Capacity = capacity;
        }

        public T GetObject()
        {
            if (_objects.TryTake(out T item))
            {
                return item;
            }

            return _factory();
        }

        public bool PutObject(T item)
        {
            if (Capacity == 0 || _objects.Count < Capacity)
            {
                _objects.Add(item);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

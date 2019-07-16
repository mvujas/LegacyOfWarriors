using System;
using System.Collections.Concurrent;
using Utils.Delegates;

namespace Utils.DataTypes
{
    public class ObjectPool<T>
    {
        private ConcurrentBag<T> m_collection;
        private Supplier<T> m_supplier;
        private int m_size;
        private bool m_growable;

        public ObjectPool(Supplier<T> supplier, int size, bool growable = false)
        {
            if (size < 1)
            {
                throw new ArgumentException("Size must be positive number");
            }
            m_size = size;
            m_supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
            m_growable = growable;
            m_collection = new ConcurrentBag<T>();
            Init();
        }

        private void Init()
        {
            for (int i = 0; i < m_size; i++)
            {
                AddNewInstance();
            }
        }

        private void AddNewInstance()
        {
            m_collection.Add(m_supplier());
        }

        public T GetObject()
        {
            T item;
            if (m_collection.TryTake(out item))
            {
                return item;
            }
            if (!m_growable)
            {
                throw new InvalidOperationException("Object pool is empty");
            }
            return m_supplier();
        }

        public void ReleaseObject(T obj)
        {
            m_collection.Add(obj);
        }
    }
}

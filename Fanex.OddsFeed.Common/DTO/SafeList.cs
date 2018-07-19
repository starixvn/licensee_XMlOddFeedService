namespace Fanex.OddsFeed.Common.DTO
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    [Serializable]
    public class SafeList<T> : IList<T>
    {
        // the (thread-unsafe) collection that actually stores everything
        private readonly List<T> _inner;

        // this is the object we shall lock on.
        private readonly object _lock = new object();

        public SafeList()
        {
            _inner = new List<T>();
        }

        public SafeList(IEnumerable<T> value)
            : this()
        {
            _inner.AddRange(value);
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _inner.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get
            {
                lock (_lock)
                {
                    return _inner[index];
                }
            }

            set
            {
                lock (_lock)
                {
                    _inner[index] = value;
                }
            }
        }

        /// <summary>
        /// Instead of returning an unsafe enumerator.
        /// We wrap it into our thread-safe class.
        /// </summary>
        /// <returns>The Enumerator.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (_lock)
            {
                return new SafeEnumerator<T>(_inner.GetEnumerator(), _lock);
            }
        }

        /// <summary>
        /// To be actually thread-safe, our collection must be locked on all other operations.
        /// </summary>
        /// <param name="item">The element.</param>
        public void Add(T item)
        {
            lock (_lock)
            {
                _inner.Add(item);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _inner.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_lock)
            {
                return _inner.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock)
            {
                _inner.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (_lock)
            {
                return _inner.Remove(item);
            }
        }

        public IEnumerator GetEnumerator()
        {
            lock (_lock)
            {
                return new SafeEnumerator<T>(_inner.GetEnumerator(), _lock);
            }
        }

        public int IndexOf(T item)
        {
            lock (_lock)
            {
                return _inner.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_lock)
            {
                _inner.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                _inner.RemoveAt(index);
            }
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            lock (_lock)
            {
                return new ReadOnlyCollection<T>(this);
            }
        }

        public void ForEach(Action<T> action)
        {
            if (action != null)
            {
                lock (_lock)
                {
                    foreach (var item in _inner)
                    {
                        action(item);
                    }
                }
            }
        }

        public bool Exists(Predicate<T> match)
        {
            if (match != null)
            {
                lock (_lock)
                {
                    foreach (var item in _inner)
                    {
                        if (match(item))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
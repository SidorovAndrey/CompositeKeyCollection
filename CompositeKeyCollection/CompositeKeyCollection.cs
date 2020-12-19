using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CompositeKeyCollection 
{
    class CompositeKeyCollection<TId, TName, TValue> : ICollection<KeyValuePair<(TId id, TName name), TValue>>
    {
        private Dictionary<TId, List<TValue>> _idValues = new();
        private Dictionary<TName, List<TValue>> _nameValues = new();
        private Dictionary<(TId, TName), TValue> _bothValues = new();

        private readonly object _lockObj = new();

        public int Count => _bothValues.Count;
        public bool IsReadOnly => false;

        public void Add(TId id, TName name, TValue value)
        {
            lock (_lockObj)
            {
                AddValue(id, name, value);
            }
        }

        public void Add(KeyValuePair<(TId id, TName name), TValue> item)
        {
            lock (_lockObj)
            {
                AddValue(item.Key.id, item.Key.name, item.Value);
            }
        }

        private void AddValue(TId id, TName name, TValue value)
        {
            if (_bothValues.ContainsKey((id, name)))
                throw new ArgumentException($"Collection already contains element with keys {id} and {name}");

            if (_idValues.TryGetValue(id, out var idValues))
            {
                if (!idValues.Any(x => x.Equals(value)))
                    idValues.Add(value);
            }
            else
            {
                _idValues[id] = new List<TValue> { value };
            }

            if (_nameValues.TryGetValue(name, out var nameValues))
            {
                if (!nameValues.Any(x => x.Equals(value)))
                    nameValues.Add(value);
            }
            else
            {
                _nameValues[name] = new List<TValue> { value };
            }

            _bothValues[(id, name)] = value;
        }

        public void Clear()
        {
            lock (_lockObj)
            {
                _bothValues.Clear();
                _idValues.Clear();
                _nameValues.Clear();
            }
       }

       public bool Contains(KeyValuePair<(TId id, TName name), TValue> item)
       {
           return _bothValues.Contains(item);
       }

       public void CopyTo(KeyValuePair<(TId id, TName name), TValue>[] items, int index)
       {
           ((ICollection<KeyValuePair<(TId id, TName name), TValue>>)_bothValues).CopyTo(items, index);
       }

        public void Remove(TId id, TName name)
        {
            lock (_lockObj)
            {
                RemoveValue(id, name);
            }
        }

        public bool Remove(KeyValuePair<(TId id, TName name), TValue> item)
        {
            lock (_lockObj)
            {
                if (!_bothValues.ContainsKey((item.Key.id, item.Key.name)))
                    return false;

                RemoveValue(item.Key.id, item.Key.name);
                return true;
            }
        }

        private void RemoveValue(TId id, TName name)
        {
            if (!_bothValues.ContainsKey((id, name)))
                throw new ArgumentException($"Collection does not contain element with keys {id} and {name}");

            var itemToRemove = GetValue(id, name);

            var idValues = _idValues[id];
            idValues.Remove(itemToRemove);
            if (idValues.Count == 0)
                _idValues.Remove(id);

            var nameValues = _nameValues[name];
            nameValues.Remove(itemToRemove);
            if (nameValues.Count == 0)
                _nameValues.Remove(name);

            _bothValues.Remove((id, name));
        }

        public void SetValue(TId id, TName name, TValue value)
        {
            lock(_lockObj)
            {
                if (_idValues.ContainsKey(id) && _nameValues.ContainsKey(name))
                    RemoveValue(id, name);

                AddValue(id, name, value);
            }
        }

        public TValue GetValue(TId id, TName name)
        {
            if (_bothValues.TryGetValue((id, name), out var value))
                return value;

            throw new ArgumentException($"No object with ids {id} and {name}");
        }

        public TValue this[TId id, TName name] 
        {
            get => GetValue(id, name);
            set => SetValue(id, name, value);
        }

        public List<TValue> GetById(TId id)
        {
            if (_idValues.TryGetValue(id, out var items))
                return items;

            throw new ArgumentException($"No objects with id {id}", nameof(id));
        }

        public List<TValue> GetByName(TName name)
        {
            if (_nameValues.TryGetValue(name, out var items))
                return items;

            throw new ArgumentException($"No objects with name {name}", nameof(name));
        }

        public IEnumerator<KeyValuePair<(TId id, TName name), TValue>> GetEnumerator()
        {
            return _bothValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
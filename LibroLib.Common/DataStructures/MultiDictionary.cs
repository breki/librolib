using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LibroLib.DataStructures
{
    /// <summary>
    /// Implementation of a dictionary that allows storing of multiple values for the same key.
    /// Duplicate values are allowed.
    /// </summary>
    /// <remarks>If you need a multi-dictionary that merges duplicate values, use <see cref="MultiSet{TKey,TValue}"/>
    /// class.</remarks>
    /// <typeparam name="TKey">Key type. Make sure it has properly implemented equality and hash methods.</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public class MultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
    {
        public int KeysCount
        {
            get { return keysValues.Count; }
        }

        public IEnumerable<TKey> Keys
        {
            get { return keysValues.Keys; }
        }

        public void Add (TKey key, TValue value)
        {
            Contract.Requires (!ReferenceEquals (key, null));

            List<TValue> values;
            if (!keysValues.TryGetValue(key, out values))
            {
                values = new List<TValue>();
                keysValues[key] = values;
            }

            values.Add(value);
        }

        public void Clear ()
        {
            keysValues.Clear();
        }

        public void ClearValues(TKey key)
        {
            Contract.Requires (!ReferenceEquals (key, null));

            keysValues.Remove(key);
        }

        public bool ContainsKey(TKey key)
        {
            Contract.Requires (!ReferenceEquals (key, null));
            return keysValues.ContainsKey(key);
        }

        public IList<TValue> this[TKey key]
        {
            get
            {
                Contract.Requires (!ReferenceEquals (key, null));

                return keysValues[key];
            }
        }

        public bool TryGetValues (TKey key, out IList<TValue> values)
        {
            Contract.Requires (!ReferenceEquals (key, null));

            List<TValue> listValues;
            if (keysValues.TryGetValue(key, out listValues))
            {
                values = listValues;
                return true;
            }

            values = null;
            return false;
        }
        
        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
        {
            return keysValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly Dictionary<TKey, List<TValue>> keysValues = new Dictionary<TKey, List<TValue>>();
    }
}
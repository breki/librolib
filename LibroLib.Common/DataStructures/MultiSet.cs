using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LibroLib.DataStructures
{
    /// <summary>
    /// Implementation of a dictionary that allows storing of multiple values for the same key.
    /// Adding duplicate values is allowed, but they are merged into a single value.
    /// </summary>
    /// <remarks>If you need a multi-dictionary that retains duplicate values, use <see cref="MultiDictionary{TKey,TValue}"/>
    /// class.</remarks>
    /// <typeparam name="TKey">Key type. Make sure it has properly implemented equality and hash methods.</typeparam>
    /// <typeparam name="TValue">Value type. Make sure it has properly implemented equality and hash methods.</typeparam>
    public class MultiSet<TKey, TValue>
    {
        public void Add (TKey key, TValue value)
        {
            Contract.Requires (!ReferenceEquals (key, null));

            HashSet<TValue> values;
            if (!keysValues.TryGetValue(key, out values))
            {
                values = new HashSet<TValue>();
                keysValues[key] = values;
            }

            values.Add(value);
        }

        public ISet<TValue> this[TKey key]
        {
            get
            {
                Contract.Requires (!ReferenceEquals (key, null));

                return keysValues[key];
            }
        }

        private readonly Dictionary<TKey, HashSet<TValue>> keysValues =
            new Dictionary<TKey, HashSet<TValue>>();
    }
}
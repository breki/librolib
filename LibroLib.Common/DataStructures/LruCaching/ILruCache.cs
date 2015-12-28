using System.Diagnostics.Contracts;

namespace LibroLib.DataStructures.LruCaching
{
    /// <summary>
    /// A least-recently-used cache.
    /// </summary>
    /// <typeparam name="TKey">Type to be used as a cache key. Make sure the <see cref="TKey"/> 
    /// has an optimal <see cref="object.GetHashCode"/> method implementation.</typeparam>
    /// <typeparam name="TValue">Type to be used as a value to be cached.</typeparam>
    [ContractClass(typeof(ILruCacheContract<,>))]
    public interface ILruCache<in TKey, TValue>
    {
        int Count { get; }

        void Add(TKey key, TValue value);
        TValue Get(TKey key);

        /// <summary>
        /// Deletes the value from the cache without synchronizing it with the underlying storage.
        /// </summary>
        /// <param name="key">The key of the value to delete from the cache.</param>
        /// <returns><c>true</c> if the value was actually found in the cache; <c>false</c> otherwise.</returns>
        bool Delete(TKey key);
        void Flush();
    }

    [ContractClassFor(typeof(ILruCache<,>))]
    internal abstract class ILruCacheContract<TKey, TValue> : ILruCache<TKey, TValue>
    {
        public int Count
        {
            get { throw new System.NotImplementedException(); }
        }

        public void Add(TKey key, TValue value)
        {
            Contract.Requires (!ReferenceEquals (key, null));
            throw new System.NotImplementedException ();
        }

        public TValue Get(TKey key)
        {
            Contract.Requires (!ReferenceEquals (key, null));
            throw new System.NotImplementedException ();
        }

        public bool Delete(TKey key)
        {
            Contract.Requires (!ReferenceEquals (key, null));
            throw new System.NotImplementedException ();
        }

        public void Flush()
        {
        }
    }
}
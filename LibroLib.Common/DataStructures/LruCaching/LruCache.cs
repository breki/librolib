using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LibroLib.DataStructures.LruCaching
{
    /// <summary>
    /// Implementation of the least-recently-used cache algorithm.
    /// </summary>
    /// <typeparam name="TKey">Type to be used as a cache key. Make sure the <see cref="TKey"/>
    /// has an optimal <see cref="object.GetHashCode"/> method implementation.</typeparam>
    /// <typeparam name="TValue">Type to be used as a value to be cached.</typeparam>
    public class LruCache<TKey, TValue> : ILruCache<TKey, TValue>
    {
        public LruCache(int cacheSize, Func<TKey, TValue> readItemFunc, Action<TKey, TValue> writeItemAction)
        {
            this.cacheSize = cacheSize;
            this.readItemFunc = readItemFunc;
            this.writeItemAction = writeItemAction;
        }

        public int Count
        {
            get { return cachedItems.Count; }
        }

        public void Add(TKey key, TValue value)
        {
            LinkedListNode<CachedItem<TKey, TValue>> cachedItemNode;

            // is the item already cached?
            if (cachedItems.TryGetValue(key, out cachedItemNode))
            {
                // replace the old value with new one
                cachedItemNode.Value.Value = value;
                // mark the item as last used
                cachedItemsByUsage.Remove(cachedItemNode);
                cachedItemsByUsage.AddLast(cachedItemNode);
                return;
            }

            SaveNewItemToCache(key, value);
        }

        public TValue Get(TKey key)
        {
            LinkedListNode<CachedItem<TKey, TValue>> itemNode;

            // is the item already cached?
            if (cachedItems.TryGetValue(key, out itemNode))
                return itemNode.Value.Value;

            TValue value = readItemFunc(key);
            SaveNewItemToCache(key, value);

            return value;
        }

        public bool Delete(TKey key)
        {
            LinkedListNode<CachedItem<TKey, TValue>> node;
            if (!cachedItems.TryGetValue(key, out node))
                return false;

            cachedItems.Remove(key);
            Contract.Assume(node != null);
            cachedItemsByUsage.Remove(node);

            return true;
        }

        public void Flush()
        {
            foreach (LinkedListNode<CachedItem<TKey, TValue>> cachedItemNode in cachedItems.Values)
            {
                CachedItem<TKey, TValue> cachedItem = cachedItemNode.Value;

                if (cachedItem.IsDirty)
                    writeItemAction(cachedItem.Key, cachedItem.Value);
            }

            cachedItems.Clear();
            cachedItemsByUsage.Clear();
        }

        public bool IsCached(TKey key)
        {
            Contract.Requires(!ReferenceEquals(key, null));

            return cachedItems.ContainsKey(key);
        }

        private void SaveNewItemToCache(TKey key, TValue value)
        {
            Contract.Requires(!ReferenceEquals(key, null));

            CachedItem<TKey, TValue> newItem = new CachedItem<TKey, TValue>(key, value, true);
            LinkedListNode<CachedItem<TKey, TValue>> newItemNode = cachedItemsByUsage.AddLast(newItem);
            cachedItems.Add(key, newItemNode);

            if (Count > cacheSize)
                RemoveItemFromCache();
        }

        private void RemoveItemFromCache()
        {
            LinkedListNode<CachedItem<TKey, TValue>> itemNodeToRemoveFromCache = cachedItemsByUsage.First;

            if (itemNodeToRemoveFromCache == null)
                throw new InvalidOperationException("BUG: cachedItemsByUsage is empty");

            CachedItem<TKey, TValue> itemToRemoveFromCache = itemNodeToRemoveFromCache.Value;
            cachedItemsByUsage.Remove(itemNodeToRemoveFromCache);
            if (!cachedItems.Remove(itemToRemoveFromCache.Key))
                throw new InvalidOperationException("BUG: the item was not in cachedItems");

            if (itemToRemoveFromCache.IsDirty)
                writeItemAction(itemToRemoveFromCache.Key, itemToRemoveFromCache.Value);
        }

        private class CachedItem<TItemKey, TItemValue> : IEquatable<CachedItem<TItemKey, TItemValue>>
        {
            public CachedItem(TItemKey key, TItemValue value, bool isDirty)
            {
                Contract.Requires(!ReferenceEquals(key, null));

                this.key = key;
                this.value = value;
                this.isDirty = isDirty;
            }

            public TItemKey Key
            {
                get
                {
                    Contract.Ensures(!ReferenceEquals(Contract.Result<TItemKey>(), null));
                    return key;
                }
            }

            public TItemValue Value
            {
                get
                {
                    return value;
                }

                set
                {
                    this.value = value;
                    isDirty = true;
                }
            }

            public bool IsDirty
            {
                get { return isDirty; }
            }

            public bool Equals(CachedItem<TItemKey, TItemValue> other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;

                return EqualityComparer<TItemKey>.Default.Equals(
                    key,
                    other.key);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((CachedItem<TItemKey, TItemValue>)obj);
            }

            public override int GetHashCode()
            {
                return EqualityComparer<TItemKey>.Default.GetHashCode(key);
            }

            [ContractInvariantMethod]
#pragma warning disable CC0091 // Use static method
            private void Invariant()
#pragma warning restore CC0091 // Use static method
            {
                Contract.Invariant(!ReferenceEquals(key, null));
            }

            private readonly TItemKey key;
            private TItemValue value;
            private bool isDirty;
        }

        private readonly int cacheSize;
        private readonly Func<TKey, TValue> readItemFunc;
        private readonly Action<TKey, TValue> writeItemAction;
        private readonly Dictionary<TKey, LinkedListNode<CachedItem<TKey, TValue>>> cachedItems = new Dictionary<TKey, LinkedListNode<CachedItem<TKey, TValue>>>();
        private readonly LinkedList<CachedItem<TKey, TValue>> cachedItemsByUsage = new LinkedList<CachedItem<TKey, TValue>>();
    }
}
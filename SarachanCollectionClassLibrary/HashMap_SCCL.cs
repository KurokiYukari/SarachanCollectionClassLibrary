using System;
using System.Collections.Generic;

namespace Sarachan.Collections
{
    /// <summary>
    /// 基于哈希表的 Map 实现
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class HashMap_SCCL<TKey, TValue> : IMap_SCCL<TKey, TValue>
    {
        #region Fields
        private const int _DEFAULT_HASHLIST_CAPACITY = 8; // _hashList 的默认（也是最小单位）Capacity
        private const double _ENLARGE_THREADHOLD = 0.75; // 当 Count / HashListCapacity > _ENLARGE_THREADHOLD 时扩大 Capacity

        /// <summary>
        /// 哈希表（ArrayList），采用拉链法（LinkedList）解决哈希冲突
        /// </summary>
        private readonly ArrayList_SCCL<LinkedList_SCCL<KeyValuePair_SCCL<TKey, TValue>>> _hashList;
        #endregion

        #region Properties
        public int Count { get; protected set; }

        /// <summary>
        /// _hashList 的容量，其大小一定为 8 * 2^n （n为自然数）
        /// </summary>
        public int HashListCapacity => _hashList.Capacity;

        public KeyValuePair_SCCL<TKey, TValue>[] Items
        {
            get
            {
                var result = new KeyValuePair_SCCL<TKey, TValue>[Count];
                int i = 0;
                foreach (var item in this)
                {
                    result[i++] = item;
                }

                return result;
            }
        }

        public TKey[] Keys
        {
            get
            {
                var result = new TKey[Count];
                int i = 0;
                foreach (var item in this)
                {
                    result[i++] = item.Key;
                }

                return result;
            }
        }

        public TValue[] Values
        {
            get
            {
                var result = new TValue[Count];
                int i = 0;
                foreach (var item in this)
                {
                    result[i++] = item.Value;
                }

                return result;
            }
        }

        public IEqualityComparer<KeyValuePair_SCCL<TKey, TValue>> PairEqualityComparer { get; init; }

        public TValue this[TKey key]
        {
            get
            {
                try
                {
                    return Get(key);
                }
                catch (ArgumentException)
                {
                    return default;
                }
            }

            set
            {
                if (!Set(key, value))
                {
                    Add(key, value);
                }
            }
        }
        #endregion

        #region Constructors
        public HashMap_SCCL() : this(null) { }

        public HashMap_SCCL(IEqualityComparer<TKey> keyComparer)
        {
            keyComparer ??= EqualityComparer<TKey>.Default;
            PairEqualityComparer = new ComparedByKey_PairComparer(keyComparer);

            // 新建一个大小为 _DEFAULT_HASHLIST_CAPACITY 的 ArrayList，并将其中每个元素填充为 null
            _hashList = new(_DEFAULT_HASHLIST_CAPACITY)
            {
                new LinkedList_SCCL<KeyValuePair_SCCL<TKey, TValue>>[_DEFAULT_HASHLIST_CAPACITY]
            };
        }
        #endregion

        #region Methods
        public TValue Get(TKey key)
        {
            (int index1, int index2) = GetKeyPos(key);
            if (index2 == -1)
            {
                throw new ArgumentException($"The key: {key} doesn't exist in this Map", nameof(key));
            }

            return _hashList[index1][index2].Value;
        }

        public bool Set(TKey key, TValue value)
        {
            (int index1, int index2) = GetKeyPos(key);
            if (index2 == -1)
            {
                return false;
            }

            _hashList[index1][index2] = new(key, value);
            return true;
        }

        public bool Add(TKey key, TValue value) => Add(new KeyValuePair_SCCL<TKey, TValue>(key, value));

        public bool Add(KeyValuePair_SCCL<TKey, TValue> item)
        {
            (_, int exist) = GetKeyPos(item.Key);

            if (exist != -1)
            {
                return false;
            }

            CheckCapacity();

            int index = Hash(item.Key); // 这里要重新计算 index，因为 CheckCapacity() 可能会导致 Hash 的结果改变
            _hashList[index] ??= new(); 
            _hashList[index].Add(item);
            Count++;
            return true;
        }

        public void Clear()
        {
            for (int i = 0; i < _hashList.Count; i++)
            {
                _hashList[i] = null;
            }

            Count = 0;
        }

        public bool Contains(KeyValuePair_SCCL<TKey, TValue> item, IEqualityComparer<KeyValuePair_SCCL<TKey, TValue>> comparer = null)
        {
            (int index1, int index2) = GetKeyPos(item.Key);
            if (index2 == -1)
            {
                return false;
            }

            comparer ??= EqualityComparer<KeyValuePair_SCCL<TKey, TValue>>.Default;
            return comparer.Equals(item, _hashList[index1][index2]);
        }

        public bool ContainsKey(TKey key)
        {
            (_, int exist) = GetKeyPos(key);

            return exist != -1;
        }
        public bool IsEmpty() => Count == 0;

        public bool Remove(TKey key) => Remove(new(key, default), PairEqualityComparer);

        public bool Remove(KeyValuePair_SCCL<TKey, TValue> item, IEqualityComparer<KeyValuePair_SCCL<TKey, TValue>> comparer = null)
        {
            var linkedlist = _hashList[Hash(item.Key)];

            if (!linkedlist?.Contains(item, comparer) ?? true)
            {
                return false;
            }

            linkedlist.Remove(item);
            if (linkedlist.Count == 0)
            {
                _hashList[Hash(item.Key)] = null;
            }

            Count--;

            return true;
        }

        /// <summary>
        /// 哈希函数
        /// </summary>
        /// <param name="key"></param>
        /// <returns>key 的哈希值。若 key 为 null，返回 0</returns>
        private int Hash(TKey key) => Math.Abs(PairEqualityComparer.GetHashCode(new KeyValuePair_SCCL<TKey, TValue>(key, default))) % HashListCapacity;

        /// <summary>
        /// 获取一个代表 key 对应的位置的元组
        /// </summary>
        /// <param name="key"></param>
        /// <returns>第一个元素 hashListIndex 代表应该在 _hashList 中的位置（不存在则是 Hash 值），第二个元素 linkedListIndex 代表在链表 _hashList[hashListIndex] 中的位置（不存在则为 -1）</returns>
        private (int hashListIndex, int linkedListIndex) GetKeyPos(TKey key)
        {
            var hashListIndex = Hash(key);

            var linkedList = _hashList[hashListIndex];
            if (linkedList is null)
            {
                return (hashListIndex, -1);
            }

            var linkedListIndex = linkedList.IndexOf(new(key, default), PairEqualityComparer);
            if (linkedListIndex == -1)
            {
                return (hashListIndex, -1);
            }
            else
            {
                return (hashListIndex, linkedListIndex);
            }
        }

        /// <summary>
        /// 将 HashListCapacity 扩大为原来的两倍
        /// </summary>
        private void EnlargeCapacity()
        {
            var originalCapacity = HashListCapacity;
            _hashList.Capacity *= 2;

            _hashList.Add(new LinkedList_SCCL<KeyValuePair_SCCL<TKey, TValue>>[originalCapacity]);

            for (int i = 0; i < originalCapacity; i++)
            {
                var linkedList = _hashList[i];
                if (linkedList == null)
                {
                    continue;
                }

                foreach (var item in linkedList)
                {
                    var newHash = Hash(item.Key);
                    if (newHash == i)
                    {
                        continue;
                    }

                    linkedList.Remove(item);

                    var newPosLinkedList = _hashList[newHash] ??= new LinkedList_SCCL<KeyValuePair_SCCL<TKey, TValue>>();
                    newPosLinkedList.Add(item);
                }
            }
        }

        /// <summary>
        /// 检测在增加 increment 数目个 item 后当前 Capacity 是否足够使用。若否，会进行扩容
        /// </summary>
        /// <param name="increment">要增加到 HashSet 中的 item 的数目</param>
        private void CheckCapacity(int increment = 1)
        {
            if (_ENLARGE_THREADHOLD * HashListCapacity < Count + increment)
            {
                EnlargeCapacity();

                // 递归调用
                CheckCapacity(increment);
            }
        }

        public IEnumerator<KeyValuePair_SCCL<TKey, TValue>> GetEnumerator()
        {
            foreach (var linkedList in _hashList)
            {
                if (linkedList is null)
                {
                    continue;
                }

                foreach (var item in linkedList)
                {
                    yield return item;
                }
            }
        }

        public override string ToString() => $"[ {string.Join(", ", this)} ]";
        #endregion

        /// <summary>
        /// 只由 TKey 决定是否相等的 KeyValuePair 比较器
        /// </summary>
        private sealed class ComparedByKey_PairComparer : IEqualityComparer<KeyValuePair_SCCL<TKey, TValue>>
        {
            private readonly IEqualityComparer<TKey> _keyComparer;

            public ComparedByKey_PairComparer(IEqualityComparer<TKey> keyComparer)
            {
                _keyComparer = keyComparer;
            }

            public bool Equals(KeyValuePair_SCCL<TKey, TValue> x, KeyValuePair_SCCL<TKey, TValue> y) => _keyComparer.Equals(x.Key, y.Key);

            public int GetHashCode([System.Diagnostics.CodeAnalysis.DisallowNull] KeyValuePair_SCCL<TKey, TValue> obj) =>
                obj.Key?.GetHashCode() ?? 0;
        }
    }
}

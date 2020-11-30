using System;
using System.Collections;
using System.Collections.Generic;

namespace Sarachan.Collections
{
    /// <summary>
    /// 基于哈希表的集合的实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class HashSet_SCCL<T> : ISet_SCCL<T>
    {
        #region Fields
        private const int _DEFAULT_HASHLIST_CAPACITY = 8; // _hashList 的默认（也是最小单位）Capacity
        private const double _ENLARGE_THREADHOLD = 0.75; // 当 Count / HashListCapacity > _ENLARGE_THREADHOLD 时扩大 Capacity

        /// <summary>
        /// 哈希表（ArrayList），采用拉链法（LinkedList）解决哈希冲突
        /// </summary>
        private readonly ArrayList_SCCL<LinkedList_SCCL<T>> _hashList;

        /// <summary>
        /// _hashList 的容量，其大小一定为 8 * 2^n （n为自然数）
        /// </summary>
        public int HashListCapacity => _hashList.Capacity;
        public int Count { get; protected set; }
        #endregion

        #region Properties
        public T[] ItemArray
        {
            get
            {
                var result = new ArrayList_SCCL<T>(Count);
                foreach (var linkedList in _hashList)
                {
                    result.Add(linkedList);
                }

                return result.ItemArray;
            }
        }

        public bool EnableReferenceEquals { get; init; }
        #endregion

        #region Constructors
        /// <summary>
        /// 默认构造函数，构造一个 Capacity 为 8 的空 HashSet
        /// </summary>
        /// <param name="enableReferenceEquals">是否使用 <see cref="object.ReferenceEquals(object?, object?)"/> 进行比较</param>
        public HashSet_SCCL(bool enableReferenceEquals = false)
        {
            EnableReferenceEquals = enableReferenceEquals;

            _hashList = new ArrayList_SCCL<LinkedList_SCCL<T>>(_DEFAULT_HASHLIST_CAPACITY)
            {
                new LinkedList_SCCL<T>[_DEFAULT_HASHLIST_CAPACITY]
            };
        }

        /// <summary>
        /// 构造一个空 HashSet，然后把 collection 中所有元素添加到 HashSet 中
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="enableReferenceEquals">是否使用 <see cref="object.ReferenceEquals(object?, object?)"/> 进行比较</param>
        public HashSet_SCCL(IEnumerable<T> collection, bool enableReferenceEquals = false) : this(enableReferenceEquals)
        {
            Add(collection);
        }
        #endregion

        #region Operator
        public static HashSet_SCCL<T> operator +(HashSet_SCCL<T> lhp, IEnumerable<T> rhp)
        {
            var result = lhp.MemberwiseClone() as HashSet_SCCL<T>;
            result.UnionWith(rhp);
            return result;
        }

        public static HashSet_SCCL<T> operator -(HashSet_SCCL<T> lhp, IEnumerable<T> rhp)
        {
            var result = lhp.MemberwiseClone() as HashSet_SCCL<T>;
            result.ExceptWith(rhp);
            return result;
        }
        #endregion

        #region Methods
        public bool Add(T item)
        {
            CheckCapacity();

            var linkedList = (_hashList[Hash(item)] ??= new LinkedList_SCCL<T>());

            if (linkedList.Contains(item, EnableReferenceEquals))
            {
                return false;
            }

            linkedList.Add(item);
            Count++;
            return true;
        }

        public void Add(IEnumerable<T> collection)
        {
            CheckCapacity(collection.Count());

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        void ICollection_SCCL<T>.Add(T item) => Add(item);

        public bool Remove(T item, bool enableReferenceEquals = false)
        {
            var linkedlist = _hashList[Hash(item)];

            if (!linkedlist?.Contains(item, EnableReferenceEquals) ?? true)
            {
                return false;
            }

            linkedlist.Remove(item);
            if (linkedlist.Count == 0)
            {
                _hashList[Hash(item)] = null;
            }

            Count--;

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

        public bool Contains(T item, bool enableReferenceEquals = false)
        {
            var linkedList = _hashList[Hash(item)];

            if (linkedList?.Contains(item, EnableReferenceEquals) ?? false)
            {
                return true;
            }

            return false;
        }

        public bool IsEmpty() => Count == 0;

        public void ExceptWith(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Remove(item, EnableReferenceEquals);
            }
        }

        public void IntersectWith(IEnumerable<T> collection)
        {
            var list = new ArrayList_SCCL<T>(collection);
            foreach (var item in this)
            {
                if (!list.Contains(item, EnableReferenceEquals))
                {
                    Remove(item, EnableReferenceEquals);
                }
            }
        }

        public void UnionWith(IEnumerable<T> collection) => Add(collection);

        public void SymmetricExceptWith(IEnumerable<T> collection)
        {
            UnionWith(collection);

            var tempSet = MemberwiseClone() as ISet_SCCL<T>;
            tempSet.IntersectWith(collection);

            ExceptWith(tempSet);
        }

        public bool IsSubsetOf(IEnumerable<T> collection)
        {
            var colSet = collection as ICollection_SCCL<T>;
            colSet ??= new HashSet_SCCL<T>(collection, EnableReferenceEquals);

            foreach (var item in this)
            {
                /* 
                 * Warning:
                 * 下面的 Contains 会根据 collection 的类型决定比较方法，在 collection 原本就是 ISet_SCCL 时可能会造成比较迷惑的结果 qwq
                 * （因为 ISet_SCCL 无视 enableReferenceEquals 参数，所以有可能会有 this 和 collection 比较方式不同的情况）
                 */
                if (!colSet.Contains(item, EnableReferenceEquals))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsSupersetOf(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                if (!Contains(item, EnableReferenceEquals))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsProperSupersetOf(IEnumerable<T> collection)
        {
            var colSet = collection as ISet_SCCL<T>;
            colSet ??= new HashSet_SCCL<T>(collection);

            if (Count == colSet.Count)
            {
                return false;
            }

            return IsSupersetOf(colSet);
        }

        public bool IsProperSubsetOf(IEnumerable<T> collection)
        {
            var colSet = collection as ISet_SCCL<T>;
            colSet ??= new HashSet_SCCL<T>(collection);

            if (Count == colSet.Count)
            {
                return false;
            }

            return IsSubsetOf(colSet);
        }

        public bool Overlaps(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                if (Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        public override string ToString() => $"[ {String.Join(", ", this)} ]";

        /// <summary>
        /// 哈希函数
        /// </summary>
        /// <param name="item"></param>
        /// <returns>item 的哈希值。若 item 为 null，返回 0</returns>
        private int Hash(T item) => (item?.GetHashCode() ?? 0) % HashListCapacity;

        /// <summary>
        /// 将 HashListCapacity 扩大为原来的两倍
        /// </summary>
        private void EnlargeCapacity()
        {
            var originalCapacity = HashListCapacity;
            _hashList.Capacity *= 2;

            _hashList.Add(new LinkedList_SCCL<T>[originalCapacity]);

            for (int i = 0; i < originalCapacity; i++)
            {
                var linkedList = _hashList[i];
                if (linkedList == null)
                {
                    continue;
                }

                foreach (var item in linkedList)
                {
                    var newHash = Hash(item);
                    if (newHash == i)
                    {
                        continue;
                    }

                    linkedList.Remove(item);

                    var newPosLinkedList = _hashList[newHash] ??= new LinkedList_SCCL<T>();
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
        #endregion

        public class Enumerator : IEnumerator<T>
        {
            private HashSet_SCCL<T> _hashSet;
            private HashSet_SCCL<T> _local_HashSet;
            private IEnumerator<LinkedList_SCCL<T>> _hashList_Enumerator;
            private IEnumerator<T> _linkedList_Enumerator;

            public Enumerator(HashSet_SCCL<T> hashSet)
            {
                _hashSet = hashSet;

                Reset();
            }

            public T Current => _linkedList_Enumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_linkedList_Enumerator?.MoveNext() ?? false)
                {
                    // 当前 _linkedList 不为空且没有遍历到终点
                    return true;
                }
                else if (_hashList_Enumerator.MoveNext())
                {
                    // 当前 _linkedList 遍历到终点或为空，但 _hashList 没有遍历到终点
                    while ((_linkedList_Enumerator = _hashList_Enumerator.Current?.GetEnumerator()) == null)
                    {
                        // 一直将 _hashList 向后遍历直到找到不为空的 _linkedList
                        if (!_hashList_Enumerator.MoveNext())
                        {
                            // 如果遍历完 _hashList 仍找不到，返回 false
                            return false;
                        }
                    }

                    // 找到了 _linkedList（由于 _linkedList 中至少有一个元素，所以可以直接 MoveNext()）
                    _linkedList_Enumerator.MoveNext();
                    return true;
                }
                else
                {
                    // _当前 _linkedList 和 _hashList 都遍历到终点
                    return false;
                }
            }

            public void Reset()
            {
                _local_HashSet = _hashSet;

                _hashList_Enumerator = _local_HashSet._hashList.GetEnumerator();
                _linkedList_Enumerator = null;
            }
        }
    }
}

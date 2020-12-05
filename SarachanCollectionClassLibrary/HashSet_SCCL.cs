using System;
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
        private readonly HashMap_SCCL<T, object> _hashMap;
        #endregion

        #region Properties
        public T[] Items
        {
            get
            {
                var result = new T[Count];
                int i = 0;
                foreach (var item in _hashMap)
                {
                    result[i++] = item.Key;
                }

                return result;
            }
        }

        public int Count => _hashMap.Count;

        public IEqualityComparer<T> EqualityComparer { get; init; } // 貌似没啥用 qwq

        /// <summary>
        /// _hashMap 的容量，其大小一定为 8 * 2^n （n为自然数）
        /// </summary>
        public int HashListCapacity => _hashMap.HashListCapacity;
        #endregion

        #region Constructors
        /// <summary>
        /// 默认构造函数，构造空 HashSet
        /// </summary>
        public HashSet_SCCL() : this(null) { }

        /// <summary>
        /// 构造一个比较器为 comparer 的空 HashSet
        /// </summary>
        /// <param name="comparer">比较器，默认使用<see cref="object.Equals(object?, object?)"/>进行比较</param>
        public HashSet_SCCL(IEqualityComparer<T> comparer)
        {
            _hashMap = new(comparer);
        }

        /// <summary>
        /// 构造一个空 HashSet，然后把 collection 中所有元素添加到 HashSet 中
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="comparer">比较器，默认使用<see cref="object.Equals(object?, object?)"/>进行比较</param>
        public HashSet_SCCL(IEnumerable<T> collection, IEqualityComparer<T> comparer = null) : this(comparer)
        {
            Add(collection);
        }
        #endregion

        #region Operators
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
        public bool Add(T item) => _hashMap.Add(item, default);

        public void Add(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public bool Remove(T item) => _hashMap.Remove(item);

        public void Clear() => _hashMap.Clear();

        public bool Contains(T item) => _hashMap.ContainsKey(item);

        public bool IsEmpty() => _hashMap.IsEmpty();

        public void ExceptWith(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Remove(item);
            }
        }

        public void IntersectWith(IEnumerable<T> collection)
        {
            var list = new ArrayList_SCCL<T>(collection);
            foreach (var item in this)
            {
                if (!list.Contains(item, EqualityComparer))
                {
                    Remove(item);
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
            colSet ??= new HashSet_SCCL<T>(collection, EqualityComparer);

            foreach (var item in this)
            {
                if (!colSet.Contains(item, EqualityComparer))
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
                if (!Contains(item))
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

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _hashMap)
            {
                yield return item.Key;
            }
        }

        public override string ToString() => $"[ {String.Join(", ", this)} ]";
        #endregion
    }
}

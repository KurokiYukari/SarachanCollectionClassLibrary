using System;
using System.Collections;
using System.Collections.Generic;

namespace Sarachan.Collections
{
    /// <summary>
    /// 可变长数组 ArrayList 的实现。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayList_SCCL<T> : IList_SCCL<T>, IStack_SCCL<T>, IQueue_SCCL<T>
    {
        #region Field
        private const int _DEFAULT_CAPACITY = 4; // ArrayList 的默认 Capacity

        private T[] _itemArray; // 存储 item 的数组
        #endregion

        #region Properties
        /// <summary>
        /// ArrayList 当前的容量
        /// </summary>
        public int Capacity
        {
            get => _itemArray.Length;
            set
            {
                if (value < Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"Capacity can't be less than Count: {Count}.");
                }

                if (value == 0)
                {
                    Clear();
                    return;
                }

                if (value == Capacity)
                {
                    return;
                }

                var tempArr = new T[value];
                Array.Copy(_itemArray, tempArr, Count);
                _itemArray = tempArr;
            }
        }

        public int Count { get; protected set; }

        public T[] ItemArray
        {
            get
            {
                var resultArr = new T[Count];
                for (int i = 0; i < Count; i++)
                {
                    resultArr[i] = _itemArray[i];
                }

                return resultArr;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 默认构造函数。
        /// 构造一个空 ArrayList，将其 Capacity 设置为默认值
        /// </summary>
        public ArrayList_SCCL() : this(_DEFAULT_CAPACITY) { }

        /// <summary>
        /// 构造一个空 ArrayList，将其 Capacity 设置为指定 capacity
        /// </summary>
        /// <param name="capacity"></param>
        public ArrayList_SCCL(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity can't be a negative number.");
            }

            _itemArray = new T[capacity];
        }

        /// <summary>
        /// 构造一个 ArrayList，并将指定 collection 中所有元素拷贝到该 ArrayList 中
        /// </summary>
        /// <param name="collection"></param>
        public ArrayList_SCCL(IEnumerable<T> collection) : this()
        {
            Add(collection);
        }
        #endregion

        #region Indexers
        public T this[int index] 
        { 
            get
            {
                if (!this.IsIndexLegal(index))
                {
                    throw new IndexOutOfRangeException($"Index: {index} out of range. Item Count: {Count}.");
                }

                return _itemArray[index];
            }

            set
            {
                if (!this.IsIndexLegal(index))
                {
                    throw new IndexOutOfRangeException($"Index: {index} out of range. Item Count: {Count}.");
                }

                _itemArray[index] = value;
            }
        }

        public T[] this[int beginIndex, int endIndex, int step = 1]
        {
            get
            {
                if (!this.IsIndexLegal(beginIndex))
                {
                    throw new IndexOutOfRangeException($"Param beginIndex: {beginIndex} out of range. Item Count: {Count}.");
                }
                if (!this.IsIndexLegal(endIndex))
                {
                    throw new IndexOutOfRangeException($"Param endIndex: {endIndex} out of range. Item Count: {Count}.");
                }

                if (step == 0)
                {
                    return new[] { _itemArray[beginIndex] };
                }

                var result_ArrayList = new ArrayList_SCCL<T>();

                if (step > 0)
                {
                    if (beginIndex > endIndex)
                    {
                        throw new InvalidOperationException("When step > 0, beginIndex must be less than endIndex");
                    }

                    for (int i = beginIndex; i <= endIndex; i += step)
                    {
                        result_ArrayList.Add(_itemArray[i]);
                    }
                }
                else
                {
                    if (beginIndex < endIndex)
                    {
                        throw new InvalidOperationException("When step < 0, beginIndex must be greater than endIndex");
                    }

                    for (int i = beginIndex; i >= endIndex; i += step)
                    {
                        result_ArrayList.Add(_itemArray[i]);
                    }
                }

                return result_ArrayList.ItemArray;
            }
        }
        #endregion

        #region Operator
        public static ArrayList_SCCL<T> operator +(ArrayList_SCCL<T> list, IEnumerable<T> collection)
        {
            var result = (ArrayList_SCCL<T>)list?.MemberwiseClone() ?? new ArrayList_SCCL<T>();
            result.Add(collection);

            return result;
        }
        #endregion

        #region Methods
        public void Add(T item)
        {
            CheckCapacity();

            _itemArray[Count++] = item;
        }

        public void Add(IEnumerable<T> collection)
        {
            collection ??= Array.Empty<T>();
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            _itemArray = Array.Empty<T>();
            Count = 0;
        }

        public bool Contains(T item, IEqualityComparer<T> comparer = null)
        {
            return IndexOf(item, comparer) != -1;
        }

        public bool IsEmpty() => Count == 0;

        public int IndexOf(T item, IEqualityComparer<T> comparer = null)
        {
            for (int i = 0; i < Count; i++)
            {
                var element = this[i];

                if (comparer.Equals(item, element))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(T item, int index)
        {
            if (!this.IsIndexLegal(index, 1))
            {
                throw new IndexOutOfRangeException($"Index: {index} out of range. Item Count: {Count}.");
            }

            CheckCapacity();

            for (int i = Count - 1; i >= index; i--)
            {
                _itemArray[i + 1] = _itemArray[i];
            }
            _itemArray[index] = item;

            Count++;
        }

        public void Insert(IEnumerable<T> collection, int index)
        {
            if (!this.IsIndexLegal(index, 1))
            {
                throw new IndexOutOfRangeException($"Index: {index} out of range. Item Count: {Count}.");
            }

            if (collection == null)
            {
                return;
            }

            var itemList = new ArrayList_SCCL<T>(collection);

            CheckCapacity(itemList.Count);

            for (int i = Count -1; i >= index; i--)
            {
                _itemArray[i + itemList.Count] = _itemArray[i];
            }

            int itemIndex = index;
            foreach (var item in itemList)
            {
                _itemArray[itemIndex] = item;
                itemIndex++;
            }

            Count += itemList.Count;
        }

        public bool Remove(T item, IEqualityComparer<T> comparer = null)
        {
            int itemIndex = IndexOf(item, comparer);
            if (itemIndex < 0)
            {
                return false;
            }

            RemoveAt(itemIndex);

            return true;
        }

        public void RemoveAt(int index)
        {
            if (!this.IsIndexLegal(index))
            {
                throw new IndexOutOfRangeException($"Index: {index} out of range. Item Count: {Count}.");
            }

            for (int i = index; i < Count - 1; i++)
            {
                _itemArray[i] = _itemArray[i + 1];
            }

            Count--;
        }

        public void PushFront(T item) => Insert(item, 0);

        public void PushBack(T item) => Add(item);

        public T PopFront()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Pop() can't be called while Stack is empty.");
            }

            var result = this[0];
            RemoveAt(0);
            return result;
        }

        public T PopBack() =>
            IsEmpty() ? throw new InvalidOperationException("Pop can't be called while Stack is empty.") : this[Count--];

        public T PeekFront() =>
            IsEmpty() ? throw new InvalidOperationException("Peek can't be called while Stack is empty.") : this[0];

        public T PeekBack() =>
            IsEmpty() ? throw new InvalidOperationException("Peek can't be called while Stack is empty.") : this[Count - 1];

        /// <summary>
        /// 根据当前的 Count 更改 Capacity
        /// </summary>
        /// <param name="hasRedudency">是否需要冗余空间。若为 true Capacity 将变为 Count 的 1.2 倍。</param>
        public void Trim(bool hasRedudency = false)
        {
            int neededCapacity = hasRedudency ? (int)Math.Ceiling(1.2 * Count) : Count;
            Capacity = neededCapacity;
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        public override string ToString() => $"[ {string.Join(", ", this)} ]";

        /// <summary>
        /// 检测在增加 increment 数目个 item 后当前 Capacity 是否足够使用。若否，会进行扩容
        /// </summary>
        /// <param name="increment">要增加到 ArrayList 中的 item 的数目</param>
        private void CheckCapacity(int increment = 1)
        {
            int neededCapacity = Count + increment;

            if (Capacity == 0)
            {
                _itemArray = neededCapacity < 4 ? new T[_DEFAULT_CAPACITY] : new T[(int)Math.Ceiling(1.2 * neededCapacity)];

                return;
            }

            if (Capacity < neededCapacity)
            {
                if (Capacity * 2 > neededCapacity)
                {
                    Capacity *= 2;
                } 
                else
                {
                    Capacity = (int)Math.Ceiling(1.2 * neededCapacity);
                }
            }
        }
        #endregion

        public sealed class Enumerator : IEnumerator<T>
        {
            private readonly ArrayList_SCCL<T> _arrayList;
            private ArrayList_SCCL<T> _local_ArrayList;
            private int _currentIndex;

            public T Current
            {
                get
                {
                    if (_currentIndex == -1 || _currentIndex == _local_ArrayList.Count)
                    {
                        return default;
                    }

                    return _local_ArrayList[_currentIndex];
                }
            }

            object IEnumerator.Current => Current;

            internal Enumerator(ArrayList_SCCL<T> arrayList)
            {
                _arrayList = arrayList;

                Reset();
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_currentIndex >= _local_ArrayList.Count - 1)
                {
                    return false;
                }

                _currentIndex++;
                return true;
            }

            public void Reset()
            {
                _local_ArrayList = (ArrayList_SCCL<T>)_arrayList.MemberwiseClone();
                _currentIndex = -1;
            }
        }
    }
}

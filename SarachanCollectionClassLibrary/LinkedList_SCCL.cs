using System;
using System.Collections;
using System.Collections.Generic;

namespace Sarachan.Collections
{
    /// <summary>
    /// 双向链表 LinkedList 的实现。
    /// 该类采用了数组缓存来加强随机存取的效率，但这也导致了另外一些操作的效率降低。
    /// 本质上是一种 双向链表 和 数组 结合的容器类。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedList_SCCL<T> : IList_SCCL<T>, IStack_SCCL<T>, IQueue_SCCL<T>
    {
        #region Fields
        // 头尾节点都是辅助节点，不存储实际数据
        protected readonly Node _headNode; // 头节点
        protected readonly Node _endNode; // 尾节点

        // 数组缓存。对 LinkedList 的增、删操作都会导致缓存失效（置为空）。
        // 该 field 应该只能通过 property NodeListCache 的 get 获取，不应被直接使用。
        private Node[] _nodeListCache = null; 
        #endregion

        #region Constructors
        /// <summary>
        /// 默认构造函数，构造一个空 LinkedList
        /// </summary>
        public LinkedList_SCCL()
        {
            _headNode = new Node(default, null, null);
            _endNode = new Node(default, _headNode, null);
            _headNode.NextNode = _endNode;
        }

        /// <summary>
        /// 构造一个 LinkedList，并将指定 collection 中的所有元素拷贝到该 LinkedList 中
        /// </summary>
        /// <param name="collection"></param>
        public LinkedList_SCCL(IEnumerable<T> collection) : this()
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

                return NodeListCache[index].Val;
            }
            set
            {
                if (!this.IsIndexLegal(index))
                {
                    throw new IndexOutOfRangeException($"Index: {index} out of range. Item Count: {Count}.");
                }

                NodeListCache[index].Val = value;
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
                    return new[] { NodeListCache[beginIndex].Val };
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
                        result_ArrayList.Add(NodeListCache[i].Val);
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
                        result_ArrayList.Add(NodeListCache[i].Val);
                    }
                }

                return result_ArrayList.Items;
            }
        }
        #endregion

        #region Properties
        public int Count { get; protected set; }

        public T[] Items
        {
            get
            {
                var result = new T[Count];
                for (int i = 0; i < Count; i++)
                {
                    result[i] = NodeListCache[i].Val;
                }

                return result;
            }
        }

        /// <summary>
        /// 该 LinkedList 中所有 Node 组成的数组
        /// </summary>
        protected Node[] NodeListCache
        {
            get
            {
                if (_nodeListCache != null)
                {
                    return _nodeListCache;
                }

                _nodeListCache = new Node[Count];
                var node = _headNode;
                for (int i = 0; i < Count; i++)
                {
                    node = node.NextNode;
                    _nodeListCache[i] = node;
                }

                return _nodeListCache;
            }
        }
        #endregion

        #region operator
        public static LinkedList_SCCL<T> operator +(LinkedList_SCCL<T> list, IEnumerable<T> collection)
        {
            var result = (LinkedList_SCCL<T>)list?.MemberwiseClone() ?? new LinkedList_SCCL<T>();
            result.Add(collection);

            return result;
        }
        #endregion

        #region Methods
        public void Add(T item)
        {
            var node = new Node(item, _endNode.PreNode, _endNode);
            node.PreNode.NextNode = node;
            node.NextNode.PreNode = node;
            Count++;
            ClearNodeCache();
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
            Count = 0;
            _headNode.NextNode = _endNode;
            _endNode.PreNode = _headNode;

            ClearNodeCache();
        }

        public bool Contains(T item, IEqualityComparer<T> comparer = null)
        {
            return IndexOf(item, comparer) != -1;
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        public bool IsEmpty() => Count == 0;

        public int IndexOf(T item, IEqualityComparer<T> comparer = null)
        {
            var node = _headNode;
            for (int i = 0; i < Count; i++)
            {
                node = node.NextNode;
                var element = node.Val;

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

            if (Count == 0 || Count == index)
            {
                Add(item);
                return;
            }

            var node = new Node(item, NodeListCache[index].PreNode, NodeListCache[index]);
            node.PreNode.NextNode = node;
            node.NextNode.PreNode = node;
            Count++;

            ClearNodeCache();
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

            if (Count == 0 || Count == index)
            {
                Add(collection);
                return;
            }

            int colCount = 0;
            var fakeHeadNode = new Node(default, null, null);
            var tailNode = fakeHeadNode;
            foreach (var item in collection)
            {
                colCount++;

                var newNode = new Node(item, tailNode, null);
                tailNode.NextNode = newNode;
                tailNode = tailNode.NextNode;
            }

            fakeHeadNode.NextNode.PreNode = NodeListCache[index].PreNode;
            NodeListCache[index].PreNode.NextNode = fakeHeadNode.NextNode;
            tailNode.NextNode = NodeListCache[index];
            NodeListCache[index].PreNode = tailNode;
            Count += colCount;

            ClearNodeCache();
        }

        public bool Remove(T item, IEqualityComparer<T> comparer = null)
        {
            int index = IndexOf(item, comparer);

            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (!this.IsIndexLegal(index))
            {
                throw new IndexOutOfRangeException($"Index: {index} out of range. Item Count: {Count}.");
            }

            NodeListCache[index].PreNode.NextNode = NodeListCache[index].NextNode;
            NodeListCache[index].NextNode.PreNode = NodeListCache[index].PreNode;
            Count--;

            ClearNodeCache();
        }

        public void PushFront(T item)
        {
            var node = new Node(item, _headNode, _headNode.NextNode);
            _headNode.NextNode = node;
            node.NextNode.PreNode = node;

            Count++;
            ClearNodeCache();
        }

        public void PushBack(T item) => Add(item);

        public T PopFront()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("DeQueue() can't be called while Queue is empty.");
            }

            var node = _headNode.NextNode;
            _headNode.NextNode = node.NextNode;
            node.NextNode.PreNode = _headNode;

            Count--;
            ClearNodeCache();

            return node.Val;
        }

        public T PopBack()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("DeQueue() can't be called while Queue is empty.");
            }

            var node = _endNode.PreNode;
            _endNode.PreNode = node.PreNode;
            node.NextNode = _endNode;

            Count--;
            // 这里可以不需要 ClearNodeCache()
            // 因为与 Cache 有关的操作都是通过 index 进行的，而 Count 保证了 index 不会越界

            return node.Val;
        }

        public T PeekFront() =>
            IsEmpty() ? throw new InvalidOperationException("DeQueue() can't be called while Queue is empty.") : _headNode.NextNode.Val;

        public T PeekBack() =>
            IsEmpty() ? throw new InvalidOperationException("DeQueue() can't be called while Queue is empty.") : _endNode.PreNode.Val;

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        public override string ToString() => $"[ {string.Join(", ", this)} ]";

        /// <summary>
        /// 应该在每次有增、删操作时调用，清除当前的 NodeList 缓存
        /// </summary>
        protected void ClearNodeCache()
        {
            _nodeListCache = null;
        }
        #endregion

        /// <summary>
        /// 链表的节点类型
        /// </summary>
        protected class Node
        {
            public T Val { get; set; }
            public Node NextNode { get; set; }
            public Node PreNode { get; set; }

            public Node(T val, Node preNode, Node nextNode)
            {
                Val = val;
                PreNode = preNode;
                NextNode = nextNode;
            }
        }

        public sealed class Enumerator : IEnumerator<T>
        {
            private readonly LinkedList_SCCL<T> _linkedListSccl;
            private LinkedList_SCCL<T> _local_LinkedListSccl;
            private Node _currentNode;

            internal Enumerator(LinkedList_SCCL<T> linkedListSccl)
            {
                _linkedListSccl = linkedListSccl;

                Reset();
            }

            public T Current => _currentNode.Val;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_currentNode.NextNode == _local_LinkedListSccl._endNode)
                {
                    return false;
                }

                _currentNode = _currentNode.NextNode;

                return true;
            }

            public void Reset()
            {
                _local_LinkedListSccl = _linkedListSccl;
                _currentNode = _local_LinkedListSccl._headNode;
            }
        }
    }
}

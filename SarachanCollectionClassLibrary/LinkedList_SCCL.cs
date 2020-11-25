using System;
using System.Collections;
using System.Collections.Generic;

namespace SarachanCollectionClassLibrary
{
    /// <summary>
    /// 双向链表 LinkedList 的实现。
    /// 该类采用了数组缓存来加强随机存取的效率，但这也导致了另外一些操作的效率降低。
    /// 本质上是一种 双向链表 和 数组 结合的容器类。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedList_SCCL<T> : IList_SCCL<T>
    {
        #region Fields
        // 头尾节点都是辅助节点，不存储实际数据
        private readonly Node _headNode; // 头节点
        private readonly Node _endNode; // 尾节点

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
                if (!IList_SCCL<T>.IsIndexLegal(this, index))
                {
                    throw new IndexOutOfRangeException($"Index: {index} out of range. Item Count: {Count}.");
                }

                return NodeListCache[index].Val;
            }
            set
            {
                if (!IList_SCCL<T>.IsIndexLegal(this, index))
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
                if (!IList_SCCL<T>.IsIndexLegal(this, beginIndex))
                {
                    throw new IndexOutOfRangeException($"Param beginIndex: {beginIndex} out of range. Item Count: {Count}.");
                }
                if (!IList_SCCL<T>.IsIndexLegal(this, endIndex))
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

                return result_ArrayList.ItemArray;
            }
        }

        T ICollection_SCCL<T>.this[int index] => this[index];
        #endregion

        #region Properties
        public int Count { get; protected set; }

        public T[] ItemArray
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

        #region Methods
        public void Add(T item)
        {
            var node = new Node(item, _endNode.PreNode, _endNode);
            node.PreNode.NextNode = node;
            node.NextNode.PreNode = node;
            Count++;

            _nodeListCache = null;
        }

        public void Add(IEnumerable<T> collection)
        {
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

            _nodeListCache = null;
        }

        public bool Contains(T item, bool enableReferenceEquals = false)
        {
            return IndexOf(item, enableReferenceEquals) != -1;
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        public int IndexOf(T item, bool enableReferenceEquals = false)
        {
            var node = _headNode;
            for (int i = 0; i < Count; i++)
            {
                node = node.NextNode;
                var element = node.Val;

                if (enableReferenceEquals)
                {
                    if (ReferenceEquals(element, item))
                    {
                        return i;
                    }
                }
                else
                {
                    if (Equals(element, item))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public void Insert(T item, int index)
        {
            if (!IList_SCCL<T>.IsIndexLegal(this, index, 1))
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

            _nodeListCache = null;
        }

        public void Insert(IEnumerable<T> collection, int index)
        {
            if (!IList_SCCL<T>.IsIndexLegal(this, index, 1))
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

            _nodeListCache = null;
        }

        public bool Remove(T item, bool enableReferenceEquals = false)
        {
            int index = IndexOf(item, enableReferenceEquals);

            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (!IList_SCCL<T>.IsIndexLegal(this, index))
            {
                throw new IndexOutOfRangeException($"Index: {index} out of range. Item Count: {Count}.");
            }

            NodeListCache[index].PreNode.NextNode = NodeListCache[index].NextNode;
            NodeListCache[index].NextNode.PreNode = NodeListCache[index].PreNode;
            Count--;

            _nodeListCache = null;
        }

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        public override string ToString() => "[ " + string.Join(", ", this) + " ]";
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

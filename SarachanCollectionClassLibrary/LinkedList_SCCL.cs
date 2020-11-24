using System;
using System.Collections;
using System.Collections.Generic;

namespace SarachanCollectionClassLibrary
{
    public class LinkedList_SCCL<T> : IList_SCCL<T>
    {
        private readonly LinkedListNode _headNode;
        private readonly LinkedListNode _endNode;

        public LinkedList_SCCL()
        {
            _headNode = new LinkedListNode(default, null, null);
            _endNode = new LinkedListNode(default, _headNode, null);
            _headNode.NextNode = _endNode;
        }

        public LinkedList_SCCL(IEnumerable<T> collection) : this()
        {
            Add(collection);
        }

        private LinkedListNode[] _nodeListCache = null;

        private LinkedListNode[] NodeListCache
        {
            get
            {
                if (_nodeListCache != null)
                {
                    return _nodeListCache;
                }

                _nodeListCache = new LinkedListNode[Count];
                var node = _headNode;
                for (int i = 0; i < Count; i++)
                {
                    node = node.NextNode;
                    _nodeListCache[i] = node;
                }

                return _nodeListCache;
            }
        }

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

        public void Add(T item)
        {
            var node = new LinkedListNode(item, _endNode.PreNode, _endNode);
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

            if (Count == 0)
            {
                Add(item);
                return;
            }

            var node = new LinkedListNode(item, NodeListCache[index].PreNode, NodeListCache[index]);
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

            if (Count == 0)
            {
                Add(collection);
                return;
            }

            int colCount = 0;
            var fakeHeadNode = new LinkedListNode(default, null, null);
            var tailNode = fakeHeadNode;
            foreach (var item in collection)
            {
                colCount++;

                var newNode = new LinkedListNode(item, tailNode, null);
                tailNode.NextNode = newNode;
                tailNode = tailNode.NextNode;
            }

            fakeHeadNode.NextNode.PreNode = NodeListCache[index].PreNode;
            NodeListCache[index].PreNode.NextNode = fakeHeadNode.NextNode.PreNode;
            tailNode.NextNode = NodeListCache[index];
            NodeListCache[index].PreNode = tailNode;
            Count += colCount;

            _nodeListCache = null;
        }

        public bool Remove(T item, bool enableReferenceEquals = false)
        {
            throw new NotImplementedException();
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

        private class LinkedListNode
        {
            public T Val { get; set; }
            public LinkedListNode NextNode { get; set; }
            public LinkedListNode PreNode { get; set; }

            public LinkedListNode(T val, LinkedListNode preNode, LinkedListNode nextNode)
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
            private LinkedListNode _currentNode;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 有序 List，采用红黑树实现
/// </summary>
namespace Sarachan.Collections
{
    public partial class SortedList_SCCL<T> : IList_SCCL<T>, ISorted_SCCL<T>
    {
        // 左子树 < 根节点 < 右子树 的红黑树
        private RBTreeNode<T> _redBlackTree = null;

        public T this[int index] 
        {
            get
            {
                if (!this.IsIndexLegal(index))
                {
                    throw new IndexOutOfRangeException($"Index: {index} out of range. Item Count: {Count}.");
                }

                var enumerator = GetEnumerator();
                for (int i = 0; i <= index; i++)
                {
                    enumerator.MoveNext();
                }

                return enumerator.Current;
            }
        } 

        public T[] this[int beginIndex, int endIndex, int step = 1]
        {
            get
            {
                var list = new ArrayList_SCCL<T>(this);
                return list[beginIndex, endIndex, step];
            }
        }

        T IList_SCCL<T>.this[int index]
        { 
            get => this[index];
            set
            {
                if (!this.IsIndexLegal(index))
                {
                    throw new IndexOutOfRangeException($"Index: {index} out of range. Item Count: {Count}.");
                }

                // 为了保证有序，set 只能通过先删除再添加的方式实现
                Remove(this[index]);
                Add(value);
            }
        }

        public int Count { get; private set; }

        public T[] Items
        {
            get
            {
                T[] result = new T[Count];
                int i = 0;
                foreach (var item in this)
                {
                    result[i++] = item;
                }

                return result;
            }
        }

        public IComparer<T> TComparer { get; init; }

        public SortedList_SCCL() : this(null) { }

        public SortedList_SCCL(IComparer<T> comparer)
        {
            if (comparer is null)
            {
                if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
                {
                    // 如果 T 实现了 IComparable<T>，则用该接口的实现方法创建 comparer
                    TComparer = new ComparableTypeDefaultComparer();
                }
                else
                {
                    // 否则，使用默认 Comparer
                    TComparer = Comparer<T>.Default;
                }
            }
            else
            {
                TComparer = comparer;
            }
        }

        public void Add(T item)
        {
            (var parentNode, bool isLeftChild) = FindInsertPos(item, _redBlackTree);

            if (parentNode is null)
            {
                _redBlackTree = new(item) { IsRed = false };
            }
            else
            {
                // 插入红色的 newNode
                var node = new RBTreeNode<T>(item, parentNode);
                if (isLeftChild)
                {
                    parentNode.LChild = node;
                }
                else
                {
                    parentNode.RChild = node;
                }

                FixRBTree(node);
            }

            Count++;

            /// <summary>
            /// 如果 this 和 Parent 都为红，会将整棵树修复回正常的红黑树
            /// </summary>
            void FixRBTree(RBTreeNode<T> node)
            {
                if (node.Parent is null)
                {
                    node.IsRed = false;
                }
                else if (node.Parent.IsRed) // parentNode 为红时，要修正红黑树
                {
                    if (ReferenceEquals(node.Parent, node.Parent.Parent.LChild)) // parentNode 是其父的 LChild 的情况
                    {
                        var uncleNode = node.Parent.Parent.RChild;
                        if (uncleNode is null || !uncleNode.IsRed) // uncleNode 为黑（null 视作黑结点）
                        {
                            if (ReferenceEquals(node, node.Parent.LChild)) // node 是其父 LChild
                            {
                                // case 1: 左左，uncle 黑
                                node.Parent.IsRed = false; // 将 parentNode 设为黑
                                node.Parent.Parent.IsRed = true; // 将 祖父 Node 设为红
                                RightRotate(node.Parent.Parent); // 将 祖父 Node 右旋
                                return;
                            }
                            else // node 是其父的 RChild
                            {
                                var tempNode = node.Parent;
                                // case 2：左右，uncle 黑
                                LeftRotate(tempNode); // 左旋，继续循环
                                // 左旋之后会转化成 case 1，继续 Fix
                                FixRBTree(tempNode);
                                return;
                            }
                        }
                        else // uncleNode 为红
                        {
                            // case 3: 左，uncle 红
                            node.Parent.IsRed = uncleNode.IsRed = false; // 将 parentNode 和 uncleNode 设为黑
                            node.Parent.Parent.IsRed = true; // 将 祖父 Node 设为红
                            FixRBTree(node.Parent.Parent); // 祖父 Node 进行 Fix
                            return;
                        }
                    }
                    else // node 是其父的 RChild。上面操作的镜像
                    {
                        var uncleNode = node.Parent.Parent.LChild;
                        if (uncleNode is null || !uncleNode.IsRed)
                        {
                            if (ReferenceEquals(node, node.Parent.RChild))
                            {
                                // case 4: 右右，uncle 黑
                                node.Parent.IsRed = false;
                                node.Parent.Parent.IsRed = true;
                                LeftRotate(node.Parent.Parent);
                                return;
                            }
                            else
                            {
                                // case 5: 右左，uncle 黑
                                var tempNode = node.Parent;
                                RightRotate(tempNode);
                                FixRBTree(tempNode);
                                return;
                            }
                        }
                        else
                        {
                            // case 3: 右，uncle 红（uncle 为红时左右无影响）
                            node.Parent.IsRed = uncleNode.IsRed = false;
                            node.Parent.Parent.IsRed = true;
                            FixRBTree(node.Parent.Parent);
                            return;
                        }
                    }
                }
            }
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
            _redBlackTree = null;
        }

        public bool Contains(T item)
        {
            (var parentNode, _) = FindInsertPos(item, _redBlackTree);
            return parentNode is not null && TComparer.Compare(item, parentNode.Value) == 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_redBlackTree is null)
            {
                yield break;
            }

            foreach (var item in _redBlackTree.PreOrderTraversal())
            {
                yield return item.Value;
            }
        }

        public int IndexOf(T item)
        {
            int result = 0;
            foreach (var element in this)
            {
                if (TComparer.Compare(element, item) == 0)
                {
                    return result;
                }

                result++;
            }

            return -1;
        }

        int IList_SCCL<T>.IndexOf(T item, IEqualityComparer<T> comparer) => IndexOf(item);

        void IList_SCCL<T>.Insert(T item, int index) => Add(item);

        void IList_SCCL<T>.Insert(IEnumerable<T> collection, int index) => Add(collection);

        public bool IsEmpty() => Count == 0;

        public bool Remove(T item)
        {
            if (IsEmpty())
            {
                return false;
            }

            (var targetNode, _) = FindInsertPos(item, _redBlackTree);
            if (TComparer.Compare(targetNode.Value, item) != 0)
            {
                return false;
            }

            RemoveNode(targetNode);

            Count--;

            return true;

            void RemoveNode(RBTreeNode<T> node)
            {
                var replaceNode = FindReplaceNode(node);
                if (replaceNode.Parent is null) // 排除只有根结点情况
                {
                    _redBlackTree = null;
                    return;
                }

                node.Value = replaceNode.Value;

                if (replaceNode.LChild is null && replaceNode.RChild is null) // 是叶子结点，递归结束
                {
                    if (!replaceNode.IsRed)
                    {
                        BalanceIfRemove(replaceNode);
                    }

                    bool isLChild = ReferenceEquals(replaceNode, replaceNode.Parent.LChild);
                    if (isLChild)
                    {
                        replaceNode.Parent.LChild = null;
                    }
                    else
                    {
                        replaceNode.Parent.RChild = null;
                    }
                }
                else // 不是叶子结点。这种情况 replaceNode 只有一种情况：黑色、只有一个右红子结点（根已经被排除了
                {
                    replaceNode.Value = replaceNode.RChild.Value;
                    replaceNode.RChild = null;
                }
            }

            RBTreeNode<T> FindReplaceNode(RBTreeNode<T> removedNode)
            {
                if (removedNode.LChild is null && removedNode.RChild is null)
                {
                    return removedNode;
                }
                else if (removedNode.LChild is null || removedNode.RChild is null)
                {
                    return removedNode.LChild ?? removedNode.RChild;
                }
                else
                {
                    var node = removedNode.RChild;
                    while (node.LChild != null)
                    {
                        node = node.LChild;
                    }
                    return node;
                }
            }

            void BalanceIfRemove(RBTreeNode<T> node)
            {
                if (node.Parent == null) // 平衡结束
                {
                    return;
                }

                if (ReferenceEquals(node, node.Parent.LChild)) // 左子结点
                {
                    var brotherNode = node.Parent.RChild;

                    if (brotherNode.IsRed) // brotherNode 为红
                    {
                        // case 1:
                        // 这种情况下结点的颜色都是确定的：
                        // node 为黑，parent 为黑，brother 红，brother 必有两黑子结点
                        brotherNode.IsRed = false;
                        brotherNode.LChild.IsRed = true;
                        LeftRotate(node.Parent);
                        return;
                    }
                    else // brother 为黑
                    {
                        if (brotherNode.RChild is not null && brotherNode.RChild.IsRed) // brother 右子结点为红
                        {
                            // case 2:
                            brotherNode.IsRed = node.Parent.IsRed;
                            node.Parent.IsRed = false;
                            brotherNode.RChild.IsRed = false;
                            LeftRotate(node.Parent);
                            return;
                        }
                        else if (brotherNode.LChild is not null && brotherNode.LChild.IsRed) // brother 左子结点为红，右为黑
                        {
                            // case 3:
                            brotherNode.IsRed = true;
                            brotherNode.LChild.IsRed = false;
                            RightRotate(brotherNode);
                            BalanceIfRemove(node); // 下一次会转到 case 2
                            return;
                        }
                        else // brother 左右都无或左右都黑
                        {
                            if (node.Parent.IsRed) // parent 为红
                            {
                                // case 4:
                                node.Parent.IsRed = false;
                                brotherNode.IsRed = true;
                                return;
                            }
                            else // parent 为黑
                            {
                                // case 5:
                                brotherNode.IsRed = true;
                                BalanceIfRemove(node.Parent); // 向上递归
                                return;
                            }
                        }
                    }
                }
                else // 右子结点
                {
                    // 上面操作的镜像
                    var brotherNode = node.Parent.LChild;

                    if (brotherNode.IsRed)
                    {
                        brotherNode.IsRed = false;
                        brotherNode.RChild.IsRed = true;
                        RightRotate(node.Parent);
                        return;
                    }
                    else 
                    {
                        if (brotherNode.LChild is not null && brotherNode.LChild.IsRed)
                        {
                            brotherNode.IsRed = node.Parent.IsRed;
                            node.Parent.IsRed = false;
                            brotherNode.LChild.IsRed = false;
                            RightRotate(node.Parent);
                            return;
                        }
                        else if (brotherNode.RChild is not null && brotherNode.RChild.IsRed)
                        {
                            brotherNode.IsRed = true;
                            brotherNode.RChild.IsRed = false;
                            LeftRotate(brotherNode);
                            BalanceIfRemove(node);
                            return;
                        } 
                        else
                        {
                            if (node.Parent.IsRed)
                            {
                                node.Parent.IsRed = false;
                                brotherNode.IsRed = true;
                                return;
                            }
                            else 
                            {
                                brotherNode.IsRed = true;
                                BalanceIfRemove(node.Parent);
                                return;
                            }
                        }
                    }
                }
            }
        }

        bool ICollection_SCCL<T>.Remove(T item, IEqualityComparer<T> comparer) => Remove(item);

        public void RemoveAt(int index) => Remove(this[index]);

        public override string ToString()
        {
            return $"[ {string.Join(", ", this)} ]";
        }

        // TODO: 将该方法移至 RBTreeNode 中？
        /// <summary>
        /// 如果要将 item 插入到树 tree 中，找到 item 应该的父节点
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tree"></param>
        /// <returns>二元组。parentNode 为 item 应该的父节点，如果树为空则返回 null；isLeftChild 用于指示 item 是否应该是左子树</returns>
        internal (RBTreeNode<T> parentNode, bool isLeftChild) FindInsertPos(T item, RBTreeNode<T> tree)
        {
            if (tree == null)
            {
                return (null, true);
            }

            var compResult = TComparer.Compare(item, tree.Value);

            if (compResult < 0)
            {
                if (tree.LChild is null)
                {
                    return (tree, true);
                }
                else
                {
                    return FindInsertPos(item, tree.LChild);
                }
            }
            else if (compResult > 0)
            {
                if (tree.RChild is null)
                {
                    return (tree, false);
                }
                else
                {
                    return FindInsertPos(item, tree.RChild);
                }
            }
            else // compResult == 0
            {
                return (tree, true);
            }
        }

        /// <summary>
        /// 左旋
        /// </summary>
        /// <returns>新的 root（也就是原本 this 的 RChild）</returns>
        internal RBTreeNode<T> LeftRotate(RBTreeNode<T> node)
        {
            var parent = node.Parent; // 该树的父 Node，如果为 null 说明是根节点
            var oRChild = node.RChild; // 原本的右子树，如果为空无法进行左旋
            if (oRChild is null)
            {
                throw new InvalidOperationException("LeftRotate can't process while RChild is null");
            }

            // 将 this 的 RChild 设为 oRChild 的 LChild
            node.RChild = oRChild.LChild;
            if (node.RChild is not null)
            {
                node.RChild.Parent = node;
            }

            // 将 oRchild 的 LChild 设为 this
            oRChild.LChild = node;
            oRChild.LChild.Parent = oRChild;

            // 设置 oRChild 的 Parent
            oRChild.Parent = parent;

            if (parent is not null)
            {
                if (ReferenceEquals(node, parent.LChild))
                {
                    parent.LChild = oRChild;
                }
                else
                {
                    parent.RChild = oRChild;
                }
            }
            else
            {
                _redBlackTree = oRChild;
            }

            return oRChild;
        }

        /// <summary>
        /// 右旋
        /// </summary>
        /// <returns>新的 root（也就是原本 this 的 LChild）</returns>
        internal RBTreeNode<T> RightRotate(RBTreeNode<T> node)
        {
            // LeftRotate 的镜像操作
            var parent = node.Parent;
            var oLChild = node.LChild;
            if (oLChild is null)
            {
                throw new InvalidOperationException("RightRotate can't process while LChild is null");
            }

            node.LChild = oLChild.RChild;
            if (node.LChild is not null)
            {
                node.LChild.Parent = node;
            }

            oLChild.RChild = node;
            oLChild.RChild.Parent = oLChild;

            oLChild.Parent = parent;

            if (parent is not null)
            {
                if (ReferenceEquals(node, parent.LChild))
                {
                    parent.LChild = oLChild;
                }
                else
                {
                    parent.RChild = oLChild;
                }
            }
            else
            {
                _redBlackTree = oLChild;
            }

            return oLChild;
        }

        

        bool ICollection_SCCL<T>.Contains(T item, IEqualityComparer<T> comparer) => Contains(item);

        private sealed class ComparableTypeDefaultComparer : IComparer<T>
        {
            public ComparableTypeDefaultComparer()
            {
                if (!typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
                {
                    throw new NotSupportedException(
                        $"Type {nameof(ComparableTypeDefaultComparer)} initialization failed. " +
                        $"Type {typeof(T).Name} must implements interface {nameof(IComparable<T>)}");
                }
            }

            public int Compare(T x, T y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                else if (x == null)
                {
                    return -1;
                } 
                else
                {
                    return (x as IComparable<T>).CompareTo(y);
                }
            }
        }
    }
}

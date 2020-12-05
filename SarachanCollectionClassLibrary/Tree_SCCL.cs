using System;
using System.Collections.Generic;

/// <summary>
/// 有序 List，采用红黑树实现
/// </summary>
namespace Sarachan.Collections
{
    class BinaryTreeNode<T>
    {
        /// <summary>
        /// 值
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// 父 Node
        /// </summary>
        public BinaryTreeNode<T> Parent { get; set; }

        /// <summary>
        /// 左子树
        /// </summary>
        public BinaryTreeNode<T> LChild { get; set; }

        /// <summary>
        /// 右子树
        /// </summary>
        public BinaryTreeNode<T> RChild { get; set; }

        public BinaryTreeNode(T value, BinaryTreeNode<T> parent = null, BinaryTreeNode<T> lChild = null, BinaryTreeNode<T> rChild = null)
        {
            Value = value;
            Parent = parent;
            LChild = lChild;
            RChild = rChild;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        // TODO: 这样的实现方式会不会引起效率问题？
        public IEnumerable<BinaryTreeNode<T>> PreOrderTraversal()
        {
            if (LChild is not null)
            {
                foreach (var item in LChild.PreOrderTraversal())
                {
                    yield return item;
                }
            }

            yield return this;

            if (RChild is not null)
            {
                foreach (var item in RChild.PreOrderTraversal())
                {
                    yield return item;
                }
            }
        }
    }

    /// <summary>
    /// 红黑树的 Node 类型
    /// 建议用初始化列表初始化
    /// </summary>
    class RBTreeNode<T> : BinaryTreeNode<T>
    {
        public new RBTreeNode<T> Parent
        {
            get => base.Parent as RBTreeNode<T>;
            set => base.Parent = value;
        }

        public new RBTreeNode<T> LChild
        {
            get => base.LChild as RBTreeNode<T>;
            set => base.LChild = value;
        }

        public new RBTreeNode<T> RChild
        {
            get => base.RChild as RBTreeNode<T>;
            set => base.RChild = value;
        }

        public bool IsRed { get; set; }

        public RBTreeNode(T value, RBTreeNode<T> parent = null,bool isRed = true, RBTreeNode<T> lChild = null, RBTreeNode<T> rChild = null) 
            : base(value, parent, lChild, rChild)
        {
            IsRed = isRed;
        }

        public override string ToString()
        {
            string color = IsRed ? "Red" : "Black";
            return $"{base.ToString()} {color}";
        }

        public new IEnumerable<RBTreeNode<T>> PreOrderTraversal()
        {
            foreach (var item in base.PreOrderTraversal())
            {
                yield return item as RBTreeNode<T>;
            }
        }
    }
}

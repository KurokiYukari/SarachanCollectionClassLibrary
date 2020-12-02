using System;

namespace Sarachan.Collections
{
    /// <summary>
    /// Collection 接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICollection_SCCL<T> : System.Collections.Generic.IEnumerable<T>
    {
        #region Properties
        /// <summary>
        /// Collection 中已存储的 items 个数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Collection 中所有 items 组成的 Array
        /// </summary>
        T[] ItemArray { get; }
        #endregion

        #region Methods
        /// <summary>
        /// 添加 item
        /// </summary>
        /// <param name="item">要添加的 item</param>
        void Add(T item);

        /// <summary>
        /// 将指定 IEnumerable 中所有 item 添加到该 Collection 中
        /// </summary>
        /// <param name="collection">指定 IEnumerable</param>
        void Add(System.Collections.Generic.IEnumerable<T> collection);

        /// <summary>
        /// 删除 Collection 中所有指定的 item
        /// </summary>
        /// <param name="item">要删除的 item</param>
        /// <param name="comparer">比较器，默认使用<see cref="object.Equals(object?, object?)"/>进行比较</param>
        /// <returns>如果 Collection 中不存在 item，则会返回 false；否则返回 true</returns>
        bool Remove(T item, System.Collections.Generic.IEqualityComparer<T> comparer = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">指定的 item</param>
        /// <param name="comparer">比较器，默认使用<see cref="object.Equals(object?, object?)"/>进行比较</param>
        /// <returns>Collection 中是否含有指定 item</returns>
        bool Contains(T item, System.Collections.Generic.IEqualityComparer<T> comparer = null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Collection 是否为空 Collection</returns>
        bool IsEmpty();

        /// <summary>
        /// 将 Collection 中的 item 全部移除
        /// </summary>
        void Clear();
        #endregion

        #region Explicit Implementions of IEnumerable
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }

    public static partial class Algorithm_SCCL
    {
        public static int Count<T>(this System.Collections.Generic.IEnumerable<T> collection)
        {
            int result = 0;
            foreach (var _ in collection)
            {
                result++;
            }

            return result;
        }
    }
}

using System;

namespace Sarachan.Collections
{
    /// <summary>
    /// ISet 接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface ISet_SCCL<T> : ICollection_SCCL<T>
    {
        /// <summary>
        /// 只读属性，ISet 内部比较方式是否使用 <see cref="object.ReferenceEquals(object?, object?)"/>
        /// </summary>
        bool EnableReferenceEquals { get; init; }

        /// <summary>
        /// 向 ISet 中添加元素
        /// </summary>
        /// <param name="item">如果 ISet 已经存在 item，则不会添加并返回 false；否则返回 true</param>
        /// <returns></returns>
        new bool Add(T item);

        /// <summary>
        /// 删除 item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="enableReferenceEquals">无效参数</param>
        /// <returns>如果 ISet 中存在 item 则返回 true，否则返回 false</returns>
        new bool Remove(T item, bool enableReferenceEquals = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="enableReferenceEquals">无效参数</param>
        /// <returns>如果 ISet 中存在 item 则返回 true，否则返回 false</returns>
        new bool Contains(T item, bool enableReferenceEquals = false);

        /// <summary>
        /// 将该 ISet 转化为自己和 collection 的并集
        /// </summary>
        /// <param name="collection"></param>
        void UnionWith(System.Collections.Generic.IEnumerable<T> collection);

        /// <summary>
        /// 将该 ISet 转化为自己和 collection 的交集
        /// </summary>
        /// <param name="collection"></param>
        void IntersectWith(System.Collections.Generic.IEnumerable<T> collection);

        /// <summary>
        /// 将该 ISet 转化为自己和 collection 的差集
        /// </summary>
        /// <param name="collection"></param>
        void ExceptWith(System.Collections.Generic.IEnumerable<T> collection);

        string ToString();
    }
}

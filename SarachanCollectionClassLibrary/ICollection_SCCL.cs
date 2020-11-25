using System;

namespace SarachanCollectionClassLibrary
{
    /// <summary>
    /// 所有 Collection 类都要实现的最基本接口，定义了 Collection 要实现的基本功能。
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

        #region Indexers
        /// <summary>
        /// 只读访问器，指向 Collection 中指定 index 的 item
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        T this[int index] { get; }
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
        /// <param name="enableReferenceEquals">使用 <seealso cref="Object.ReferenceEquals(object?, object?)"/> 方法来确定 item 是否删除，默认为 false (使用运算符 == 判断)</param>
        /// <returns>如果 Collection 中不存在 item，则会返回 false；否则返回 true</returns>
        bool Remove(T item, bool enableReferenceEquals = false);
        // TODO: 关于 Remove 的 enableReferenceEquals 参数可能会出现比较异常的情况，详见 https://docs.microsoft.com/zh-cn/dotnet/api/system.object.referenceequals?view=netcore-3.1#--

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">指定的 item</param>
        /// <param name="enableReferenceEquals">使用 <seealso cref="Object.ReferenceEquals(object?, object?)"/> 方法来确定 item 是否存在，默认为 false (使用运算符 == 判断)</param>
        /// <returns>Collection 中是否含有指定 item</returns>
        bool Contains(T item, bool enableReferenceEquals = false);

        /// <summary>
        /// 将 Collection 中的 item 全部移除
        /// </summary>
        void Clear();
        #endregion
    }
}

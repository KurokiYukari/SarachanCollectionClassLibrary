using System;

namespace Sarachan.Collections
{
    /// <summary>
    /// ISet 接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISet_SCCL<T> : ICollection_SCCL<T>
    {
        /// <summary>
        /// 只读属性，ISet 内部比较方式是否使用 <see cref="object.ReferenceEquals(object?, object?)"/>
        /// </summary>
        bool EnableReferenceEquals { get; init; }

        public static ISet_SCCL<T> operator +(ISet_SCCL<T> lhp, System.Collections.Generic.IEnumerable<T> rhp)
        {
            ISet_SCCL<T> result;

            // 采用反射机制调用默认构造函数构造构造
            try
            {
                result = (ISet_SCCL<T>)Activator.CreateInstance(lhp?.GetType() ?? typeof(HashSet_SCCL<T>));
            }
            catch (Exception)
            {
                result = new HashSet_SCCL<T>();
            }

            result.UnionWith(rhp);
            return result;
        }

        public static ISet_SCCL<T> operator -(ISet_SCCL<T> lhp, System.Collections.Generic.IEnumerable<T> rhp)
        {
            ISet_SCCL<T> result;

            // 采用反射机制调用默认构造函数构造构造
            try
            {
                result = (ISet_SCCL<T>)Activator.CreateInstance(lhp?.GetType() ?? typeof(HashSet_SCCL<T>));
            }
            catch (Exception)
            {
                result = new HashSet_SCCL<T>();
            }

            result.ExceptWith(rhp);
            return result;
        }

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
        /// 执行 ISet = ISet ∪ collection
        /// </summary>
        /// <param name="collection"></param>
        void UnionWith(System.Collections.Generic.IEnumerable<T> collection);

        /// <summary>
        /// 执行 ISet = ISet ∩ collection
        /// </summary>
        /// <param name="collection"></param>
        void IntersectWith(System.Collections.Generic.IEnumerable<T> collection);

        /// <summary>
        /// 执行 ISet = ISet - collection
        /// </summary>
        /// <param name="collection"></param>
        void ExceptWith(System.Collections.Generic.IEnumerable<T> collection);

        /// <summary>
        /// 执行 ISet = (ISet ∪ collection) - (ISet ∩ collection)
        /// </summary>
        /// <param name="collection"></param>
        void SymmetricExceptWith(System.Collections.Generic.IEnumerable<T> collection);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>ISet 是否是 collection 的子集</returns>
        bool IsSubsetOf(System.Collections.Generic.IEnumerable<T> collection);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>ISet 是否是 collection 的超集</returns>
        bool IsSupersetOf(System.Collections.Generic.IEnumerable<T> collection);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>ISet 是否是 collection 的真子集</returns>
        bool IsProperSupersetOf(System.Collections.Generic.IEnumerable<T> collection);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>ISet 是否是 collection 的真超集</returns>
        bool IsProperSubsetOf(System.Collections.Generic.IEnumerable<T> collection);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>ISet ∩ collection 是否为空集</returns>
        bool Overlaps(System.Collections.Generic.IEnumerable<T> collection);

        string ToString();
    }
}

using System;

namespace Sarachan.Collections
{
    public interface IMap_SCCL<TKey, TValue> : ICollection_SCCL<KeyValuePair_SCCL<TKey, TValue>>
    {
        #region Properties
        /// <summary>
        /// TKey 的比较器
        /// </summary>
        System.Collections.Generic.IEqualityComparer<KeyValuePair_SCCL<TKey, TValue>> PairEqualityComparer { get; init; }

        /// <summary>
        /// 只读属性，所有 keys 组成的数组
        /// </summary>
        TKey[] Keys { get; }

        /// <summary>
        /// 只读属性，所有 values 组成的数组
        /// </summary>
        TValue[] Values { get; }
        #endregion

        #region Indexers
        /// <summary>
        /// 读写访问器，指向 key 所对应的 value。
        /// 如果 key 不存在，get 会得到 default(TValue)，set 会创建 key => value 的键值对
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue this[TKey key] { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>IMap 中是否存在 key</returns>
        bool ContainsKey(TKey key);

        /// <summary>
        /// 若 key 在 IMap 中不存在，则向 IMap 中添加 key => value 的键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>如果 IMap 中已经存在 key，返回 false；否则返回 true</returns>
        bool Add(TKey key, TValue value);

        /// <summary>
        /// 若 item.Key 在 IMap 中不存在，则向 IMap 中添加 item
        /// </summary>
        /// <param name="item"></param>
        /// <returns>如果 IMap 中已经存在 item.Key，返回 false；否则返回 true</returns>
        new bool Add(KeyValuePair_SCCL<TKey, TValue> item);

        /// <summary>
        /// 若 key 在 IMap 中存在，删除 key 对应的 KeyValuePair
        /// </summary>
        /// <param name="key"></param>
        /// <returns>若 IMap 中不存在 key，返回 false；否则返回 true</returns>
        bool Remove(TKey key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>key 对应的 Value</returns>
        /// <exception cref="ArgumentException">当 IMap 中不存在 key 时抛出</exception>
        TValue Get(TKey key);

        /// <summary>
        /// 若 key 在 IMap 中存在，将 key 对应的值设为 value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>若 IMap 中不存在 key，返回 false；否则返回 true</returns>
        bool Set(TKey key, TValue value);

        string ToString();
        #endregion

        #region Explicit Implementations of ICollection_SCCL
        void ICollection_SCCL<KeyValuePair_SCCL<TKey, TValue>>.Add(KeyValuePair_SCCL<TKey, TValue> item) => Add(item);

        void ICollection_SCCL<KeyValuePair_SCCL<TKey, TValue>>.Add(System.Collections.Generic.IEnumerable<KeyValuePair_SCCL<TKey, TValue>> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }
        #endregion
    }

    /// <summary>
    /// 键值对 record
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed record KeyValuePair_SCCL<TKey, TValue>
    {
        public TKey Key { get; init; }
        public TValue Value { get; init; }

        public KeyValuePair_SCCL(TKey key, TValue value)
        {
            (Key, Value) = (key, value);
        }

        public override string ToString() => $"({Key} => {Value})";
    }
}

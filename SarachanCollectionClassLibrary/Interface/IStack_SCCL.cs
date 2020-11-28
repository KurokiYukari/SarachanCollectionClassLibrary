using System;

namespace Sarachan.Collections
{
    /// <summary>
    /// 栈接口
    /// 方法名后还是加上了 Back（本可以不需要），是为了和 IQueue 接口兼容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStack_SCCL<T> : ICollection_SCCL<T>
    {
        /// <summary>
        /// 入栈
        /// </summary>
        /// <param name="item"></param>
        public void PushBack(T item);

        /// <summary>
        /// 出栈
        /// </summary>
        /// <returns>出栈元素</returns>
        public T PopBack();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>栈顶元素</returns>
        public T PeekBack();
    }
}

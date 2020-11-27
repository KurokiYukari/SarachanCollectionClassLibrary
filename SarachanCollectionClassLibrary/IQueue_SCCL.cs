using System;

namespace Sarachan.Collections
{
    /// <summary>
    /// 队列接口
    /// EnQueue 和 DeQueue 采用了默认实现，必须通过该接口才可以访问（无法通过实现了该接口的类访问）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueue_SCCL<T> : ICollection_SCCL<T>
    {
        /// <summary>
        /// 插入元素到队列头
        /// </summary>
        /// <param name="item"></param>
        public void PushFront(T item);

        /// <summary>
        /// 插入元素到队列尾
        /// </summary>
        /// <param name="item"></param>
        public void PushBack(T item);

        /// <summary>
        /// 删除队列头元素
        /// </summary>
        /// <returns>被删除的元素</returns>
        public T PopFront();

        /// <summary>
        /// 删除队列尾元素
        /// </summary>
        /// <returns>被删除的元素</returns>
        public T PopBack();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>队列头元素</returns>
        public T PeekFront();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>队列尾元素</returns>
        public T PeekBack();

        /// <summary>
        /// 进队
        /// </summary>
        /// <param name="item"></param>
        public void EnQueue(T item) => PushFront(item);

        /// <summary>
        /// 出队
        /// </summary>
        /// <returns>队尾元素</returns>
        public T DeQueue() => PopBack();
    }
}

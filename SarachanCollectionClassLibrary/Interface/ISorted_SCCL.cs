using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarachan.Collections
{
    /// <summary>
    /// 有序容器接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISorted_SCCL<T> : ICollection_SCCL<T>
    {
        /// <summary>
        /// 决定有序容器内元素比较方式的比较器
        /// </summary>
        IComparer<T> TComparer { get; init; }

        /// <summary>
        /// 只读访问器，指向第 index 个元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; }
    }
}

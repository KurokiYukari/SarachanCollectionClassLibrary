using System;

namespace Sarachan.Collections
{
    /// <summary>
    /// List 接口。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IList_SCCL<T> : ICollection_SCCL<T>
    {
        #region Indexers
        /// <summary>
        /// 读写访问器，指向 Collection 中指定 index 的 item
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; set; }

        /// <summary>
        /// 只读访问器，指向从 beginIndex 到 endIndex，步长为 step 的所有 item 构成的数组
        /// </summary>
        /// <param name="beginIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        T[] this[int beginIndex, int endIndex, int step = 1] { get; }
        #endregion

        #region Operator
        // TODO: 尝试进行运算符重载，但存在问题：接口中的运算符重载接受参数没有友元性质？
        public static IList_SCCL<T> operator +(IList_SCCL<T> list, System.Collections.Generic.IEnumerable<T> collection)
        {
            // 下面这句 arr 无法调用 object 的 protected 函数 MemberwiseClone()
            //var result = (IList_SCCL<T>)arr.MemberwiseClone();

            IList_SCCL<T> result;

            // 采用反射机制调用默认构造函数构造构造
            try
            {
                result = (IList_SCCL<T>)Activator.CreateInstance(list?.GetType() ?? typeof(ArrayList_SCCL<T>));
            }
            catch (Exception)
            {
                result = new ArrayList_SCCL<T>();
            }
            
            result.Add(list);
            result.Add(collection);

            return result;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="comparer">比较器，默认使用<see cref="object.Equals(object?, object?)"/>进行比较</param>
        /// <returns>指定 item 的 index，如果不存在则返回 -1</returns>
        int IndexOf(T item, System.Collections.Generic.IEqualityComparer<T> comparer = null);

        /// <summary>
        /// 在指定 index 处插入指定 item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        void Insert(T item, int index);

        /// <summary>
        /// 在指定 index 处插入指定 collection 的所有元素
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="index"></param>
        void Insert(System.Collections.Generic.IEnumerable<T> collection, int index);

        /// <summary>
        /// 移除在 Collection 中指定 index 处的 item
        /// </summary>
        /// <param name="index"></param>
        void RemoveAt(int index);

        string ToString();
        #endregion
    }


    /* 
     * IList 相关算法实现。
     * 
     * TODO: 由于 C# 的一个奇怪的特性：接口方法如果在接口中实现了的话（接口默认方法），则实现了该接口的类默认是没有该方法的。
     * 举个栗子qwq：接口 IList 有已经实现的默认方法 Sort()。实现了该接口的类 ArrayList 是无法调用 Sort() 方法的，除非 ArrayList 自己重新声明并实现了 Sort()，或者将 ArrayList 强转为 IList。
     * 所以，为了达成类似接口默认方法，但实现了接口的类也可以调用的效果，这里选择用扩展方法来实现。(但这种操作总让我有种怪怪的感觉....)
     * （PS：如果有好哥哥知道 C# 为什么要设计成这样或者有什么好的方法可以解决这样的问题，希望能通过邮件等方式救救孩子qwq）
     */
    public static partial class Algorithm_SCCL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="lengthCompensation">list 的补偿大小。一般在插入时确认 index 是否合法使用，默认为 0。</param>
        /// <returns>在所给的 list 中所给的 index 是否合法</returns>
        public static bool IsIndexLegal<T>(this IList_SCCL<T> list, int index, int lengthCompensation = 0) => 
            index >= 0 && index < list.Count + lengthCompensation;

        /// <summary>
        /// 采用快速排序对 list 进行排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="comparer">比较器</param>
        /// <param name="reverse">是否是逆序</param>
        public static void Sort<T>(this IList_SCCL<T> list, System.Collections.Generic.IComparer<T> comparer = null, bool reverse = false) => 
            Sort(list, 0, list.Count - 1, comparer, reverse);

        /// <summary>
        /// 采用快速排序对 list 的指定区间 [beginIndex, endIndex] 进行排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="beginIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="comparer">比较器</param>
        /// <param name="reverse">是否是逆序</param>
        public static void Sort<T>(IList_SCCL<T> list, int beginIndex, int endIndex, System.Collections.Generic.IComparer<T> comparer = null, bool reverse = false)
        {
            if (beginIndex >= endIndex)
            {
                return;
            }

            comparer ??= System.Collections.Generic.Comparer<T>.Default;

            T tempVal = list[beginIndex];
            int tempIndex = beginIndex;

            int i = beginIndex + 1, j = endIndex;

            while (i <= j)
            {
                while (i <= j)
                {
                    int compResult = comparer.Compare(tempVal, list[j]);
                    if (reverse)
                    {
                        compResult = -compResult;
                    }

                    if (compResult > 0)
                    {
                        list[tempIndex] = list[j];
                        tempIndex = j--;
                        break;
                    }

                    j--;
                }

                while (i <= j)
                {
                    int compResult = comparer.Compare(tempVal, list[i]);
                    if (reverse)
                    {
                        compResult = -compResult;
                    }

                    if (compResult < 0)
                    {
                        list[tempIndex] = list[i];
                        tempIndex = i++;
                        break;
                    }

                    i++;
                }
            }

            list[tempIndex] = tempVal;
            Sort(list, beginIndex, tempIndex - 1, comparer, reverse);
            Sort(list, tempIndex + 1, endIndex, comparer, reverse);
        }
    }
}

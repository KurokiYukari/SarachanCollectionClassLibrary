using System;

namespace SarachanCollectionClassLibrary
{
    interface IList_SCCL<T> : ICollection_SCCL<T>
    {
        #region Indexers
        /// <summary>
        /// 读写访问器，指向 Collection 中指定 index 的 item
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new T this[int index] { get; set; }

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
        public static IList_SCCL<T> operator +(IList_SCCL<T> arr, System.Collections.Generic.IEnumerable<T> collection)
        {
            // 下面这句 arr 无法调用 object 的 protected 函数 MemberwiseClone()
            //var result = (IList_SCCL<T>)arr.MemberwiseClone();

            // 采用反射机制构造
            var result = (IList_SCCL<T>)Activator.CreateInstance(arr.GetType()); // 这里选择使用默认无参构造函数而没有选择接收 IEnumerable 的构造函数（防止构造函数不存在
            result.Add(arr);
            result.Add(collection);

            return result;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>所给的 index 是否合法</returns>
        static bool IsIndexLegal(IList_SCCL<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        /// <summary>
        /// 采用快速排序对 list 进行排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="comparer">比较器</param>
        /// <param name="reverse">是否是逆序</param>
        static void Sort(IList_SCCL<T> list, System.Collections.Generic.IComparer<T> comparer = null, bool reverse = false)
        {
            Sort(list, 0, list.Count - 1, comparer, reverse);
        }

        /// <summary>
        /// 采用快速排序对 list 的指定区间 [beginIndex, endIndex] 进行排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="beginIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="reverse">是否是逆序</param>
        static void Sort(IList_SCCL<T> list, int beginIndex, int endIndex, System.Collections.Generic.IComparer<T> comparer = null, bool reverse = false)
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
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="enableReferanceEquals">使用 <seealso cref="Object.ReferenceEquals(object?, object?)"/> 方法来确定 item 是否删除，默认为 false (使用运算符 == 判断)</param>
        /// <returns>指定 item 的 index，如果不存在则返回 -1</returns>
        int IndexOf(T item, bool enableReferanceEquals = false);

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
        #endregion
    }
}

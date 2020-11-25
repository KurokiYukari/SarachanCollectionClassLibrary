using System;

namespace SarachanCollectionClassLibrary
{
    internal class MainClass
    {
        private static void Main()
        {
            IList_SCCL<int> list1 = new LinkedList_SCCL<int> { 3, 9, 2};
            IList_SCCL<int> list2 = new ArrayList_SCCL<int> { 111, 222 };

            list1.Insert(list2, 2);

            Console.WriteLine(list1);
        }
    }
}
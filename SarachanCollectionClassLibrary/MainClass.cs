using System;

namespace SarachanCollectionClassLibrary
{
    internal class MainClass
    {
        private static void Main()
        {
            IList_SCCL<int> list1 = new ArrayList_SCCL<int> { 3, 9, 2, 5, 6, 7, 1 };
            IList_SCCL<int>.Sort(list1, null, false);

            Console.WriteLine(list1);
        }
    }
}
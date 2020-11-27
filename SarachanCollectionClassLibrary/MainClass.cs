using System;
using Sarachan.Collections;

namespace Sarachan
{
    internal class MainClass
    {
        private static void Main()
        {
            System.Collections.Generic.IEnumerable<int> list1 = new LinkedList_SCCL<int> { 3, 9, 2};
            var list2 = new ArrayList_SCCL<int> { 111, 222 };
            

            Console.WriteLine(list2 + list1);
        }
    }
}
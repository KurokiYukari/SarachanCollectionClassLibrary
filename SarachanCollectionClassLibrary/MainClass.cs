using System;
using Sarachan.Collections;

namespace Sarachan
{
    internal class MainClass
    {
        private static void Main()
        {
            var hashSet = new HashSet_SCCL<int>();
            hashSet.Remove(1);

            hashSet.Add(1);
            hashSet.Add(1);
            hashSet.Remove(1);

            for (int i = 0; i < 100; i++)
            {
                hashSet.Add(i*4);
            }

            Console.WriteLine(hashSet);
        }
    }
}
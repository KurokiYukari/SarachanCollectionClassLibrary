# SarachanCollectionClassLibrary
一个实现了各种数据结构的 C# 类库。  
因为本来就是在造轮子，所以本库尽可能少地使用 BCL 中的各种已有的实现。以下是使用到的 BCL 中的接口：  
+ System.Collections.Generic.IEnumerable<T>：支持 foreach 和 LINQ。
+ System.Collections.Generic.IComparer<T>：支持自定义比较（多用于排序）。
+ System.Collections.Generic.IEqualityComparer<T>：支持自定义相等比较（用于 Set 等需要相等比较的地方）。
  
本库的接口可以理解为一种访问器，表示以该接口的方法访问容器，比如：
``` C#
// 代表以 Stack 的形式来访问 ArrayList
IStack_SCCL<int> stack = new ArrayList_SCCL<int>();
```

## 接口
ICollection_SCCL<> : 所有容器类都要实现的接口，除非特别生命，容器类都默认实现了该接口  
IList_SCCL<> : List 接口  
IQueue_SCCL<> : Queue 接口  
IStack_SCCL<> : Stack 接口  
ISet_SCCL<> : Set 接口
IMap_SCCL<> : Map 接口  

## 目前实现的所有容器类
ArrayList_SCCL<> : 可变长数组。实现了 IList_SCCL, IQueue_SCCL, IStack_SCCL  
LinkedList_SCCL<> : 链表。实现了 IList_SCCL, IQueue_SCCL, IStack_SCCL  
HashSet<> : 哈希集合。实现了 ISet_SCCL  
HashMap_SCCL<> : 哈希Map。实现了 IMap_SCCL  

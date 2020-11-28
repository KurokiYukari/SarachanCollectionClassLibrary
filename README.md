# SarachanCollectionClassLibrary
一个实现了各种数据结构的 C# 类库。  
为了能够使用 foreach 进行遍历，所有容器类都依赖于 System.Collections.Generic.IEnumerable<>。  
  
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

## 目前实现的所有容器类
ArrayList_SCCL<> : 可变长数组。实现了 IList_SCCL, IQueue_SCCL, IStack_SCCL  
LinkedList_SCCL<> : 链表。实现了 IList_SCCL, IQueue_SCCL, IStack_SCCL  
HashSet<> : 哈希集合。实现了 ISet_SCCL  

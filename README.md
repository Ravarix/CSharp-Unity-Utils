# CSharp-Unity-Utils

ObjectPool.cs
-------------

Simple generic Object Pool that supports LIFO and FIFO access. Can Eager or Lazy load objects.

PooledObject.cs
---------------

Utilizes the ObjectPool to generically wrap Objects into a Disposable pattern, returning to pool after use.

SmartLinkedList.cs
------------------

Implementation of Standard Doubly LinkedList that recycles LinkNodes to prevent Garbage Collection. Supports Stack and Queue interfaces. Utilizes a struct based Enumerator for GC-free looping.

SmartEvent.cs
-------------

A replacement for C# events which are backed by a native LinkedList, which generates garbage. This utilizes the SmartLinkedList to prevent GC. Supports '+=' and '-=' syntax naively.
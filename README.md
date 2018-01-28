# CSharp-Unity-Utils

GameObjectPool.cs
-----------------

Simple Static GameObject Pool that can be prewarmed, and lazily expanded.

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

ReactiveValue.cs
----------------

Generic Observable Wrapper that allows for `Subscribe` and Predicate filtered `Select` Actions on Value updates. Very useful for binding to UI changes.


EnumMap.cs
----------

Extension of Dictionary that prevents boxing Enum keys, and saves GC.
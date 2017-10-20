
using System;
using System.Collections;
using System.Collections.Generic;

public class SmartLinkedList<T> : ICollection<T>
{
    public class LinkNode<T2> : IDisposable
    {
        public T2 item;
        public LinkNode<T2> prev;
        public LinkNode<T2> next;
        
        public void Dispose()
        {
            item = default(T2);
            prev = null;
            next = null;
        }
    }
    
    public LinkNode<T> First { get; private set; }
    public LinkNode<T> Last { get; private set; }
    public int Count { get; private set; }
    public bool IsReadOnly => false;
    private int version;

    private readonly SmartLinkedList<LinkNode<T>> _nodePool = new SmartLinkedList<LinkNode<T>>();
    
    public void AddFirst(T item)
    {
        if(First == null)
            AddEmpty(item);
        else
            AddBefore(First, item);
    }

    public void AddLast(T item)
    {
        if (Last == null)
            AddEmpty(item);
        else
            AddAfter(Last, item);
    }

    private void AddBefore(LinkNode<T> target, T item)
    {
        var node = GetNode(item, target.prev, target);
        if (target.prev == null)
            First = node;
        target.prev = node;
        Count++;
        version++;
    }
    
    private void AddAfter(LinkNode<T> target, T item)
    {
        var node = GetNode(item, target, target.next);
        if (target.next == null)
            Last = node;
        target.next = node;
        node.prev = target;
        Count++;
        version++;
    }

    private void AddEmpty(T item)
    {
        var node = GetNode(item, null, null);
        First = node;
        Last = node;
        Count++;
        version++;
    }

    #region Queue Controls
    public void Enqueue(T item)
    {
        AddFirst(item);
    }

    public T Dequeue()
    {
        if(Count == 0)
            throw new ArgumentOutOfRangeException();
        var item = Last.item;
        InternalRemove(Last);
        return item;
    }
    #endregion

    #region Stack Controls
    public void Push(T item)
    {
        AddFirst(item);
    }

    public T Pop()
    {
        if(Count == 0)
            throw new ArgumentOutOfRangeException();
        var item = First.item;
        InternalRemove(First);
        return item;
    }

    public T Peek()
    {
        if(Count == 0)
            throw new ArgumentOutOfRangeException();
        return First.item;
    }
    #endregion

    public LinkNode<T> Find(T item)
    {
        var currentNode = First;
        var equalityComparer = EqualityComparer<T>.Default;

        if (currentNode == null) 
            return null;
        if (item != null)
        {
            while (!equalityComparer.Equals(currentNode.item, item))
            {
                currentNode = currentNode.next;
                if (currentNode == null)
                    return null;
            }
            return currentNode;
        }
        //find null
        while (currentNode.item != null)
        {
            currentNode = currentNode.next;
            if (currentNode == null)
                return null;
        }

        return null;
    }
    
    public bool Remove(T item)
    {
        var node = Find(item);
        if (node == null)
            return false;
        InternalRemove(node);
        return true;
    }

    private void InternalRemove(LinkNode<T> node)
    {
        if (node == First && node == Last)
        {
            First = null;
            Last = null;
        }
        else
        {
            if (node.next != null)
                node.next.prev = node.prev;
            if (node.prev != null)
                node.prev.next = node.next;
        }
        ReleaseNode(node);
        Count--;
        version++;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public void Add(T item)
    {
        AddFirst(item);
    }

    public void Clear()
    {
        var node = First;
        while (node != null)
        {
            var dummy = node;
            node = node.next;
            ReleaseNode(dummy);
        }
        First = null;
        Last = null;
        Count = 0;
        version++;
    }

    public bool Contains(T item)
    {
        return Find(item) != null;
    }

    public void CopyTo(T[] array, int index)
    {
        if (index < 0 || index > array.Length)
            throw new ArgumentOutOfRangeException();
        if(array.Length - index < Count)
            throw new ArgumentException(); //not enough space
        var node = this.First;
        while (node != null)
        {
            array[index++] = node.item;
            node = node.next;
        }
    }

    private LinkNode<T> GetNode(T item, LinkNode<T> prev, LinkNode<T> next)
    {
        var node = _nodePool.Count == 0 
                    ? new LinkNode<T>() 
                    : _nodePool.Dequeue();
        node.item = item;
        node.prev = prev;
        node.next = next;
        return node;
    }

    private void ReleaseNode(LinkNode<T> node)
    {
        node.Dispose();
        _nodePool.Enqueue(node);
    }

    public struct Enumerator : IEnumerator<T>
    {
        public T Current { get; private set; }
        private SmartLinkedList<T> list;
        private LinkNode<T> node;
        private int index;
        private int version;

        public Enumerator(SmartLinkedList<T> list)
        {
            this.list = list;
            version = list.version;
            node = list.First;
            Current = default(T);
            index = 0;
        }
        
        public void Dispose()
        {
            //we a struct bruh
        }

        public bool MoveNext()
        {
            if (version != list.version)
                throw new InvalidOperationException();
            if (node == null)
            {
                index = list.Count + 1;
                return false;
            }
            index++;
            Current = node.item;
            node = node.next;
            return true;
        }

        public void Reset()
        {
            if (version != list.version)
                throw new InvalidOperationException();
            Current = default(T);
            node = list.First;
            index = 0;
        }

        object IEnumerator.Current
        {
            get
            {
                if (index == 0 || index == list.Count + 1)
                    throw new InvalidOperationException();
                return Current;
            }
        }
    }
}
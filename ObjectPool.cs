using System;
using System.Collections.Generic;

public sealed class ObjectPool<T> : IDisposable
{
    public enum AccessMode { FIFO, LIFO };
    public enum LoadingMode { Eager, Lazy };
    
    private readonly int size;
    private readonly IItemStore store;
    private readonly Func<T> factory;
    private readonly LoadingMode loadingMode;

    const string factoryMessage = "Factory must not be null.";
    const string sizeMessage = "The size of the pool must be greater than zero.";

    public ObjectPool(Func<T> factory, int size, LoadingMode loadingMode = LoadingMode.Eager, 
        AccessMode accessMode = AccessMode.FIFO)
    {
        if (factory == null)
            throw new ArgumentNullException(factoryMessage);
        
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), size, sizeMessage);

        this.factory = factory;
        this.size = size;
        this.loadingMode = loadingMode;
        
        switch (accessMode)
        {
            case AccessMode.FIFO:
                store = new QueueStore(size);
                break;
            case AccessMode.LIFO:
                store = new StackStore(size);
                break;
        }
        
        if (loadingMode == LoadingMode.Eager)
            PreloadItems();
    }
    
    private void PreloadItems()
    {
        for (int i = 0; i < size; ++i)
            store.Store(factory());
    }

    public T Get()
    {
        return store.Count > 0 
                ? store.Fetch() 
                : factory();
    }
    
    public void Put(T item)
    {
        store.Store(item);
    }

    public void Dispose()
    {
        while (store.Count > 0)
            (store.Fetch() as IDisposable)?.Dispose();
    }
    
    private interface IItemStore
    {
        T Fetch();
        void Store(T item);
        int Count { get; }
    }
    
    private class QueueStore : Queue<T>, IItemStore
    {
        public QueueStore(int capacity) : base(capacity)
        {}

        public T Fetch() => Dequeue();
        public void Store(T item) => Enqueue(item);
    }

    private class StackStore : Stack<T>, IItemStore
    {
        public StackStore(int capacity) : base(capacity)
        {}

        public T Fetch() => Pop();
        public void Store(T item) => Push(item);
    }
}
using System;

public class PooledObject<T> : IDisposable where T : IDisposable, new()
{
    public const int defaultSize = 10;
    private static ObjectPool<PooledObject<T>> _pool;
    private static ObjectPool<PooledObject<T>> Pool
    {
        get
        {
            if (_pool != null)
                return _pool;
            _pool = new ObjectPool<PooledObject<T>>(() => new PooledObject<T>(new T()), defaultSize);
            return _pool;
        }
    }
    
    public static PooledObject<T> Get() => Pool.Get();
    
    public T inner { get; }

    public PooledObject(T instance)
    {
        inner = instance;
    }

    public void Dispose()
    {
        inner.Dispose();
        Pool.Put(this);
    }

}
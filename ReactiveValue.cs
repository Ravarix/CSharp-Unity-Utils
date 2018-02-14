    
using System;
using System.Collections.Generic;

namespace Unitilities
{
    public class ReactiveValue<T> : IObservable<T>
    {
        public delegate void Callback(T value);
        
        private Callback _onChanged;
        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value)) 
                    return;
                _value = value;
                _onChanged?.Invoke(value);
            }
        }
        
        public ReactiveValue(T value)
        {
            _value = value;
        }

        public void Set(T value) => Value = value;

        public void Subscribe(Callback callback, bool immediate = true)
        {
            _onChanged += callback;
            if (immediate)
                callback(Value);
        }

        public void Select(Predicate<T> filter, Action<T> callback, bool immediate = true)
        {
            _onChanged += val => {
                if (filter(val))
                    callback(val);
            };
            if (immediate && filter(Value))
                callback(Value);
        }

        public void Unsubscribe(Callback callback) => _onChanged -= callback;
        
        public static implicit operator T(ReactiveValue<T> rv) => rv.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override bool Equals(object obj) => Value.Equals(obj);
        
        public override string ToString() => Value.ToString();

        public class Ticket : IDisposable
        {
            private readonly ReactiveValue<T> _rv;
            private readonly Callback _cb;

            public Ticket(ReactiveValue<T> rv, Callback cb)
            {
                _rv = rv;
                _cb = cb;
            }

            public void Dispose() => _rv.Unsubscribe(_cb);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Callback cb = observer.OnNext;
            _onChanged += cb;
            return new Ticket(this, cb);
        }
    }
}
    
using System;
using System.Collections.Generic;

namespace Unitilities
{
    public class ReactiveValue<T> 
    {
        private T _value;
        private SmartEvent<T> _onChanged;
        public SmartEvent<T> OnChanged => _onChanged ?? (_onChanged = new SmartEvent<T>());
        public bool Dirty { get; private set; }
        public static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;

        public T Value
        {
            get { return _value; }
            set
            {
                if (Comparer.Equals(_value, value)) 
                    return;
                Dirty = true;
                _value = value;
                _onChanged?.Invoke(value);
            }
        }

        public void Set(T value) => Value = value;

        public void Subscribe(Action<T> callback, bool immediate = true)
        {
            OnChanged.Add(callback);
            if (immediate)
                callback(Value);
        }

        public void Select(Predicate<T> filter, Action<T> callback, bool immediate = true)
        {
            OnChanged.Add(obj => {
                if (filter(obj))
                    callback(obj);
            });
            if (immediate && filter(Value))
                callback(Value);
        }

        public void Clean()
        {
            Dirty = false;
        }

        public ReactiveValue(T value, bool dirty = false)
        {
            _value = value;
            Dirty = dirty;
        }

        public static implicit operator T(ReactiveValue<T> rv) => rv.Value;

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
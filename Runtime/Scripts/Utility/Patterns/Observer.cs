using System;
using UnityEngine.Events;

namespace TigerFrogGames.Core
{
    public class Observer<T> : IDisposable
    {
        public UnityEvent<T> OnStatChange;
        private T currentValue;
        public T baseValue;

        private int numOfSubs = 0;
        
        public T Value
        {
            get => currentValue;
            set => Set(value);
        }

        public Observer(T Value)
        {
            currentValue = Value;
            baseValue = Value;

            OnStatChange = new();
        }
        
        public void Set(T Value)
        {
            if (currentValue.Equals(Value)) return;
            
            currentValue = Value;
            Invoke();
        }

        private void Invoke()
        {
            OnStatChange.Invoke(currentValue);
        }

        public void AddListener(UnityAction<T> callback)
        {
            if(callback == null) return;
            if(OnStatChange == null) OnStatChange = new UnityEvent<T>();
            
            numOfSubs++;
            OnStatChange.AddListener(callback);
        }
        
        public void RemoveListener(UnityAction<T> callback)
        {
            if(callback == null) return;
            if(OnStatChange == null) OnStatChange = new UnityEvent<T>();

            numOfSubs--;
            OnStatChange.RemoveListener(callback);
        }

        private void RemoveAllListeners()
        {
            if(OnStatChange == null) return;

            numOfSubs = 0;
            OnStatChange.RemoveAllListeners();
        }

        public void Dispose()
        {
            RemoveAllListeners();
            OnStatChange = null;
            Value = default;
        }

        public override string ToString()
        {
            return $"Currently held value: {Value} - Num of subscriptions: {numOfSubs}";
        }

        public bool IsListenedToo()
        {
            return numOfSubs > 0;
        }
    }
}
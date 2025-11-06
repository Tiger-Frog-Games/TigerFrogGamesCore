using System;
using System.Collections;
using System.Collections.Generic;

namespace TigerFrogGames.Core
{
    public interface IObservableList<T>
    {
        event Action<List<T>> AnyValueChanged;
        int Count { get; }
        T this[int index] { get;}

        void Swap(int index1, int index2);
        void Add(T item);
        void AddRange(IEnumerable<T> itemsToAdd);
        bool TryRemove(T item);
        void Clear();
    }
    [Serializable]
    public class ObservableList<T> : IObservableList<T>, IEnumerable
    {
        private List<T> items;
        
        public event Action<List<T>> AnyValueChanged;

        public int Count => items.Count;
        public T this[int index] => items[index];

        public ObservableList(IList<T> initialList = null)
        {
            items = new List<T>();
            if (initialList != null)
            {
                items.AddRange(initialList);
                Invoke();
            }
        }
        
        public void Invoke() => AnyValueChanged?.Invoke(items);
        
        public void Swap(int index1, int index2)
        {
            (items[index1], items[index2]) = (items[index2], items[index1]);
            Invoke();
        }
        
        public void Add(T item)
        {
            items.Add(item);
            Invoke();
        }

        public void AddRange(IEnumerable<T> itemsToAdd)
        {
            items.AddRange(itemsToAdd);
            Invoke();
        }
        
        public bool TryRemove(T item)
        {
            if (items.Remove(item))
            {
                Invoke();
                return true;
            }
            return false;
        }
        
        public void RemoveAt(int index)
        {
            if(index < items.Count || items.Count == 0) return;
            
            items.RemoveAt(index);
            Invoke();
        }
        
        public void Clear()
        {
            items = new List<T>();
            Invoke();
        }

        public List<T> GetList()
        {
            return items;
        }

        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
using System.Collections.Concurrent;

namespace RealtimeChat.Entities
{
    public class ConcurrentSet<T>
    {
        private readonly ConcurrentDictionary<T, bool> dict = new();
        public ConcurrentSet(params T[] values) 
        { 
            foreach(var value in values)
            {
                Add(value);
            }
        }
        public void Add(T item)
        {
            dict.AddOrUpdate(item,true,(x,y) => true);
        }
        public void Remove(T item)
        {
            dict.Remove(item, out bool _);
        }
        public bool ContainsKey(T item)
        {
            return dict.ContainsKey(item);
        }
        public IEnumerable<T> Keys()
        {
            foreach(var item in dict.Keys)
            {
                yield return item;
            }
        }
        public int Count {  get => dict.Count; }
    }
}

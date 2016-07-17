using System;
using System.Collections.Generic;

namespace Utilities
{
    [Serializable]
    public class SerializableListDictionary<TKey, TValue> : SerializableDictionary<TKey, List<TValue>>
    {
        public virtual void Add(TKey key, TValue value)
        {
            List<TValue> list;
            if (TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<TValue>();

                list.Add(value);
                Add(key, list);
            }
        }
    }

    public class SortedListList<TKey, TValue> : SortedList<TKey, List<TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            List<TValue> list;
            if (TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<TValue>();

                list.Add(value);
                Add(key, list);
            }
        }
    }

}
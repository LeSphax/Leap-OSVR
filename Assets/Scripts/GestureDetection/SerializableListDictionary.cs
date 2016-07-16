using SerializeDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SerializableListDictionary<TKey, TValue> : SerializableDictionary<TKey, List<TValue>>
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


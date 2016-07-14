using SaveManagement;
using System;
using UnityEngine;

public static class GestureDataManager
{
    private const string path = "/GestureData.xml";

    private static GestureData _data;
    public static GestureData data
    {
        get
        {
            if (_data == null)
            {
                _data = Load();
                if (_data == null)
                {
                    _data = new GestureData();
                }
            }
            return _data;
        }
        private set
        {
            _data = value;
        }
    }

    private static GestureData Load()
    {
        return Saving.Load<GestureData>(path);
    }

    private static void Save()
    {
        Saving.Save(path, data);
    }

    public static void Add(string name, Gesture gesture)
    {
        data.Add(name, gesture);
        Save();
    }

    public static void Clear()
    {
        data = new GestureData();
        Save();
    }

}

[Serializable]
public class GestureData : SerializableListDictionary<string, Gesture>
{

    public int GetNumberGestures()
    {
        int result = 0;
        foreach(string key in Keys)
        {
            result += this[key].Count;
        }
        return result;
    }
}


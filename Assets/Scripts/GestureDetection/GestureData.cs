using SaveManagement;
using System;
using System.Collections.Generic;
using System.Linq;
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
        data.toCSV();
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
        foreach (string key in Keys)
        {
            result += this[key].Count;
        }
        return result;
    }


    public void toCSV()
    {
        string[] keys = Keys.ToArray();
        List<string> lines = new List<string>();

        int maxNumberPoints = GetMaxNumberPoints();
        foreach (string key in Keys)
        {
            string gestures = string.Join(Environment.NewLine + Environment.NewLine, this[key].Select(gesture => gesture.ToCSV(maxNumberPoints)).ToArray());
            string[] pair = { key, gestures };
            lines.Add(string.Join(Environment.NewLine, pair));
        }

        string[] content = { "sep=;", string.Join(
                Environment.NewLine,
                lines.ToArray()
            )};

        string csv = string.Join(Environment.NewLine,content );
        try
        {
            System.IO.File.WriteAllText("C:/Users/Sebas/Desktop" + "/test.csv", csv);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private int GetMaxNumberPoints()
    {
        int maxNumberPoints = 0;
        foreach (string key in Keys)
        {
            foreach (Gesture gesture in this[key])
            {
                if (maxNumberPoints < gesture.NumberPoints)
                {
                    maxNumberPoints = gesture.NumberPoints;
                }
            }
        }
        return maxNumberPoints;
    }
}


using SaveManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

public static class GestureDataManager
{
    private const string pathGestures = "/GestureData.xml";
    private const string pathClasses = "/Classes.xml";

    public static Map<string, int> _classesMap;
    public static Map<string, int> classesMap
    {
        get
        {
            if (_classesMap == null)
            {
                _classesMap = LoadClasses();
                if (_classesMap == null)
                {
                    _classesMap = new Map<string, int>();
                    _classesMap.Add("None", -1);
                }
            }
            return _classesMap;
        }
        set
        {
            _classesMap = value;
        }
    }

    private static Map<string, int> LoadClasses()
    {
        return Saving.Load<Map<string, int>>(pathClasses);
    }

    private static int currentClassIndex = 0;
    public static int numberClasses
    {
        get
        {
            return classesMap.Count;
        }
    }

    private static GestureData _data;

    public static GestureData Data
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
        return Saving.Load<GestureData>(pathGestures);
    }

    public static void Save()
    {
        Saving.Save(pathGestures, Data);
        Saving.Save(pathClasses, classesMap);
        Data.toCSV();

    }

    public static void Add(string name, Gesture gesture)
    {
        if (!classesMap.ContainsKey(name))
        {
            classesMap.Add(name, currentClassIndex);
            currentClassIndex++;
        }
        Data.Add(name, gesture);
    }

    public static int GetClassNumber(string className)
    {
        return classesMap.Forward[className];
    }

    public static string GetClassName(int classNumber)
    {
        return classesMap.Reverse[classNumber];
    }


    public static void Clear()
    {
        Data = new GestureData();
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

    public override void Add(string key, Gesture value)
    {
        base.Add(key, value);
    }


    public void toCSV()
    {
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

        string csv = string.Join(Environment.NewLine, content);
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


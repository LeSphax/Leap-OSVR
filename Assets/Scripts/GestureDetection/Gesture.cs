using GestureDetection.Algorithms;
using Leap.Unity;
using SerializeDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class Gesture : SerializableDictionary<string, List<Vector3>>
{

    public const string PalmPositions = "PalmPosition";
    public const string PalmDirections = "PalmDirection";
    private const int NUMBER_POINTS_BEGINNING = 10;
    public static readonly string[] Fingers = { "Finger0", "Finger1", "Finger2", "Finger3", "Finger4" };

    public static List<Parameter> parameters = null;

    public override void ReadXml(System.Xml.XmlReader reader)
    {
        base.ReadXml(reader);
    }

    public int NumberPoints
    {
        get
        {
            Parameter param = parameters[0];
            int NumberPoints = this[param.key].Count;
            return NumberPoints;
        }
    }

    public Gesture()
    {
        if (parameters == null)
            InitParameters();
        foreach (Parameter parameter in parameters)
            this[parameter.key] = new List<Vector3>();
    }

    private static void InitParameters()
    {
        parameters = new List<Parameter>();
        parameters.Add(new PositionParameter());
        parameters.Add(new Parameter(PalmDirections, 15F, 3F, Color.green, (model => model.GetLeapHand().PalmNormal.ToVector3())));
        Color colorFingers = Color.red;
        int index = 0;
        foreach (string fingerIndex in Fingers)
        {
            parameters.Add(new FingerParameter(index));
            colorFingers -= new Color(0.2F, 0.0F, 0.0F, 0.0F);
            index++;
        }
    }

    public Gesture GetSubGesture(int index, int count)
    {
        Gesture newGesture = new Gesture();
        foreach (string parameterName in Keys)
        {
            newGesture[parameterName] = this[parameterName].GetRange(index, count);
        }
        return newGesture;
    }

    public string GetGestureClass(ClassificationAlgorithm algorithm)
    {
       return algorithm.Recognize(AlgorithmInput.GestureToData(this));
    }

    public static bool AreGesturesSimilar(Gesture savedGesture, Gesture gestureToRecognize)
    {
        if (savedGesture.Count != gestureToRecognize.Count)
            return false;
        Assert.AreEqual(savedGesture.Count, gestureToRecognize.Count);
        int startIndex = GetSimilarityBeginning(savedGesture, gestureToRecognize);
        // Debug.Log(startIndex);
        //int startIndex = 0;
        if (startIndex != -1)
        {
            bool[] results = GetSimilarity(savedGesture, gestureToRecognize, startIndex);
            for (int i = 0; i < results.Length; i++)
            {
                if (!results[i])
                {
                    return false;
                }
            }
            return true;
        }
        else return false;
    }

    public bool IsSimilarToBeginning(Gesture otherGesture)
    {
        if (otherGesture.NumberPoints < 10)
            return false;
        //if (otherGesture.NumberPoints > 15)
        //{
        //    EditorApplication.isPaused = true;
        //}
        int index = 0;
        double[] results = new double[parameters.Count];
        foreach (Parameter param in parameters)
        {
            Vector3[] arrayToRecognize = param.NormalizeValues(otherGesture[param.key].GetRange(otherGesture[parameters[0].key].Count - 10, 10).ToArray());
            Vector3[] arraySaved = param.NormalizeValues(this[param.key].GetRange(0, 10).ToArray());

            double diff = Distance.DynamicTimeWarpVector3(arraySaved, arrayToRecognize, Vector3.Distance);

            results[index] = diff;
            // 
            //    string s = "";
            //    string s2 = "";
            //    foreach (Vector3 point in this[param.key].ToArray())
            //    {
            //        s += point + ",";
            //    }
            //    foreach (Vector3 point in arrayToRecognize)
            //    {
            //        s2 += point + ",";
            //    }
            //    Debug.Log(s);
            //    Debug.Log("vs " + s2);
            //}
            index++;
        }

        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] > parameters[i].thresholdBeginning)
            {
                // Debug.Log(otherGesture.NumberPoints + " Points - " +parameters[i].key + " : " + results[i]);
                return false;
            }
        }
        return true;
    }


    private static int GetSimilarityBeginning(Gesture savedGesture, Gesture gestureToRecognize)
    {
        int currentStartIndex = 0;
        if (gestureToRecognize.NumberPoints - savedGesture.NumberPoints < NUMBER_POINTS_BEGINNING)
        {
            return 0;
        }
        while (currentStartIndex < gestureToRecognize.NumberPoints - savedGesture.NumberPoints)
        {
            Debug.Log(currentStartIndex);
            bool[] results = new bool[parameters.Count];
            int parameterIndex = 0;
            foreach (Parameter param in parameters)
            {
                Vector3[] arrayToRecognize = param.NormalizeValues(gestureToRecognize[param.key].GetRange(currentStartIndex, NUMBER_POINTS_BEGINNING).ToArray());
                Vector3[] arraySaved = param.NormalizeValues(savedGesture[param.key].GetRange(0, NUMBER_POINTS_BEGINNING).ToArray());

                double diff = Distance.DynamicTimeWarpVector3(arraySaved, arrayToRecognize, Vector3.Distance);
                results[parameterIndex] = diff < param.threshold / 8;
                if (!results[parameterIndex] || currentStartIndex == 0)
                {
                    Debug.Log(parameters[parameterIndex].key + " : " + diff);
                    //string s = "", s2 = "";
                    //foreach (Vector3 point in savedGesture[param.key].GetRange(0, 10).ToArray())
                    //{
                    //    s += point + ",";
                    //}
                    //foreach (Vector3 point in arrayToRecognize)
                    //{
                    //    s2 += point + ",";
                    //}
                    //Debug.Log(s);
                    //Debug.Log("vs " + s2);
                }
                parameterIndex++;
            }
            bool similar = true;
            for (int i = 0; i < results.Length; i++)
            {
                if (!results[i])
                {
                    similar = false;
                }
            }
            if (similar)
                return currentStartIndex;
            else
                currentStartIndex++;
        }

        return -1;
    }

    private static bool[] GetSimilarity(Gesture savedGesture, Gesture gestureToRecognize, int startIndex)
    {
        int index = 0;
        bool[] results = new bool[parameters.Count];
        foreach (Parameter param in parameters)
        {
            Vector3[] arrayToRecognize = param.NormalizeValues(gestureToRecognize[param.key].GetRange(startIndex, gestureToRecognize[param.key].Count - startIndex).ToArray());
            Vector3[] arraySaved = param.NormalizeValues(savedGesture[param.key].ToArray());

            double diff = Distance.DynamicTimeWarpVector3(arraySaved, arrayToRecognize, Vector3.Distance);

            results[index] = diff < param.threshold;

            if (!results[index])
            {
                //Debug.Log(parameters[index].key + " : " + diff);
                string s = "";
                string s2 = "";
                foreach (Vector3 point in savedGesture[param.key].ToArray())
                {
                    s += point + ",";
                }
                foreach (Vector3 point in arrayToRecognize)
                {
                    s2 += point + ",";
                }
                //Debug.Log(s);
                //Debug.Log("vs " + s2);
            }
            index++;
        }

        return results;
    }

    public void DrawGizmos(Color color)
    {
        foreach (Parameter parameter in parameters)
        {
            DrawGizmosParameter(color, parameter);
        }
    }

    private void DrawGizmosParameter(Color color, Parameter parameter)
    {
        foreach (Vector3 position in this[parameter.key])
        {
            parameter.DrawPoint(position, color);
        }
    }

    public void DrawGizmos()
    {
        foreach (Parameter parameter in parameters)
        {
            DrawGizmosParameter(parameter.gizmoColor, parameter);
        }
    }


    public string ToCSV(int numberPoints)
    {
        List<string> parameters = new List<string>();

        foreach (string key in Keys)
        {
            List<string> values = this[key].Select(vector => vector.ToString()).ToList<string>();
            string[] emptyStrings = new string[numberPoints - values.Count];
            emptyStrings.Populate<string>("");
            values.InsertRange(values.Count, emptyStrings);
            string[] line = {";"+ key, string.Join(";", values.ToArray()) };
            parameters.Add(string.Join(";",line));
        }

        return string.Join(Environment.NewLine, parameters.ToArray());
    }
}



using GestureDetection.Algorithms;
using Leap.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;

[Serializable]
public class Gesture : SerializableDictionary<string, ParameterValues>
{

    public const string PalmPositions = "PalmPosition";
    public const string PalmDirections = "PalmDirection";
    private const int NUMBER_POINTS_BEGINNING = 10;
    public static readonly string[] Fingers = { "Finger0", "Finger1", "Finger2", "Finger3", "Finger4" };

    public static List<Parameter> parameters
    {
        get
        {
            return _parameters.Values.ToList();
        }
    }

    public static List<string> parameterKeys
    {
        get
        {
            return _parameters.Keys.ToList();
        }
    }


    private static Dictionary<string, Parameter> __parameters;
    private static Dictionary<string,Parameter> _parameters
    {
        get
        {
            if (__parameters == null)
                InitParameters();
            return __parameters;
        }
        set
        {
            __parameters = value;
        }
    }
    public static Parameter GetParameter(string parameterKey)
    {
        return _parameters[parameterKey];
    }

    public override void ReadXml(System.Xml.XmlReader reader)
    {
        base.ReadXml(reader);
    }

    public int NumberPoints
    {
        get
        {
            Parameter param = parameters[0];
            int NumberPoints = this[param.key].values.Count;
            return NumberPoints;
        }
    }

    public Gesture()
    {
        
        foreach (Parameter parameter in parameters)
            this[parameter.key] = new ParameterValues(parameter.key);
    }

    private static void InitParameters()
    {
        _parameters = new Dictionary<string, Parameter>();
        AddParameter(new PositionParameter());
        AddParameter(new Parameter(PalmDirections, 15F, 3F, Color.green, (model => model.GetLeapHand().PalmNormal.ToDoubleArray())));
        Color colorFingers = Color.red;
        for(int index = 0; index <Fingers.Length; index++)
        {
            AddParameter(new FingerParameter(index));
            colorFingers -= new Color(0.2F, 0.0F, 0.0F, 0.0F);
        }
    }

    private static void AddParameter(Parameter parameter)
    {
        _parameters.Add(parameter.key, parameter);
    }

    public Gesture GetSubGesture(int index, int count)
    {
        Gesture newGesture = new Gesture();
        foreach (string parameterName in Keys)
        {
            newGesture[parameterName].values = this[parameterName].values.GetRange(index, count);
        }
        return newGesture;
    }

    public string GetGestureClass(AlgorithmComponent algorithm, out List<string> errors)
    {
        DistanceResult result = algorithm.Recognize(this, out errors);
        return GestureDataManager.GetClassName(result.Best());
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
        int num = 0;
        List<double[]> myValues = this[parameter.key].values;
        foreach (double[] position in myValues)
        {
            if (num == 0)
            {
                parameter.DrawPoint(position.ToVector3(), Color.magenta);
            }
            else if (num == myValues.Count)
            {
                parameter.DrawPoint(position.ToVector3(), Color.white);
            }
            else
            {
                parameter.DrawPoint(position.ToVector3(), color);
            }
            num++;
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
            List<string> values = this[key].values.Select(vector => vector.Print()).ToList<string>();
            string[] emptyStrings = new string[numberPoints - values.Count];
            emptyStrings.Populate<string>("");
            values.InsertRange(values.Count, emptyStrings);
            string[] line = {";"+ key, string.Join(";", values.ToArray()) };
            parameters.Add(string.Join(";",line));
        }

        return string.Join(Environment.NewLine, parameters.ToArray());
    }
}



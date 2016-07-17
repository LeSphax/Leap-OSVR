using Leap.Unity;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;

public class Parameter
{


    public delegate double[] GetParameter(IHandModel hand);
    public string key;
    public float threshold;
    public float thresholdBeginning;
    public Color gizmoColor;
    public GetParameter getParameter;

    public Parameter()
    {

    }

    public Parameter(string key, float threshold, float thresholdBeginning, Color color, GetParameter getParameter)
    {
        SetParameterVariables(key, threshold, thresholdBeginning, color, getParameter);
    }

    public virtual int Dimension()
    {
        return 3;
    }

    public virtual double[][] NormalizeValues(double[][] values)
    {
        return values;
    }

    protected double[][] GetValues(Gesture gesture)
    {
        return gesture[key].values.ToArray();
    }

    protected void SetParameterVariables(string key, float threshold, float thresholdBeginning, Color color, GetParameter getParameter)
    {
        this.key = key;
        this.threshold = threshold;
        this.thresholdBeginning = thresholdBeginning;
        gizmoColor = color;
        this.getParameter = getParameter;
    }


    public virtual void DrawPoint(Vector3 position, Color color)
    {
        Utils.DrawCircle(position, -Camera.main.transform.forward, 0.005f, color);
    }
}

[Serializable]
public class ParameterValues
{

    public ParameterValues()
    {

    }

    public Parameter parameter
    {
        get
        {
            return Gesture.GetParameter(parameterKey);
        }
    }
    public static bool once = false;
    public double[][] GetNormalizedValues()
    {
        Assert.IsNotNull(values);
        return parameter.NormalizeValues(values.ToArray());
    }

    public int Dimension
    {
        get
        {
            return parameter.Dimension();
        }
    }

    public string parameterKey;

    public List<double[]> values = new List<double[]>();

    public ParameterValues(string parameterKey)
    {
        this.parameterKey = parameterKey;
    }

    public void Add(double[] value)
    {
        values.Add(value);
    }
}



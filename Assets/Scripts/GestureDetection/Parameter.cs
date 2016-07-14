using Leap.Unity;
using UnityEngine;

public class Parameter
{

    public delegate Vector3 GetParameter(IHandModel hand);
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

    protected void SetParameterVariables(string key, float threshold, float thresholdBeginning, Color color, GetParameter getParameter)
    {
        this.key = key;
        this.threshold = threshold;
        this.thresholdBeginning = thresholdBeginning;
        gizmoColor = color;
        this.getParameter = getParameter;
    }

    public virtual Vector3[] NormalizeValues(Vector3[] values)
    {
        return values;
    }

    public virtual void DrawPoint(Vector3 position, Color color)
    {
        Utils.DrawCircle(position, -Camera.main.transform.forward, 0.005f, color);
    }

}



using Leap.Unity;
using UnityEngine;

public class PositionParameter : Parameter
{
    public PositionParameter()
    {
        SetParameterVariables("PalmPosition", 5F, 0.45F, Color.yellow, (model => model.GetLeapHand().PalmPosition.ToVector3()));
    }

    public override Vector3[] NormalizeValues(Vector3[] values)
    {
        Vector3 firstPoint = values[0];
        Vector3[] result = new Vector3[values.Length];
        for (int i =1; i < values.Length; i++)
        {
            result[i] = values[i] - firstPoint;
        }
        return result;
    }
}

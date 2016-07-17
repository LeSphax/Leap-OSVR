using Leap.Unity;
using UnityEngine;
using UnityEngine.Assertions;

public class PositionParameter : Parameter
{
    public PositionParameter()
    {
        SetParameterVariables("PalmPosition", 5F, 0.45F, Color.yellow, (model => model.GetLeapHand().PalmPosition.ToDoubleArray()));
    }

    public override double[][] NormalizeValues(double[][] values)
    {
        double[][] result = new double[values.Length][];
        result[0] = values[0];
        for (int i = 1; i < values.Length; i++)
        {
            result[i] = new double[values[i].Length];
            for (int y = 0; y < values[i].Length; y++)
                result[i][y] = values[i][y] - result[0][y];
        }
        Assert.IsTrue(result.Length == values.Length);
        return result;
    }
}

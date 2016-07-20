using Leap.Unity;
using UnityEngine;
using Utilities;

public class FingerParameter : Parameter
{
    private int fingerIndex;

    public FingerParameter(int index)
    {
        fingerIndex = index;
        Color colorFinger = Color.red - new Color(0.2F, 0.0F, 0.0F, 0.0F) * fingerIndex;
        SetParameterVariables(Gesture.Fingers[index], 15F, 2F, colorFinger, (model => ((double)GetFingerDirection(model, fingerIndex)).ValueToArray()));
    }

    public static float GetFingerDirection(IHandModel model, int i)
    {
        return (Quaternion.Inverse(model.GetLeapHand().Rotation.ToQuaternion()) * GetFinger(model, i).Direction.ToVector3()).z;
    }

    private static Leap.Finger GetFinger(IHandModel model, int i)
    {
        return model.GetLeapHand().Fingers[i];
    }

    public override int Dimension()
    {
        return 1;
    }

    public override void DrawPoint(Vector3 position, Color color)
    {
        base.DrawPoint(position/2 + Vector3.right/5 * fingerIndex, color);
    }
}

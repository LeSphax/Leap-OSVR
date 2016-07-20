using GestureDetection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GesturesVisualizer : MonoBehaviour
{

    public GestureDetector detector;
    int currentIndex = 0;

    List<string> currentErrors
    {
        get
        {
            return gesturesToVisualize[gestureList[currentIndex]];
        }
    }

    List<Gesture> gestureList;
    private bool visualizing = false;

    private Dictionary<Gesture, List<string>> gesturesToVisualize;

    private Gesture currentGesture
    {
        get
        {
            return gestureList[currentIndex];
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            VisualiseGestures();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            VisualiseNextGesture();
        }
        else
        {

        }
    }

    public void VisualiseGestures()
    {
        gesturesToVisualize = detector.subGesturesToVisualize;
        //detector.enabled = false;
        gestureList = gesturesToVisualize.Keys.ToList();
        visualizing = true;
    }

    private void VisualiseNextGesture()
    {
        currentIndex++;
    }

    void OnDrawGizmos()
    {
        if (visualizing)
        {
            Vector3 position = new Vector3(200, Screen.height - 100, 0);
            CustomGizmos.drawUIString(currentGesture.NumberPoints+"", position, Color.green);
            position += Vector3.down * 20;
            foreach (string error in currentErrors)
            {
                CustomGizmos.drawUIString(error, position, Color.green);
                position += Vector3.down * 20;
            }

            currentGesture.DrawGizmos();
        }
    }
}

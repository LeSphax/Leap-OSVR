using UnityEngine;
using System.Collections;

public class SaveOnStop : MonoBehaviour {

	void OnApplicationQuit()
    {
        GestureDataManager.Save();
    }
}

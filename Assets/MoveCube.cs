using UnityEngine;
using System.Collections;

public class MoveCube : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space))
        {
            transform.localPosition += 0.02f * Vector3.back;
        }
        if (Input.GetKey(KeyCode.Return))
        {
            transform.localPosition += 0.02f * Vector3.forward;
        }
    }
}

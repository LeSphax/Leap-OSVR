﻿using UnityEngine;
using System.Collections;
using Leap.Unity;
using GestureDetection;
using Leap;

public class FireballLauncher : MonoBehaviour {

    private GameObject FireballPrefab;

    private GameObject Fireball;

    private GestureDetector detector;

    // Use this for initialization
    void Start () {
        detector = GetComponent<GestureDetector>();
        FireballPrefab = Resources.Load<GameObject>("Fireball");
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.L))
        {
            detector.AddGesture("Load",CreateFireball);
            Debug.Log("SAVE");

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            detector.AddGesture("Send",LaunchFireball);
            Debug.Log("SAVE");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            detector.AddGesture("Nothing",DoNothing);
            Debug.Log("SAVE");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GestureDataManager.Clear();
            Debug.Log("CLEAR");
        }
    }

    public void DoNothing(Hand hand)
    {
        Debug.LogWarning("Do Nothing");
    }

    public void CreateFireball(Hand hand)
    {
        Destroy(Fireball);
        Fireball = (GameObject)Instantiate(FireballPrefab, hand.PalmPosition.ToVector3(), Quaternion.identity);
    }

    public void LaunchFireball(Hand hand)
    {
        Fireball.GetComponent<Rigidbody>().velocity = Vector3.forward * 5f;
    }
}
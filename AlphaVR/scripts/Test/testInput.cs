using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    Debug.LogError("Current Key is : " + keyCode.ToString());
                }
            }
        }
    }
}

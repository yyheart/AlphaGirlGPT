using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testInteract : MonoBehaviour {
    Interaction theInteraction;
	// Use this for initialization
	void Start () {
        theInteraction = VRController.instance.GetComponent<Interaction>();
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

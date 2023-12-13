using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour {
   
    [SerializeField]
    private float walkSpeed = 3f;
    [SerializeField]
    private float rotateSpeed = 50f;
   
    
   
    private Transform Head;
    private Transform Hand;
    // Use this for initialization
    void Start () {
        Head = transform.Find("Head");
        Hand = transform.Find("Hand");

    }
	
	// Update is called once per frame
	void Update () {

        
          


            
                transform.position = transform.position + Hand.forward * AlphaMotion.instance.GetAnalog(1) * Time.deltaTime * walkSpeed;
            
            
            
                transform.Rotate(0, AlphaMotion.instance.GetAnalog(0) * Time.deltaTime * rotateSpeed, 0, Space.World);

            //if (AlphaMotion.instance.GetButton(1))
            //{
            //    print("anxiaoadjaodfjasf");
            //}



        

        
    }
}


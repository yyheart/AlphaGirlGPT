using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotation : MonoBehaviour
{
    bool turnArround = false;
    public Transform Player;         
    public Transform LookTarget;     
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(TurnArround());
    }
    IEnumerator TurnArround() 
    {
        Player.rotation = Quaternion.Slerp(Player.rotation, 
                                Quaternion.LookRotation(new Vector3(LookTarget.position.x - Player.position.x, 0, LookTarget.position.z - Player.position.z)),
                                5.0f * Time.deltaTime);

        yield return new WaitForSeconds(1f);
        turnArround = false;
    }
}

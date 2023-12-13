using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Validate.OnValidateSuccess += () =>
        {
            print("验证成功");
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

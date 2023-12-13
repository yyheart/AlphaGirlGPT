using CrazyMinnow.SALSA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickBtn : VRActorBase
{
    private Salsa3D aaa;

    // Start is called before the first frame update
    void Start()
    {
        aaa = GameObject.FindObjectOfType<Salsa3D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnActorClicked(Transform trans)
    {
        if (aaa != null)
        {
            aaa.Play();
        }
    }
}

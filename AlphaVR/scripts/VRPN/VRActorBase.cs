using UnityEngine;
using System.Collections;

public class VRActorBase : MonoBehaviour {
    public virtual void OnActorPressed(Transform trans, bool status)
    {
        Debug.Log(string.Format("{0} Press Status: {1}", trans, status));
    }
    public virtual void OnActorHovered(Transform trans, bool status)
    {
        Debug.Log(string.Format("{0} Hover Status: {1}", trans, status));
    }
    public virtual void OnActorClicked(Transform trans)
    {
        Debug.Log(string.Format("{0} is Clicked", trans));
    }
}

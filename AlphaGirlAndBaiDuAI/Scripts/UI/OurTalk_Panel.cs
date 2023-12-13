using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OurTalk_Panel : MonoBehaviour
{
    public Text ourTalkText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetOurTalkText(string str)
    {
        ourTalkText.text = str;
    }
}

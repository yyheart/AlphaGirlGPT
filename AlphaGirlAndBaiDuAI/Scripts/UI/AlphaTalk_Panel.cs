using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaTalk_Panel : MonoBehaviour
{
    public Text alphaTalk_Text;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetAlphaTalkText(string str)
    {
        alphaTalk_Text.text = str;
    }
}

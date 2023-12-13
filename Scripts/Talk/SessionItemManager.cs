using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SessionItemManager : MonoBehaviour
{
    private Button btn_SessionItem;
    private Text text_SessionContent;

    public void Init()
    {
        btn_SessionItem = GetComponent<Button>();
        text_SessionContent = GetComponentInChildren<Text>();
        btn_SessionItem.onClick.AddListener(OnClickButton);
    }

    public void SetData(string name)
    {
        text_SessionContent.text = name;

    }

    private void OnClickButton()
    {
        TalkPanelManager.Instance.OnYuYinShiBieResultEx(text_SessionContent.text);
    }
}

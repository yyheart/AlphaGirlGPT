using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public struct ToneType 
{
    public string toneName;
    public string toneId;
}

public class ToneShiftPanelManager : MonoBehaviour
{
    public List<ToneType> toneTypeList = new List<ToneType>();
    private List<Toggle> toggleList = new List<Toggle>();
    private Transform ToneTypeTemplate;
    private Transform contentParent;
    private ToggleGroup toggleGroup;
    private string currentToneID;
    private void Start()
    {
        ToneTypeTemplate = transform.ZYFindChild("Toggle_ToneShiftTemplate");
        contentParent = transform.ZYFindChild("Content");
        toggleGroup = contentParent.GetComponent<ToggleGroup>();
        Init();
    }
    private void Init()
    {

        for (int i = 0; i < toneTypeList.Count; i++)
        {
            int x = i;
            GameObject go = GameObject.Instantiate(ToneTypeTemplate, contentParent).gameObject;
            go.SetActive(true);
            go.GetComponentInChildren<Text>().text = toneTypeList[x].toneName;
            Toggle toggle_Go = go.GetComponent<Toggle>();
            toggle_Go.isOn = false;
            toggle_Go.group = toggleGroup;
            toggle_Go.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    currentToneID= toneTypeList[x].toneId;
                }
                BaiDuAI.Instance.SetCurrentTone(currentToneID);
               
            });
            toggleList.Add(toggle_Go);
        }
      
    }
    
}

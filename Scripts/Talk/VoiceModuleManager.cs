using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public struct VoiceModuleItem
{
    
    public string skillName;
    public string skillId;
}
public class VoiceModuleManager : MonoBehaviour
{
   public List<VoiceModuleItem> voiceModuleList=new List<VoiceModuleItem>();
   public List<Toggle> toggleList=new List<Toggle>();
   private List<string> skill_IdList=new List<string>();
   private Transform VoiceModuleTemplate;
   private Transform contentParent;
    private ToggleGroup toggleGroup;
    private void Start()
    {
        VoiceModuleTemplate = transform.ZYFindChild("Toggle_VoiceModuleTemplate");
        contentParent = transform.ZYFindChild("Content");
        toggleGroup=contentParent.GetComponent<ToggleGroup>();
        Init();
    }
    private void Init()
    {
        
        for (int i = 0; i < voiceModuleList.Count; i++)
        {
            int x = i;
            GameObject go = GameObject.Instantiate(VoiceModuleTemplate, contentParent).gameObject;
            go.SetActive(true);
            go.GetComponentInChildren<Text>().text = voiceModuleList[x].skillName;
            Toggle toggle_Go = go.GetComponent<Toggle>();
            toggle_Go.isOn = false;
            toggle_Go.group = toggleGroup;
            toggle_Go.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    skill_IdList.Add(voiceModuleList[x].skillId);
                    //SessionListManager.Instance.SetSessionType(voiceModuleList[x].skillName);
                    GameManager.Instance.mSessionListManager.SetSessionType(voiceModuleList[x].skillName);
                }
                else
                {
                    skill_IdList.Remove(voiceModuleList[x].skillId);
                   
                }
                BaiDuAI.Instance.SetSkill_IdList(skill_IdList);
                CheckToggleGroupIsOn();
            });
            toggleList.Add(toggle_Go);
            
        }
       
    }
    public void CheckToggleGroupIsOn()
    {
        bool isInit = true;
        for (int i = 0; i < toggleList.Count; i++)
        {
            if (toggleList[i].isOn)
            {
                isInit = false;
            }
        }
        if (isInit)
        {
            GameManager.Instance.mSessionListManager.SetSessionType("д╛хо");
        }
    }
}

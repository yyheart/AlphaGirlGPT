using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SessionListManager : MonoBehaviour
{
    private GameObject btn_SessionTemplate;
    private Transform contentParent;
    private List<Transform> sessionTransformList = new List<Transform>();
    private SessionItemGroup mSessionItemGroup;
    private string sessionType = "д╛хо";



    public void Init()
    {
        btn_SessionTemplate = transform.ZYFindChild("Btn_SessionTemplate").gameObject;
        contentParent = transform.ZYFindChild("Content");
        mSessionItemGroup = DataManager.Instance.GetSessionItemGroup(sessionType);
        List<SessionItem> sessionItemList = mSessionItemGroup.sessionItemList;
        for (int i = 0; i < sessionItemList.Count; i++)
        {
            var item = GetSessionItem(i);
            item.SetData(sessionItemList[i].SessionContent);
            
        }
        if (sessionTransformList.Count > sessionItemList.Count)
        {
            for (int i = sessionTransformList.Count; i > sessionItemList.Count; i--)
            {
                sessionTransformList[i - 1].gameObject.SetActive(false);
            }
        }


    }

    public SessionItemManager GetSessionItem(int index)
    {
        SessionItemManager item = null;
        if (sessionTransformList.Count > index)
        {
            item = sessionTransformList[index].GetComponent<SessionItemManager>();
          

        }
        else
        {
           
            GameObject sessionItemGo = GameObject.Instantiate(btn_SessionTemplate, contentParent);
            item = sessionItemGo.AddComponent<SessionItemManager>();
            item.Init();
            sessionTransformList.Add(sessionItemGo.transform);
        }
        item.gameObject.SetActive(true);
        return item;
    }



    public void SetSessionType(string type)
    {
        sessionType = type;

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SessionItem
{
    public string SessionContent;
}
[System.Serializable]
public class SessionItemGroup
{
    public string sessionType;
    public List<SessionItem> sessionItemList;
    public Dictionary<string, SessionItem> sessionItemDic;
    public SessionItemGroup()
    {
        sessionItemList = new List<SessionItem>();
        sessionItemDic = new Dictionary<string, SessionItem>();
    }
    public void InitData()
    {
        for (int i = 0; i < sessionItemList.Count; i++)
        {
            if (!sessionItemDic.ContainsKey(sessionItemList[i].SessionContent))
            {
                sessionItemDic.Add(sessionType, sessionItemList[i]);
            }
            else
            {
                Debug.LogErrorFormat("SessionItem具有相同键{0}",sessionItemList[i].SessionContent);
            }
        }
    }
}

[CreateAssetMenu(fileName ="SessionListHouseConfig",menuName="CreatSessionListHouse",order =0)]
public class SessionListHouse : ScriptableObject
{
    public List<SessionItemGroup> sessionItemGroupList;
    private Dictionary<string, SessionItemGroup> sessionItemDic;
    public SessionListHouse()
    {
        sessionItemGroupList = new List<SessionItemGroup>();
        for (int i = 0; i < sessionItemGroupList.Count; i++)
        {
            sessionItemGroupList[i].InitData();
        }
    }
    public void InitData()
    {
        sessionItemDic = new Dictionary<string, SessionItemGroup>();
        for(int i = 0; i < sessionItemGroupList.Count; i++)
        {
            
            if (!sessionItemDic.ContainsKey(sessionItemGroupList[i].sessionType))
            {
                sessionItemDic.Add(sessionItemGroupList[i].sessionType, sessionItemGroupList[i]);
            }
            else
            {
                Debug.LogErrorFormat("SessionItemGroup具有相同键：{0}", sessionItemGroupList[i]);
            }

        }
    }
    public SessionItemGroup GetSessionItemGroup(string sessionType)
    {
        if (!sessionItemDic.ContainsKey(sessionType))
        {
            Debug.LogErrorFormat("sessionItemDic没有找到键：{0}", sessionType);
            return null;
        }
        else
        {
            return sessionItemDic.TryGet(sessionType);
        }

    }
}

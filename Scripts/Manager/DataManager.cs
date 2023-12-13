using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonAuto<DataManager>
{
    private SessionListHouse mSessionListHouse;
    protected override void Awake()
    {
        base.Awake();
        mSessionListHouse = Resources.Load<SessionListHouse>("Config/SessionListHouseConfig");
        if (mSessionListHouse == null)
        {
            Debug.LogError("SessionListHouseÎª¿Õ");
        }
        else
        {
            mSessionListHouse.InitData();

        }
    }
    public SessionItemGroup GetSessionItemGroup(string sessionType)
    {
        return mSessionListHouse.GetSessionItemGroup(sessionType);

    }
}

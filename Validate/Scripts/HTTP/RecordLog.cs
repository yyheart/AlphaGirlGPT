using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
public class RecordLog : MonoBehaviour {
   // string Logurl = "http://47.92.236.31:21016/admin/diary/setdiary?diaryStr=";
    string Logurl ="";

    string logPath;
    private string device;
    //有网络就直接上传，没有网络就保存本地；
    public event UnityEngine.Events.UnityAction OnUpdated;
    public void UpdateLog(string log)
    {
        StartCoroutine(Updating(Logurl + device + log+":" + TimeTools.ConvertDateTimeToInt().ToString()));

    }

    IEnumerator Updating(string str)
    {
       
        UnityWebRequest request = UnityWebRequest.Get(str);
       
        yield return request.SendWebRequest();
       
         if (request.isNetworkError ||request.isHttpError)
        {
           
            //安卓设备没有接入外网，开始本地连接
            Debug.LogError(request.error);
            SaveLog(logPath, str.Split(';')[1]);
        }
        else
        {
           
            string receiveContent = request.downloadHandler.text;
           

            // print(result["code"].ToString());
            if (receiveContent.CompareTo("false") == 0)
            {
                SaveLog(logPath, str.Split(';')[1]);
            }
            else
            {
                string log = ReadLog(logPath);
                if (!string.IsNullOrEmpty(log))
                {
                    UpdateLog(log);
                }
            }
           
        }

        if (OnUpdated !=null)
        {
            OnUpdated();
        }
    }

    private void SaveLog(string path, string content)
    {
       
            File.AppendAllText(path, content + ",");
        
        
    }

    private string ReadLog(string path)
    {
        string str = null;
        if (File.Exists(path))
        {
            str = File.ReadAllText(path);
            str = str.Substring(0, str.Length - 1);
            File.Delete(path);
        }
        return str;
    }

    // Use this for initialization
    //void Start()
    //{
    //    //logPath = Application.dataPath + "/Managed/Mono.Data.Log.dll";
    //    StartCoroutine(Updating("http://47.92.236.31:21027/admin/diary/setdiary?diaryStr=ec413511470c5f8ba40c29b3028c8320;Unity2034:1668836337"));
    //}
    public void LogInit(string D,string url)
    {
#if UNITY_EDITOR
        logPath = Application.dataPath + "/Validate/Managed/Mono.Data.Log.dll";
#else
        logPath = Application.dataPath + "/Managed/Mono.Data.Log.dll";
#endif
        device = D+";";
        Logurl = url;
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}

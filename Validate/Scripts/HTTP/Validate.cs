using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using System.IO;
using UnityEngine.UI;
using System.Diagnostics;
[RequireComponent(typeof(Text))]
public class Validate : MonoBehaviour
{
    public delegate void ValidateOK();
    public static event ValidateOK OnValidateSuccess;
    /*
     * 第一次运行时需要注册，下载LOGO；
     * 以后开启后需要进行验证，有网时联网验证，没网时本地验证
     
         
         */
    // private string RegistUrl = "http://47.92.236.31:21027/admin/device/setdevice?number=";
    //private string CheckUrl = "http://47.92.236.31:21027/admin/device/getdevice?number=";
    private string RegistUrl = "";
    private string CheckUrl = "";
    private string LogUrl = "";
    private string device;
    private Text ShowTip;
    string path = "";
    private void Awake()
    {
        if (ShowTip == null)
        {
            ShowTip = GetComponent<Text>();
        }

        path = Application.dataPath + "/Managed/ASPOSE.exe";
#if UNITY_EDITOR
       path = Application.dataPath + "/Validate/Plugins/ASPOSE.exe";
      //  path = Application.dataPath + "/../Tools/ASPOSE.exe";

#endif
        if (PlayerPrefs.HasKey("Device"))
        {
            device = PlayerPrefs.GetString("Device");
            return;

        }
            
        if (File.Exists(path))
        {
         
            string content = OpenArguments(path, "CheckDevice");

            if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
            {
                if (!PlayerPrefs.HasKey("DeviceNull"))
                {
                    string tempDevice = MD5Tools.GetMD5(System.DateTime.Now.ToShortDateString(), 2);

                    PlayerPrefs.SetString("DeviceNull", tempDevice);
                    PlayerPrefs.Save();
                    content = tempDevice + "ev";
                }
                else
                {
                    content = PlayerPrefs.GetString("DeviceNull") + "tv";
                }

            }

            device = content.Substring(0, content.Length - 2);
            //print(device);

            /*
            print(content);
            if (content.EndsWith("tv"))
            {
                device = content;
                ShowTip.text = "";
                //  isstart = true;
                ShowTip.transform.gameObject.SetActive(false);
                // SceneManager.LoadScene(1);
            }
            else
            {
                ShowTip.text = "联系版权方注册设备：" + content;
                File.WriteAllText("DeviceID", content);
                ShowTip.gameObject.SetActive(true);
                Invoke("ExitFunc", 4);
                //StartCoroutine(delayLoad(3));
            }
            */
        }
        else
        {
            Showing("缺少系统文件！");
            Invoke("ExitFunc", 2);
        }
    }
    public  string OpenArguments(string Path, string arguments = null)
    {
        Process p = new Process();
        //设置.net的程序路径
        p.StartInfo.FileName = Path;
        p.StartInfo.Arguments = arguments;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.StandardOutputEncoding = System.Text.Encoding.Unicode;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        p.Start();
        StreamReader s = p.StandardOutput;
        p.WaitForExit();
        string content = s.ReadToEnd();
        // Manager(s.ReadToEnd());
        s.Close();

        // UnityEngine.Debug.Log(content);
        return content;
    }
    public void ExitFunc()
    {
      //  PlayerPrefs.DeleteAll();
        Application.Quit();
#if UNITY_EDITOR
        //  StartCoroutine(LoadNextScene("main"));
#endif
    }

    private IEnumerator Start()
    {

        //        device = "147258369";
        //#if UNITY_EDITOR
        //        device = "147258369";
        //#elif UNITY_ANDROID
        //       device =  Pvr_UnitySDKAPI.System.UPvr_GetDeviceSN();
        //#else
        //                  device = SystemInfo.deviceUniqueIdentifier;
        //#endif
        //新设备
        if (ShowTip==null)
        {
            ShowTip = GetComponent<Text>();
        }
        transform.SetAsLastSibling();
        yield return new WaitForSeconds(2);
        string urls = "";
        if ( !PlayerPrefs.HasKey("URLs"))
        {
           
                 urls = OpenArguments(path, "GetURLs");
                PlayerPrefs.SetString("URLs", urls);
                PlayerPrefs.Save();
               
              //  print(urls);
            

        }
        else
        {
            urls = PlayerPrefs.GetString("URLs");
        }
        string[] allURLs = urls.Split(new string[] { "###" },System.StringSplitOptions.RemoveEmptyEntries);
        RegistUrl = allURLs[0];
        CheckUrl = allURLs[1];
        LogUrl = allURLs[2];
       // print(RegistUrl);
       // print(CheckUrl);
       ////// device = WordTools.CheckDevice;
       //if (string.IsNullOrEmpty(device))
       //{

        //    PlayerPrefs.DeleteAll();
        //}
        //  else
        // {
        if (!PlayerPrefs.HasKey("Device"))
            {
                //安卓设备没有连接WIFI，PC端是没有连接外网
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    Showing( "打开设置连接WIFI,注册设备");
                    StartCoroutine(WaitWIFI());

                    //  StartCoroutine( delayQuit(3));
                }

                else
                {
                    StartCoroutine(Regist(RegistUrl + device));
                }
        }
            else
            {
               

                //  MsgCenter.Instance.Dispatch(AreaCode.APP, AppEvent.SHOWTIP, "验证");
                //安卓设备没有连接WIFI，PC端是没有连入外网
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    //本地验证
                    LocalCheck();
                }
                else
                {
                    
                    StartCoroutine(Check(CheckUrl + device));
                }
          //  }
        }
        
       
        
        
        // StartCoroutine(Get(RegistUrl+SystemInfo.deviceUniqueIdentifier));
        //StartCoroutine(Get(RegistUrl + "147258369"));
        // StartCoroutine(Get(CheckUrl + "147258369"));
      //  StartCoroutine(GetHTTP(CheckUrl + SystemInfo.deviceUniqueIdentifier));


    }
    void Showing(string content)
    {
        ShowTip.text = content;
        ShowTip.gameObject.SetActive(true);
    }
    IEnumerator WaitWIFI()
    {
        yield return new WaitForSeconds(1);
        Showing("等待网络连接");
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return null;
        }
        yield return null;
        StartCoroutine(Regist(RegistUrl + device));
    }

    void LocalCheck()
    {
        if (!CheckingTime())
        {
           // Showing( "验证失败！");
            StartCoroutine(delayQuit(3));
        }
        else
        {
           
            //testShow.Showtext = "本地验证成功！";
            StartCoroutine(Flash(3));
        }
    }

    IEnumerator delayQuit(float t)
    {
       
        yield return new WaitForSeconds(t);
        Application.Quit();
    }

    IEnumerator GetHTTP(string str)
    {
        UnityWebRequest request = UnityWebRequest.Get(str);
        yield return request.SendWebRequest();
        if (request.isNetworkError )
        {
           UnityEngine.Debug.LogError(request.error);
        }
        else
        {
            string receiveContent = request.downloadHandler.text;
            print(receiveContent);
            JsonData result = JsonMapper.ToObject(receiveContent);
            print(result["code"].ToString());
           
        }
    }

    IEnumerator Regist(string str)
    {
        SaveLast();
        UnityWebRequest request = UnityWebRequest.Get(str);
        yield return request.SendWebRequest();
        if (request.isNetworkError )
        {
            //安卓没有接入外网
            UnityEngine.Debug.LogError(request.error);
           Showing("注册网络异常："+ request.error);
            StartCoroutine(delayQuit(3));

        }
        else
        {
            string receiveContent = request.downloadHandler.text;

           
            if (string.Compare(receiveContent,"true")==0)
            {
                //注册成功
                
                
              //  StartCoroutine(Flash(3));
                PlayerPrefs.SetString("Device", device);
                PlayerPrefs.Save();
            }
            else
            {
                
               // Showing("注册失败！");
              //  StartCoroutine(delayQuit(3));
            }
            StartCoroutine(Check(CheckUrl + device));

        }
    }

    IEnumerator Check(string str)
    {
       
        UnityWebRequest request = UnityWebRequest.Get(str);
        yield return request.SendWebRequest();
        if (request.isNetworkError )
        {
            //安卓设备没有接入外网，开始本地连接
            UnityEngine.Debug.LogError(request.error);
           // Showing("验证网络异常：" + request.error);
            LocalCheck();
        }
        else
        {
            string receiveContent = request.downloadHandler.text;
            
           
            // print(result["code"].ToString());
            if (receiveContent.CompareTo("false") == 0)
            {
                Showing( "网络验证失败！");
                File.WriteAllText("DeviceID", device+"fv");
                StartCoroutine(delayQuit(3));
               
            }
            else
            {
                if (!PlayerPrefs.HasKey("Device"))
                {
                    PlayerPrefs.SetString("Device", device);
                    PlayerPrefs.Save();
                }
                
                //testShow.Showtext = "网络验证成功！";
                //从网站拿到截止日期，进行比对，保存截止日期
                string[] recvdata = receiveContent.Split(':');
                long endTime = 0;
              
                if (long.TryParse(recvdata[recvdata.Length - 1], out endTime))
                {

                    
                    SaveEnd(endTime);
                    LocalCheck();
                  //  StartCoroutine(Flash(3));
                } ;
               
                
            }
#if UNITY_EDITOR
          //  StartCoroutine(Flash(3));
#endif
        }
    }
    //IEnumerator DownloadImage(string imgPath,string fileName)
    //{

    //    UnityWebRequest www = UnityWebRequestTexture.GetTexture(imgPath);
    //    yield return www.SendWebRequest();

    //    if (www.isNetworkError || www.isHttpError)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
    //        File.WriteAllBytes(fileName, myTexture.EncodeToPNG());
    //      //  Sprite createSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));
    //      //  gameObject.GetComponent<Image>().sprite = createSprite;
    //    }
    //}

//    IEnumerator ShowImage(string imgPath, Image img )
//    {
        
//#if UNITY_ANDROID
//        imgPath = "file:///"+ imgPath ;
//#endif
//        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imgPath);
//        yield return www.SendWebRequest();

//        if (www.isNetworkError || www.isHttpError)
//        {
           
//            Debug.Log(www.error);
//        }
//        else
//        {
//            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
           
//            Sprite createSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));
            
//            img.GetComponent<RectTransform>().sizeDelta = new Vector2(myTexture.width, myTexture.height);
//            img.sprite = createSprite;
           
//        }
        


//    }

    IEnumerator Flash(float t)
    {
        ShowTip.text = "";
        yield return null;
        // float total = t;
        //// CanvasGroup canvasGroup = gameObject.AddComponent<CanvasGroup>();
        // while (t>0)
        // {
        // //    canvasGroup.alpha = t / total;
        //     t -= Time.deltaTime;
        //     yield return null;
        // }

        // yield return null;
        // gameObject.SetActive(false);
        RecordLog recordLog = gameObject.GetComponent<RecordLog>();
        if (recordLog==null)
        {
            recordLog = gameObject.AddComponent<RecordLog>();
        }
        
        recordLog.LogInit(device, LogUrl);
        //  recordLog.UpdateLog(Application.productName);
      //  print(Process.GetCurrentProcess().MainModule.FileName);
        recordLog.UpdateLog(Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName) );
        //  SendMessage("LogInit", SendMessageOptions.DontRequireReceiver);
        //  SendMessage("UpdateLog", Application.productName, SendMessageOptions.DontRequireReceiver);
        // SendMessage("StartGame",SendMessageOptions.DontRequireReceiver);
        if (OnValidateSuccess!=null)
        {
           // OnValidateSuccess.Invoke();
            OnValidateSuccess();
        }  
         yield return null;
        recordLog.OnUpdated += ()=>
        ShowTip.gameObject.SetActive(false);
    }
    private void SaveLast()
    {
        //往注册表和文件夹里各存一份
        string last = TimeTools.ConvertDateTimeToInt().ToString();
        byte[] lastbyte = System.Text.Encoding.UTF32.GetBytes(last);
        for (int i = 0; i < lastbyte.Length; i++)
        {
            if (lastbyte[i] > 47)
            {
                lastbyte[i] = (byte)((lastbyte[i] - 48) * 13 + 5);
            }
            else
            {
                int a = Random.Range(0, 2);
                lastbyte[i] = (byte)(a > 0 ? Random.Range(0, 5) : Random.Range(123, 128));
            }

           
        }
        PlayerPrefs.SetString("LastTime",MD5Tools.GetMD5( last,2));
        PlayerPrefs.Save();
#if UNITY_EDITOR
        File.WriteAllBytes(Application.dataPath + "/Validate/Managed/Mono.Data.Last.dll", lastbyte);
        //  path = Application.dataPath + "/../Tools/ASPOSE.exe";
#else
    File.WriteAllBytes(Application.dataPath + "/Managed/Mono.Data.Last.dll", lastbyte);
#endif


    }
    private void SaveEnd(long end)
    {
        //往注册表和文件夹里各存一份
        string endStr = end.ToString();
        byte[] endbyte = System.Text.Encoding.UTF32.GetBytes(endStr);
        
        for (int i = 0; i < endbyte.Length; i++)
        {
            if (endbyte[i]>47)
            {
                endbyte[i] = (byte)((endbyte[i] - 48) * 13 + 5);
            }
            else
            {
                int a = Random.Range(0, 2);
                endbyte[i] = (byte)(a > 0 ? Random.Range(0, 5) : Random.Range(123, 128));
            }
            
           
        }
        PlayerPrefs.SetString("EndTime",MD5Tools.GetMD5( endStr,2));
        PlayerPrefs.Save();
#if UNITY_EDITOR
        
        File.WriteAllBytes(Application.dataPath + "/Validate/Managed/Mono.Data.End.dll", endbyte);
        //  path = Application.dataPath + "/../Tools/ASPOSE.exe";
#else
    File.WriteAllBytes(Application.dataPath + "/Managed/Mono.Data.End.dll", endbyte);
#endif


    }
    private bool CheckingTime()
    {
        string lastpath, endpath;
#if UNITY_EDITOR
        lastpath = Application.dataPath + "/Validate/Managed/Mono.Data.Last.dll";
        endpath = Application.dataPath + "/Validate/Managed/Mono.Data.End.dll";
#else
        lastpath = Application.dataPath + "/Managed/Mono.Data.Last.dll";
        endpath = Application.dataPath + "/Managed/Mono.Data.End.dll";
#endif
        bool result = false;
        //if (CheckTime("LastTime", lastpath, true)) {
        //    if ( CheckTime("EndTime", endpath, false))
        //    {
        //        result = true;
        //    }
        //    else
        //    {
        //        Showing("软件到期，请联系厂家！");
        //        result = false;
        //    }
        //}
        //else
        //{
        //    Showing("日期错误，请联系厂家！");
        //    result = false;
        //}
        //SaveLast();
        if (CheckTime("EndTime", endpath, false))
        {
            if (File.Exists(lastpath))
            {
                if (CheckTime("LastTime", lastpath, true))
                {
                    result = true;
                }
                else
                {
                    Showing("日期错误，请联系厂家！");
                    result = false;
                }
            }
            else
            {
              
                result = true;
            }
        }
        else
        {
            //File.WriteAllText("DeviceID", device + "dv");
            Showing("软件到期，请联系厂家！\n"+ device + "dv");
            result = false;
        }
        SaveLast();
        return result;
    }
    private bool CheckTime(string lastKey, string lastPath,bool big)
    {
        bool result = false;
        long lastTime = 0;
        
       // string lastKey = "LastTime";
       // string lastPath = Application.dataPath + "/Managed/Mono.Data.Last.dll";
        //大于上一次的时间，小于截止时间
        if (PlayerPrefs.HasKey(lastKey))
        {
           
            if (File.Exists(lastPath))
            {
               
                string lastStr = PlayerPrefs.GetString(lastKey);
                byte[] tempByte = File.ReadAllBytes(lastPath);
               
                for (int i = 0; i < tempByte.Length; i++)
                {
                   // print(tempByte[i]);
                    if (tempByte[i]>4 && tempByte[i] < 123)
                    {
                        tempByte[i] = (byte)((tempByte[i] - 5) / 13 + 48);
                    }
                    else
                    {
                        tempByte[i] = 0;
                    }
                    
                   
                }
                string tempfile = System.Text.Encoding.UTF32.GetString(tempByte);
                
                if (MD5Tools.Check(tempfile, lastStr,2))
                {
                   
                    if (long.TryParse(tempfile, out lastTime))
                    {
                        long curTime = TimeTools.ConvertDateTimeToInt();
                      
                        if (big)
                        {
                            if (curTime > lastTime)
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            if (curTime < lastTime)
                            {
                                result = true;
                            }
                        }
                        
                    }  
                }  
            }
        }
        return result;
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}

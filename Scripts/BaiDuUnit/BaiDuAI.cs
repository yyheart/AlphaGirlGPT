using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Collections;
using LitJson;

public class BaiDuAI : SingletonAuto<BaiDuAI>
{
    protected override void Awake()
    {
        base.Awake();
        //myRobotID = XMLManager.Instance.GetStringParam("我的机器人ID", "S84966");
        //apiKey = XMLManager.Instance.GetStringParam("语音技术APIKey", "ijOO7KGpogkZYPfh6L5DK8rB");
        //secretKey = XMLManager.Instance.GetStringParam("语音技术SecretKey", "Dj1rVAcjmGt5EN6xqxGGA8l2fc5fDTuH");
        //clientId = XMLManager.Instance.GetStringParam("智能对话UNITAPIKey", "bMH0tRWPgEghYch5NqDvQWMW");
        //clientSecret = XMLManager.Instance.GetStringParam("智能对话UNITSecretKey", "rRA4Djwvjjd3CIz3RrcAEAHe0GgQQ130");
        //WXQF_APIKey = XMLManager.Instance.GetStringParam("文心千帆APIKey", "x83UuBuid0PoT9uaOhrYvKFk");
        //WXQF_SecretKey = XMLManager.Instance.GetStringParam("文心千帆SecretKey", "GnuvwkS3wp6oWHpYMjFg7X1RLGsifEV2");

        string[] data = File.ReadAllLines(Application.streamingAssetsPath + "/百度智能语音助手.txt");
        apiKey = data[0].Split(':')[1];
        secretKey = data[1].Split(':')[1];
        clientId = data[2].Split(':')[1];
        clientSecret = data[3].Split(':')[1];
        myRobotID = data[4].Split(':')[1];
        WXQF_APIKey = data[5].Split(':')[1];
        WXQF_SecretKey = data[6].Split(':')[1];
    }


    //开始对话 Index（用户开始录音一次 算一次）
    public int StartIndex;
    //结束对话 Index（AI回答消息回调一次 算一次）
    public int EndIndex;

    //语音识别成功 要做的事情
    public Action<string> YuYinShiBieResult = null;
    //AI回答成功 要做的事情
    public Action<string> AIAnswerResult = null;
    //文字转语音成功 要做的事情
    public Action<AudioClip> WenZiToAudio = null;

    //接收到数据要做的事
    public Action<byte[]> OnRecvData = null;

    //private void Start()
    //{
    //    string[] data = File.ReadAllLines(Application.streamingAssetsPath + "/百度智能语音助手.txt");
    //    apiKey = data[0].Split(':')[1];
    //    secretKey = data[1].Split(':')[1];
    //    clientId = data[2].Split(':')[1];
    //    clientSecret = data[3].Split(':')[1];
    //    myRobotID = data[4].Split(':')[1];
    //    WXQF_APIKey = data[5].Split(':')[1];
    //    WXQF_SecretKey = data[6].Split(':')[1];
    //}


    //1
    #region 百度智能语音助手-语音识别（将录音内容识别成文字）
    //百度智能语音助手令牌-apiKey
    private string apiKey = string.Empty;

    //百度智能语音助手令牌-secretKey
    private string secretKey = string.Empty;

    //百度智能语音助手令牌 AccessToken令牌
    private string accessToken_ = string.Empty;

    /// <summary>
    /// 获取 百度智能语音助手AccessToken
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAccessToken()
    {
        Debug.Log("百度智能语音语音助手-获取Token开始\n");
        var uri = string.Format("https://aip.baidubce.com/oauth/2.0/token?client_id={0}&client_secret={1}&grant_type=client_credentials", apiKey, secretKey);
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(uri);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isDone)
        {
            var downloadHandler = unityWebRequest.downloadHandler.text;
            Match match = Regex.Match(downloadHandler, @"access_token.:.(.*?).,");
            if (match.Success)
            {
                //表示正则匹配到了accessToken
                accessToken_ = match.Groups[1].ToString();
                Debug.LogFormat("百度智能语音语音助手-获取Token成功\n\nToken：{0}\n内容：{1}", accessToken_, downloadHandler);
            }
            else
            {
                accessToken_ = string.Empty;
                Debug.LogErrorFormat("百度智能语音语音助手-获取Token错误!!!\n内容：{0}", downloadHandler);
                OnEndIndex();
            }
        }
        else
        {
            Debug.LogErrorFormat("百度智能语音语音助手-获取Token失败，消息错误!!!\n内容：{0}", unityWebRequest.error);
            OnEndIndex();
        }
        //Debug.Log(accessToken_);
    }



    //语音识别 录音频率,控制录音质量(8000,16000)
    private int recordFrequency = 8000;

    /// <summary>
    /// 发送语音识别请求
    /// </summary>
    /// <param name="_saveAudioClip"></param>
    /// <param name="_trueLength"></param>
    public void SendYuYinToBaiDu(AudioClip _saveAudioClip, int _trueLength)
    {
        StartCoroutine(_StartBaiduYuYin(_saveAudioClip, _trueLength));
    }

    /// <summary>
    /// 发起语音识别请求
    /// </summary>
    /// <returns></returns>
    /// <summary>
    /// 发起语音识别请求
    /// </summary>
    /// <returns></returns>
    IEnumerator _StartBaiduYuYin(AudioClip _saveAudioClip, int _trueLength)
    {
        if (string.IsNullOrEmpty(accessToken_))
        {
            yield return GetAccessToken();
        }

        string getResultFrom = string.Empty;

        //处理当前录音数据为PCM16
        float[] samples = new float[recordFrequency * _trueLength * _saveAudioClip.channels];
        _saveAudioClip.GetData(samples, 0);
        var samplesShort = new short[samples.Length];
        for (var index = 0; index < samples.Length; index++)
        {
            samplesShort[index] = (short)(samples[index] * short.MaxValue);
        }
        byte[] datas = new byte[samplesShort.Length * 2];
        Buffer.BlockCopy(samplesShort, 0, datas, 0, datas.Length);

        string url = string.Format("{0}?cuid={1}&token={2}", "https://vop.baidu.com/server_api", SystemInfo.deviceUniqueIdentifier, accessToken_);

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddBinaryData("audio", datas);

        UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, wwwForm);

        unityWebRequest.SetRequestHeader("Content-Type", "audio/pcm;rate=" + recordFrequency);

        yield return unityWebRequest.SendWebRequest();

        if (string.IsNullOrEmpty(unityWebRequest.error))
        {
            var downloadHandler = unityWebRequest.downloadHandler.text;
            if (Regex.IsMatch(downloadHandler, @"err_msg.:.success"))
            {
                Match match = Regex.Match(downloadHandler, "result.:..(.*?)..]");
                if (match.Success)
                {
                    var asrResult = match.Groups[1].ToString();
                    Debug.LogFormat("语音识别-成功：{0}\n内容：{1}", asrResult, downloadHandler);
                    YuYinShiBieResult?.Invoke(asrResult);
                }
                else
                {
                    //UIManager.Instance.ShowMsgUIAuto("输入语音内容异常，请重新输入");
                    Debug.LogErrorFormat("语音识别-回应异常!!!\n内容：{0}", downloadHandler);
                    OnEndIndex();
                }
            }
            else
            {
                Debug.LogErrorFormat("语音识别-回应失败\"success\"为空!!!\n内容：{0}", downloadHandler);
                OnEndIndex();
            }
        }
        else
        {
            Debug.LogErrorFormat("语音识别-回应消息失败!!!\n内容：{0}", unityWebRequest.error);
            OnEndIndex();
        }
    }
    #endregion 百度-语音识别（将录音内容识别成文字）


    //2
    #region 百度-Unit （智能对话功能）（根据文字做出回答）
    /// <summary>
    /// 人工智能对话功能
    /// </summary>
    /// <param name="mysay"></param>
    /// <param name="session_id"></param>
    /// <param name="action_id"></param>
    private string myRobotID;

    private List<string> skill_IdList = new List<string>();

    public void SetSkill_IdList(List<string> skill_IdList)
    {
        this.skill_IdList = skill_IdList;
    }



    //文字对话请求
    public void Unit_NLP(string mysay, string session_id = "", string action_id = "")
    {
        //string token = accessToken;
        string token = getAccessToken();
        string host = "https://aip.baidubce.com/rpc/2.0/unit/service/v3/chat?access_token=" + token;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
        request.Method = "post";
        request.ContentType = "application/json";
        request.KeepAlive = true;
        JsonData send = new JsonData();
        //    string str = "{\"log_id\":\"UNITTEST_10000\",\"version\":\"3.0\",\"service_id\":\"S82142\",\"session_id\":\"\",\"request\":{\"terminal_id\":\"88888\",\"query\":\"你好\",\"user_id\":\"88888\"},\"dialog_state\":{\"contexts\":{\"SYS_REMEMBERED_SKILLS\":[\"1057\"]}}}"; // json格式  s100000是你机器人的ID需要改  你好 是传入的对话

        send["version"] = "3.0";
        send["service_id"] = myRobotID;
        if (skill_IdList.Count != 0)
        {
            JsonData skill_idArray = new JsonData();
            for (int i = 0; i < skill_IdList.Count; i++)
            {
                skill_idArray.Add(skill_IdList[i]);
                Debug.Log(skill_IdList[i].ToString());
            }
            send["skill_ids"] = skill_idArray;
        }
        send["log_id"] = "UNITTEST_10000";
        send["session_id"] = "";
        //send["action_id"] = "1296240";
        send["request"] = new JsonData();
        send["request"]["terminal_id"] = "88888";
        send["request"]["user_id"] = "01";
        send["request"]["query"] = mysay;
        send["request"]["query_info"] = new JsonData();
        send["request"]["query_info"]["type"] = "TEXT";
        JsonData bot_session = new JsonData();
        bot_session["session_id"] = "";
        send["bot_session"] = JsonMapper.ToJson(bot_session);
        string sendStr = JsonMapper.ToJson(send);
        byte[] buffer = Encoding.UTF8.GetBytes(sendStr);
        request.ContentLength = buffer.Length;
        request.GetRequestStream().Write(buffer, 0, buffer.Length);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        var result = reader.ReadToEnd();
        Debug.Log(result);

        JsonDecode(result);
    }



    /// <summary>
    /// 对回复进行Json解析
    /// </summary>
    /// <param name="js"></param>
    /// <returns></returns>
    public List<string> JsonDecode(string js)
    {
        List<string> Says = new List<string>();

        //var json = SimpleJson.SimpleJson.DeserializeObject<SimpleJson.JsonObject>(js);
        var json = JsonMapper.ToObject(js);

        //if (json.ContainsKey("result"))
        if (json.ContainsKey("result"))
        {
            //var result = (SimpleJson.JsonObject)json["result"];
            var result = json["result"];
            if (result.ContainsKey("responses"))
            {
                //var resArray = (SimpleJson.JsonArray)result["response_list"];
                var resArray = result["responses"];
                //var res = (SimpleJson.JsonObject)resArray[0];
                var res = resArray[0];
                if (res.ContainsKey("actions"))
                {
                    //var actArray = (SimpleJson.JsonArray)res["action_list"];
                    var actArray = res["actions"];
                    //var act = (SimpleJson.JsonObject)actArray[0];
                    var act = actArray[0];
                    if (act.ContainsKey("say"))
                    {
                        var say = (string)act["say"];
                        Says.Add(say);
                        AIAnswerResult?.Invoke(say);
                    }
                }
            }
        }
        else
        {
            Debug.Log("没有Result");
        }
        return Says;
    }

    public static String TOKEN = String.Empty;
    // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务：oDQl4EoCfq5cWZiTACDqfiHU
    private static String clientId = "";
    // 百度云中开通对应服务应用的 Secret Key：6NdSNThqWGwUuvoPGjvbf0TyGbPQz4WT
    private static String clientSecret = "";

    /// <summary>
    /// 获取AccessToken请求令牌_连接UNIT机器人使用
    /// </summary>
    /// <returns></returns>
    public static String getAccessToken()
    {
        String authHost = "https://aip.baidubce.com/oauth/2.0/token";
        HttpClient client = new HttpClient();
        List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
        paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
        paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
        paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));

        HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
        String result = response.Content.ReadAsStringAsync().Result;

        string[] resultStr = result.Split(',');
        string[] tokenStr = resultStr[3].Split(':');
        TOKEN = tokenStr[1];
        return TOKEN;
    }
    #endregion 百度-Unit （智能对话功能）（根据文字做出回答）


    //2
    #region 百度-文心千帆（AI-ERNIE-Bot-turbo）（根据文字做出回答）
    //文心千帆 APIKey
    private string WXQF_APIKey = string.Empty;

    //文心千帆 SecretKey
    private string WXQF_SecretKey = string.Empty;


    //文心千帆 AccessToken
    private string WXQF_AccessToken = string.Empty;


    /// <summary>
    /// 请求 文心千帆 AccessToken
    /// </summary>
    /// <returns></returns>
    private IEnumerator WXQF_GetAccessToken()
    {
        Debug.LogFormat("文心千帆-获取Token开始\n");
        var url = string.Format("https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id={0}&client_secret={1}", WXQF_APIKey, WXQF_SecretKey);
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isDone)
        {
            var downloadHandler = unityWebRequest.downloadHandler.text;
            Match match = Regex.Match(downloadHandler, @"access_token.:.(.*?).,");
            if (match.Success)
            {
                //表示正则匹配到了accessToken
                WXQF_AccessToken = match.Groups[1].ToString();
                Debug.LogFormat("文心千帆-获取Token成功：{0}\n内容：{1}", WXQF_AccessToken, downloadHandler);
            }
            else
            {
                WXQF_AccessToken = string.Empty;
                Debug.LogErrorFormat("文心千帆-获取Token解析错误!!!\"access_token\"为空!!!\n内容：{0}", downloadHandler);
                OnEndIndex();
            }

            //Match match2 = Regex.Match(unityWebRequest.downloadHandler.text, @"expires_in.:(.*?),");
            //if (match2.Success)
            //{
            //    //表示正则匹配到了expires_in
            //    var expires_in = match2.Groups[1].ToString();
            //    Debug.Log(expires_in);
            //}
            //else
            //{
            //    Debug.LogError("验证错误,获取expires_in失败!!!");
            //}
        }
        else
        {
            Debug.LogErrorFormat("文心千帆-获取Token失败!!!\n内容：{0}", unityWebRequest.error);
            OnEndIndex();
        }
    }



    /// <summary>
    /// 请求 文心千帆 对话回答
    /// </summary>
    /// <param name="value"></param>
    public void RequestWXQFSpeak(string value)
    {
        StartCoroutine(WXQFSpeak(value));
    }

    private IEnumerator WXQFSpeak(string value)
    {
        if (string.IsNullOrEmpty(WXQF_AccessToken))
        {
            yield return WXQF_GetAccessToken();
        }

        Debug.LogFormat("文心千帆-回应开始\n内容：{0}", value);

        var messagesData = new messagesData();
        messagesData.messages = new List<message>();
        var message = new message();
        message.role = "user";
        message.content = value;
        messagesData.messages.Add(message);

        string json = JsonMapper.ToJson(messagesData);
        json = Regex.Unescape(json);

        var postBytes = Encoding.UTF8.GetBytes(json);
        var url = string.Format("https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/eb-instant?access_token={0}", WXQF_AccessToken);
        UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

        unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postBytes);
        unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isDone)
        {
            var downloadHandler = unityWebRequest.downloadHandler.text;
            Match match = Regex.Match(downloadHandler, @"result.:.(.*?).,");
            if (match.Success)
            {
                var answer = match.Groups[1].ToString();
                Debug.LogFormat("文心千帆-回应成功：{0}\n内容：{1}", answer, downloadHandler);
                AIAnswerResult?.Invoke(answer);
            }
            else
            {
                Debug.LogErrorFormat("文心千帆-回应异常!!!\n内容：{0}", downloadHandler);
                OnEndIndex();
            }
        }
        else
        {
            Debug.LogErrorFormat("文心千帆-回应消息失败!!!\n内容：{0}", unityWebRequest.error);
            OnEndIndex();
        }
    }
    #endregion


    //3
    #region 百度-合成语音(将文字合成语音)
    //合成语音 音色设置
    private string currentToneId = "103";

    /// <summary>
    /// 设置当前语音音色
    /// </summary>
    /// <param name="toneID"></param>
    public void SetCurrentTone(string toneID)
    {
        currentToneId = toneID;
    }

    /// <summary>
    /// 发起合成语音请求
    /// </summary>
    /// <param name="_text"></param>
    public void StartTTS(string _text, bool addDuiHuaIndex = false)
    {
        if (addDuiHuaIndex)
        {
            OnStartIndex();
        }
        StartCoroutine(_StartTTS(_text));
    }

    /// <summary>
    /// 发起合成语音请求
    /// </summary>
    /// <returns></returns>
    IEnumerator _StartTTS(string _text)
    {
        if (string.IsNullOrEmpty(accessToken_))
        {
            yield return GetAccessToken();
        }

        Debug.LogFormat("合成语音-开始\n内容：{0}", _text);

        var url = "https://tsn.baidu.com/text2audio";

        var param = new Dictionary<string, string>();
        param.Add("tex", _text);
        param.Add("tok", accessToken_);
        param.Add("cuid", SystemInfo.deviceUniqueIdentifier);
        param.Add("ctp", "0");
        param.Add("lan", "zh");
        param.Add("spd", "5");
        param.Add("pit", "5");
        param.Add("vol", "10");
        param.Add("per", currentToneId);
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_UWP
        param.Add("aue", "6"); //Windows设置为wav格式，移动端需要mp3格式
#endif
        int i = 0;
        foreach (var p in param)
        {
            url += i != 0 ? "&" : "?";
            url += p.Key + "=" + p.Value;
            i++;
        }
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_UWP  //根据不同平台，获取不同类型的音频格式
        var www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
#else
        var www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
#endif
        //Debug.Log("[WitBaiduAip]" + www.url);
        yield return www.SendWebRequest();
        OnEndIndex();
        if (EndIndex == StartIndex)
        {
            //if (www.result == UnityWebRequest.Result.ConnectionError)
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogErrorFormat("合成语音-消息失败!!!\n内容：{0}", www.error);
            }
            else
            {
                var type = www.GetResponseHeader("Content-Type");
                if (type.Contains("audio"))
                {
                    Debug.LogFormat("合成语音-成功\n内容：{0}", _text);
                    WenZiToAudio?.Invoke(DownloadHandlerAudioClip.GetContent(www));
                }
                else
                {
                    var textBytes = www.downloadHandler.data;
                    var errorText = Encoding.UTF8.GetString(textBytes);
                    Debug.LogErrorFormat("合成语音-解析错误\"audio\"为空!!!\n内容：{0}", errorText);
                    //Debug.LogErrorFormat("合成语音-解析错误\"audio\"为空!!!\n内容：{0}\n内容2：{1}", errorText, www.downloadHandler.text);
                }
            }
        }
    }
    #endregion 百度-合成语音(将文字合成语音)

    public void OnStartIndex()
    {
        StartIndex++;
    }

    public void OnEndIndex()
    {
        EndIndex++;
        if (EndIndex > StartIndex)
        {
            EndIndex = StartIndex;
        }
    }


    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Y))
    //    {
    //        RequestWXQFSpeak("哈喽");
    //    }
    //}
}

//文心千帆 对话消息内容
public class messagesData
{
    public List<message> messages;
}

public class message
{
    public string role;
    public string content;
}
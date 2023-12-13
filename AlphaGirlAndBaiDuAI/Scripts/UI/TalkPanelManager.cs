using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CrazyMinnow.SALSA;
using System.IO;
using UnityEngine.Networking;

public class TalkPanelManager : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public GameObject TalkPanelUI;
    public Button talk_Btn;
    private SocketServer server;
    
    private AudioClipToWAV toWAV;
    //正在录音时候的话筒动画
    public Transform talk_Anim;
    //是否正在录音
    bool isRecording = false;
    //标记是否有麦克风
    private bool isHaveMic = false;
    //当前录音设备名称
    string currentDeviceName = string.Empty;

    //上次按下时间戳
    double lastPressTimestamp = 0;

    //录音频率,控制录音质量(8000,16000)
    int recordFrequency = 8000;

    //录音的最大时长
    int recordMaxLength = 10;

    //实际录音长度(由于unity的录音需先指定长度,导致识别上传时候会上传多余的无效字节)
    //通过该字段,获取有效录音长度,上传时候剪切到无效的字节数据即可
    int trueLength = 0;
    float currentTime;
    //存储录音的片段
    [HideInInspector]
    public AudioClip saveAudioClip;

    public AlphaTalk_Panel AlphaTalk_Panel;
    public OurTalk_Panel OurTalk_Panel;

    public string beginAlphaGirlText = "你好!\n我是AlphaGirl，可以问我一些新闻、成语、日期等问题，我也可以讲笑话，或者跟我随便聊聊吧！";
    // Start is called before the first frame update

    /// <summary>
    /// 工具箱的引用
    /// </summary>
    private Toggle toggle_ShowTextBox;
    private Toggle toggle_ToneShift;
    private Toggle toggle_ToolBox;
    private Toggle toggle_Session;
    private Toggle toggle_VoiceModule;
    private Toggle toggle_Exit;
    private Transform toneShiftPanel;
    private Transform sessionPanel;
    private Transform voiceModulePanel;
    //子面板引用
    [HideInInspector]
    public SessionListManager sessionListManager;
    [HideInInspector]
    public VoiceModuleManager voiceModuleManager;
    private static TalkPanelManager instance;
    public static TalkPanelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TalkPanelManager>();
            }
            return instance;
        }
    }
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    void Start()
    {
        //子面板引用
        sessionListManager = transform.ZYFindChild("SessionListPanel").GetComponent<SessionListManager>();
        voiceModuleManager = transform.ZYFindChild("VoiceModuleListPanel").GetComponent<VoiceModuleManager>();
        talk_Anim.gameObject.SetActive(false);
        talk_Btn.onClick.AddListener(TalkBtnIsOnClick);
        toggle_ShowTextBox = transform.ZYFindChild("toggle_ShowTextBox").GetComponent<Toggle>();
        toggle_ShowTextBox.onValueChanged.AddListener(ControllTextBox);
        toggle_ToneShift = transform.ZYFindChild("toggle_ToneShift").GetComponent<Toggle>();
        toneShiftPanel = transform.ZYFindChild("ToneShiftPanel");
        toggle_ToneShift.onValueChanged.AddListener(ControllToneShiftPanel);
        toggle_Exit = transform.ZYFindChild("toggle_Exit").GetComponent<Toggle>();
        toggle_Exit.onValueChanged.AddListener(OnClosePanel);
        toggle_ToolBox = transform.ZYFindChild("toggle_ToolBox").GetComponent<Toggle>();
        sessionPanel = transform.ZYFindChild("SessionListPanel");
        toggle_Session = transform.ZYFindChild("toggle_Session").GetComponent<Toggle>();
        toggle_Session.onValueChanged.AddListener(Toggle_SessionOnClick);
        voiceModulePanel = transform.ZYFindChild("VoiceModuleListPanel");
        toggle_VoiceModule = transform.ZYFindChild("toggle_VoiceModule").GetComponent<Toggle>();
        toggle_VoiceModule.onValueChanged.AddListener(Toggle_VoiceModuleOnclick);
        toggle_ToolBox.onValueChanged.AddListener(Toggle_ToolBoxOnclick);
        toggle_ToolBox.isOn = false;
        UIDoTweenType.Instance.GameObjectDoScaleHide(toneShiftPanel.gameObject);
        UIDoTweenType.Instance.GameObjectDoScaleHide(sessionPanel.gameObject);
        UIDoTweenType.Instance.GameObjectDoScaleHide(voiceModulePanel.gameObject);
        UIDoTweenType.Instance.GameObjectDoScaleHide(OurTalk_Panel.gameObject);
        UIDoTweenType.Instance.GameObjectDoScaleHide(AlphaTalk_Panel.gameObject);
        //获取麦克风设备，判断是否有麦克风设备
        if (Microphone.devices.Length > 0)
        {
            isHaveMic = true;
            currentDeviceName = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("没有麦克风");
        }
        //TalkPanelUI.SetActive(false);
        OnEnter();

        server = gameObject.AddComponent<SocketServer>();
        toWAV = gameObject.AddComponent<AudioClipToWAV>();
    }

    public void Init()
    {
        Debug.Log("发送语音");
        //AlphaTalk_Panel.SetAlphaTalkText(beginAlphaGirlText);
        //BaiDuAI.Instance.StartTTS(beginAlphaGirlText);

        BaiDuAI.Instance.WenZiToAudio += PlayTalk;
        BaiDuAI.Instance.YuYinShiBieResult += OnYuYinShiBieResult;
        BaiDuAI.Instance.AIAnswerResult += AIHuiDa;
        //BaiDuAI.Instance.OnRecvData += SaveRecvDataAndTalk;
        BaiDuAI.Instance.OnRecvData += PlayChatGptResponse;
    }

    private void PlayTalk(AudioClip clip)
    {
        Debug.Log("模型开始讲话");
        Debug.Log($"语音回复时长：{Time.time - currentTime}s");
        currentTime = Time.time;
        GameManager.Instance.mAlphaGrilMove.PlayTalk(clip);
    }


    /// <summary>
    /// 录音按钮点击事件
    /// </summary>
    void TalkBtnIsOnClick()
    {
        isRecording = !isRecording;
        if (isRecording)
        {
            Debug.Log("开始录音");
            talk_Anim.gameObject.SetActive(true);
            talk_Btn.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            //停止播放alphaGirl
            AudioManager.Instance.OtherAudioSourceStop();
            GameManager.Instance.mAlphaGrilMove.StopTalk();
            BaiDuAI.Instance.OnStartIndex();
            //开始录音
            StartRecording();
        }
        else
        {
            Debug.Log("录音结束");

            //录音结束
            talk_Anim.gameObject.SetActive(false);
            talk_Btn.GetComponent<Image>().color = new Color(255, 255, 255, 255);

            //记录录制时长，时长>1则发送给ChatGPT，时长小于1则提示录音时长过短
            trueLength = EndRecording();
            if (trueLength > 1)
            {
                Debug.Log("录制时长：" + trueLength);
                currentTime = Time.time;
                // BaiDuAI.Instance.SendYuYinToBaiDu(saveAudioClip, trueLength);


                //额外的代码，yhw添加的程序
                bool isTestVideo = false;
                if (isTestVideo)
                {
                    string wavPath = toWAV.ConvertToWAVStreamAndSaveAsFile(saveAudioClip, trueLength);   //AudioClip转为WAV文件并保存，返回文件路径
                    StartCoroutine(server.Chat(wavPath));    //与中转服务器进行交互，取得的回复数据保存到byteRecvArray中 
                }
                else
                {
                    byte[] byteSendArray = toWAV.ConvertToWAVStream(saveAudioClip, trueLength);
                    StartCoroutine(server.Chat(byteSendArray));
                }
                //额外的代码
            }
            else
            {
                Debug.LogError("录制时间过短！");
                BaiDuAI.Instance.OnEndIndex();
            }
        }
    }

    /// <summary>
    /// 保存音频数据为wav文件，方便验证语音是否正确
    /// </summary>
    /// <param name="byteRecvArray">从服务器接收到的音频字节流</param>
    void SaveRecvDataAndTalk(byte[] byteRecvArray) 
    {
        string responseFileName = "Sounds/tempResponse.wav";
        string responseFilePath = System.IO.Path.Combine(Application.dataPath, "Resources", responseFileName);
        SaveRawDataAsFile(byteRecvArray, responseFilePath);     //将回复数据写入到文件并保存为WAV格式

        string filePath = "file:///" + responseFilePath;
        Debug.Log("filePath:" + filePath);

        StartCoroutine(ChatGptTalk(filePath));
    }

    void PlayChatGptResponse(byte[] byteRecvArray) 
    {
        AudioClip clip = toWAV.ToAudioClip(byteRecvArray);
        PlayTalk(clip);
    }


    IEnumerator ChatGptTalk(string filePath) 
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV))
        {
            yield return uwr.SendWebRequest();
            

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                
                PlayTalk(clip);
            }
        }
        
        
    }

    void SaveRawDataAsFile(byte[] byteData, string filePath) 
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create)) 
        {
            fileStream.Write(byteData, 0, byteData.Length);
        }
    }

    

    //用户语音识别结果（识别内容）
    public void OnYuYinShiBieResult(string resultStr)
    {
        //将识别的语音发送给百度机器人
        //BaiDuUnit.Instance.Unit_NLP(resultStr);

        OurTalk_Panel.SetOurTalkText(resultStr);
        //将识别的语音发送给文心千帆
        BaiDuAI.Instance.RequestWXQFSpeak(resultStr);
    }

    //用户语音识别结果（识别内容）
    public void OnYuYinShiBieResultEx(string resultStr)
    {
        //将识别的语音发送给百度机器人
        //BaiDuUnit.Instance.Unit_NLP(resultStr);

        GameManager.Instance.mAlphaGrilMove.StopTalk();
        BaiDuAI.Instance.OnStartIndex();

        OurTalk_Panel.SetOurTalkText(resultStr);
        AlphaTalk_Panel.SetAlphaTalkText(string.Empty);
        //将识别的语音发送给文心千帆
        BaiDuAI.Instance.RequestWXQFSpeak(resultStr);
    }


    /// <summary>
    /// 开始录音
    /// </summary>
    /// <param name="isLoop"></param>
    /// <returns></returns>
    public bool StartRecording(bool isLoop = false) //8000,16000
    {
        if (isHaveMic == false || Microphone.IsRecording(currentDeviceName))
        {
            return false;
        }

        lastPressTimestamp = GetTimestampOfNowWithMillisecond();

        saveAudioClip = Microphone.Start(currentDeviceName, isLoop, recordMaxLength, recordFrequency);

        return true;
    }
    /// <summary>
    /// 录音结束,返回实际的录音时长
    /// </summary>
    /// <returns></returns>
    public int EndRecording()
    {
        if (isHaveMic == false || !Microphone.IsRecording(currentDeviceName))
        {
            return 0;
        }

        //结束录音
        Microphone.End(currentDeviceName);

        //向上取整,避免遗漏录音末尾
        return Mathf.CeilToInt((float)(GetTimestampOfNowWithMillisecond() - lastPressTimestamp) / 1000f);
    }

    /// <summary>
    /// 获取毫秒级别的时间戳,用于计算按下录音时长
    /// </summary>
    /// <returns></returns>
    public double GetTimestampOfNowWithMillisecond()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;  //毫秒
        //return ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000); //秒
    }



    //AI回答结果（回答内容）
    private void AIHuiDa(string value)
    {
        if (value.Contains("文心一言") || value.Contains("ERNIE Bot"))
        {
            value = beginAlphaGirlText;
        }
        AlphaTalk_Panel.SetAlphaTalkText(value.Replace("\\n", "\n"));
        BaiDuAI.Instance.StartTTS(value);
    }



    /// <summary>
    ///工具栏里的功能弹出
    /// </summary>
    public void Toggle_ToolBoxOnclick(bool isShow)
    {
        if (isShow)
        {
            UIDoTweenType.Instance.GameObjectDoScaleShow(toggle_Session.gameObject);
            UIDoTweenType.Instance.GameObjectDoScaleShow(toggle_VoiceModule.gameObject);
            UIDoTweenType.Instance.GameObjectDoScaleShow(toggle_Exit.gameObject);
            UIDoTweenType.Instance.GameObjectDoScaleShow(toggle_ShowTextBox.gameObject);
            UIDoTweenType.Instance.GameObjectDoScaleShow(toggle_ToneShift.gameObject);
        }
        else
        {
            UIDoTweenType.Instance.GameObjectDoScaleHide(toggle_Session.gameObject);
            UIDoTweenType.Instance.GameObjectDoScaleHide(toggle_VoiceModule.gameObject);
            UIDoTweenType.Instance.GameObjectDoScaleHide(toggle_Exit.gameObject);
            UIDoTweenType.Instance.GameObjectDoScaleHide(toggle_ShowTextBox.gameObject);
            UIDoTweenType.Instance.GameObjectDoScaleHide(toggle_ToneShift.gameObject);
        }

    }
    /// <summary>
    /// 点击沟通模板按钮时
    /// </summary>
    public void Toggle_SessionOnClick(bool isShow)
    {
        if (isShow)
        {
            UIDoTweenType.Instance.GameObjectDoScaleShow(sessionPanel.gameObject);
            sessionListManager.Init();

        }
        else
        {
            UIDoTweenType.Instance.GameObjectDoScaleHide(sessionPanel.gameObject);
        }
    }
    /// <summary>
    /// 点击语音功能模块
    /// </summary>
    public void Toggle_VoiceModuleOnclick(bool isShow)
    {
        if (isShow)
        {
            UIDoTweenType.Instance.GameObjectDoScaleShow(voiceModulePanel.gameObject);

        }
        else
        {
            UIDoTweenType.Instance.GameObjectDoScaleHide(voiceModulePanel.gameObject);
        }
    }
    /// <summary>
    /// 显示隐藏文本框方法
    /// </summary>
    public void ControllTextBox(bool isShow)
    {
        if (isShow)
        {
            UIDoTweenType.Instance.GameObjectDoScaleShow(OurTalk_Panel.gameObject);
            UIDoTweenType.Instance.GameObjectDoScaleShow(AlphaTalk_Panel.gameObject);
        }
        else
        {
            UIDoTweenType.Instance.GameObjectDoScaleHide(OurTalk_Panel.gameObject);
            UIDoTweenType.Instance.GameObjectDoScaleHide(AlphaTalk_Panel.gameObject);
        }
    }
    /// <summary>
    /// 显示隐藏音色选择面板
    /// </summary>
    public void ControllToneShiftPanel(bool isShow)
    {
        if (isShow)
        {
            UIDoTweenType.Instance.GameObjectDoScaleShow(toneShiftPanel.gameObject);
        }
        else
        {
            UIDoTweenType.Instance.GameObjectDoScaleHide(toneShiftPanel.gameObject);
        }
    }
    public void OnEnter()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        TalkPanelUI.SetActive(true);
        Init();
        Debug.Log("OnEnter");
    }


    public void OnPause()
    {

    }


    public void OnResume()
    {

    }


    public void OnExit()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        TalkPanelUI.SetActive(false);


        if (BaiDuAI.Instance != null)
        {
            BaiDuAI.Instance.WenZiToAudio -= PlayTalk;
            BaiDuAI.Instance.YuYinShiBieResult -= OnYuYinShiBieResult;
            BaiDuAI.Instance.AIAnswerResult -= AIHuiDa;
        }
    }
    public void OnClosePanel(bool isExit)
    {
        if (isExit)
        {
            //UIManager.Instance.PopPanel();
            GameManager.Instance.mAlphaGrilMove.StopTalk();
            //CameraManager.Instance.SetCameraPos("Init", 1, 0);
            AudioManager.Instance.OtherAudioSourceStop();
            toggle_ToolBox.isOn = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        CastMouseRay();
    }

    public void CastMouseRay()
    {
        if (VRController.instance == null)
        {
           //Debug.LogError("TalkPanelManager.CastMouseRay VRController == null");
            return;
        }
        Ray ray = VRController.instance.ray;
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100, LayerMask.GetMask("Default")))
        {
            GameObject go = hitInfo.transform.gameObject;
            if (go.name == "xunijuese")
            {
                if (AlphaMotion.instance.GetButtonDown(0))
                {
                    GameManager.Instance.mAlphaGrilMove.StopTalk();
                    TalkBtnIsOnClick();
                }
            }
        }
    }
}

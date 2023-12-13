using CrazyMinnow.SALSA;
using DG.Tweening;
using Microsoft.Win32;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
public class CasePanel : MonoBehaviour
{
    //[DllImport("user32.dll", CharSet = CharSet.Auto)]
    //public static extern bool SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);


    //程序按钮Part------------------------------------------
    private Transform ExeButtonParent;
    private RectTransform ExeButtonParentRect;
    //-----------------------------------------------------

    //异常提示Part------------------------------------------
    private Text Text_Tips;
    //-----------------------------------------------------

    //启动提示Part------------------------------------------
    private Image Image_ProcessTips;
    private Text Text_ProcessTips;
    //-----------------------------------------------------

    //存储所有程序Exe路径 的 文件路径
    private string ExePathFileUrl = "";
    //所有程序Exe路径 List
    private List<string> ExePathList;
    //所有程序Exe Btn
    private List<ExeButton> ExeButtonList;
    //预制体
    private ExeButton ExeButtonPrefab;


    private ScrollRect ScrollRect;
    private ButtonEx BtnUp;
    private ButtonEx BtnDown;



    [SerializeField]
    private string AlphaLinkLauncStr = "AlphaLink启动，";
    [SerializeField]
    private string AlphaHomeLauncStr = "AlphaWorld启动，";
    [SerializeField]
    private string ConfigPathNullStr = "配置文件不存在";
    [SerializeField]
    private string ExeUrlNullStr = "案例记录路径不存在";
    [SerializeField]
    private string ExeCountStr = "案例记录数量为0";

    [SerializeField]
    private string curProcessTips = "正在启动\"{0}\"，请耐心等待......";


    private string configPath;
    private Process curProcess;

    private bool isAlphaLinkLaunch;

    private Salsa3D aaa;

    void Start()
    {
        ExeButtonParent = transform.ZYFindChild("ExeButtonParent");
        ExeButtonParentRect = ExeButtonParent.GetComponent<RectTransform>();
        ExePathList = new List<string>();
        ExeButtonList = new List<ExeButton>();
        ExeButtonPrefab = Resources.Load<ExeButton>("Case/ExeButton");

        Image_ProcessTips = transform.ZYFindChild("Image_ProcessTips").GetComponent<Image>();
        Image_ProcessTips.gameObject.SetActive(false);
        Text_ProcessTips = transform.ZYFindChild("Text_ProcessTips").GetComponent<Text>();
        Text_Tips = transform.ZYFindChild("Text_Tips").GetComponent<Text>();
        Text_Tips.gameObject.SetActive(false);

        ScrollRect = transform.ZYFindChild("Scroll View").GetComponent<ScrollRect>();
        BtnUp = transform.ZYFindChild("btn_Up").GetComponent<ButtonEx>();
        BtnUp.onClick.AddListener(OnClickUp);
        BtnDown = transform.ZYFindChild("btn_Down").GetComponent<ButtonEx>();
        BtnDown.onClick.AddListener(OnClickDown);

        InitConfigPath();
    }

    //初始化 配置路径
    void InitConfigPath()
    {
        string[] CommandLineArgs = Environment.GetCommandLineArgs();
        if (CommandLineArgs.Length > 1 && !Application.isEditor)
        {
            configPath = CommandLineArgs[1];
            isAlphaLinkLaunch = true;
        }
        else
        {
            configPath = VRController.instance.ConfigPath;
            isAlphaLinkLaunch = false;
        }

        if (!File.Exists(configPath))
        {
            ShowTips("{0}{1}\n配置路径：{2}", isAlphaLinkLaunch ? AlphaLinkLauncStr : AlphaHomeLauncStr, ConfigPathNullStr, configPath);
            return;
        }

        InitExeUrl();
    }

    //初始化 案例记录路径
    void InitExeUrl()
    {
        if (isAlphaLinkLaunch)
        {
            Process[] processes = Process.GetProcessesByName("AlphaLink");
            if (processes.Length > 0)
            {
                Process pro = processes[0];
                //UnityEngine.Debug.LogError(pro.MainModule.FileName);
                ExePathFileUrl = Path.Combine(Path.GetDirectoryName(pro.MainModule.FileName), "AlphaLink_Data/StreamingAssets/oldExeUrl.txt");
            }
            else
            {
                ExePathFileUrl = "运行程序中 未找到名称为\"AlphaLink\"的程序\n请使用正式版\"AlphaLink\"启动本程序！！！";
                ShowTips("{0}{1}\n问题：{2}", AlphaLinkLauncStr, ExeUrlNullStr, ExePathFileUrl);
                return;
            }
        }
        else
        {
            ExePathFileUrl = Application.streamingAssetsPath + "/oldExeUrl.txt";
        }
        ReadExeUrl();
    }

    //读取 记录案例路径 txt文件
    private void ReadExeUrl()
    {
        if (File.Exists(ExePathFileUrl))
        {
            string[] task = null;
            task = File.ReadAllLines(ExePathFileUrl);
            for (int i = 0; i < task.Length; i++)
            {
                if (File.Exists(task[i]))
                {
                    if (!string.IsNullOrEmpty(task[i]) && !ExePathList.Contains(task[i]) && !task[i].Contains("AlphaHome") && !task[i].Contains("AlphaWorld"))
                    {
                        ExePathList.Add(task[i]);
                        //pathQueues.
                    }
                }
            }
            InitExePanel();
        }
        else
        {
            ShowTips("{0}{1}\n案例记录路径：{2}", isAlphaLinkLaunch ? AlphaLinkLauncStr : AlphaHomeLauncStr, ExeUrlNullStr, ExePathFileUrl);
        }
    }

    //初始化 案例Button
    private void InitExePanel()
    {
        if (ExePathList.Count <= 0)
        {
            ShowTips("{0}{1}\n案例记录路径：{2}", isAlphaLinkLaunch ? AlphaLinkLauncStr : AlphaHomeLauncStr, ExeCountStr, ExePathFileUrl);
            return;
        }

        var maxCount = ExeButtonList.Count;
        var curCount = ExePathList.Count;
        for (int i = 0; i < curCount; i++)
        {
            CreateOne(i, ExePathList[i]);
        }

        if (maxCount > curCount)
        {
            for (int i = maxCount; i > curCount; i--)
            {
                ExeButtonList[i - 1].gameObject.SetActive(false);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(ExeButtonParentRect);
    }

    //创建对象
    private void CreateOne(int index, string exePath)
    {
        var exeButton = GetExeButton(index);
        exeButton.gameObject.SetActive(true);
        exeButton.SetExeName(exePath);
    }

    //获取程序按钮
    private ExeButton GetExeButton(int index)
    {
        if (ExeButtonList.Count > index)
        {
            return ExeButtonList[index];
        }
        else
        {
            var exeButton = Instantiate(ExeButtonPrefab, ExeButtonParent);
            exeButton.Init();
            exeButton.OnClickAction = ClickExeBtn;

            //exeButton.OnPointerEnterEvent = ShowDescribe;

            //exeButton.OnPointerExitEvent = (string exeName) => { Text_IntroducePartTip.text = "无"; };

            exeButton.OnClickClose = (string exePath) =>
            {
                exeButton.gameObject.SetActive(false);
                ExePathList.Remove(exePath);
                //Destroy(exeButton.gameObject);
            };

            ExeButtonList.Add(exeButton);
            return exeButton;
        }
    }

    //点击程序按钮
    private void ClickExeBtn(string exePath, string exeName)
    {
        //Process.Start(newexePath, configPath);
        if (curProcess != null && !curProcess.HasExited)
        {
            if (curProcess.ProcessName.Equals(exeName))
            {
                //SwitchToThisWindow(curProcess.MainWindowHandle, true);    // 激活，显示在最前  
                SetForegroundWindow(curProcess.MainWindowHandle);
                return;
            }
            else
            {
                ResetCurProcess();//杀掉 下面重开
            }
        }

        ShowProcessTips(exeName);
        curProcess = new Process();
        curProcess.StartInfo = new ProcessStartInfo(exePath, configPath); ;
        curProcess.StartInfo.UseShellExecute = false;
        //curProcess.StartInfo.RedirectStandardOutput = true;
        curProcess.StartInfo.RedirectStandardError = true;
        curProcess.EnableRaisingEvents = true;

        //curProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        //{
        //    ShowProcessTips("完成");
        //});

        curProcess.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            ShowProcessTips();
            ResetCurProcess();
        });

        curProcess.Exited += new EventHandler((sender, e) =>
        {
            ShowProcessTips();
        });

        curProcess.Start();

        SendMessage("UpdateLog", exeName, SendMessageOptions.DontRequireReceiver);
    }

    //清理子进程
    private void ResetCurProcess()
    {
        if (curProcess != null)
        {
            if (!curProcess.HasExited)
            {
                curProcess.Kill();
            }
            curProcess = null;
        }
    }


    //红色 常显异常
    private void ShowTips(string format, params object[] value)
    {
        Text_Tips.text = string.Format(format, value);
        Text_Tips.gameObject.SetActive(true);
    }

    //白色 启动子进程提示
    private void ShowProcessTips(string value = "")
    {
        if (!string.IsNullOrEmpty(value))
        {
            Text_ProcessTips.text = string.Format(curProcessTips, value);
            Image_ProcessTips.gameObject.SetActive(true);
        }
        else
        {
            Text_ProcessTips.text = value;
            Image_ProcessTips.gameObject.SetActive(false);
        }
    }


    ////开始删除
    //private void ClearExePanel()
    //{
    //    DeleteOKBtn.gameObject.SetActive(true);
    //    DeleteBtn.gameObject.SetActive(false);
    //    Transform theParent = transform.Find("Scroll View/Viewport/Content");
    //    for (int i = 0; i < theParent.childCount; i++)
    //    {
    //        Transform child = theParent.GetChild(i);
    //        child.GetComponent<Button>().enabled = false;
    //        child.Find("ButtonClose").gameObject.SetActive(true);
    //    }
    //}

    ////取消删除
    //private void OKClearExePanel()
    //{
    //    DeleteOKBtn.gameObject.SetActive(false);
    //    DeleteBtn.gameObject.SetActive(true);
    //    Transform theParent = transform.Find("Scroll View/Viewport/Content");
    //    for (int i = 0; i < theParent.childCount; i++)
    //    {
    //        Transform child = theParent.GetChild(i);
    //        child.GetComponent<Button>().enabled = true;
    //        child.Find("ButtonClose").gameObject.SetActive(false);
    //    }
    //    WriteExeUrl();
    //}

    ////添加案例（案例路径选择窗口展开）
    //private void AddCase()
    //{
    //    string filePath = Application.dataPath;
    //    string[] paths = StandaloneFileBrowser.OpenFilePanel("选择项目案例", filePath, "exe", false);
    //    if (paths.Length > 0)
    //    {
    //        string exePath = paths[0].ToString();
    //        if (!string.IsNullOrEmpty(exePath))
    //        {
    //            //刷新列表
    //            if (!pathList.Contains(exePath))
    //            {
    //                pathList.Add(exePath);
    //                WriteExeUrl();
    //                btnNameList.Add(exePath);
    //                CreateOne(exePath);
    //            }
    //        }
    //    }
    //}

    ////写入记录案例路径 txt文件
    //private void WriteExeUrl()
    //{
    //    System.Text.StringBuilder exePaths = new System.Text.StringBuilder();
    //    for (int i = 0; i < exePathList.Count; i++)
    //    {
    //        exePaths.AppendLine(exePathList[i]);
    //    }

    //    File.WriteAllText(exePathFileUrl, exePaths.ToString());
    //}

    private void OnClickUp()
    {
        var ooo = ExeButtonParentRect.localPosition.y - 281;
        var offset = ooo > 0 ? ooo : -50;
        ExeButtonParentRect.DOLocalMoveY(offset, 0.5f);
    }

    private void OnClickDown()
    {
        var ooo = ExeButtonParentRect.sizeDelta.y - ExeButtonParentRect.localPosition.y - 826;
        var offset = ExeButtonParentRect.localPosition.y + (ooo > 281 ? 281 : 100);
        ExeButtonParentRect.DOLocalMoveY(offset, 0.5f);
    }
}
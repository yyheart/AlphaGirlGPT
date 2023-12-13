using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public delegate void BoolDelegate(Transform obj, bool isHover);
public delegate void VoidDelegate(Transform obj);

public class VRController : MonoBehaviour
{
    public Camera MainCamera;
    public bool showCursor=false;
    public static VRController instance;

    [HideInInspector]
    public Transform Head;
    [HideInInspector]
    public Transform Hand;
    [HideInInspector]
    public Transform Quad;
    [HideInInspector]
    public Transform offset;//手柄旋转偏差

    private bool isInit = false;
    private bool isConnect = false;

    private MeshRenderer rayRenderer;

    //配置文件的绝对路径
    private string m_configPath;
    public string ConfigPath
    {
        get { return m_configPath; }
    }
    //读取配置文件参数
    [HideInInspector]
    public string path;
    [HideInInspector]
    public string _defaultConfig;
    string defaultName = "defaultConfig.Xml";
    HardWare _hardWare;
    XmlManager xm = new XmlManager();
    public Config myconfig;
    RenderPC _renderPC;
    VRScreen _vrScreen;
    Head _head;
    Hand _hand;

   
    public Ray ray
    {
        get
        {
            return new Ray(offset.position, offset.forward);
        }
    }

    public MeshRenderer RayRenderer {
        get { return rayRenderer; }
        set { rayRenderer = value; }
    }

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
        }
        
        _defaultConfig = "/StreamingAssets/" + defaultName;
        //Debug.Log(Application.dataPath+ GetComponent<VRAppTools>().path);
        path =  GetComponent<VRAppTools>().path;
        ReadConfig();

        //PrintData();
        //Debug.Log(myconfig.renderPC.eyeSeparation);

        //  Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        Screen.SetResolution((int)myconfig.screen.resoWidth,(int) myconfig.screen.resoHeight, true);
        Cursor.visible = showCursor;
      // QualitySettings.SetQualityLevel(5);
       
       
       //删除场景中的EventSystem;
       GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem!=null)
        {
            Destroy(eventSystem);
        }
      
       
        CreatCamera();
        Head = transform.Find("Head");
        Hand = transform.Find("Hand");
        Quad = transform.Find("Quad");
        offset = Hand.Find("offset");

        Init();
        CreateWand(Hand);
        SetHandShader();//设置手柄的渲染队列
    }
    /// <summary>
    /// 设置手柄的渲染队列
    /// </summary>
    public void SetHandShader()
    {
        MeshRenderer[] mrs = Hand.transform.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < mrs.Length; i++)
        {
            mrs[i].material.renderQueue = 3500;
        }

    }


    void PrintData()
    {
        Debug.Log(myconfig.renderPC.pcName + " ");
    }
    // Use this for initialization
    void Start()
    {
       
    }
    void CreatCamera()
    {
        Transform Head = new GameObject("Head").transform;
        Head.SetParent(transform,false);
        Transform Hand = new GameObject("Hand").transform;
        Hand.SetParent(transform,false);
        Transform Screen = new GameObject("Quad").transform;
        Screen.localPosition = new Vector3(0, Screen.localScale.y / 2f, 2f);
        Screen.gameObject.AddComponent<MeshCollider>();
        
        Screen.SetParent(transform,false);
        //Transform LeftCam = new GameObject("LeftCam").transform;
        //LeftCam.SetParent(Head,false);
        //  LeftCam.gameObject.AddComponent<Camera>();
        //  LeftCam.gameObject.AddComponent<FlareLayer>();

        Transform RightCam = null;
            if (MainCamera==null)
        {
            RightCam = new GameObject("RightCam").transform;
            RightCam.gameObject.AddComponent<Camera>();
            
        }
        else
        {
            RightCam = MainCamera.transform;
            RightCam.name = "RightCam";
        }
      //  RightCam.tag = "MainCamera";
       // RightCam.tag = "";

        RightCam.SetParent(Head,false);
        RightCam.localPosition = Vector3.zero;
        RightCam.localRotation = Quaternion.identity;

        Transform LeftCam = Instantiate(RightCam, Head, false);
        LeftCam.name = "LeftCam";
        if (LeftCam.GetComponent<AudioListener>()!=null)
        {
            Destroy(LeftCam.GetComponent<AudioListener>());
        }

        Transform offset = new GameObject("offset").transform;
        offset.SetParent(Hand,false);

        //Transform _EventSystem = new GameObject("EventSystem").transform;
        //_EventSystem.SetParent(transform);
        //_EventSystem.gameObject.AddComponent<RayEventSystem>();
        //_EventSystem.gameObject.AddComponent<RayInputModule>();


        //相机设定
     
       // LeftCam.gameObject.AddComponent<GUILayer>();

       
      //  RightCam.gameObject.AddComponent<FlareLayer>();
      //  RightCam.gameObject.AddComponent<GUILayer>();

        LeftCam.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Left;
        RightCam.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Right;

        float eyeSeparation = myconfig.renderPC.eyeSeparation;
       // RightCam.GetComponent<Camera>().stereoSeparation = eyeSeparation;
        LeftCam.localPosition = new Vector3(-eyeSeparation / 2f, 0, 0);
        RightCam.localPosition = new Vector3(eyeSeparation / 2f, 0, 0);

          LeftCam.gameObject.AddComponent<EyeSet>();
         RightCam.gameObject.AddComponent<EyeSet>();

        LeftCam.gameObject.GetComponent<EyeSet>().plane = Screen;
        LeftCam.gameObject.GetComponent<EyeSet>().eye = StereoTargetEyeMask.Left;
        RightCam.gameObject.GetComponent<EyeSet>().plane = Screen;
        RightCam.gameObject.GetComponent<EyeSet>().eye = StereoTargetEyeMask.Right;

        //屏幕设定
        Screen.localPosition = new Vector3(myconfig.screen.position.x, myconfig.screen.position.y, myconfig.screen.position.z);
        Screen.localEulerAngles = new Vector3(myconfig.screen.rotation.x, myconfig.screen.rotation.y, myconfig.screen.rotation.z);
        Screen.localScale = new Vector3(myconfig.screen.sizeWidth, myconfig.screen.sizeHeight, 1f);
      

    }

    /// <summary>

    /// </summary>
    // Update is called once per frame
    private int headIndex = 0;
    private int handIndex = 1;
    [SerializeField]
    private float LerpSpeed = 20f;
    void Update()
    {
        AlphaTracker handTracker = AlphaMotion.instance.GetTracker(handIndex);
        AlphaTracker headTracker = AlphaMotion.instance.GetTracker(headIndex);
        Hand.localPosition = Vector3.Lerp(Hand.localPosition, handTracker.pos, Time.deltaTime * LerpSpeed);
        Hand.localRotation = Quaternion.Lerp(Hand.localRotation, handTracker.rotation, Time.deltaTime * LerpSpeed);

        Head.localPosition = Vector3.Lerp(Head.localPosition, headTracker.pos, Time.deltaTime * LerpSpeed);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    public void Init()
    {
        //AlphaMotion.instance.Register("127.0.0.1");
        AlphaMotion.instance.Register(myconfig.hardWare.trackerIP, myconfig.hardWare.head.name, myconfig.hardWare.hand.name);
        VRCN.OnConnected += OnConnected;
        VRCN.OnDisConnect += OnDisConnect;
       
    }
   
    private void OnConnected()
    {
        isConnect = true;
        //Debug.Log("VRPN连接" + isConnect);
    }

    private void OnDisConnect()
    {
        isConnect = false;
        //Debug.Log("VRPN断开连接" + isConnect);
    }
  
    private void SetTransformValue(Transform current, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, int layer)
    {
        current.SetParent(parent);
        current.localPosition = position;
        current.localRotation = rotation;
        current.localScale = scale;
        current.gameObject.layer = layer;
    }
    Transform CreateWand(Transform parent)
    {
        Transform DefaultWandRepresentation = new GameObject("DefaultWandRepresentation").transform;
        SetTransformValue(DefaultWandRepresentation, parent.Find("offset"), Vector3.zero, Quaternion.identity, Vector3.one, 5);

        UnityEngine.Object Gmontion_Controller = Resources.Load("Gmontion_Controller");
        Transform Gmontion_Controller_Clone = (Instantiate(Gmontion_Controller) as GameObject).transform;
        Gmontion_Controller_Clone.name = "Gmontion_Controller";
        SetTransformValue(Gmontion_Controller_Clone, DefaultWandRepresentation, Vector3.zero, Quaternion.identity, Vector3.one , 5);
        for (int i = 0; i < Gmontion_Controller_Clone.childCount; i++)
        {
            Gmontion_Controller_Clone.GetChild(i).gameObject.layer = 5;
        }

        Transform WandRay = new GameObject("WandRay").transform;
        SetTransformValue(WandRay, DefaultWandRepresentation, Vector3.zero, Quaternion.identity, Vector3.one, 5);
        UnityEngine.Object cone = Resources.Load("cone");
        GameObject coneClone = Instantiate(cone) as GameObject;
        Transform RayMesh = coneClone.transform.Find("Cone");
        SetTransformValue(RayMesh, WandRay, new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(1, 1, 50), 5);
        Destroy(coneClone);
        RayRenderer = RayMesh.GetComponent<MeshRenderer>();

        GameObject newObj = Resources.Load<GameObject>("UICamera");
        GameObject eventCam = Instantiate(newObj, DefaultWandRepresentation.parent);

        //RayMesh.GetComponent<MeshRenderer>().material.color = new Color(13f / 255, 235f / 255, 255f / 255, 255f / 255)
        return DefaultWandRepresentation;
    }
    private void ReadConfig()
    {
        string cmdInfo = "";
        string[] cmdArgs = Environment.GetCommandLineArgs();
        foreach (string arg in cmdArgs)
        {
            cmdInfo += arg.ToString() + ";";
        }
        //txtConfigPath.text = "cmdinfo=**"+cmdInfo;
        if (Application.isEditor)
        {
            m_configPath = null;
        }
        else
        {
            if (cmdArgs.Length > 1)
            {
                m_configPath = cmdArgs[1].Replace("###", " ");
            }
            else
            {
                //Application.Quit();
                m_configPath = null;
            }
        }
        


        if (string.IsNullOrEmpty(m_configPath))//读取StreamingAssets下面的配置文件
            m_configPath = string.Format("{0}{1}", Application.dataPath, string.IsNullOrEmpty(path) ? _defaultConfig : path);
      //  Debug.Log(path);
        Debug.Log("absolutePath=" + m_configPath);
        //txtConfigPath1.text = "absolutePath=**" + absolutePath;
        if (File.Exists(m_configPath)) {
            //加载配置文件
            LoadXml(m_configPath);
        }
        else
        {
            Debug.LogError("文件不存在！");
        }
        
          
    }
    public void LoadXml(string xmlPath)
    {
      
        if (xm.hasFile(xmlPath))
        {
            string dataString = xm.LoadXML(xmlPath);
            Config _configFromXML = xm.deserializeObject<Config>(dataString, typeof(Config)) as Config;
            if (_configFromXML != null)
            {
                myconfig = _configFromXML;
            }
            else
            {
                Debug.LogError("_configFromXML is NULL");
            }
        }
        else
        {
            Debug.LogError("xml文件不存在！");
            return;
        }
    }
}

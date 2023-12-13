using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MakeFullScreenUGUI : MonoBehaviour {
    /// <summary>
    /// 缩放模式
    /// </summary>
    public enum ScaleType
    {
        /// <summary>
        /// 拉伸
        /// </summary>
        Stretch,
        /// <summary>
        /// 按比例缩放
        /// </summary>
        Scale
    }

    /// <summary>
    /// 操作的Canvas
    /// </summary>
    private RectTransform uguiCanvas;

    /// <summary>
    /// 是否自动对齐Screen
    /// </summary>
    private bool isAuto = true;

    
    private Transform plane;

    /// <summary>
    /// 存放MakeFullScreenUGUI的字典，方便通过名字去查找
    /// </summary>
    private static Dictionary<string, MakeFullScreenUGUI> dic = new Dictionary<string, MakeFullScreenUGUI>();

    /// <summary>
    /// 定义缩放的模式，Stretch即按照当前Canvas分辨率进行缩放，Scale即按照Viewport分辨率进行缩放
    /// </summary>
    [SerializeField]
    private ScaleType type = ScaleType.Stretch;

    /// <summary>
    /// 因为自动的时候Canvas设置是异步的，所以设置一个委托方便其他人获取完成的状态
    /// </summary>
    public delegate void VoidDelegate();
    public VoidDelegate onFinish;

    private int width;
    private int height;
   
    /// <summary>
    /// 通过名字去获取MakeFullScreenUGUI实例
    /// </summary>
    /// <param name="name">名字</param>
    /// <returns></returns>
    public static MakeFullScreenUGUI GetInstance(string name)
    {
        if (dic.ContainsKey(name))
        {
            return dic[name];
        }
        return null;
    }

    //private static Material _mat;
    //public static Material mat
    //{
    //    get
    //    {
    //        if (_mat == null)
    //        {
    //            Shader shader = Resources.Load<Shader>("UIOverlay");
    //            _mat = new Material(shader);
    //        }
    //        return _mat;
    //    }
    //}

    void Awake()
    {
        if(!dic.ContainsKey(name))
            dic.Add(name, this);
        else
            Debug.LogErrorFormat("有名字为{0}的Canvas发生重名，无法添加到字典中", name);
        //Graphic[] graphics = GetComponentsInChildren<Graphic>(true);
        //for (int i = 0; i < graphics.Length; i++)
        //{
        //    graphics[i].material = mat;
        //}
    }

	void Start () {
        uguiCanvas = GetComponent<RectTransform>();
        if(!uguiCanvas)
        {
            enabled = false;
            Debug.LogErrorFormat("找不到依赖的RectTransform，关闭脚本！");
        }
        plane = VRController.instance.Quad;

      

    }

    void OnDestroy()
    {
        if (dic!=null)
        {
            dic.Remove(name);
        }
        
        
    }
	
	void Update () {

        if (isAuto)
        {
            if (Screen.width != width || Screen.height != height)
            {
                width = Screen.width;
                height = Screen.height;
                Make();
                //设置Canvas下子物体的材质球
                SetCanvasChildMaterial();
            }
        }
     
    }

    /// <summary>
    /// 设置Canvas下的子物体的Materials
    /// </summary>
    public void SetCanvasChildMaterial()
    {
        Material shaderMaterial = new Material(Shader.Find("UI/Overlay"));
       // Debug.Log("设置Canvas下的子物体的Material");
        for (int i = 0; i < gameObject.GetComponentsInChildren<Image>(true).Length; i++)
        {
            gameObject.GetComponentsInChildren<Image>(true)[i].material = shaderMaterial;
        }
        for (int i = 0; i < gameObject.GetComponentsInChildren<Text>(true).Length; i++)
        {
            gameObject.GetComponentsInChildren<Text>(true)[i].material = shaderMaterial;
        }
    }

    /// <summary>
    /// 实现Canvas的缩放
    /// </summary>
    public void Make()
    {

       // uguiCanvas.GetComponent<Canvas>().worldCamera = VRAppTools.instance.m_Camera;
        
        Transform parent = plane;
        uguiCanvas.SetParent(parent);
        uguiCanvas.localPosition = Vector3.zero;
        uguiCanvas.localRotation = Quaternion.identity;
        if (type == ScaleType.Stretch)
        {
            uguiCanvas.sizeDelta = new Vector2(1792f, 1024f);
            uguiCanvas.localScale = new Vector3(1f / 1792f, 1f / 1024f, 1f);

        }
        else
        {
            uguiCanvas.sizeDelta = new Vector2(1792f, 1024f);
            float t = Screen.width * 1.0f / Screen.height;
            if (t >= 16f / 9f)
            {
                uguiCanvas.localScale = new Vector3(1f / 1792f / (t / 16f * 9f), 1f / 1024f, 1f);
                uguiCanvas.sizeDelta = new Vector2(1024f / Screen.height * Screen.width, 1024f);
            }
            else
            {
                uguiCanvas.localScale = new Vector3(1f / 1792f, 1f / 1024f / (16f / 9f / t), 1f);
                uguiCanvas.sizeDelta = new Vector2(1792f, 1792f / Screen.width * Screen.height);
            }
        }
        

        if (onFinish != null)
        {
            onFinish();
        }
    }
}

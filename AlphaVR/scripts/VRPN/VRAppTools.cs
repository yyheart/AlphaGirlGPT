using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRAppTools : MonoBehaviour {
    public static VRAppTools instance;
    public static string currentSaveContent;
    public static List<Transform> selectObjs = new List<Transform>();
    public static List<Transform> allObjs = new List<Transform>();
    public static List<Transform> disabledObjs = new List<Transform>();
    public static List<MeshRenderer> transparentObjs = new List<MeshRenderer>();
    public static float transparentValue = 0.4f;
  
    
    private static bool MouseMove = false;
    private static Vector3 MousePosition = Vector3.zero;
    

    [HideInInspector]
    public Camera m_Camera;

    [HideInInspector]
    public string path;//配置文件路径
    public Ray m_Ray;
    public static Ray ray
    {
        get
        {
            return VRController.instance.ray;
        }
    }
   
   
    public static bool pressed
    {
        get
        {
            if (instance != null )
            {
                return AlphaMotion.instance.GetButtonDown(0);
            }
            return Input.GetMouseButtonDown(0);
        }
    }
    public static bool released
    {
        get
        {
            if (instance != null)
            {
                return AlphaMotion.instance.GetButtonUp(0);
            }
            return Input.GetMouseButtonUp(0);
        }
    }
  
    private void Awake()
    {
        instance = this;
       
       
    }
    // Use this for initialization
    void Start () {
        m_Camera = GameObject.Find("RightCam").GetComponent<Camera>();
      //  RayInputManager.EventCam = m_Camera;
       
    }
  

  
    // Update is called once per frame
    void Update () {
        if (Input.touchCount > 0)
        {
            MouseMove = false;
        }
        else
        {
            if (MousePosition != Input.mousePosition)
            {
                MouseMove = true;
                MousePosition = Input.mousePosition;
            }
        }
    }

    public  bool pointOverUI
    {
        get
        {
            //if (Input.touchCount == 0 && Input.mousePresent)
            //{
                return EventSystem.current.IsPointerOverGameObject();
            //}
            //else
            //{
            //    return EventSystem.current.GetComponent<RayInputModule>().TouchOverUI();
            //}
        }
    }
    /// <summary>
    /// 设置摄像机矩阵
    /// </summary>
    /// <param name="plane">摄像机看向的平面</param>
    /// <param name="cam"></param>
    public  void SetProjectionMatrix(Transform plane, Camera cam, float offset)
    {
        float distance;
        float offsetX, offsetY;
        float left, right, top, bottom;
        cam.transform.rotation = plane.rotation;
        Vector2 ScreenSize = new Vector2(plane.localScale.x, plane.localScale.y);
        Vector3 Position = cam.transform.position - plane.position + cam.transform.rotation * new Vector3(offset, 0, 0);
        distance = Vector3.Dot(Position, cam.transform.forward);
        offsetX = Vector3.Dot(Position, cam.transform.right);
        offsetY = Vector3.Dot(Position, cam.transform.up);
        left = (ScreenSize.x / 2f + offsetX) / distance * cam.nearClipPlane;
        right = -(ScreenSize.x / 2f - offsetX) / distance * cam.nearClipPlane;
        bottom = (ScreenSize.y / 2f + offsetY) / distance * cam.nearClipPlane;
        top = -(ScreenSize.y / 2f - offsetY) / distance * cam.nearClipPlane;
        cam.projectionMatrix = GetFrustum(cam.nearClipPlane, cam.farClipPlane, left, right, top, bottom);
    }

    public  void SetStereoProjectionMatrix(Camera.StereoscopicEye eye, Transform plane, Camera cam, float offset)
    {
        float distance;
        float offsetX, offsetY;
        float left, right, top, bottom;
        cam.transform.rotation = plane.rotation;
        Vector2 ScreenSize = new Vector2(plane.localScale.x, plane.localScale.y);
        Vector3 Position = cam.transform.position - plane.position + cam.transform.rotation * new Vector3(offset, 0, 0);
        distance = Vector3.Dot(Position, cam.transform.forward);
        offsetX = Vector3.Dot(Position, cam.transform.right);
        offsetY = Vector3.Dot(Position, cam.transform.up);
        left = (ScreenSize.x / 2f + offsetX) / distance * cam.nearClipPlane;
        right = -(ScreenSize.x / 2f - offsetX) / distance * cam.nearClipPlane;
        bottom = (ScreenSize.y / 2f + offsetY) / distance * cam.nearClipPlane;
        top = -(ScreenSize.y / 2f - offsetY) / distance * cam.nearClipPlane;
        cam.SetStereoProjectionMatrix(eye, GetFrustum(cam.nearClipPlane, cam.farClipPlane, left, right, top, bottom));
    }

    /// <summary>
    /// 获得摄像机矩阵
    /// </summary>
    /// <param name="n"></param>
    /// <param name="f"></param>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <param name="t"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public  Matrix4x4 GetFrustum(float n, float f, float l, float r, float t, float b)
    {
        Matrix4x4 M = Matrix4x4.identity;
        M[0, 0] = 2 * n / (r - l); M[0, 1] = 0; M[0, 2] = (r + l) / (r - l); M[0, 3] = 0;
        M[1, 0] = 0; M[1, 1] = 2 * n / (t - b); M[1, 2] = (t + b) / (t - b); M[1, 3] = 0;
        M[2, 0] = 0; M[2, 1] = 0; M[2, 2] = -(f + n) / (f - n); M[2, 3] = -2 * n * f / (f - n);
        M[3, 0] = 0; M[3, 1] = 0; M[3, 2] = -1; M[3, 3] = 0;
        return M;
    }
   

    public static void SetPressed(ref bool pressed, string name)
    {
        pressed = VRAppTools.pressed;
    }

    public static void SetReleased(ref bool released, string name)
    {
        released = VRAppTools.released;
    }
}

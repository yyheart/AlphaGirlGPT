using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour {
    
    private MeshRenderer rayRenderer;
    private MeshRenderer joystickRenderer;
    [SerializeField]
    private float JoystickScale = 1f;
    [HideInInspector]
    public Transform PointImage;
    [HideInInspector]
    public Transform PointSphere;
    
    /*手柄射线颜色*/
    private string ColorString = "_Color";
    public Color normalColor = Color.white;
    public Color hoverColor = Color.green;
    public Color UIHoverColor = Color.blue;
    public Color PointHoverColor = Color.red;
    [SerializeField]
    private float wandNormalLength = 50f;
    private float lastClickTime = 0f;
    private Transform lastClickObj = null;

    private Transform activeTransform;
    private Transform _currentTransform;

    public BoolDelegate onHover;
    public VoidDelegate onClick;
    public VoidDelegate onDoubleClick;
    public BoolDelegate onPress;

    //手柄射线相关参数
    public RaycastHit Hit { get { return _hit; } }
    private RaycastHit _hit;
    private Collider _collider;
    /// <summary>
    /// 设置射线长度
    /// </summary>
    public float RaycastDistance = 100;
    /// <summary>
    /// 射线可以射到的层
    /// </summary>
    public LayerMask LayerMask = -1;

    /// <summary>
    /// 如何发射射线
    /// </summary>
    /// <summary>
    /// 手柄射线进入物体时触发
    /// </summary>
    public Action<RaycastHit> onEnterObj;
    /// <summary>
    /// 手柄射线离开物体时触发
    /// </summary>
    public Action<Collider> onExitObj;
    /// <summary>
    /// 手柄射线悬停在物体上时触发
    /// </summary>
    public Action<RaycastHit> onHoverObj;

    private Transform currentTransform
    {
        set
        {
            if (_currentTransform != value)
            {
                if (_currentTransform != null)
                {
                    VRActorBase actor = _currentTransform.GetComponent<VRActorBase>();
                    if (actor != null) actor.OnActorHovered(_currentTransform, false);
                    if (onHover != null) onHover(_currentTransform, false);
                    SetColor(rayRenderer, normalColor);
                }
                _currentTransform = value;
                if (_currentTransform != null)
                {
                    VRActorBase actor = _currentTransform.GetComponent<VRActorBase>();
                    if (actor != null) actor.OnActorHovered(_currentTransform, true);
                    if (onHover != null) onHover(_currentTransform, true);
                    SetColor(rayRenderer, hoverColor);
                    
                     //   Sphere.GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                   
                     //   Sphere.GetComponent<MeshRenderer>().enabled = false;
                   

                }
            }
        }
        get
        {
            return _currentTransform;
        }
    }
    MeshRenderer lastRenderer;
    Color lastColor;
    void SetColor(MeshRenderer renderer, Color color)
    {
        
        if (renderer==lastRenderer && color == lastColor)
        {
            return;
        }
        lastRenderer = renderer;
        lastColor = color;
        if (renderer != null) renderer.material.SetColor(ColorString, color);
        //Image.GetComponent<UnityEngine.UI.Image>().color = new Color(color.r, color.g, color.b);
        //Image.GetComponent<TrailRenderer>().material.SetColor("_Color", new Color(color.r, color.g, color.b));
    }

    public void RefreshColor()
    {
        SetColor(rayRenderer, normalColor);
    }
    Vector3 initPointSphereScal ;
    
    // Use this for initialization
    void Start () {
        rayRenderer = VRController.instance.RayRenderer;
        //加载PointImage
        PointImage =Instantiate( Resources.Load<GameObject>("PointImage")).transform;
        
        Material shaderMaterial = new Material(Shader.Find("UI/Overlay"));
        PointImage.GetComponent<UnityEngine.UI.Image>().material = shaderMaterial;
        PointImage.GetComponent<UnityEngine.UI.Image>().color = PointHoverColor;

        PointSphere = Instantiate(Resources.Load<GameObject>("PointSphere")).transform;
        PointSphere.SetParent(VRController.instance.Hand, false);
        PointSphere.GetComponent<MeshRenderer>().material.renderQueue = 3500;
        initPointSphereScal = PointSphere.localScale;
         
        SetColor(PointSphere.GetComponent<MeshRenderer>(), PointHoverColor);
        GazeInputModule.OnEnterTarget += OnEnterUITatget;
        GazeInputModule.OnExitTarget += OnExitUITatget;

    }
    GameObject hoveredObj;
    public void OnEnterUITatget(GameObject uiTarget)
    {
        hoveredObj = uiTarget;
    }
    public void OnExitUITatget(GameObject uiTarget)
    {
        hoveredObj = null;
    }

    // Update is called once per frame
    void Update () {
        Ray ray = VRController.instance.ray;

        if (VRAppTools.instance.pointOverUI)
        {
            if (currentTransform!=null)
            {
                NotHit();
            }

            // Sphere.position = ray.GetPoint(RayInputManager.distance);
            //Image.GetComponent<UnityEngine.UI.Image>().enabled = true;
            //Image.localScale = Vector3.one * (1 / RayInputManager.distance);



            if (hoveredObj != null)
            {
                if (hoveredObj.GetComponentInParent<UnityEngine.UI.Selectable>())
                {
                    Transform theparet = hoveredObj.GetComponentInParent<Canvas>().transform;
                    PointImage.SetParent(theparet);
                    PointImage.localRotation = Quaternion.identity;

                    //计算点位置
                    float dis = 0;
                    
                    CanvasPlane worldCanvas = new CanvasPlane(theparet.position, theparet.forward);
                    dis = worldCanvas.GetDistance(ray);
                    PointImage.position = ray.origin + ray.direction * dis;
                    PointImage.localScale = Vector3.one * dis * 0.2f;
                    PointImage.gameObject.SetActive(true);

                    SetColor(rayRenderer, UIHoverColor);
                    if (rayRenderer != null)
                    {
                        rayRenderer.transform.parent.localScale = new Vector3(JoystickScale, JoystickScale, dis);
                    }
                }
                else
                {
                    PointImage.gameObject.SetActive(false);
                    SetColor(rayRenderer, normalColor);
                }

            }



            return;
        }
        else
        {
            PointImage.gameObject.SetActive(false);
        }
        
        
         //   Image.GetComponent<UnityEngine.UI.Image>().enabled = false;

        if (Physics.Raycast(ray, out _hit, RaycastDistance, LayerMask))
        {
            // Debug.DrawLine(ray.origin, _hit.point, Color.blue);

            HitObj(_hit);
        }

        else
            NotHit();

        bool downStatus = false;
        bool upStatus = false;

        downStatus = AlphaMotion.instance.GetButtonDown(0);
        upStatus = AlphaMotion.instance.GetButtonUp(0);
        if (downStatus)
        {
            activeTransform = currentTransform;
            if (activeTransform != null)
            {
                Debug.Log("OnClcik.................................." + _hit.transform.gameObject.name);

                VRActorBase actor = activeTransform.GetComponent<VRActorBase>();
                if (actor != null) actor.OnActorPressed(_currentTransform, true);
                if (onPress != null) onPress(activeTransform, true);
            }
        }

        if (upStatus)
        {
            if (activeTransform != null)
            {
                VRActorBase actor = activeTransform.GetComponent<VRActorBase>();
                if (actor != null) actor.OnActorPressed(activeTransform, false);
                if (onPress != null) onPress(activeTransform, false);
            }
            if (activeTransform == currentTransform)
            {
                if (activeTransform != null)
                {
                    VRActorBase actor = activeTransform.GetComponent<VRActorBase>();
                    if (actor != null) actor.OnActorClicked(_currentTransform);
                }
                if (onClick != null) onClick(activeTransform);
                if (lastClickTime != 0 && Time.time - lastClickTime < 0.3f && lastClickObj == activeTransform)
                {
                    if (onDoubleClick != null) onDoubleClick(lastClickObj);
                }
                lastClickObj = activeTransform;
                lastClickTime = Time.time;
            }
            activeTransform = null;
        }
    }

    public void HitObj(RaycastHit hit)
    {
       
        PointSphere.gameObject.SetActive(true);
        PointSphere.position = hit.point;
        PointSphere.localScale = initPointSphereScal * hit.distance;
        currentTransform = hit.transform;
        if (rayRenderer != null)
        {
            rayRenderer.transform.parent.localScale = new Vector3(JoystickScale, JoystickScale, hit.distance);
        }

        //if (Sphere.GetComponent<MeshRenderer>().enabled)
        //{
        //    Sphere.position = hit.point;
        //}
        //后面增加的，三个射线事件系统：OnenterObj,OnHoverObj,OnExitObj
        //  Debug.Log("click..................");
        if (hit.collider != _collider)
        {
            if (_collider != null)
            {
                if (onExitObj != null)
                {
                    onExitObj.Invoke(_collider);
                }

            }

            _collider = hit.collider;
            if (onEnterObj == null)
            {
                onEnterObj += ClickSphere;
            }

            onEnterObj.Invoke(hit);

        }
        else
        {
            if (onHoverObj != null)
            {
                onHoverObj.Invoke(hit);
            }

        }
    }
    void NotHit()
    {
        currentTransform = null;
        if (rayRenderer != null)
        {
            SetColor(rayRenderer, normalColor);
            rayRenderer.transform.parent.localScale = new Vector3(JoystickScale, JoystickScale, wandNormalLength);
            PointSphere.gameObject.SetActive(false);
        }
    }

    public void ClickSphere(RaycastHit hit)
    {
        // Sphere.GetComponent<MeshRenderer>().material.color = Color.red;
        SetColor(rayRenderer, hoverColor);
    }

    void OnDisable()
    {

        GazeInputModule.OnEnterTarget -= OnEnterUITatget;
        GazeInputModule.OnExitTarget -= OnExitUITatget;
    }
}

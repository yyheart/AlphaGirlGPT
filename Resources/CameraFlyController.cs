using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;

public class CameraFlyController : MonoBehaviour
{
    public List<GameObject> gameObjects;

    private float speed = 2f;
    public float forwardSpeed = 0.6f;
    public float leftSpeed = 0.6f;
    public float limitedHeight = 150;


    private Transform tr;

    private bool rmbDownInRect;
    private Vector3 lastMousePos;
    private Vector3 moveLastMousePos;
    private Vector3 originalRotation;
    [Tooltip("是否可操作位移 慎用")]
    public bool MEnabled = true;

    [Tooltip("UI上是否可操作位移 慎用")]
    public bool MUIEnabled = false;

    [ContextMenu("开启屏蔽")]
    void EditorFunc()
    {
        MEnabled = false;
    }

    [ContextMenu("关闭屏蔽")]
    void EditorFunc1()
    {
        MEnabled = true;
    }

    [ContextMenu("开启UI屏蔽")]
    void EditorFunc2()
    {
        MUIEnabled = false;
    }

    [ContextMenu("关闭UI屏蔽")]
    void EditorFunc3()
    {
        MUIEnabled = true;
    }

    private Vector3 mousePosition
    {
        get
        {
            Camera cam = GetComponent<Camera>();
            return cam == null ? Vector3.Scale(Input.mousePosition, new Vector3(1f / Screen.width, 1f / Screen.height, 1f)) : cam.ScreenToViewportPoint(Input.mousePosition);
        }
    }

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        MEnabled = true;
        MUIEnabled = false;
        tr = GetComponent<Transform>();
    }


    private float t;
    void OnEnable()
    {
        t = Time.realtimeSinceStartup;
    }


    bool rmbDown = false;//鼠标右键按下
    bool rmbHeld = false;//鼠标右键按住

    bool gunLunDown = false;//滚轮按下
    bool gunLunHeld = false;//滚轮按住

    float gunLundv = 0;//滚轮滚动值 >0向上滚动 <0向下滚动
    bool gunLunRoll = false;//滚轮滚动中

    bool mouseInCameraRect = false;//鼠标指针在视图范围内

    bool speedMultiplierChange; //shift 加速按键按下状态变化
    bool speedMultiplierDown;   //shift 加速按键是否按住 按住=True
    float speedMultiplier = 0;  //shift 加速按键对应的值

    private bool inUIBegin = false;

    void Update()
    {
        if (!MEnabled)
        {
            return;
        }
        speedMultiplierChange = speedMultiplierDown != Input.GetKey(KeyCode.LeftShift);
        speedMultiplierDown = Input.GetKey(KeyCode.LeftShift);
        speedMultiplier = speedMultiplierDown ? 2f : 1f;

        rmbDown = Input.GetMouseButtonDown(1);
        rmbHeld = Input.GetMouseButton(1);

        gunLunDown = Input.GetMouseButtonDown(2);
        gunLunHeld = Input.GetMouseButton(2);

        gunLundv = Input.GetAxis("Mouse ScrollWheel");
        gunLunRoll = gunLundv > 0.01f || gunLundv < -0.01f;

        if (rmbDown || gunLunDown || gunLunRoll)
        {
#if IPHONE || ANDROID
            inUIBegin = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#else
            inUIBegin = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
#endif
        }

        if (inUIBegin && !MUIEnabled)
        {
            return;
        }

        Vector3 nowMousePos = mousePosition;
        mouseInCameraRect = nowMousePos.x >= 0f && nowMousePos.x < 1f && nowMousePos.y >= 0f && nowMousePos.y < 1f;

        //（为了卡 鼠标在视图按下 + 右键按住）或（鼠标在视图范围 + 右键按下）
        rmbDownInRect = (rmbDownInRect && (rmbHeld || gunLunHeld || gunLunRoll)) || (mouseInCameraRect && (rmbDown || gunLunDown || gunLunRoll));

        float timeNow = Time.realtimeSinceStartup;
        float dT = timeNow - t;
        t = timeNow;

        // Movement 键盘控制 前后左右 上下 移动
        if (rmbDownInRect || (mouseInCameraRect && !rmbHeld && !gunLunHeld && !gunLunRoll))
        {
            float forward = 0f;
            float right = 0f;
            float up = 0f;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { forward += forwardSpeed; }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { forward -= forwardSpeed; }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { right += leftSpeed; }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { right -= leftSpeed; }

            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space)) { up += forwardSpeed; }
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.C)) { up -= forwardSpeed; }

            //tr.position += tr.TransformDirection(new Vector3(right, up, forward) * speed * speedMultiplier);
            tr.position = ConversionLimitedHeight(tr.position + tr.TransformDirection(new Vector3(right, up, forward) * speed * speedMultiplier * dT));
            RefreshFollowObj();
        }

        // Rotation
        if (rmbDownInRect)
        {
            // Right Mouse Button Down
            if (rmbDown || speedMultiplierChange)
            {
                originalRotation = tr.localEulerAngles;
                lastMousePos = nowMousePos;
            }

            // Right Mouse Button Hold 按住鼠标右键 控制上下左右 旋转
            if (rmbHeld)
            {
                Vector2 offs = new Vector2((nowMousePos.x - lastMousePos.x), (nowMousePos.y - lastMousePos.y));
                //tr.localEulerAngles = originalRotation + new Vector3(offs.y * 360f, offs.x * 360f, 0f);

                float eulerAngles_x = originalRotation.y;
                float eulerAngles_y = originalRotation.x;

                eulerAngles_x += (offs.x) * speed * 50 * speedMultiplier;
                eulerAngles_y -= (offs.y) * speed * 50 * speedMultiplier;
                if (offs.y > 0)
                {
                    if ((eulerAngles_y < 330 && eulerAngles_y > 90) || eulerAngles_y < -30)
                    {
                        eulerAngles_y = 330;
                    }
                }
                else if (offs.y < 0)
                {
                    if ((eulerAngles_y > 90 && eulerAngles_y < 330) || eulerAngles_y > 450)
                    {
                        eulerAngles_y = 90;
                    }
                }
                Quaternion quaternion = Quaternion.Euler(eulerAngles_y, eulerAngles_x, (float)0);
                tr.rotation = quaternion;
            }

            //middle  Mouse Button Down
            if (gunLunDown)
            {
                moveLastMousePos = nowMousePos;
            }

            // middle Mouse Button Hold 按住鼠标滚轮 控制前后左右 移动
            if (gunLunHeld)
            {
                Vector3 direction = new Vector3((nowMousePos.x - moveLastMousePos.x), (nowMousePos.y - moveLastMousePos.y), 0);
                float tranY = direction.y * (float)Math.Sin(Math.Round(tr.localRotation.eulerAngles.x, 2) * Math.PI / 180.0);
                float tranZ = direction.y * (float)Math.Cos(Math.Round(tr.localRotation.eulerAngles.x, 2) * Math.PI / 180.0);
                tr.Translate(new Vector3(-direction.x, -tranY, -tranZ) * speed * tr.position.y * speedMultiplier, Space.Self);
                moveLastMousePos = nowMousePos;
                RefreshFollowObj();
            }

            // 鼠标滚轮向上滚动 || 鼠标滚轮向下滚动 滚动滚轮 控制视角远近 移动
            if (gunLundv > 0.01f || gunLundv < -0.01f)
            {
                tr.Translate(ConversionLimitedHeight(Vector3.forward * gunLundv * speed * tr.position.y * speedMultiplier), Space.Self);
                RefreshFollowObj();
            }
        }
    }

    private bool CheckConversionLimitedHeight(Vector3 pos)
    {
        return pos.y > limitedHeight;
    }

    private Vector3 ConversionLimitedHeight(Vector3 pos)
    {
        if (CheckConversionLimitedHeight(pos))
        {
            var juli = pos.y - limitedHeight;
            var poss = tr.position + tr.forward * juli;
            return poss;
        }
        else
        {
            return pos;
        }
    }

    public void ClearFollowObj()
    {
        if (gameObjects != null)
        {
            gameObjects.Clear();
        }
    }

    //添加跟随物体(仅位置跟随)
    public void AddFollowObj(GameObject obj)
    {
        if (gameObjects == null)
        {
            gameObjects = new List<GameObject>();
        }
        if (!gameObjects.Contains(obj))
        {
            gameObjects.Add(obj);
        }
    }

    //刷新跟随物体(仅位置跟随,其他旋转等跟随不能加在这个List里)
    private void RefreshFollowObj()
    {
        if (gameObjects == null)
        {
            return;
        }
        foreach (var obj in gameObjects)
        {
            obj.transform.position = tr.position;
        }
    }

    ////移动相机（有动画）
    //public void SetCameraPos(CameraInit cameraInit, float time = 0, float delay = 0, UnityAction unityAction = null)
    //{
    //    SetCameraPos(cameraInit.CameraMove, cameraInit.CameraRot, time, delay, unityAction);
    //}

    //移动相机
    public void SetCameraPos(Transform trans, float time = 0, float delay = 0)
    {
        SetCameraPos(trans.position, trans.rotation.eulerAngles, time, delay);
    }

    Tweener posTweener;
    Tweener rotTweener;

    bool posComplete;
    bool rotComplete;

    UnityAction tweenUnityAction;
    //移动相机
    public void SetCameraPos(Vector3 pos, Vector3 rot, float time = 0, float delay = 0, UnityAction unityAction = null)
    {
        KillTween();

        if (time > 0)
        {
            tweenUnityAction = unityAction;

            posTweener = this.tr.DOLocalMove(ConversionLimitedHeight(pos), time).SetRelative(false).SetDelay(delay).SetEase(Ease.InOutQuad);
            posTweener.onComplete = () => { posComplete = true; TweenCallBack(); };
            rotTweener = this.tr.DOLocalRotate(rot, time).SetRelative(false).SetDelay(delay).SetEase(Ease.InOutQuad);
            rotTweener.onComplete = () => { rotComplete = true; TweenCallBack(); };
        }
        else
        {
            tr.position = ConversionLimitedHeight(pos);
            tr.rotation = Quaternion.Euler(rot);

            RefreshFollowObj();
            unityAction?.Invoke();
        }
    }

    private void KillTween()
    {
        if (posTweener != null)
        {
            posTweener.Kill();
            posTweener = null;
        }
        if (rotTweener != null)
        {
            rotTweener.Kill();
            rotTweener = null;
        }
    }

    private void TweenCallBack()
    {
        if (posComplete && rotComplete)
        {
            RefreshFollowObj();
            tweenUnityAction?.Invoke();

            tweenUnityAction = null;
            posComplete = false;
            rotComplete = false;

            KillTween();
        }
    }
}

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
    [Tooltip("�Ƿ�ɲ���λ�� ����")]
    public bool MEnabled = true;

    [Tooltip("UI���Ƿ�ɲ���λ�� ����")]
    public bool MUIEnabled = false;

    [ContextMenu("��������")]
    void EditorFunc()
    {
        MEnabled = false;
    }

    [ContextMenu("�ر�����")]
    void EditorFunc1()
    {
        MEnabled = true;
    }

    [ContextMenu("����UI����")]
    void EditorFunc2()
    {
        MUIEnabled = false;
    }

    [ContextMenu("�ر�UI����")]
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


    bool rmbDown = false;//����Ҽ�����
    bool rmbHeld = false;//����Ҽ���ס

    bool gunLunDown = false;//���ְ���
    bool gunLunHeld = false;//���ְ�ס

    float gunLundv = 0;//���ֹ���ֵ >0���Ϲ��� <0���¹���
    bool gunLunRoll = false;//���ֹ�����

    bool mouseInCameraRect = false;//���ָ������ͼ��Χ��

    bool speedMultiplierChange; //shift ���ٰ�������״̬�仯
    bool speedMultiplierDown;   //shift ���ٰ����Ƿ�ס ��ס=True
    float speedMultiplier = 0;  //shift ���ٰ�����Ӧ��ֵ

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

        //��Ϊ�˿� �������ͼ���� + �Ҽ���ס�����������ͼ��Χ + �Ҽ����£�
        rmbDownInRect = (rmbDownInRect && (rmbHeld || gunLunHeld || gunLunRoll)) || (mouseInCameraRect && (rmbDown || gunLunDown || gunLunRoll));

        float timeNow = Time.realtimeSinceStartup;
        float dT = timeNow - t;
        t = timeNow;

        // Movement ���̿��� ǰ������ ���� �ƶ�
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

            // Right Mouse Button Hold ��ס����Ҽ� ������������ ��ת
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

            // middle Mouse Button Hold ��ס������ ����ǰ������ �ƶ�
            if (gunLunHeld)
            {
                Vector3 direction = new Vector3((nowMousePos.x - moveLastMousePos.x), (nowMousePos.y - moveLastMousePos.y), 0);
                float tranY = direction.y * (float)Math.Sin(Math.Round(tr.localRotation.eulerAngles.x, 2) * Math.PI / 180.0);
                float tranZ = direction.y * (float)Math.Cos(Math.Round(tr.localRotation.eulerAngles.x, 2) * Math.PI / 180.0);
                tr.Translate(new Vector3(-direction.x, -tranY, -tranZ) * speed * tr.position.y * speedMultiplier, Space.Self);
                moveLastMousePos = nowMousePos;
                RefreshFollowObj();
            }

            // ���������Ϲ��� || ���������¹��� �������� �����ӽ�Զ�� �ƶ�
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

    //��Ӹ�������(��λ�ø���)
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

    //ˢ�¸�������(��λ�ø���,������ת�ȸ��治�ܼ������List��)
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

    ////�ƶ�������ж�����
    //public void SetCameraPos(CameraInit cameraInit, float time = 0, float delay = 0, UnityAction unityAction = null)
    //{
    //    SetCameraPos(cameraInit.CameraMove, cameraInit.CameraRot, time, delay, unityAction);
    //}

    //�ƶ����
    public void SetCameraPos(Transform trans, float time = 0, float delay = 0)
    {
        SetCameraPos(trans.position, trans.rotation.eulerAngles, time, delay);
    }

    Tweener posTweener;
    Tweener rotTweener;

    bool posComplete;
    bool rotComplete;

    UnityAction tweenUnityAction;
    //�ƶ����
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeSet : MonoBehaviour {
    private Camera cam;
    [SerializeField]
    public Transform plane;
    [SerializeField]
    public StereoTargetEyeMask eye;
    public bool inverseEye=false;
    bool once = true;

    void Awake()
    {
        QualitySettings.antiAliasing = 0;
    }
	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        if (!inverseEye)
        {
            //Toggle();
            RestoreEye();
        }
        else
        {
            InverseEye();
        }
    }
    //重新写
    //初始化左右眼设置。
    void RestoreEye()
    {
        cam.stereoTargetEye = eye;
        cam.stereoSeparation = 0f;
    }
    //反转左右眼
    /// <summary>
    /// 反转左右眼
    /// </summary>
    void InverseEye()
    {
        if (once)
        {
            once = false;
            if (eye == StereoTargetEyeMask.Left)
            {
                eye = StereoTargetEyeMask.Right;
            }
            else if (eye == StereoTargetEyeMask.Right)
            {
                eye = StereoTargetEyeMask.Left;
            }
            cam.stereoTargetEye = eye;
            cam.stereoSeparation = 0f;
            StartCoroutine(WaitforTime());
        }
       
    }
    IEnumerator WaitforTime()
    {
        yield return new WaitForSeconds(1f);
        once = true;
    }
    void Set()
    {
        cam.stereoTargetEye = eye;
        cam.stereoSeparation = 0f;
    }

    void Toggle()
    {
        Set();
    }
	
	// Update is called once per frame
	void Update () {
        if (cam.stereoEnabled)
        {
            if (eye == StereoTargetEyeMask.Left)
               VRAppTools.instance.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, plane, cam, 0f);
            else if (eye == StereoTargetEyeMask.Right)
                VRAppTools.instance.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, plane, cam, 0f);
            //if (eye == StereoTargetEyeMask.Left)
            //    VRAppTools.instance.SetProjectionMatrix(plane, cam, 0f);
            //else if (eye == StereoTargetEyeMask.Right)
            //    VRAppTools.instance.SetProjectionMatrix(plane, cam, 0f);
        }
        //else
        //{
        //    VRAppTools.instance.SetProjectionMatrix(plane, cam, 0f);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha4) &&AlphaMotion.instance.GetButtonDown(7))
        if (AlphaMotion.instance.GetButton(0) && AlphaMotion.instance.GetButtonDown(6))
        {
            //Toggle();
            InverseEye();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpawnEffect : MonoBehaviour
{
    public Vector3 modelScale = new Vector3(1.5f, 1.5f, 1.5f);

    [Header("相机移动相关")]
    public Vector3 cameraMoveStartPos;  //相机启动时的初始视角
    public Vector3 cameraMoveStartRot;  //相机启动时的初始旋转

    public float cameraMoveQTime = 2;                                   //相机推进移动时间
    public Vector3 cameraMoveQPos = new Vector3(-0.32f, 0.25f, -7.5f);  //相机推进移动坐标
    public Vector3 cameraMoveQRot = new Vector3(0f, 0f, 0f);            //相机推进移动坐标

    public float cameraMoveWTime = 2;                                   //相机推进Girl面前移动时间
    public Vector3 cameraMoveWPos = new Vector3(-1.42f, 0.25f, -4.38f); //相机推进Girl面前移动坐标
    public Vector3 cameraMoveWRot = new Vector3(0, -12.5f, 0);          //相机推进Girl面前移动旋转

    public float cameraMoveETime = 4;                                   //相机推进显示背景移动时间
    public Vector3 cameraMoveEPos = new Vector3(1.03f, 0.25f, -4.38f);  //相机推进显示背景移动坐标
    public Vector3 cameraMoveERot = new Vector3(0, -12.5f, 0);          //相机推进显示背景移动旋转

    public float cameraMoveRTime = 2;                                   //相机返回移动时间
    public Vector3 cameraMoveRPos = new Vector3(-0.32f, 0.25f, -7.5f);  //相机返回移动坐标
    public Vector3 cameraMoveRRot = new Vector3(0, 0, 0);               //相机返回移动旋转

    [Header("出场相关")]
    public float girlEnterTime = 14f;   //AlphaGirl 延迟出场时间
    public float girlTalkTime = 4f;     //AlphaGirl 延迟说话时间
    public float bossEixtTime = 20f;    //老板 延迟退场时间 
    [Header("特效播放时长")]
    public float spawnEffectTime = 1.5f;


    private ParticleSystem ps;

    private Transform vRController564;

    private Animator girlAnimator;
    private Transform girlTrans;
    private Transform girlEnterEffect;
    private Transform bossEnterEffect;

    private UnityEngine.Video.VideoPlayer videoPlayer;

    private Transform exeButtonParent;


    private Tweener bossTweenerEnterEffect; //老板出场
    private Tweener bossTweenerEixtEffect;  //老板出场

    private Tweener girlTweenerEnterTrans;  //girl出场缩放
    private Tweener girlTweenerEnterEffect; //girl出场特效
    private Tweener girlTweenerTalkEffect;  //girl讲话

    private CameraFlyController CameraFlyController;

    public bool Use;
    private bool switchKey;


    private void Update()
    {
        if (Use)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                switchKey = !switchKey;
                CameraFlyController.MEnabled = !switchKey;
            }

            if (!switchKey)
            {
                return;
            }

            //位移
            if (Input.GetKeyDown(KeyCode.Q))
            {
                MoveQ();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveW();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                MoveE();
                PlayVideo();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                MoveBack();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                StartPos();
            }


            //出场
            if (Input.GetKeyDown(KeyCode.A))
            {
                PlaySpawnEffect();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                HideModel();
            }


            //说话
            if (Input.GetKeyDown(KeyCode.D))
            {
                GirlEnterEffect(false);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                OnEffectCompleteTalk();
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                var aaa = GameObject.FindObjectOfType<CrazyMinnow.SALSA.Salsa3D>();
                if (aaa != null)
                {
                    aaa.Stop();
                }
            }

            //视频
            if (Input.GetKeyDown(KeyCode.V))
            {
                exeButtonParent.DOLocalMoveY(1154, 0);
                //PlayVideo();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                VideoCom(null);
            }
           
            girlTrans.LookAt(vRController564);
            girlTrans.transform.eulerAngles = new Vector3(0, girlTrans.transform.eulerAngles.y, 0);
        }
    }



    private void Start()
    {
        girlTrans = transform.ZYFindChild("xunijuese").transform;
        girlEnterEffect = transform.ZYFindChild("Respawn").transform;
        bossEnterEffect = transform.ZYFindChild("Respawn (1)").transform;

        CameraFlyController = GameObject.FindObjectOfType<CameraFlyController>();

        var canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            videoPlayer = canvas.transform.ZYFindChild("Video Player").GetComponent<UnityEngine.Video.VideoPlayer>();
            videoPlayer.gameObject.SetActive(false);
            videoPlayer.loopPointReached += VideoCom;
        }

        if (Use)
        {
            girlAnimator = girlTrans.GetComponentInChildren<Animator>();

            vRController564 = GameObject.FindObjectOfType<VRController>().transform;
            if (vRController564 != null)
            {
                cameraMoveStartPos = vRController564.position;
                cameraMoveStartRot = vRController564.eulerAngles;
            }

            if (canvas != null)
            {
                exeButtonParent = canvas.transform.ZYFindChild("ExeButtonParent");
                exeButtonParent.position = new Vector3(exeButtonParent.position.x, 880, exeButtonParent.position.z);
            }
        }
        else
        {
            CameraFlyController.MEnabled = false;
            transform.DOScale(Vector3.one, 1).onComplete = OnEffectCompleteTalk;
        }

        HideModel();
    }

    private void StartPos()
    {
        vRController564.DOLocalMove(cameraMoveStartPos, cameraMoveQTime).SetEase(Ease.Linear);
        vRController564.DORotate(cameraMoveStartRot, cameraMoveQTime).SetEase(Ease.Linear);
    }

    private void MoveQ()
    {
        vRController564.DOLocalMove(cameraMoveQPos, cameraMoveQTime).SetEase(Ease.Linear);
        vRController564.DORotate(cameraMoveQRot, cameraMoveQTime).SetEase(Ease.Linear);
        exeButtonParent.DOLocalMoveY(0, 1.5f).SetEase(Ease.InOutQuad).SetDelay(1);
    }

    private void MoveW()
    {
        vRController564.DOLocalMove(cameraMoveWPos, cameraMoveWTime).SetEase(Ease.Linear);
        vRController564.DORotate(cameraMoveWRot, cameraMoveWTime).SetEase(Ease.Linear);
        transform.DOScale(Vector3.one, 1).onComplete = () =>
        {
            girlAnimator.Play("Talk");
        };
    }

    private void MoveE()
    {
        vRController564.DOLocalMove(cameraMoveEPos, cameraMoveETime).SetEase(Ease.Linear);
        vRController564.DORotate(cameraMoveERot, cameraMoveETime).SetEase(Ease.Linear);
    }

    private void MoveBack()
    {
        vRController564.DOLocalMove(cameraMoveRPos, cameraMoveRTime).SetEase(Ease.Linear);
        vRController564.DORotate(cameraMoveRRot, cameraMoveRTime).SetEase(Ease.Linear);
    }

    //开始效果
    private void PlaySpawnEffect()
    {
        HideModel();

        if (bossEnterEffect != null)
        {
            OnExitEffect();

            bossTweenerEnterEffect = bossEnterEffect.DOScale(1.5f, spawnEffectTime);
            bossTweenerEnterEffect.onComplete = () =>
            {
                GirlEnterEffect(true);

                BossExitEffect();
            };
        }
    }


    //AlphaGirl 出场效果
    private void GirlEnterEffect(bool talk)
    {
        if (girlTrans != null)
        {
            girlTweenerEnterTrans = girlTrans.DOScale(modelScale, spawnEffectTime).SetDelay(talk ? girlEnterTime : 0);
        }

        if (girlEnterEffect != null)
        {
            girlTweenerEnterEffect = girlEnterEffect.DOScale(1f, 0).SetDelay(talk ? girlEnterTime : 0);
            girlTweenerEnterEffect.onComplete = () => { OnDelayComplete(talk); };
        }
    }

    //AlphaGirl 出场特效 延迟播放回调 播放出场特效
    private void OnDelayComplete(bool talk)
    {
        girlEnterEffect.gameObject.SetActive(true);
        ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.duration = spawnEffectTime;
            ps.Play();

            girlTweenerTalkEffect = girlEnterEffect.DOScaleY(1f, 0).SetDelay(girlTalkTime);
            if (talk)
            {
                girlTweenerTalkEffect.onComplete = OnEffectCompleteTalk;
            }
        }
    }

    //AlphaGirl 出场特效 播放完成回调  开始说话
    private void OnEffectCompleteTalk()
    {
        var aaa = GameObject.FindObjectOfType<CrazyMinnow.SALSA.Salsa3D>();
        if (aaa != null)
        {
            aaa.Play();
        }

        //girlAnimator.Play("Hello");
    }


    //老板 退场效果
    private void BossExitEffect()
    {
        if (bossEnterEffect != null)
        {
            bossTweenerEixtEffect = bossEnterEffect.DOScale(1f, 0).SetDelay(bossEixtTime);
            bossTweenerEixtEffect.onComplete = OnExitEffect;
        }
    }

    //老板 退场特效播放 完成回调
    private void OnExitEffect()
    {
        bossEnterEffect.gameObject.SetActive(true);
        ps = bossEnterEffect.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.duration = spawnEffectTime;

            ps.Play();
        }
    }

    //播放 背景视频
    private void PlayVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.Play();
        }
    }
    //停止 背景视频
    private void VideoCom(UnityEngine.Video.VideoPlayer video)
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
        }
    }

    public void HideModel()
    {
        if (bossTweenerEnterEffect != null)
        {
            bossTweenerEnterEffect?.Kill();
            bossTweenerEnterEffect = null;
        }

        if (bossTweenerEixtEffect != null)
        {
            bossTweenerEixtEffect?.Kill();
            bossTweenerEixtEffect = null;
        }

        if (girlTweenerEnterTrans != null)
        {
            girlTweenerEnterTrans?.Kill();
            girlTweenerEnterTrans = null;
        }

        if (girlTweenerEnterEffect != null)
        {
            girlTweenerEnterEffect?.Kill();
            girlTweenerEnterEffect = null;
        }

        if (girlTweenerTalkEffect != null)
        {
            girlTweenerTalkEffect?.Kill();
            girlTweenerTalkEffect = null;
        }

        if (bossEnterEffect != null)
        {
            bossEnterEffect.gameObject.SetActive(false);
        }

        if (girlTrans != null)
        {
            girlTrans.localScale = Use ? Vector3.zero : modelScale;
        }

        if (girlEnterEffect != null)
        {
            girlEnterEffect.gameObject.SetActive(false);
        }

        VideoCom(null);
    }
}
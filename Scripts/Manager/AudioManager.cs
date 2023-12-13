using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : SingletonAuto<AudioManager>
{
    //背景音效---------------
    [SerializeField]
    private int bgmAudioVolume = 5;
    public float BGMAudioVolume
    {
        get { return (float)bgmAudioVolume / 100f; }
    }

    //背景音乐的AudioSource
    private AudioSource BGMAudioSource;
    //背景音乐是否在暂停
    public bool IsBGMPause
    {
        get; private set;
    }
    public UnityEvent<bool> OnIsBGStateChange;

    //其他音效---------------
    [SerializeField]
    private int otherAudioVolume = 100;
    private float beforeMuteOtherAudioVolume = 100;
    public float OtherAudioVolume
    {
        get { return (float)otherAudioVolume / 100f; }
    }

    public AudioSource OtherAudioSource
    {
        private set;
        get;
    }

    //按钮音效---------------
    [SerializeField]
    private int btnAudioVolume = 50;
    public float BtnAudioVolume
    {
        get { return (float)btnAudioVolume / 100f; }
    }

    //介绍功能AudioSource
    private AudioSource ButtonAudioSource;
    public AudioClip HoverSound;
    public AudioClip ClickSound;


    protected override void Awake()
    {
        base.Awake();
        Init();
    }


    void Init()
    {
        HoverSound = Resources.Load<AudioClip>("AudioClip/Click/点击");
        ClickSound = Resources.Load<AudioClip>("AudioClip/Click/选中");

        //得到背景音乐的source
        GameObject BGMAudioSourceObj = new GameObject("BGMAudioSource");
        BGMAudioSource = BGMAudioSourceObj.AddComponent<AudioSource>();
        BGMAudioSource.transform.SetParent(this.transform, false);
        BGMAudioSource.playOnAwake = true;
        BGMAudioSource.loop = true;
        BGMAudioSource.volume = BGMAudioVolume;
        //BGMAudioSource.clip = Resources.Load<AudioClip>("中国风格-经典笛子演奏中国风音乐-清幽寂静自由空灵-流水潺潺_爱给网_aigei_com");
        //BGMAudioSource.Play();

        //其他音频 AudioSource
        GameObject OtherAudioSourceObj = new GameObject("OtherAudioSource");
        OtherAudioSource = OtherAudioSourceObj.AddComponent<AudioSource>();
        OtherAudioSource.playOnAwake = false;
        OtherAudioSource.volume = OtherAudioVolume;
        OtherAudioSource.transform.SetParent(this.transform, false);

        //按钮音频 AudioSource
        GameObject ButtonAudioSourceObj = new GameObject("ButtonAudioSource");
        ButtonAudioSource = ButtonAudioSourceObj.AddComponent<AudioSource>();
        ButtonAudioSource.playOnAwake = false;
        ButtonAudioSource.volume = BtnAudioVolume;
        ButtonAudioSource.transform.SetParent(this.transform, false);
    }

    public void PlayBGM(string name)
    {
        LoadResourcesManager.Instance.GetAudioClip(name, (audio) =>
        {
            BGMAudioSource.clip = audio;
            if (audio != null)
            {
                BGMAudioSource.Play();
            }
        });
    }

    //void Start()
    //{
    //    //得到背景音乐的source
    //    var BGMAudioSourceObj = transform.Find("BGMAudioSource");
    //    if (BGMAudioSourceObj != null)
    //    {
    //        BGMAudioSource = BGMAudioSourceObj.GetComponent<AudioSource>();
    //        BGMAudioSource.volume = BGMAudioVolume;
    //    }
    //    else
    //    {
    //        Debug.LogError("AudioSourceManager.Start()，BGMAudioSource is null");
    //    }

    //    //其他音频 AudioSource
    //    var OtherAudioSourceObj = transform.Find("OtherAudioSource");
    //    if (OtherAudioSourceObj != null)
    //    {
    //        OtherAudioSource = OtherAudioSourceObj.GetComponent<AudioSource>();
    //    }
    //    else
    //    {
    //        Debug.LogError("AudioSourceManager.Start()，OtherAudioSource is null");
    //    }

    //    var ButtonAudioSourceObj = transform.Find("ButtonAudioSource");
    //    if (ButtonAudioSourceObj != null)
    //    {
    //        ButtonAudioSource = ButtonAudioSourceObj.GetComponent<AudioSource>();
    //    }
    //    else
    //    {
    //        Debug.LogError("AudioSourceManager.Start()，ButtonAudioSource is null");
    //    }
    //}


    // 静音 ([暂停 背景音频]  [停止 其他音频] )
    public void Mute()
    {
        SetBGMState(true);
        OtherAudioSourceStop();
    }

    // 取消静音 ([恢复 背景音频])
    public void UnMute()
    {
        SetBGMState(false);
    }


    #region 背景音乐相关
    // UI点击设置 背景音频播放状态 true=暂停  flase=播放 
    public void SetBGMState(bool state)
    {
        if (IsBGMPause != state)
        {
            if (state)//暂停
            {
                BGMAudioSource.Pause();
            }
            else
            {
                BGMAudioSource.UnPause();
            }

            IsBGMPause = state;
            OnIsBGStateChange?.Invoke(state);
        }
    }

    //背景音乐的声音大小
    public void SetBGMVolume(float value)
    {
        bgmAudioVolume = (int)(value * 100);
        if (BGMAudioSource != null)
        {
            BGMAudioSource.volume = value;
        }
    }
    #endregion 背景音乐相关

    #region 其他音频
    //静音
    public void MuteOtherVolume()
    {
        beforeMuteOtherAudioVolume = OtherAudioVolume;
        SetOtherVolume(0);
    }

    //取消静音
    public void UnMuteOtherVolume()
    {
        SetOtherVolume(beforeMuteOtherAudioVolume);
    }

    public void SetOtherVolume(float value)
    {
        otherAudioVolume = (int)(value * 100);
        if (OtherAudioSource != null)
        {
            OtherAudioSource.volume = value;
        }
    }

    public bool OtherAudioSourceIsPlaying
    {
        get { return OtherAudioSource.isPlaying; }
    }

    Coroutine OtherCoroutine;
    UnityAction OtherUnityAction;

    public void OtherAudioSourcePlay(AudioClip audioClip, UnityAction unityAction)
    {
        //Debug.LogError("播放开始");

        if (OtherAudioSource != null)
        {
            if (audioClip == null)
            {
                Debug.LogError("AudioSourceManager.OtherAudioSourcePlay()，audioClip is null");
                OtherAudioSource.Stop();
                unityAction?.Invoke();
                return;
            }
            OtherAudioSource.clip = audioClip;
            OtherAudioSource.Stop();
            OtherAudioSource.Play();

            OtherUnityAction = unityAction;
            StartOtherCoroutine(true);
        }
        else
        {
            Debug.LogError("AudioSourceManager.OtherAudioSourcePlay()，OtherAudioSource is null");
        }
    }


    public void OtherAudioSourcePause()
    {
        //Debug.LogError("播放暂停");

        if (OtherAudioSource != null)
        {
            ClearOtherCoroutine(false);

            OtherAudioSource.Pause();
        }
        else
        {
            Debug.LogError("AudioSourceManager.OtherAudioSourcePause()，OtherAudioSource is null");
        }
    }

    public void OtherAudioSourceUnPause()
    {
        //Debug.LogError("播放恢复");

        if (OtherAudioSource != null)
        {
            StartOtherCoroutine(false);

            OtherAudioSource.UnPause();
        }
        else
        {
            Debug.LogError("AudioSourceManager.OtherAudioSourceUnPause()，OtherAudioSource is null");
        }
    }


    public bool OtherAudioSourceStop(bool isCallBack = true)
    {
        //Debug.LogError("播放停止");

        if (OtherAudioSource != null)
        {
            OtherAudioSource.Stop();

            return ClearOtherCoroutine(isCallBack);
        }
        else
        {
            Debug.LogError("AudioSourceManager.OtherAudioSourceStop()，OtherAudioSource is null");
            return false;
        }
    }

    private void StartOtherCoroutine(bool isStart)
    {
        ClearOtherCoroutine(false);

        if (OtherCoroutine != null || isStart)
        {
            float totalTime = (float)OtherAudioSource?.clip.length;
            var time = OtherAudioSource.time;

            //Debug.LogError("携程时间   " + (totalTime - time));

            OtherCoroutine = StartCoroutine(OtherAudioCallBack(totalTime - time));
        }
    }

    private bool ClearOtherCoroutine(bool isCallBack)
    {
        bool isPlay = OtherCoroutine != null;
        if (isPlay)
        {
            //Debug.LogError("停止携程   " + (isCallBack ? "执行回调" : "仅仅停止"));

            StopCoroutine(OtherCoroutine);
            OtherCallBack(isCallBack);
        }
        return isPlay;
    }

    private IEnumerator OtherAudioCallBack(float time)
    {
        yield return new WaitForSeconds(time);
        //Debug.LogError("播放结束");
        OtherCallBack(true);
    }

    private void OtherCallBack(bool isCallBack)
    {
        //Debug.LogError("其他音频回调");
        OtherCoroutine = null;
        if (isCallBack)
        {
            var tempOtherUnityAction = OtherUnityAction;//中转一层 为了先清理回调函数再通知回调 防止空回调调用上次回调
            OtherUnityAction = null;
            tempOtherUnityAction?.Invoke();
        }
        else
        {
            OtherUnityAction = null;
        }
    }
    #endregion 其他音频

    #region 按钮音频
    public void SetBtnVolume(float value)
    {
        btnAudioVolume = (int)(value * 100);

        if (ButtonAudioSource != null)
        {
            ButtonAudioSource.volume = value;
        }
    }
    public void ButtonPlaySound(AudioClip audioClip)
    {
        if (ButtonAudioSource != null)
        {
            if (audioClip == null)
            {
                Debug.LogError("AudioSourceManager.ButtonPlaySound()，audioClip is null");
                return;
            }
            ButtonAudioSource.clip = audioClip;
            ButtonAudioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSourceManager.ButtonPlaySound()，ButtonAudioSource is null");
        }
    }

    public void ButtonPlayHoverSound()
    {
        if (ButtonAudioSource == null)
        {
            Debug.LogError("AudioSourceManager.ButtonPlayHoverSound()，ButtonAudioSource is null");
        }
        else if (HoverSound == null)
        {
            Debug.LogError("AudioSourceManager.ButtonPlayHoverSound()，HoverSound is null");
        }
        else
        {
            ButtonAudioSource.clip = HoverSound;
            ButtonAudioSource.Play();
        }
    }

    public void ButtonPlayClickSound()
    {
        if (ButtonAudioSource == null)
        {
            Debug.LogError("AudioSourceManager.ButtonPlayClickSound()，ButtonAudioSource is null");
        }
        else if (ClickSound == null)
        {
            Debug.LogError("AudioSourceManager.ButtonPlayClickSound()，ClickSound is null");
        }
        else
        {
            ButtonAudioSource.clip = ClickSound;
            ButtonAudioSource.Play();
        }
    }
    #endregion 按钮音频
}
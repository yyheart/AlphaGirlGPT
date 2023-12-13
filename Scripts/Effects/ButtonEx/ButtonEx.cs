using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonExSoundEffectType
{
    None = 0,
    通用音效 = 1,
    单独音效 = 2,
}


public class ButtonEx : Button
{
    private bool pointerWasUp;
    public Vector3 initialPosition;
    public Vector3 initialScale;

    [Header("空白不响应点击")]
    public bool useBlockBlank = false;

    [Header("音效类型")]
    public ButtonExSoundEffectType soundEffectType;

    public bool useHoverSoundClip = true;
    public AudioClip hoverSoundClip;
    public bool usePressedSoundClip = true;
    public AudioClip pressedSoundClip;

    [Header("悬浮动效")]
    public bool useTransformEffect = false;
    public Transform variationTransform;
    public float variationDuration = 0.2f;
    public Vector3 variationScale;
    public Vector3 variationDistance;


    public UnityEngine.Events.UnityAction OnPointerEnterEvent;
    public UnityEngine.Events.UnityAction OnPointerExitEvent;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        if (variationTransform)
        {
            initialPosition = variationTransform.localPosition;
            initialScale = variationTransform.localScale;
        }

        if (useBlockBlank)
        {
            var child = transform.GetComponentsInChildren<Image>();
            for (int i = 0; i < child.Length; i++)
            {
                child[i].alphaHitTestMinimumThreshold = 0.1f;
            }
        }
    }

    //按下
    public override void OnPointerDown(PointerEventData eventData)
    {
        PlayTransformEffect(false, "OnPointerDown");

        base.OnPointerDown(eventData);
    }

    //抬起
    public override void OnPointerUp(PointerEventData eventData)
    {
        //pointerWasUp = true;

        base.OnPointerUp(eventData);
    }

    //进入
    public override void OnPointerEnter(PointerEventData eventData)
    {
        //if (pointerWasUp)
        //{
        //    pointerWasUp = false;
        //}
        //else
        //{
        if (useHoverSoundClip)
        {
            if (soundEffectType == ButtonExSoundEffectType.通用音效)
            {
                AudioManager.Instance?.ButtonPlayHoverSound();
            }
            else if (soundEffectType == ButtonExSoundEffectType.单独音效)
            {
                if (hoverSoundClip != null)
                {
                    AudioManager.Instance?.ButtonPlaySound(hoverSoundClip);
                }
                else
                {
                    Debug.LogErrorFormat("ButtonEx.OnPointerEnter()，HoverSoundClip is null\nTransformPaht：{0}", GetPath());
                }
            }
        }
        //}
        PlayTransformEffect(true, "OnPointerEnter");

        base.OnPointerEnter(eventData);
        if (OnPointerEnterEvent != null)
        {
            OnPointerEnterEvent.Invoke();
        }
    }

    //离开
    public override void OnPointerExit(PointerEventData eventData)
    {
        //pointerWasUp = false;
        PlayTransformEffect(false, "OnPointerExit");

        base.OnPointerExit(eventData);
        if (OnPointerExitEvent != null)
        {
            OnPointerExitEvent.Invoke();
        }
    }

    //点击（按下+抬起）
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (usePressedSoundClip)
        {
            if (soundEffectType == ButtonExSoundEffectType.通用音效)
            {
                AudioManager.Instance?.ButtonPlayClickSound();
            }
            else if (soundEffectType == ButtonExSoundEffectType.单独音效)
            {
                if (pressedSoundClip != null)
                {
                    AudioManager.Instance?.ButtonPlaySound(pressedSoundClip);
                }
                else
                {
                    Debug.LogErrorFormat("ButtonEx.OnPointerClick()，PressedSoundClip is null\nTransformPaht：{0}", GetPath());
                }
            }
        }

        base.OnPointerClick(eventData);
    }

    [ContextMenu("打开悬浮动效")]
    private void EditorFunc()
    {
        useTransformEffect = true;
        EditorFunc2();
    }

    [ContextMenu("关闭悬浮动效")]
    private void EditorFunc1()
    {
        useTransformEffect = false;
    }

    [ContextMenu("刷新悬浮动效")]
    private void EditorFunc2()
    {
        if (useTransformEffect)
        {
            variationTransform = transform;
            variationDuration = 0.2f;
            variationScale = Vector2.one * 1.02f;
            variationDistance = Vector3.back * 0.3f;
        }
    }

    //true = 动画到目标效果 false = 还原
    private void PlayTransformEffect(bool value, string func)
    {
        if (useTransformEffect)
        {
            if (variationTransform == null)
            {
                Debug.LogErrorFormat("ButtonEx.{0}()，TransformTransform is null\nTransformPaht：{1}", func, GetPath());
            }
            else
            {
                variationTransform.DOScale(value ? variationScale : initialScale, variationDuration);
                variationTransform.DOLocalMove(value ? initialPosition + variationDistance : initialPosition, variationDuration);
            }
        }
    }


    private string GetPath()
    {
        string pathFormat = "{0}/{1}";

        Transform trans = transform;
        string transPath = trans.name;

        while (trans.parent != null)
        {
            transPath = string.Format(pathFormat, trans.parent.name, transPath);
            trans = trans.parent;
        }

        return transPath;
    }
}
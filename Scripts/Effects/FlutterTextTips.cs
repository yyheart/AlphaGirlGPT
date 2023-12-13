using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

[RequireComponent(typeof(UnityEngine.UI.Text))]//Text组件是必须的
public class FlutterTextTips : MonoBehaviour
{
    private Text Text_Tips;

    [SerializeField]
    private Ease m_TweenEase = Ease.InBack;

    [SerializeField]
    private Color m_Color = Color.white;

    private Tweener Tweener;

    private string m_StrValue;

    [SerializeField]
    private float m_Duration = 1;

    private UnityAction m_UnityAction;

    private void Init()
    {
        if (Text_Tips == null)
        {
            Text_Tips = GetComponent<Text>();
        }
    }

    public void ShowFlutterTips(string strValue, Color color, float duration = 1f, UnityAction unityAction = null)
    {
        m_StrValue = strValue;
        m_Duration = duration;
        m_UnityAction = unityAction;

        Init();

        Text_Tips.color = color;

        FlutterStart();
    }


    public void ShowFlutterTips(string strValue, float duration = 1f, UnityAction unityAction = null)
    {
        m_StrValue = strValue;
        m_Duration = duration;
        m_UnityAction = unityAction;

        Init();

        FlutterStart();
    }

    private void FlutterStart()
    {
        if (Tweener != null)
        {
            Tweener.Kill(true);
        }

        if (string.IsNullOrEmpty(m_StrValue))
        {
            m_StrValue = "飘字内容未设置！！！";
            Debug.LogError("飘字内容未设置！！！");
        }

        Text_Tips.text = m_StrValue;
        Text_Tips.DOFade(1, 0);

        gameObject.SetActive(true);
        Tweener = Text_Tips.DOFade(0, m_Duration).SetEase(m_TweenEase);
        Tweener.onComplete = FlutterComplete;
    }

    private void FlutterComplete()
    {
        Tweener = null;
        Text_Tips.color = m_Color;
        gameObject.SetActive(false);

        m_UnityAction?.Invoke();
    }
}

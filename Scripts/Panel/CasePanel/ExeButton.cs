using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExeButton : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [HideInInspector]
    public UnityAction<string, string> OnClickAction;
    [HideInInspector]
    public UnityAction<string> OnPointerEnterEvent;
    [HideInInspector]
    public UnityAction<string> OnPointerExitEvent;
    [HideInInspector]
    public UnityAction<string> OnClickClose;

    private ButtonEx ButtonSelf;
    private Image ImageIcon;
    private Text TextExeName;
    private ButtonEx ButtonClose;

    private string m_exePath;
    private string m_exeName;

    private Sprite none;

    public void Init()
    {
        ButtonSelf = GetComponent<ButtonEx>();
        ButtonSelf.onClick.AddListener(() => { OnClickAction?.Invoke(m_exePath, m_exeName); });
        ButtonSelf.OnPointerEnterEvent = () => { OnPointerEnterEvent?.Invoke(m_exeName); };
        ButtonSelf.OnPointerExitEvent = () => { OnPointerExitEvent?.Invoke(m_exeName); };

        ImageIcon = transform.ZYFindChild("Icon").GetComponent<Image>();

        TextExeName = transform.ZYFindChild("Text").GetComponent<Text>();

        ButtonClose = transform.ZYFindChild("ButtonClose").GetComponent<ButtonEx>();
        ButtonClose.onClick.AddListener(() => { OnClickClose?.Invoke(m_exePath); });
        none = ImageIcon.sprite;
    }


    public void SetExeName(string exePath)
    {
        if (string.IsNullOrEmpty(exePath))
        {
            return;
        }
        m_exeName = Path.GetFileNameWithoutExtension(exePath);
        m_exePath = exePath;
        TextExeName.text = m_exeName;
        SetIcon();
    }

    private void SetIcon()
    {
        //var str = exePath.Replace("exe", "png");
        //if (!File.Exists(str))
        //{
        var str = m_exePath.Replace("exe", "jpg");
        if (!File.Exists(str))
        {
            ImageIcon.sprite = none;
            return;
        }
        //}
        FileStream fileStream = new FileStream(str, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
        int width = 1920;
        int height = 1080;
        Texture2D tex = new Texture2D(width, height);
        tex.LoadImage(bytes);
        ImageIcon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //var a = eventData.position.y - eventData.pressPosition.y;
        //if (Mathf.Abs(a) >= aaa)
        //Debug.LogError("OnBeginDrag：" + eventData.position.y);

        //if (Mathf.Abs(eventData.delta.sqrMagnitude) >= aaa)
        //{
        //    transform.parent.parent.parent.GetComponent<MyCancelDragScrollRect>().OnBeginDrag(eventData);
        //}

        //Debug.LogError("OnBeginDrag：" + eventData.delta.sqrMagnitude);

        //if (eventData.button != PointerEventData.InputButton.Left)
        //    return;

        //if (!IsActive())
        //    return;

        //UpdateBounds();

        //m_PointerStartLocalCursor = Vector2.zero;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);
        //m_ContentStartPosition = m_Content.anchoredPosition;
        //m_Dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.LogError("OnDrag");
        //transform.parent.parent.parent.GetComponent<MyCancelDragScrollRect>().OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.LogError("OnDrag");
        //transform.parent.parent.parent.GetComponent<MyCancelDragScrollRect>().OnDrag(eventData);
    }
}

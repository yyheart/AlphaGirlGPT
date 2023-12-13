using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
public class UIDoTweenType : SingletonAuto<UIDoTweenType>
{
    public void GameObjectDoScaleShow(GameObject go, float speedTime = 0.25f,UnityAction action=null)
    {
        go.SetActive(true);
        Vector3 max = new Vector3(1, 1, 1);
        go.transform.DOScale(max, speedTime).OnComplete(() =>
        {
            if (action != null)
            {
                action.Invoke();
            }

        });

    }
    public void GameObjectDoScaleHide(GameObject go, float speedTime = 0.25f, UnityAction action = null)
    {
        Vector3 max = new Vector3(0, 0, 0);
        go.transform.DOScale(max, speedTime).OnComplete(() =>
        {
            if (action != null)
            {
                action.Invoke();
            }
            go.SetActive(false);
        });
    }
}

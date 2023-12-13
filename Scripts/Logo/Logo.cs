using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
public class Logo : MonoBehaviour
{
    public Image img_startPage;
    // Use this for initialization
    void Start()
    {
        Validate.OnValidateSuccess += () =>
        {
            print("验证成功");
            StartCoroutine(LoadNextScene());
        };
    }

    IEnumerator LoadNextScene(string sceneName = null)
    {
        Tween tween_0 = img_startPage.DOFade(1f, 1.2f);

        yield return tween_0.WaitForCompletion();

        yield return new WaitForSeconds(1.8f);

        Tween tween = img_startPage.DOFade(0f, 1f);
        //SceneManager.LoadSceneAsync(sceneName);
        yield return tween.WaitForCompletion();
        if (string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadSceneAsync(1);
        }
        else
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadResourcesManager : SingletonAuto<LoadResourcesManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public const string SpritePath = "{0}/ImageFolder/{1}.{2}";
    public const string AudioPath = "{0}/AudioFolder/{1}.{2}";

    #region 图片加载
    public Dictionary<string, Sprite> SpriteDic = new Dictionary<string, Sprite>();

    //自动补齐流文件夹路径：StreamingAssetsPath/ImageFolder/{0}
    private string GetSpriteFolderPath(string path, string suffix = "png")
    {
        return string.Format(SpritePath, Application.streamingAssetsPath, path, suffix);
    }



    /// <summary>
    /// 慎用！！！ 尝试获取 图片资源（自动补齐流文件夹路径：StreamingAssetsPath/ImageFolder/{0}）
    /// 【未加载过返回Null 不执行加载逻辑】
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isShowTips"></param>
    /// <returns></returns>
    public Sprite TryGetSprite(string path, string suffix = "png", bool isShowTips = true)
    {
        return TryGetFullPathSprite(GetSpriteFolderPath(path, suffix), isShowTips);
    }

    /// <summary>
    /// 慎用！！！ 尝试获取 图片资源（全路径）
    /// 【未加载过返回Null 不执行加载逻辑】
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isShowTips"></param>
    /// <returns></returns>
    public Sprite TryGetFullPathSprite(string path, bool isShowTips = true)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogErrorFormat("LoadResourcesManager.TryGetFullPathSprite()，强制获取 图片资源 失败，参数异常！！！\nPath：{0}", path);
            return null;
        }

        if (SpriteDic.TryGetValue(path, out Sprite sprite))
        {
            Debug.LogFormat("LoadResourcesManager.TryGetFullPathSprite()，强制获取 图片资源 成功\nPath：{0}", path);
            return sprite;
        }
        else
        {
            if (isShowTips)
            {
                Debug.LogErrorFormat("LoadResourcesManager.TryGetFullPathSprite()，强制获取 图片资源 失败，未加载！！！\nPath：{0}", path);
            }
            return null;
        }
    }



    /// <summary>
    /// 获取 图片资源（自动补齐流文件夹路径：StreamingAssetsPath/ImageFolder/{0}）
    /// 【有就直接返回】/【无就执行加载并返回】
    /// </summary>
    /// <param name="path"></param>
    /// <param name="action"></param>
    public void GetSprite(string path, Image image, UnityAction<Image, Sprite> action, string suffix = "png")
    {
        if (string.IsNullOrEmpty(path) || image == null || action == null)
        {
            Debug.LogErrorFormat("LoadResourcesManager.GetSprite()，获取/加载 图片资源 失败，参数异常！！！\nPath：{0}\nimage：{1}\naction：{2}", path, image, action.Method);
            return;
        }

        GetSprite(path, (sprite) => { action(image, sprite); }, suffix);
    }

    /// <summary>
    /// 获取 图片资源（自动补齐流文件夹路径：StreamingAssetsPath/ImageFolder/{0}）
    /// 【有就直接返回】/【无就执行加载并返回】
    /// </summary>
    /// <param name="path"></param>
    /// <param name="action"></param>
    public void GetSprite(string path, UnityAction<Sprite> action, string suffix = "png")
    {
        GetFullPathSprite(GetSpriteFolderPath(path, suffix), action);
    }

    /// <summary>
    /// 获取 图片资源（全路径）
    /// 【有就直接返回】/【无就执行加载并返回】
    /// </summary>
    /// <param name="path"></param>
    /// <param name="action"></param>
    public void GetFullPathSprite(string path, UnityAction<Sprite> action)
    {
        if (string.IsNullOrEmpty(path) || action == null)
        {
            Debug.LogErrorFormat("LoadResourcesManager.GetFullPathSprite()，获取/加载 图片资源 失败，参数异常！！！\nPath：{0}\naction：{1}", path, action.Method);
            return;
        }

        var sprite = TryGetFullPathSprite(path, false);
        if (sprite == null)//未加载过
        {
            Debug.LogFormat("LoadResourcesManager.GetFullPathSprite()，获取/加载 图片资源 执行加载逻辑\nPath：{0}\naction：{1}", path, action.Method);
            StartCoroutine(ImageWebRequest(path, (t2d) =>
            {
                if (t2d != null)
                {
                    sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
                    if (!SpriteDic.ContainsKey(path))
                    {
                        SpriteDic.Add(path, sprite);
                    }
                }
                action?.Invoke(sprite);
            }));
        }
        else
        {
            Debug.LogFormat("LoadResourcesManager.GetFullPathSprite()，获取/加载 图片资源 直接返回图片\nPath：{0}\naction：{1}", path, action.Method);
            action?.Invoke(sprite);
        }
    }



    public IEnumerator ImageWebRequest(string path, UnityAction<Texture2D> action)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(path);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.error != null)
        {
            Debug.LogErrorFormat("LoadResourcesManager.TextureReader()，图片资源不存在：{0}\nError：{1}", path, unityWebRequest.error);
            if (action != null)
            {
                action(null);
            }
        }
        else
        {
            Texture2D texture = TextureToTexture2D(DownloadHandlerTexture.GetContent(unityWebRequest));
            if (action != null)
            {
                action(texture);
            }
        }
    }

    //提升图片清晰度，，因为unity外部加载图片会是默认格式（压缩处理），若是图片分辨率太大加载会有些模糊
    private Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }
    #endregion 图片加载



    #region 音频加载
    public Dictionary<string, AudioClip> AudioDic = new Dictionary<string, AudioClip>();

    //自动补齐流文件夹路径：StreamingAssetsPath/AudioFolder/{0}
    private string GetAudioFolderPath(string path, string suffix = "wav")
    {
        return string.Format(AudioPath, Application.streamingAssetsPath, path, suffix);
    }



    /// <summary>
    /// 慎用！！！ 尝试获取 音频资源（自动补齐流文件夹路径：StreamingAssetsPath/AudioFolder/{0}）
    /// 【未加载过返回Null 不执行加载逻辑】
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isShowTips"></param>
    /// <returns></returns>
    public AudioClip TryGetAudioClip(string path, string suffix = "wav", bool isShowTips = true)
    {
        return TryGetFullPathAudioClip(GetAudioFolderPath(path, suffix), isShowTips);
    }

    /// <summary>
    /// 慎用！！！ 尝试获取 音频资源（全路径）
    /// 【未加载过返回Null 不执行加载逻辑】
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isShowTips"></param>
    /// <returns></returns>
    public AudioClip TryGetFullPathAudioClip(string path, bool isShowTips = true)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogErrorFormat("LoadResourcesManager.TryGetFullPathAudioClip()，强制获取 音频资源 失败，参数异常！！！\nPath：{0}", path);
            return null;
        }

        if (AudioDic.TryGetValue(path, out AudioClip audioClip))
        {
            Debug.LogFormat("LoadResourcesManager.TryGetFullPathAudioClip()，强制获取 音频资源 成功\nPath：{0}", path);
            return audioClip;
        }
        else
        {
            if (isShowTips)
            {
                Debug.LogErrorFormat("LoadResourcesManager.TryGetFullPathAudioClip()，强制获取 音频资源 失败，未加载！！！\nPath：{0}", path);
            }
            return null;
        }
    }



    /// <summary>
    /// 获取 音频资源（自动补齐流文件夹路径：StreamingAssetsPath/AudioFolder/{0}）
    /// 【有就直接返回】/【无就执行加载并返回】
    /// </summary>
    /// <param name="path"></param>
    /// <param name="action"></param>
    public void GetAudioClip(string path, UnityAction<AudioClip> action, string suffix = "wav")
    {
        GetFullPathAudioClip(GetAudioFolderPath(path, suffix), action);
    }

    /// <summary>
    /// 获取 音频资源（全路径）
    /// 【有就直接返回】/【无就执行加载并返回】
    /// </summary>
    /// <param name="path"></param>
    /// <param name="action"></param>
    public void GetFullPathAudioClip(string path, UnityAction<AudioClip> action)
    {
        if (string.IsNullOrEmpty(path) || action == null)
        {
            Debug.LogErrorFormat("LoadResourcesManager.GetFullPathAudioClip()，获取/加载 音频资源 失败，参数异常！！！\nPath：{0}\naction：{1}", path, action.Method);
            return;
        }

        var audioClip = TryGetFullPathAudioClip(path, false);
        if (audioClip == null)//未加载过
        {
            Debug.LogFormat("LoadResourcesManager.GetFullPathAudioClip()，获取/加载 音频资源 执行加载逻辑\nPath：{0}\naction：{1}", path, action.Method);
            StartCoroutine(AudioWebRequest(path, AudioType.WAV, (clip) =>
            {
                if (clip != null && !AudioDic.ContainsKey(path))
                {
                    AudioDic.Add(path, clip);
                }
                action?.Invoke(clip);
            }));
        }
        else
        {
            Debug.LogFormat("LoadResourcesManager.GetFullPathAudioClip()，获取/加载 音频资源 直接返回音频\nPath：{0}\naction：{1}", path, action.Method);
            action?.Invoke(audioClip);
        }
    }



    /// <summary>
    /// 携程加载音频
    /// </summary>
    /// <param name="_url"></param>
    /// <param name="_audioType">AudioType 貌似不支持Mp3 格式,请使用Wav或者是Acc等等格式</param>
    /// <returns></returns>
    private IEnumerator AudioWebRequest(string path, AudioType _audioType, UnityAction<AudioClip> action)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(path, _audioType);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.error != null)
        {
            Debug.LogErrorFormat("LoadResourcesManager.AudioWebRequest()，音频资源不存在：{0}\nError：{1}", path, unityWebRequest.error);
            if (action != null)
            {
                action(null);
            }
        }
        else
        {
            if (action != null)
            {
                action(DownloadHandlerAudioClip.GetContent(unityWebRequest));
            }
        }
    }
    #endregion




    #region 视频加载（未梳理，可能有问题）

    public Dictionary<string, byte[]> videoDic = new Dictionary<string, byte[]>();


    //public void SetSprite(Image image, string path, string suffix = "png")
    public void GetVideo(string path, UnityAction<byte[]> defaultSprite = null, string suffix = "mp4")
    {
        if (videoDic.TryGetValue(path, out byte[] sprite))
        {
            defaultSprite.Invoke(sprite);
        }
        else
        {
            //var path = string.Format("{0}/ImageFolder/{1}.{2}", Application.streamingAssetsPath, path, suffix);
            StartCoroutine(TextureReader(path, (t2d) =>
            {
                if (t2d == null)
                {
                    defaultSprite.Invoke(null);
                }
                else
                {
                    defaultSprite.Invoke(t2d);
                    if (!videoDic.ContainsKey(path))
                    {
                        videoDic.Add(path, t2d);
                    }
                }
            }));
        }
    }

    public IEnumerator TextureReader(string path, UnityAction<byte[]> action)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(path);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.error != null)
        {
            Debug.LogErrorFormat("LoadResourcesManager.TextureReader()，视频资源不存在：{0}\nError：{1}", path, unityWebRequest.error);
            if (action != null)
            {
                action(null);
            }
        }
        else
        {
            //byte[] bts = unityWebRequest.downloadHandler.data;
            //if (action != null)
            //{
            //    action(DownloadHandlerTexture.GetContent(unityWebRequest));
            //}

            if (action != null)
            {
                //action(Decrypt(unityWebRequest.downloadHandler.text));
                action(AesDecrypt(unityWebRequest.downloadHandler.data));
                //action((unityWebRequest.downloadHandler.data));
            }
        }
    }



    const string KEY = "WgsergmzsjdfgOEWRGNJRNGmkzaksme";
    const string IV = "SOJGGpjrepgjaskgpojAJGoe;lrmg";
    /// <summary>
    /// 将字符串解码到固定长度的数组中
    /// </summary>
    static byte[] StringEncodeFixedLengthArray(string content, int length)
    {
        byte[] data = Encoding.ASCII.GetBytes(content);
        Array.Resize(ref data, length);
        return data;
    }

    /// <summary>
    /// Aes解密
    /// </summary>
    public static byte[] AesDecrypt(byte[] data)
    {
        byte[] result;
        using (Aes aes = Aes.Create())
        {
            aes.Key = StringEncodeFixedLengthArray(KEY, 32);//32不要乱改，改成其他值可能会出错
            aes.IV = StringEncodeFixedLengthArray(IV, 16);//16不要乱改，改成其他值可能会出错

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            try
            {
                result = decryptor.TransformFinalBlock(data, 0, data.Length);
                decryptor.Dispose();
            }
            catch (Exception ex)
            {
                result = null;
                Debug.LogError(ex);
            }

        }
        return result;
    }
    #endregion
}
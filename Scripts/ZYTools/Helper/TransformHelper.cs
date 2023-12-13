using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelper
{

    #region 获取子物体、脚本
    struct TransformChild
    {
        public Transform Parent;
        public string Name;
    }

    static Dictionary<TransformChild, Transform> Dic_TransChilds = new Dictionary<TransformChild, Transform>();

    /// <summary>
    /// 找子物体
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform ZYFindChild( this Transform trans,string name)
    {
        TransformChild transformChild = new TransformChild();
        transformChild.Parent = trans;
        transformChild.Name = name;

        if (Dic_TransChilds.ContainsKey(transformChild) && Dic_TransChilds[transformChild])
        {
            return Dic_TransChilds[transformChild];
        }

        var GameObj = _FingChild(trans, name);
        if (GameObj)
        {
            Dic_TransChilds.Add(transformChild, GameObj);
        }
        return GameObj;
    }


    private static Transform _FingChild(this Transform parent, string goName)
    {
        //找自身的孩子
        var child = parent.Find(goName);
        if (child != null) return child;
        //如果没有在查找后代中有没有这个对象
        for (int i = 0; i < parent.childCount; i++)
        {
            child = parent.GetChild(i);
            var go = _FingChild(child, goName);
            if (go != null)
            {
                return go;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="goName"></param>
    /// <returns></returns>
    public static T ZYFindChild<T>(this Transform parent, string goName) where T : UnityEngine.Object
    {
        TransformChild ClildItem = new TransformChild();
        ClildItem.Parent = parent;
        ClildItem.Name = goName;

        if (Dic_TransChilds.ContainsKey(ClildItem) && Dic_TransChilds[ClildItem])
            return Dic_TransChilds[ClildItem].GetComponent<T>();

        var go = ZYFindChild(parent, goName);
        if (go)
        {
            Dic_TransChilds.Add(ClildItem, go);
        }
        T t = go.GetComponent<T>();
        if (t == null)
            Debug.Log(parent + "___" + goName);
        return go.GetComponent<T>();
    }

    #endregion

    #region 旋转、位移

    /// <summary>
    /// 旋转、面向目标
    /// </summary>
    /// <param name="Vec_Dir">注视的方向</param>
    /// <param name="trans">要旋转的目标</param>
    /// <param name="rotateSpeed">旋转速度</param>
    public static void LookTarget(Vector3 Vec_Dir, Transform trans, float rotateSpeed)
    {
        if (Vec_Dir != Vector3.zero)
        {
            Quaternion quaternion = Quaternion.LookRotation(Vec_Dir);
            trans.rotation = Quaternion.Lerp(trans.rotation, quaternion, rotateSpeed);
        }
    }

    /// <summary>
    /// 设置物体的X值
    /// </summary>
    /// <param name="trans">修改的物体</param>
    /// <param name="x">修改的X值</param>
    /// <param name="IsWord">是否是直接坐标</param>
    public static void SetTransformPostionX(this Transform trans, float x, bool IsWord = false)
    {
        if (IsWord)
        {
            trans.position = new Vector3(x, trans.position.y, trans.position.z);
        }
        else
        {
            trans.localPosition = new Vector3(x, trans.localPosition.y, trans.localPosition.z);
        }
    }

    /// <summary>
    /// 设置物体的Z值
    /// </summary>
    /// <param name="trans">修改的物体</param>
    /// <param name="y">修改的Y值</param>
    /// <param name="IsWord">是否是世界坐标</param>
    public static void SetTransformPostionY(this Transform trans, float y, bool IsWord = false)
    {
        if (IsWord)
        {
            trans.position = new Vector3(trans.position.x, y, trans.position.z);
        }
        else
        {
            trans.localPosition = new Vector3(trans.localPosition.x, y, trans.localPosition.z);
        }
    }

    /// <summary>
    /// 设置物体的Z值
    /// </summary>
    /// <param name="trans">修改的物体</param>
    /// <param name="z">修改的Z值</param>
    /// <param name="IsWord">是否是世界坐标</param>
    public static void SetTransformPostionZ(this Transform trans, float z, bool IsWord = false)
    {
        if (IsWord)
        {
            trans.position = new Vector3(trans.position.x, trans.position.y, z);
        }
        else
        {
            trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, z);
        }
    }

    /// <summary>
    /// 设置物体欧拉角X值
    /// </summary>
    /// <param name="trans">修改的物体</param>
    /// <param name="X">修改的值</param>
    /// <param name="IsWord">是否是世界坐标</param>
    public static void SetTransformLocalEulerAnglesX(this Transform trans, float X, bool IsWord = false)
    {
        if (IsWord)
        {
            trans.eulerAngles = new Vector3(X, trans.eulerAngles.y, trans.eulerAngles.z);
        }
        else
        {
            trans.localEulerAngles = new Vector3(X, trans.localEulerAngles.y, trans.localEulerAngles.z);
        }
    }

    /// <summary>
    /// 修改物体欧拉角Y值
    /// </summary>
    /// <param name="trans">修改的物体</param>
    /// <param name="Y">修改的Y值</param>
    /// <param name="IsWord">是否是世界坐标</param>
    public static void SetTransformLocalEulerAnglesY(this Transform trans, float Y, bool IsWord = false)
    {
        if (IsWord)
        {
            trans.eulerAngles = new Vector3(trans.eulerAngles.x, Y, trans.eulerAngles.z);
        }
        else
        {
            trans.localEulerAngles = new Vector3(trans.eulerAngles.x, Y, trans.localEulerAngles.z);
        }
    }

    /// <summary>
    /// 修改物体欧拉角Z值
    /// </summary>
    /// <param name="trans">修改的物体</param>
    /// <param name="Z">修改的Z值</param>
    /// <param name="IsWord">是否是世界坐标</param>
    public static void SetTransformLocalEulerAnglesZ(this Transform trans, float Z, bool IsWord = false)
    {
        if (IsWord)
        {
            trans.eulerAngles = new Vector3(trans.eulerAngles.x, trans.eulerAngles.y, Z);
        }
        else
        {
            trans.localEulerAngles = new Vector3(trans.eulerAngles.x, trans.localEulerAngles.y, Z);
        }
    }
    #endregion

    /// <summary>    /// 世界坐标转UI坐标    /// </summary>
    /// <param name="value">包含世界坐标的位置和要创建的数字</param>
    public static Vector3 WorldPointToUIPoint(this Transform target, Canvas canvas, Camera camera)
    {
        Vector3 worldpoint = new Vector3();
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, RectTransformUtility.WorldToScreenPoint(camera, target.position), canvas.worldCamera, out worldpoint);
        return worldpoint;
    }

    /// <summary>    /// 获取鼠标3D坐标 Z轴基于调用者 最后配合循环检测使用如Updata   /// </summary>
    /// <param name="target">调用者</param>
    /// <returns></returns>
    public static Vector3 GetWorldMousePos(this Transform target)
    {
        Vector3 OneSelfPos = Camera.main.WorldToScreenPoint(target.position);
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, OneSelfPos.z);
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Button;
using static UnityEngine.UI.Selectable;
using static UnityEngine.UI.Toggle;

public struct ButtonExParam
{
    public Transition transition;
    public Graphic targetGraphic;
    public ColorBlock colors;
    public SpriteState spriteState;
    public AnimationTriggers animationTriggers;
    public Navigation navigation;
    public ButtonClickedEvent onClick;
}

public struct ToggleExParam
{
    public bool interactable;
    public Transition transition;
    public Graphic targetGraphic;
    public ColorBlock colors;
    public SpriteState spriteState;
    public AnimationTriggers animationTriggers;
    public Navigation navigation;
    public bool isOn;
    public ToggleTransition toggleTransition;
    public Graphic graphic;
    public ToggleGroup group;
    public ToggleEvent onValueChanged;
}

/// <summary>
/// Unity菜单栏扩展
/// </summary>
public class MyMenu : MonoBehaviour
{
    [MenuItem("Tools/ChangeButtonEx")]
    public static void ChangeButtonEx()
    {
        var trans = GameObject.Find("Canvas");

        var list = trans.GetComponentsInChildren<Button>(true);
        int count = 0;
        foreach (var btn in list)
        {
            //存
            var btnObj = btn.gameObject;

            ButtonExParam param = GetButtonEXParam(btn);

            GameObject.DestroyImmediate(btn);

            if (!SetButtonEx(btnObj, param))
            {
                continue;
            }
            count++;

            EditorUtility.SetDirty(btnObj);
            AssetDatabase.SaveAssets();
        }
        Debug.LogFormat("{0}，共计{1}个，替换{2}个", list?.Length == count ? "替换完成" : "替换失败", list?.Length, count);
    }

    private static ButtonExParam GetButtonEXParam(Button btn)
    {
        var param = new ButtonExParam();
        param.transition = btn.transition;
        param.targetGraphic = btn.targetGraphic;
        if (param.transition == Transition.ColorTint)
        {
            param.colors = btn.colors;
        }
        else if (param.transition == Transition.SpriteSwap)
        {
            param.spriteState = btn.spriteState;
        }
        else if (param.transition == Transition.Animation)
        {
            param.animationTriggers = btn.animationTriggers;
        }
        param.navigation = btn.navigation;
        param.onClick = btn.onClick;
        return param;
    }

    private static bool SetButtonEx(GameObject btnObj, ButtonExParam param)
    {
        var btnEx = btnObj.AddComponent<ButtonEx>();
        if (btnEx == null)
        {
            Debug.LogErrorFormat("添加失败，ObjName：{0}", btnObj.name);
            return false;
        }

        btnEx.transition = param.transition;
        btnEx.targetGraphic = param.targetGraphic;
        if (param.transition == Transition.ColorTint)
        {
            btnEx.colors = param.colors;
        }
        else if (param.transition == Transition.SpriteSwap)
        {
            btnEx.spriteState = param.spriteState;
        }
        else if (param.transition == Transition.Animation)
        {
            btnEx.animationTriggers = param.animationTriggers;
        }
        btnEx.navigation = param.navigation;
        btnEx.onClick = param.onClick;

        btnEx.useBlockBlank = false;
        btnEx.soundEffectType = ButtonExSoundEffectType.通用音效;
        btnEx.useHoverSoundClip = false;
        btnEx.useTransformEffect = false;
        return true;
    }




    [MenuItem("Tools/ChangeToggleEx")]
    public static void ChangeToggleEx()
    {
        var trans = GameObject.Find("Canvas");

        var list = trans.GetComponentsInChildren<Toggle>(true);
        int count = 0;
        foreach (var toggle in list)
        {
            var toggleObj = toggle.gameObject;

            var param = GetToggleExParam(toggle);

            GameObject.DestroyImmediate(toggle);

            if (!SetToggleEx(toggleObj, param))
            {
                continue;
            }
            count++;

            EditorUtility.SetDirty(toggleObj);
            AssetDatabase.SaveAssets();
        }
        Debug.LogFormat("{0}，共计{1}个，替换{2}个", list?.Length == count ? "替换完成" : "替换失败", list?.Length, count);
    }

    private static ToggleExParam GetToggleExParam(Toggle toggle)
    {
        var param = new ToggleExParam();
        param.interactable = toggle.interactable;
        param.transition = toggle.transition;
        param.targetGraphic = toggle.targetGraphic;
        if (param.transition == Transition.ColorTint)
        {
            param.colors = toggle.colors;
        }
        else if (param.transition == Transition.SpriteSwap)
        {
            param.spriteState = toggle.spriteState;
        }
        else if (param.transition == Transition.Animation)
        {
            param.animationTriggers = toggle.animationTriggers;
        }
        param.navigation = toggle.navigation;
        param.isOn = toggle.isOn;
        param.toggleTransition = toggle.toggleTransition;
        param.graphic = toggle.graphic;
        param.group = toggle.group;
        param.onValueChanged = toggle.onValueChanged;
        return param;
    }

    private static bool SetToggleEx(GameObject toggleObj, ToggleExParam param)
    {
        var toggleEx = toggleObj.AddComponent<ToggleEx>();
        if (toggleEx == null)
        {
            Debug.LogErrorFormat("添加失败，ObjName：{0}", toggleObj.name);
            return false;
        }

        toggleEx.interactable = param.interactable;
        toggleEx.transition = param.transition;
        toggleEx.targetGraphic = param.targetGraphic;
        if (param.transition == Transition.ColorTint)
        {
            toggleEx.colors = param.colors;
        }
        else if (param.transition == Transition.SpriteSwap)
        {
            toggleEx.spriteState = param.spriteState;
        }
        else if (param.transition == Transition.Animation)
        {
            toggleEx.animationTriggers = param.animationTriggers;
        }
        toggleEx.navigation = param.navigation;

        toggleEx.isOn = param.isOn;
        toggleEx.toggleTransition = param.toggleTransition;
        toggleEx.graphic = param.graphic;
        toggleEx.group = param.group;
        toggleEx.onValueChanged = param.onValueChanged;

        toggleEx.useBlockBlank = false;
        toggleEx.soundEffectType = ButtonExSoundEffectType.通用音效;
        toggleEx.useHoverSoundClip = false;
        toggleEx.useTransformEffect = false;
        return true;
    }


    [MenuItem("Tools/ChangeShader")]
    public static void ChangeShader()
    {
        var trans = GameObject.Find("model");
        var shader = Resources.Load<Shader>("BoxStandard");
        var list = trans.GetComponentsInChildren<MeshRenderer>(true);
        int count = 0;
        foreach (var mesh in list)
        {
            var materials = mesh.sharedMaterials;
            foreach (var item in materials)
            {
                item.shader = shader;
            }
            count++;

            EditorUtility.SetDirty(mesh.gameObject);
            AssetDatabase.SaveAssets();
        }
        Debug.LogFormat("{0}，共计{1}个，替换{2}个", list?.Length == count ? "替换完成" : "替换失败", list?.Length, count);
    }



    [MenuItem("Tools/ChangePinyin")]
    public static void ChangePinyin()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Unfiltered);
        foreach (var tf in transforms)
        {
            //tf.name = Common.Pinyin.GetPinyin(tf.name);
            //Debug.LogError(tf.name);
            //var childCount = tf.childCount;
            ////var childTrans = tf.GetComponentsInChildren<Transform>(true);
            //for (int i = 0; i < childCount; i++)
            //{
            //    //var childTrans = tf.GetChild(0);
            //    //childTrans.SetSiblingIndex(childCount - i - 1);
            //}
            EditorUtility.SetDirty(tf);
        }
    }

    [MenuItem("Assets/ChangePinyin")]
    public static void ChangePinyin2()
    {
        //string[] ssss = Selection.assetGUIDs;
        //foreach (var item in ssss)
        //{
        //    string 啊啊啊啊 = AssetDatabase.GUIDToAssetPath(item);
        //    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(啊啊啊啊);

        //    Debug.LogError(go.name);
        //}
        UnityEngine.Object[] gameObjects = Selection.objects;
        foreach (var item in gameObjects)
        {
            string path_g = AssetDatabase.GetAssetPath(item);//获得选中物的路径
            //AssetDatabase.RenameAsset(path_g, Common.Pinyin.GetPinyin(item.name));//改名API
            Debug.LogError(item.name);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
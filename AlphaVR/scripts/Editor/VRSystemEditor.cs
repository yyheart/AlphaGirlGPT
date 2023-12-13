//======= Copyright (c) GDI, All rights reserved. ===============
//
// Purpose: 修改SVRSystem脚本编辑器界面
//
//=============================================================================

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(VRAppTools))]
public class VRSystemEditor : UnityEditor.Editor
{
    private VRAppTools _root;


    void OnEnable()
    {
        _root = target as VRAppTools;

    }
    public override void OnInspectorGUI()
    {
       

        serializedObject.UpdateIfRequiredOrScript();

        EditorGUILayout.BeginHorizontal();
        string path;
        EditorGUI.BeginChangeCheck();
        path = EditorGUILayout.TextField("Path", _root.path);
        if (GUILayout.Button("+", EditorStyles.miniButton))
        {
            path = EditorUtility.OpenFilePanelWithFilters("选择配置文件", Application.dataPath, new[] { "配置文件", "Xml" });
            Debug.Log(path);
            if (!string.IsNullOrEmpty(path) && path.Length > Application.dataPath.Length)
            {
                path = path.Remove(0, Application.dataPath.Length);
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_root, "change path");
            _root.path = path;
            EditorUtility.SetDirty(_root);
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

}

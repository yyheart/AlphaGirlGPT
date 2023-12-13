using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildTool : MonoBehaviour
{
    [MenuItem("Build/win x64")]

    static void BuildWindows_x64()

    {
        BuildWindows(BuildTarget.StandaloneWindows64);

    }

    static void BuildWindows(BuildTarget _bt)

    {
        string path = EditorUtility.SaveFilePanel(_bt.ToString(), EditorPrefs.GetString("BuildPath"), PlayerSettings.productName, "exe");

        if (string.IsNullOrEmpty(path))

            return;

        BuildPlayerOptions _buildOptions = new BuildPlayerOptions();

        _buildOptions.locationPathName = path;

        _buildOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);

        _buildOptions.target = _bt;

        BuildPipeline.BuildPlayer(_buildOptions);



        //加密

       // EncryptAssemblyCSharp(path);

        //替换解密mono.dll

       // ReplaceMonoDll(path);
        CopyFile(path, "ASPOSE.exe");
        //CopyFile(path, "DeviceTools.dll");
        //CopyFile(path, "libzmq-mt-4_3_3.dll");
        //CopyFile(path, "Nolo_Device.dll");
        //CopyFile(path, "NoloServer.exe");

        int num = path.LastIndexOf("/");

        path = path.Substring(0, num);

        EditorPrefs.SetString("BuildPath", path);

        EditorUtility.OpenWithDefaultApp(path);

    }
    static void CopyFile(string path, string source = "GUID.exe", string sourcePath = "")
    {
        string dest = path.Replace(".exe", "_Data/Managed") + "/" + source;
        source = Application.dataPath + "/Validate/Plugins/" + sourcePath + source;
        if (File.Exists(source))//必须判断要复制的文件是否存在
        {
            // byte[] _readByte = File.ReadAllBytes(source);
            //  File.WriteAllBytes(dest, _readByte);
            File.Copy(source, dest, true);//三个参数分别是源文件路径，存储路径，若存储路径有相同文件是否替换
        }
    }
}

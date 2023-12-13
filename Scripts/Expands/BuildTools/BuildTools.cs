#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildTools
{
    [MenuItem("Build/打包-PC", false, 1)]
    public static void EditorBuild_PC()
    {
        BuildPack(Platform.PC);
    }
    //[MenuItem("Build/打包-SteamVR", false, 2)]
    //public static void EditorBuild_SteamVR()
    //{
    //    BuildPack(Platform.SteamVR);
    //}
    //[MenuItem("Build/打包-AlphaVR", false, 3)]
    //public static void EditorBuild_AlphaVR()
    //{
    //    BuildPack(Platform.AlphaVR);
    //}
    [MenuItem("Build/打包-WebGL", false, 4)]
    public static void EditorBuild_WebGL()
    {
        BuildPack(Platform.WebGL);
    }


    [MenuItem("Build/打包-PC(日志模式)", false, 21)]
    public static void EditorBuild_LogMode_PC()
    {
        BuildPack(Platform.PC, true);
    }
    //[MenuItem("Build/打包-SteamVR(日志模式)", false, 22)]
    //public static void EditorBuild_LogMode_SteamVR()
    //{
    //    BuildPack(Platform.SteamVR, true);
    //}
    //[MenuItem("Build/打包_AlphaVR(日志模式)", false, 23)]
    //public static void EditorBuild_LogMode_AlphaVR()
    //{
    //    BuildPack(Platform.AlphaVR, true);
    //}
    [MenuItem("Build/打包-WebGL(日志模式)", false, 24)]
    public static void EditorBuild_LogMode_WebGL()
    {
        BuildPack(Platform.WebGL, true);
    }


    [MenuItem("Build/切换-PC版本(不打包)", false, 41)]
    public static void SwitchPlatform_Pc()
    {
        SwitchPlatform(Platform.PC);
    }
    //[MenuItem("Build/切换-SteamVR版本(不打包)", false, 42)]
    //public static void SwitchPlatform_SteamVR()
    //{
    //    SwitchPlatform(Platform.SteamVR);
    //}
    //[MenuItem("Build/切换-AlphaVR版本(不打包)", false, 43)]
    //public static void SwitchPlatform_AlphaVR()
    //{
    //    SwitchPlatform(Platform.AlphaVR);
    //}
    [MenuItem("Build/切换-WebGL版本(不打包)", false, 44)]
    public static void SwitchPlatform_WebGL()
    {
        SwitchPlatform(Platform.WebGL);
    }



    //切换平台
    public static void SwitchPlatform(Platform value)
    {
        //var buildtargetGroup = BuildTargetGroup.Standalone;

        //UnityEngine.XR.Management.XRGeneralSettings androidXRSettings = UnityEditor.XR.Management.XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildtargetGroup);
        //if (androidXRSettings == null)
        //{
        //    var assignedSettings = ScriptableObject.CreateInstance<UnityEngine.XR.Management.XRManagerSettings>() as UnityEngine.XR.Management.XRManagerSettings;
        //    androidXRSettings.AssignedSettings = assignedSettings;
        //    EditorUtility.SetDirty(androidXRSettings); // Make sure this gets picked up for serialization later.
        //}

        ////取消当前选择的
        //System.Collections.Generic.IReadOnlyList<UnityEngine.XR.Management.XRLoader> list = androidXRSettings.Manager.activeLoaders;
        //int hasCount = list.Count;
        //for (int i = 0; i < hasCount; i++)
        //{
        //    string nameTemp = list[0].GetType().FullName;
        //    Debug.Log("disable xr plug:" + nameTemp);
        //    UnityEditor.XR.Management.Metadata.XRPackageMetadataStore.RemoveLoader(androidXRSettings.Manager, nameTemp, buildtargetGroup);
        //}
        //if (value == Platform.SteamVR)
        //{
        //    //启用
        //    //string loaderTypeName = "Google.XR.Cardboard.XRLoader";
        //    string loaderTypeName = "Unity.XR.OpenVR.OpenVRLoader";
        //    UnityEditor.XR.Management.Metadata.XRPackageMetadataStore.AssignLoader(androidXRSettings.Manager, loaderTypeName, buildtargetGroup);
        //}

        //var platformStr = value.ToString();
        //File.WriteAllText(GameController.PlatformPath, platformStr);

        var buildTargetGroup = GetBuildTargetGroup(value);
        var buildTarget = GetBuildTarget(value);
        if (EditorUserBuildSettings.activeBuildTarget == buildTarget)
        {
            Debug.Log("平台相同，跳过切换");
            return;
        }
        Debug.LogFormat("执行切换逻辑：当前 buildTarget：{0}", EditorUserBuildSettings.activeBuildTarget);
        Debug.LogFormat("执行切换逻辑：目标 buildTarget：{0}", buildTarget);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
    }
    private static BuildTargetGroup GetBuildTargetGroup(Platform platform)
    {
        switch (platform)
        {
            case Platform.PC:
            case Platform.SteamVR:
            case Platform.AlphaVR:
                return BuildTargetGroup.Standalone;
            case Platform.WebGL:
                return BuildTargetGroup.WebGL;
            default:
                return BuildTargetGroup.Standalone;
        }
    }
    private static BuildTarget GetBuildTarget(Platform platform)
    {
        switch (platform)
        {
            case Platform.PC:
            case Platform.SteamVR:
            case Platform.AlphaVR:
                return BuildTarget.StandaloneWindows64;
            case Platform.WebGL:
                return BuildTarget.WebGL;
            default:
                return BuildTarget.StandaloneWindows64;
        }
    }




    //打包
    public static void BuildPack(Platform value, bool isDevelopment = false)
    {
        //var platformStr = value.ToString();
        //File.WriteAllText(GameController.PlatformPath, platformStr);
        SwitchPlatform(value);

        var buildPath = GetBuildPath(value, isDevelopment);
        //Debug.LogErrorFormat("BuildPath:" + buildPath);
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, buildPath, GetBuildTarget(value), isDevelopment ? BuildOptions.Development : BuildOptions.None);
    }
    private static string GetBuildPath(Platform platform, bool isDevelopment = false)
    {
        //"D:/Pack/{0}Pack/{1}/{0}_{1}_{2}/{0}.exe"

        //D:/Pack/{项目名}Pack/{平台}/{项目名}_{平台}_{时间}/{项目名}.exe

        //D:/Pack/Tool_DemoPack/PC/Tool_Demo_PC_09.01.160208/Tool_Demo.exe

        var projectName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(Application.dataPath));

        var time = System.DateTime.Now.ToString("MM.dd.HHmmss");
        switch (platform)
        {
            case Platform.PC:
            case Platform.SteamVR:
            case Platform.AlphaVR:
            default:
                var exeName = string.IsNullOrEmpty(PlayerSettings.productName) ? projectName : PlayerSettings.productName;
                //return string.Format("D:/Pack/{0}Pack/{1}/{0}_{1}_{2}/{0}_{1}/{3}.exe", projectName, value, time, exeName);
                var exeTime = System.DateTime.Now.ToString("MMddHHmm");
                var developmentStr = isDevelopment ? "_Development" : string.Empty;
                return string.Format("D:/Pack/{0}Pack/{1}/{0}_{1}_{2}{3}/{4}{5}/{4}.exe", projectName, platform, time, developmentStr, exeName, exeTime);
            case Platform.WebGL:
                return string.Format("D:/Pack/{0}Pack/{1}/{0}_{1}_{2}/{0}_{1}", projectName, platform, time);
        }
    }



    /// <summary>
    /// Build完成后的回调
    /// </summary>
    /// <param name="target">打包的目标平台</param>
    /// <param name="pathToBuiltProject">包体的完整路径</param>
    [UnityEditor.Callbacks.PostProcessBuild(1)]
    public static void AfterBuild(BuildTarget target, string pathToBuiltProject)
    {
        AddBuildLog(target, pathToBuiltProject);

        CopyFile(pathToBuiltProject, "ASPOSE.exe");
        Debug.Log("Build Success  输出平台: " + target + "  输出路径: " + pathToBuiltProject);

        int index = pathToBuiltProject.LastIndexOf("/");
        var folderPath = pathToBuiltProject.Substring(0, index);
        Debug.Log("导出包体的目录 :" + folderPath);

        ClearFile(pathToBuiltProject);

        //打开文件或文件夹
        EditorUtility.OpenWithDefaultApp(folderPath);
    }

    public static void AddBuildLog(BuildTarget target, string pathToBuiltProject)
    {
        switch (target)
        {
            case BuildTarget.StandaloneWindows64:
                File.WriteAllText(Path.Combine(pathToBuiltProject.Replace(".exe", "_Data/StreamingAssets/Build_Log.txt")), Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(pathToBuiltProject))));
                break;
            case BuildTarget.WebGL:
                File.WriteAllText(Path.Combine(pathToBuiltProject, "StreamingAssets/Build_Log.txt"), Path.GetFileName(Path.GetDirectoryName(pathToBuiltProject)));
                break;
            default:
                break;
        }
    }

    public static void CopyFile(string path, string source = "GUID.exe", string sourcePath = "")
    {
        string dest = path.Replace(".exe", "_Data/Managed") + "/" + source;
        source = Application.dataPath + "/Validate/Plugins/" + sourcePath + source;
        if (File.Exists(source))//必须判断要复制的文件是否存在
        {
            File.Copy(source, dest, true);//三个参数分别是源文件路径，存储路径，若存储路径有相同文件是否替换
        }
    }

    public static void ClearFile(string path)
    {
        var replace1 = "UnityCrashHandler32.exe";
        var replace2 = "UnityCrashHandler64.exe";

        var replaceStr = Path.GetFileName(path);

        //删除包体文件中的 Uniry exe
        replace1 = path.Replace(replaceStr, replace1);
        if (File.Exists(replace1))
        {
            File.Delete(replace1);
            Debug.LogFormat("删除Unity exe：{0}", replace1);
        }

        replace2 = path.Replace(replaceStr, replace2);
        if (File.Exists(replace2))
        {
            File.Delete(replace2);
            Debug.LogFormat("删除Unity exe：{0}", replace2);
        }
    }

    //[UnityEditor.Callbacks.PostProcessBuild(1)]
    //public static void AfterBuild(BuildTarget target, string pathToBuiltProject)
    //{
    //    Debug.Log("Build Success  输出平台: " + target + "  输出路径: " + pathToBuiltProject);
    //    string buildPath = pathToBuiltProject.Substring(0, pathToBuiltProject.LastIndexOf("/"));

    //    Debug.Log(buildPath);

    //    string configName = "config.ini";
    //    string allConfigDirName = "SceneModeInFo";

    //    DirectoryInfo appDir = new DirectoryInfo(Application.dataPath);
    //    File.Copy(Path.Combine(appDir.Parent.FullName, configName), Path.Combine(buildPath, configName), true);


    //    string copyDir = Path.Combine(appDir.Parent.FullName, allConfigDirName);
    //    string tgtDir = Path.Combine(buildPath, allConfigDirName);

    //    CopyDirectory(copyDir, tgtDir);

    //    Debug.Log("打包完成！");

    //}


    ///// <summary>
    ///// 复制文件夹
    ///// </summary>
    ///// <param name="srcDir"></param>
    ///// <param name="tgtDir"></param>
    //public static void CopyDirectory(string srcDir, string tgtDir)
    //{
    //    DirectoryInfo source = new DirectoryInfo(srcDir);
    //    DirectoryInfo target = new DirectoryInfo(tgtDir);

    //    if (target.FullName.StartsWith(source.FullName, System.StringComparison.CurrentCultureIgnoreCase))
    //    {
    //        throw new System.Exception("父目录不能拷贝到子目录！");
    //    }
    //    if (!source.Exists)
    //    {
    //        return;
    //    }
    //    if (!target.Exists)
    //    {
    //        target.Create();
    //    }
    //    FileInfo[] files = source.GetFiles();
    //    for (int i = 0; i < files.Length; i++)
    //    {
    //        File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
    //    }
    //    DirectoryInfo[] dirs = source.GetDirectories();
    //    for (int j = 0; j < dirs.Length; j++)
    //    {
    //        CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
    //    }
    //}

    //public class SwitchPlatformExample
    //{
    //    [MenuItem("Example/Switch Platform")]
    //    public static void PerformSwitch()
    //    {
    //        EditorUserBuildSettings.activeBuildTargetChanged = delegate () {
    //            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
    //            {
    //                Debug.Log("DoSomeThings");
    //            }
    //        };
    //        // Switch to Windows standalone build.
    //        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
    //    }
    //}
}
#endif

public enum Platform
{
    PC,
    SteamVR,
    AlphaVR,
    WebGL,
}
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEngine;

public enum eExeTab
{
    生命科学,
    动物世界,
    探索宇宙,
    党建中心,
    机械,
    军事,
    医学,
    建筑漫游,
    能源分类,
}

/// <summary>
/// 案例
/// </summary>
[System.Serializable]
public struct ExeStep
{
    public string exeName;
    public string exeDescribe;

    public ExeStep(string exeName, string exeDescribe)
    {
        this.exeName = exeName;
        this.exeDescribe = exeDescribe;
    }
}


/// <summary>
/// 案例组
/// </summary>
[System.Serializable]
public class ExeGroup
{
    public eExeTab type;
    public List<ExeStep> exeStepList;


    private int exeNameCount;
    private Dictionary<string, ExeStep> exeStepDic;

    public ExeGroup(eExeTab type)
    {
        this.type = type;
        this.exeStepList = new List<ExeStep>();
    }

    public void InitData()
    {
        exeStepDic = new Dictionary<string, ExeStep>();
        exeNameCount = exeStepList.Count;

        for (var i = 0; i < exeNameCount; ++i)
        {
            var exeStep = exeStepList[i];
            exeStepDic.Add(exeStep.exeName, exeStep);
        }
    }

    public bool CheckHaveCount()
    {
        return exeNameCount > 0;
    }

    public bool CheckHaveStep(string exeName)
    {
        return exeStepDic.ContainsKey(exeName);
    }

    public ExeStep GetStep(string exeName)
    {
        if (exeStepDic.TryGetValue(exeName, out ExeStep exeStep))
        {
            return exeStep;
        }
        return new ExeStep();
    }
}


[CreateAssetMenu(fileName = "ExeWarehouseConfig", menuName = "CreatExeWarehouseConfig", order = 0)]
public class ExeWarehouse : ScriptableObject/*, ISerializationCallbackReceiver*/
{
    [SerializeField]
    public List<ExeGroup> exeGroupList;

    private int exeGroupCount;
    private Dictionary<eExeTab, ExeGroup> exeGroupDic;

    public ExeWarehouse()
    {
        exeGroupList = new List<ExeGroup>();
    }


    public void InitData()
    {
        exeGroupCount = exeGroupList.Count;
        exeGroupDic = new Dictionary<eExeTab, ExeGroup>();
        for (var i = 0; i < exeGroupCount; ++i)
        {
            var exeGroup = exeGroupList[i];
            exeGroup.InitData();
            if (!exeGroupDic.ContainsKey(exeGroup.type))
            {
                exeGroupDic.Add(exeGroup.type, exeGroup);
            }
            else
            {
                Debug.LogErrorFormat("ExeWarehouseConfig 出现重复键：{0}", exeGroup.type);
            }
        }
        AddLogExePath();
    }


    public bool CheckHaveGroup()
    {
        return exeGroupCount > 0;
    }

    public ExeGroup GetGroup(eExeTab type)
    {
        if (exeGroupDic.TryGetValue(type, out ExeGroup exeGroup))
        {
            return exeGroup;
        }
        Debug.LogErrorFormat("ExeWarehouse.GetGroup()，找不到键 eExeTab：{0}", type);
        return null;
    }

    //private LogExeButton GetLogExeButton(int index)
    //{
    //    if (exeGroupList.Count > index)
    //    {
    //        return exeGroupList[index];
    //    }
    //    else
    //    {
    //        var exeButton = Instantiate(LogExeButtonPrefab, LogExeButtonParent);
    //        exeButton.Init();
    //        exeButton.OnClickAction = ClickExeBtn;

    //        exeGroupList.Add(exeButton);
    //        return exeButton;
    //    }
    //}
    //添加案例Log 末尾的 最新
    private void AddLogExePath()
    {
        //List<string> list = new List<string>();
        //Dictionary<string, string> dic = new Dictionary<string, string>();

        //for (int index = 0; index < 10; index++)
        //{
        //    list.Add("listvalue_" + index);
        //    dic.Add("key_" + index, "value_" + index);
        //}
        //string list_json = JsonUtility.ToJson(list);
        //Debug.Log("list to json: " + list_json);
        //list_json = SerializeList.ListToJson<string>(list);
        //Debug.Log("list to json: " + list_json);

        //string dic_json = JsonUtility.ToJson(dic);
        //Debug.Log("dic to json: " + dic_json);
        //dic_json = SerializeDictionary.DicToJson<string, string>(dic);
        //Debug.Log("dic to json: " + dic_json);

        //string dic_json = SerializeList.ListToJson<ExeGroup>(exeGroupList);
        //Debug.LogError(dic_json);

        //System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        //var jsonStr = JsonMapper.toJSONString().ToJson(exeGroupList);
        //Debug.LogError(jsonStr);
        //foreach (var item in exeGroupDic)
        //{
        //    var keyStr = item.Key.ToString();
        //    var valueStr = string.Empty;
        //    string aaa = "[程序名称：{0};描述：{1};]";
        //    foreach (var exeStep in item.Value.exeStepList)
        //    {
        //        var nameStr = exeStep.exeName.ToString();
        //        var describeStr = exeStep.exeDescribe.ToString();
        //        string.Format(aaa, nameStr, describeStr);
        //    }


        //}
        ////|分类名称：类型;[程序名称：exe；描述：desc];[程序名称：exe；描述：desc];|分类名称：[程序名称：exe；描述：desc];[程序名称：exe；描述：desc];
        //if (string.IsNullOrEmpty(logExePath))
        //{
        //    return;
        //}

        //LogExePathList.Remove(logExePath);
        //LogExePathList.Add(logExePath);
        //WriteLogExeUrl();
        //InitLogExePanel();
    }

    private void WriteLogExeUrl()
    {
        //System.Text.StringBuilder logExePaths = new System.Text.StringBuilder();
        //for (int i = 0; i < LogExePathList.Count; i++)
        //{
        //    logExePaths.AppendLine(LogExePathList[i]);
        //}

        //File.WriteAllText(LogExePathFileUrl, logExePaths.ToString());
    }
}

///*********************序列化/反序列化Dictionary***********************/

//public class SerializeDictionary
//{
//    public static string DicToJson<TKey, TValue>(Dictionary<TKey, TValue> dic)
//    {
//        return JsonUtility.ToJson(new SerializeDictionary<TKey, TValue>(dic));
//    }

//    public static Dictionary<TKey, TValue> DicFromJson<TKey, TValue>(string str)
//    {
//        return JsonUtility.FromJson<SerializeDictionary<TKey, TValue>>(str).ToDictionary();
//    }
//}
///******************************************************************/
//[Serializable]
//public class SerializeDictionary<TKey, TValue> : ISerializationCallbackReceiver
//{
//    [SerializeField]
//    List<TKey> keys;
//    [SerializeField]
//    List<TValue> values;

//    Dictionary<TKey, TValue> target;
//    public Dictionary<TKey, TValue> ToDictionary() { return target; }

//    public SerializeDictionary(Dictionary<TKey, TValue> target)
//    {
//        this.target = target;
//    }

//    public void OnBeforeSerialize()
//    {
//        keys = new List<TKey>(target.Keys);
//        values = new List<TValue>(target.Values);
//    }

//    public void OnAfterDeserialize()
//    {
//        var count = Math.Min(keys.Count, values.Count);
//        target = new Dictionary<TKey, TValue>(count);
//        for (var i = 0; i < count; ++i)
//        {
//            target.Add(keys[i], values[i]);
//        }
//    }
//}

///****************************序列化/反序列化List****************************/

//public class SerializeList
//{
//    public static string ListToJson<T>(List<T> l)
//    {
//        return JsonUtility.ToJson(new SerializationList<T>(l));
//    }

//    public static List<T> ListFromJson<T>(string str)
//    {
//        return JsonUtility.FromJson<SerializationList<T>>(str).ToList();
//    }
//}
///*********************************************************************/

//// List<T>
//[Serializable]
//public class SerializationList<T>
//{
//    [SerializeField]
//    List<T> target;
//    public List<T> ToList() { return target; }

//    public SerializationList(List<T> target)
//    {
//        this.target = target;
//    }
//}

////使用封装后的工具类序列化List和Dictionary


//public class Serialization : MonoBehaviour
//{
//    private void Start()
//    {
//        List<string> list = new List<string>();
//        Dictionary<string, string> dic = new Dictionary<string, string>();

//        for (int index = 0; index < 10; index++)
//        {
//            list.Add("listvalue_" + index);
//            dic.Add("key_" + index, "value_" + index);
//        }
//        string list_json = JsonUtility.ToJson(list);
//        Debug.Log("list to json: " + list_json);
//        list_json = SerializeList.ListToJson<string>(list);
//        Debug.Log("list to json: " + list_json);

//        string dic_json = JsonUtility.ToJson(dic);
//        Debug.Log("dic to json: " + dic_json);
//        dic_json = SerializeDictionary.DicToJson<string, string>(dic);
//        Debug.Log("dic to json: " + dic_json);
//    }
//}

////public class Serialization : MonoBehaviour
////{
////    private void Start()
////    {
////        List<Book> list_books = new List<Book>();
////        Dictionary<string, Book> dic_books = new Dictionary<string, Book>();
////        for (int index = 0; index < 10; index++)
////        {
////            Book book = new Book("100" + index, "book_" + index);
////            list_books.Add(book);
////            dic_books.Add("100" + index, book);
////        }
////        string list_json = SerializeList.ListToJson<Book>(list_books);
////        Debug.Log("list to json: " + list_json);

////        string dic_json = SerializeDictionary.DicToJson<string, Book>(dic_books);
////        Debug.Log("dic to json: " + dic_json);
////    }
////}

//[Serializable]
//public class Book
//{
//    public string book_id;
//    public string book_name;
//    public Book(string b_id, string b_name)
//    {
//        book_id = b_id;
//        book_name = b_name;
//    }
//}


////所有表数据的基类  存储的属性
//public class TableDatabase
//{
//    public int ID;
//}

////角色表内数据结构
//public class RoleDatabase : TableDatabase
//{
//    public string Type;//名称
//    public string Name;//名称
//    public ExeStep ModelPath;//模型路径
//}
////角色表
//public class RoleTable : ConfigTable<RoleDatabase, RoleTable>
//{
//    void Awake()
//    {
//        //加载对应表所对应的路径
//        load("Config/RoleTable.csv");
//    }
//}



////表格基类<存储的属性,具体表类>
//public class ConfigTable<TDatabase, T> : SingleCase<T>
//    where TDatabase : TableDatabase, new()
//    where T : SingleCase<T>
//{
//    //id，数据条目
//    public Dictionary<int, TDatabase> _cache = new Dictionary<int, TDatabase>();

//    protected void load(string tablePath)
//    {
//        MemoryStream tableStream;

//#if UNITY_EDITOR
//        //开发期，读Progect/Config下的csv文件
//        var srcPath = Application.dataPath + "/../" + tablePath;
//        tableStream = new MemoryStream(File.ReadAllBytes(srcPath));
//#else
//        //发布后  从Resources/Config读表，并需要将.csv文件后面加上.bytes后缀名
//        //读表
//        var table = Resources.Load<TextAsset>(tablePath);
//        //内存流 表里的二进制作为数据源
//        tableStream = new MemoryStream(table.bytes);
//#endif

//        //内存流读取器 using 自动关闭流
//        using (var reader = new StreamReader(tableStream, Encoding.GetEncoding("gb2312")))
//        {
//            //存储第一行的属性
//            var fieldNameStr = reader.ReadLine();
//            //，号分割各个属性
//            var fieldNameArray = fieldNameStr.Split(',');
//            //每个属性对应 所属于的类型  列表
//            List<FieldInfo> allFieldInfo = new List<FieldInfo>();
//            //遍历对应的TDatabase对应的表属性 所属的类型 存储到列表中
//            foreach (var fieldName in fieldNameArray)
//            {
//                allFieldInfo.Add(typeof(TDatabase).GetField(fieldName));
//            }

//            //下面是正式数据
//            var lineStr = reader.ReadLine();
//            while (lineStr != null)
//            {   //读取每一条数据 存储到缓存中
//                TDatabase DataDB = readLine(allFieldInfo, lineStr);
//                _cache[DataDB.ID] = DataDB;
//                lineStr = reader.ReadLine();
//            }
//        }
//    }

//    /// <summary>
//    /// 读取每行的数据 
//    /// </summary>
//    /// <param name="allFieldInfo">每条属性对应的类型列表</param>
//    /// <param name="lineStr">一条数据</param>
//    private static TDatabase readLine(List<FieldInfo> allFieldInfo, string lineStr)
//    {
//        //读到内存 （，分割当前行数据）
//        var itemStrArray = lineStr.Split(',');
//        var DataDB = new TDatabase();
//        //对每个字段解析
//        for (int i = 0; i < allFieldInfo.Count; ++i)
//        {
//            var field = allFieldInfo[i];//当前属性的类型
//            var data = itemStrArray[i];//当前属性对应的具体数据
//            //整数
//            if (field.FieldType == typeof(int))
//            {
//                field.SetValue(DataDB, int.Parse(data));
//            }
//            //字符串
//            else if (field.FieldType == typeof(string))
//            {
//                field.SetValue(DataDB, data);
//            }
//            //浮点型
//            else if (field.FieldType == typeof(float))
//            {
//                field.SetValue(DataDB, float.Parse(data));
//            }
//            //布尔型
//            else if (field.FieldType == typeof(bool))
//            {
//                //var v = int.Parse(data);
//                //field.SetValue(DataDB, v != 0);
//                field.SetValue(DataDB, bool.Parse(data));
//            }
//            //整数数组
//            else if (field.FieldType == typeof(List<int>))
//            {
//                var list = new List<int>();
//                //1$2$3$4$ 以$分割数组中的数据
//                foreach (var itemStr in data.Split('$'))
//                {
//                    list.Add(int.Parse(itemStr));
//                }
//                field.SetValue(DataDB, list);
//            }
//            //浮点数数组
//            else if (field.FieldType == typeof(List<float>))
//            {
//                var list = new List<float>();
//                foreach (var itemStr in data.Split('$'))
//                {
//                    list.Add(float.Parse(itemStr));
//                }
//                field.SetValue(DataDB, list);
//            }
//            //字符串数组
//            else if (field.FieldType == typeof(List<string>))
//            {
//                field.SetValue(DataDB, new List<string>(data.Split('$')));
//            }//同一dll的Type类型
//            else if (field.FieldType == typeof(Type))
//            {
//                //Type type = Type.GetType();
//                //field.SetValue(DataDB, type);
//            }
//        }

//        return DataDB;
//    }

//    //获取表格数据
//    public TDatabase this[int index]
//    {
//        get
//        {
//            TDatabase db;
//            _cache.TryGetValue(index, out db);
//            return db;
//        }
//    }
//    //得到整张表
//    public Dictionary<int, TDatabase> GetAll()
//    {
//        return _cache;
//    }

//}

//public class SingleCase<T> where T : SingleCase<T>
//{
//}

//class ConfigData
//{
//    public string Type;
//    public string Name;
//    public string Data;
//}

//public class aaa
//{
//    private static string excelPath;
//    private static string _rootPath;
//    private static string _dataPath;

//    public void AAA()
//    {
//        string[] files = Directory.GetFiles(excelPath, "*.xlsx");
//        List<string> codeList = new List<string>();
//        Dictionary<string, List<ConfigData[]>> dataDict = new Dictionary<string, List<ConfigData[]>>();
//        for (int i = 0; i < files.Length; ++i)
//        {
//            //打开excel
//            string file = files[i];
//            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read);
//            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
//            if (!excelReader.IsValid)
//            {
//                Console.WriteLine("invalid excel " + file);
//                continue;
//            }


//            string[] types = null;
//            string[] names = null;
//            List<ConfigData[]> dataList = new List<ConfigData[]>();
//            int index = 1;

//            //开始读取
//            while (excelReader.Read())
//            {
//                //这里读取的是每一行的数据
//                string[] datas = new string[excelReader.FieldCount];
//                for (int j = 0; j < excelReader.FieldCount; ++j)
//                {
//                    datas[j] = excelReader.GetString(j);
//                }

//                //空行不处理
//                if (datas.Length == 0 || string.IsNullOrEmpty(datas[0]))
//                {
//                    ++index;
//                    continue;
//                }

//                //第三行表示类型
//                if (index == 4) types = datas;
//                //第四行表示变量名
//                else if (index == 5) names = datas;
//                //后面的表示数据
//                else if (index > 5)
//                {
//                    //把读取的数据和数据类型,名称保存起来,后面用来动态生成类
//                    List<ConfigData> configDataList = new List<ConfigData>();
//                    for (int j = 0; j < datas.Length; ++j)
//                    {
//                        ConfigData data = new ConfigData();
//                        data.Type = types[j];
//                        data.Name = names[j];
//                        data.Data = datas[j];
//                        if (string.IsNullOrEmpty(data.Type) || string.IsNullOrEmpty(data.Data))
//                            continue;
//                        configDataList.Add(data);
//                    }
//                    dataList.Add(configDataList.ToArray());
//                }
//                ++index;
//            }
//            //类名
//            string className = excelReader.Name;
//            //根据刚才的数据来生成C#脚本
//            ScriptGenerator generator = new ScriptGenerator(className, names, types);
//            //所有生成的类最终保存在这个链表中
//            codeList.Add(generator.Generate());
//            if (dataDict.ContainsKey(className)) Console.WriteLine("相同的表名 " + className);
//            else dataDict.Add(className, dataList);

//            //编译代码,序列化数据
//            Assembly assembly = CompileCode(codeList.ToArray(), null);
//            string path = _rootPath + _dataPath;
//            if (Directory.Exists(path)) Directory.Delete(path, true);
//            Directory.CreateDirectory(path);
//            foreach (KeyValuePair<string, List<ConfigData[]>> each in dataDict)
//            {
//                object container = assembly.CreateInstance(each.Key + "Container");
//                Type temp = assembly.GetType(each.Key);
//                Serialize(container, temp, each.Value, path);
//            }
//        }
//    }

//    //编译代码
//    private static Assembly CompileCode(string[] scripts, string[] dllNames)
//    {
//        string path = _rootPath + _dllPath;
//        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
//        //编译参数
//        CSharpCodeProvider codeProvider = new CSharpCodeProvider();
//        CompilerParameters objCompilerParameters = new CompilerParameters();
//        objCompilerParameters.ReferencedAssemblies.AddRange(new string[] { "System.dll" });
//        objCompilerParameters.OutputAssembly = path + "Config.dll";
//        objCompilerParameters.GenerateExecutable = false;
//        objCompilerParameters.GenerateInMemory = true;

//        //开始编译脚本
//        CompilerResults cr = codeProvider.CompileAssemblyFromSource(objCompilerParameters, scripts);
//        if (cr.Errors.HasErrors)
//        {
//            Console.WriteLine("编译错误：");
//            foreach (CompilerError err in cr.Errors)
//                Console.WriteLine(err.ErrorText);
//            return null;
//        }
//        return cr.CompiledAssembly;
//    }

//    //序列化对象
//    private static void Serialize(object container, Type temp, List<ConfigData[]> dataList, string path)
//    {
//        //设置数据
//        foreach (ConfigData[] datas in dataList)
//        {
//            object t = temp.Assembly.CreateInstance(temp.FullName);
//            foreach (ConfigData data in datas)
//            {
//                FieldInfo info = temp.GetField(data.Name);
//                info.SetValue(t, ParseValue(data.Type, data.Data));
//            }

//            object id = temp.GetField("id").GetValue(t);
//            FieldInfo dictInfo = container.GetType().GetField("Dict");
//            object dict = dictInfo.GetValue(container);

//            bool isExist = (bool)dict.GetType().GetMethod("ContainsKey").Invoke(dict, new System.Object[] { id });
//            if (isExist)
//            {
//                Console.WriteLine("repetitive key " + id + " in " + container.GetType().Name);
//                Console.Read();
//                return;
//            }
//            dict.GetType().GetMethod("Add").Invoke(dict, new System.Object[] { id, t });
//        }

//        IFormatter f = new BinaryFormatter();
//        Stream s = new FileStream(path + temp.Name + ".bytes", FileMode.OpenOrCreate,
//                  FileAccess.Write, FileShare.Write);
//        f.Serialize(s, container);
//        s.Close();
//    }

//    //创建数据管理器脚本
//    private static void CreateDataManager(Assembly assembly)
//    {
//        IEnumerable types = assembly.GetTypes().Where(t => { return t.Name.Contains("Container"); });

//        StringBuilder source = new StringBuilder();
//        source.Append("/*Auto create\n");
//        source.Append("Don't Edit it*/\n");
//        source.Append("\n");

//        source.Append("using System;\n");
//        source.Append("using UnityEngine;\n");
//        source.Append("using System.Runtime.Serialization;\n");
//        source.Append("using System.Runtime.Serialization.Formatters.Binary;\n");
//        source.Append("using System.IO;\n\n");
//        source.Append("[Serializable]\n");
//        source.Append("public class DataManager : SingletonTemplate<DataManager>\n");
//        source.Append("{\n");

//        //定义变量
//        foreach (Type t in types)
//        {
//            source.Append("\tpublic " + t.Name + " " + t.Name.Remove(0, 2) + ";\n");
//        }
//        source.Append("\n");

//        //定义方法
//        foreach (Type t in types)
//        {
//            string typeName = t.Name.Remove(t.Name.IndexOf("Container"));
//            string funcName = t.Name.Remove(0, 2);
//            funcName = funcName.Substring(0, 1).ToUpper() + funcName.Substring(1);
//            funcName = funcName.Remove(funcName.IndexOf("Container"));
//            source.Append("\tpublic " + typeName + " Get" + funcName + "(int id)\n");
//            source.Append("\t{\n");
//            source.Append("\t\t" + typeName + " t = null;\n");
//            source.Append("\t\t" + t.Name.Remove(0, 2) + ".Dict.TryGetValue(id, out t);\n");
//            source.Append("\t\tif (t == null) Debug.LogError(" + '"' + "can't find the id " + '"' + " + id " + "+ " + '"' + " in " + t.Name + '"' + ");\n");
//            source.Append("\t\treturn t;\n");
//            source.Append("\t}\n");
//        }

//        //加载所有配置表
//        source.Append("\tpublic void LoadAll()\n");
//        source.Append("\t{\n");
//        foreach (Type t in types)
//        {
//            string typeName = t.Name.Remove(t.Name.IndexOf("Container"));
//            source.Append("\t\t" + t.Name.Remove(0, 2) + " = Load(" + '"' + typeName + '"' + ") as " + t.Name + ";\n");
//        }
//        source.Append("\t}\n\n");

//        //反序列化
//        source.Append("\tprivate System.Object Load(string name)\n");
//        source.Append("\t{\n");
//        source.Append("\t\tIFormatter f = new BinaryFormatter();\n");
//        source.Append("\t\tTextAsset text = Resources.Load<TextAsset>(" + '"' + "ConfigBin/" + '"' + " + name);\n");
//        source.Append("\t\tStream s = new MemoryStream(text.bytes);\n");
//        source.Append("\t\tSystem.Object obj = f.Deserialize(s);\n");
//        source.Append("\t\ts.Close();\n");
//        source.Append("\t\treturn obj;\n");
//        source.Append("\t}\n");

//        source.Append("}\n");
//        //保存脚本
//        string path = _rootPath + _scriptPath;
//        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
//        StreamWriter sw = new StreamWriter(path + "DataManager.cs");
//        sw.WriteLine(source.ToString());
//        sw.Close();
//    }


//    public static T[] ParseArray<T>(string text) where T : class, new()
//    {
//        string[] textArray = null;
//        var count = 0;
//        if (!string.IsNullOrEmpty(text))
//        {
//            textArray = text.Split(';');
//            count = textArray.Length;
//        }

//        var array = new T[count];

//        if (count == 0)
//        {
//            return array;
//        }

//        var type = typeof(T);
//        var fields = type.GetFields();
//        var fieldCount = fields.Length;
//        var fieldConverters = new TypeConverter[fieldCount];
//        for (var i = 0; i < fieldCount; ++i)
//        {
//            fieldConverters[i] = TypeDescriptor.GetConverter(fields[i].FieldType);
//        }

//        for (var i = 0; i < count; ++i)
//        {
//            var textFields = textArray[i].Split(':');
//            var obj = new T();
//            array[i] = obj;

//            //#if UNITY_EDITOR
//            if (textFields.Length != fieldCount)
//            {
//                Debug.LogError(string.Format("Failed to parse {0}, expect type {1}", textArray[i], type));
//                break;
//            }

//            // parse each filed
//            for (var j = 0; j < fieldCount; ++j)
//            {
//                var converter = fieldConverters[j];
//                var textField = textFields[j];
//                if (!converter.IsValid(textField))
//                {
//                    Debug.LogError(string.Format("Failed to parse {0}, expect type {1}", textArray[i], type));
//                    break;
//                }

//                var value = converter.ConvertFromString(textField);
//                fields[j].SetValue(obj, value);
//            }
//            //#else
//            //            // parse each filed
//            //            for (var j = 0; j < fieldCount; ++j)
//            //            {
//            //                var value = fieldConverters[j].ConvertFromString(textFields[j]);
//            //                fields[j].SetValue(obj, value);
//            //            }
//            //#endif
//        }

//        return array;
//    }


//}

//脚本生成器
class ScriptGenerator
{
    public string[] Fileds;
    public string[] Types;
    public string ClassName;

    public ScriptGenerator(string className, string[] fileds, string[] types)
    {
        ClassName = className;
        Fileds = fileds;
        Types = types;
    }

    //开始生成脚本
    public string Generate()
    {
        if (Types == null || Fileds == null || ClassName == null)
            return null;
        return CreateCode(ClassName, Types, Fileds);
    }

    //创建代码。   
    private string CreateCode(string tableName, string[] types, string[] fields)
    {
        //生成类
        StringBuilder classSource = new StringBuilder();
        classSource.Append("/*Auto create\n");
        classSource.Append("Don't Edit it*/\n");
        classSource.Append("\n");
        classSource.Append("using System;\n");
        classSource.Append("using System.Reflection;\n");
        classSource.Append("using System.Collections.Generic;\n");
        classSource.Append("[Serializable]\n");
        classSource.Append("public class " + tableName + "\n");
        classSource.Append("{\n");
        //设置成员
        for (int i = 0; i < fields.Length; ++i)
        {
            classSource.Append(PropertyString(types[i], fields[i]));
        }
        classSource.Append("}\n");

        //生成Container
        classSource.Append("\n");
        classSource.Append("[Serializable]\n");
        classSource.Append("public class " + tableName + "Container\n");
        classSource.Append("{\n");
        classSource.Append("\tpublic " + "Dictionary<int, " + tableName + ">" + " Dict" + " = new Dictionary<int, " + tableName + ">();\n");
        classSource.Append("}\n");
        return classSource.ToString();
    }

    private string PropertyString(string type, string propertyName)
    {
        if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(propertyName))
            return null;

        if (type == "List<int>") type = "List<int>";
        else if (type == "List<float>") type = "List<float>";
        else if (type == "List<string>") type = "List<string>";
        StringBuilder sbProperty = new StringBuilder();
        sbProperty.Append("\tpublic " + type + " " + propertyName + ";\n");
        return sbProperty.ToString();
    }

}
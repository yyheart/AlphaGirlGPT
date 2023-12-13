using UnityEngine;

/// <summary>
/// 基类自动创建并归类到 SingletonAutoGroup 节点下，不能预制在场景中！！！DontDestroyOnLoad
/// </summary>
public abstract class SingletonAuto<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarningFormat("[SingletonAuto] Instance '{0}' already destroyed on application quit. Won't create again, returning null.", typeof(T));
                return _instance;
            }

            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogErrorFormat("[SingletonAuto] Instance classes that inherit from SingletonAuto<T> should not be prefabricated into scenarios, please check!!!" +
                            "\nSingleton: {0}，ScenesName：{1}", typeof(T), UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        var startTime = Time.realtimeSinceStartup;
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString();

                        Debug.Log(string.Format("[SingletonAuto] An instance of {0} was created with DontDestroyOnLoad, elapsed: {1}.", typeof(T), Time.realtimeSinceStartup - startTime));
                    }
                    else
                    {
                        Debug.Log(string.Format("[SingletonAuto] {0} Using instance of already created {1}!", typeof(T), _instance.gameObject.name));
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
#if UNITY_EDITOR
        if (FindObjectsOfType(typeof(T)).Length > 1)
        {
            Debug.LogErrorFormat("[SingletonAuto] Instance classes that inherit from SingletonAuto<T> should not be prefabricated into scenarios, please check!!!" +
                    "\nSingletonAuto: {0}，ScenesName：{1}", typeof(T), UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            //Debug.LogError(string.Format("[SingletonAuto] Something went really wrong "
            //    + "- there should never be more than 1 singleton: {0}! Reopenning the scene might fix it.", typeof(T)));
            return;
        }
#endif
        _instance = this as T;
        //DontDestroyOnLoad(_instance.gameObject);

        _instance.gameObject.name = typeof(T).ToString();
        SetParent(_instance.gameObject);
    }

    private void SetParent(GameObject obj)
    {
        var parent = GameObject.Find("SingletonAutoGroup");
        if (parent == null)
        {
            parent = new GameObject("SingletonAutoGroup");
        }
        DontDestroyOnLoad(parent);
        obj.transform.SetParent(parent.transform);
    }


    private static bool applicationIsQuitting = false;

    public static bool ApplicationIsQuit
    {
        get { return applicationIsQuitting; }
    }

    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a SingletonAuto is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        //if (Global.LOG) Debug.Log(string.Format("SingletonAuto {0} destoryed!", GetType()));
        applicationIsQuitting = true;
    }
}

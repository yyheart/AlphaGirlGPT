using UnityEngine;
using System.Collections.Generic;
public class AlphaTracker
{
    public Vector3 pos;
    public Quaternion rotation;

    public AlphaTracker(Vector3 pos, Quaternion rot)
    {
        this.pos = pos;
        this.rotation = rot;
    }

    public AlphaTracker() { pos = Vector3.zero; rotation = Quaternion.identity; }

    public static AlphaTracker Zero
    {
        get
        {
            return new AlphaTracker();
        }
    }
        
}
public class AlphaMotion : MonoBehaviour {
    public delegate void BoolDelegate(int number, bool status);
    public delegate void FloatDelegate(int number, float value);
    public static event BoolDelegate OnButtonChanged;
    public static event FloatDelegate OnAnalogChanged;
    private static AlphaMotion _instance;
    public static AlphaMotion instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("Alpha");
                _instance = obj.AddComponent<AlphaMotion>();
            }
            return _instance;
        }

        private set
        {
            _instance = value;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }
    public Dictionary<int, float> analogs = new Dictionary<int, float>();
    public Dictionary<int, bool> buttonsUp = new Dictionary<int, bool>();
    public Dictionary<int, bool> buttonsDown = new Dictionary<int, bool>();
    public Dictionary<int, bool> buttons = new Dictionary<int, bool>();
    public Dictionary<int, AlphaTracker> trackers = new Dictionary<int, AlphaTracker>();

  
    public string TrackerAddress;
    public string ButtonAddress;
    public string AxisAddress;

    public void Register(string address)
    {
        this.TrackerAddress = "head@127.0.0.1";
        this.ButtonAddress = "hand@127.0.0.1";
        this.AxisAddress = address;
    }
    public void Register(string addressTracker,string buttonTracker)
    {
        this.TrackerAddress = addressTracker;
        this.ButtonAddress = buttonTracker;
       
    }
    public void Register(string address, string head, string hand)
    {
        this.TrackerAddress = head + "@" + address;
        this.ButtonAddress = hand+ "@" + address;
        this.AxisAddress = address;
    }
    //public void Register(Config config)
    //{
    //    this.TrackerAddress = config.TrackerAddress;
    //    this.ButtonAddress = config.ButtonAddress;
    //    this.AxisAddress = config.AxisAddress;
    //}

    /// <summary>
    /// 0代表横轴，1代表纵轴
    /// </summary>
    /// <param name="num_channel"></param>
    /// <returns></returns>
    public float GetAnalog(int num_channel)
    {
        //if (analogs.ContainsKey(num_channel))
        //{
        //    return analogs[num_channel];
        //}
        //return 0;
        if (Input.GetKey(KeyCode.LeftArrow) && num_channel == 0)
        {
            return -1f;
        }
        if (Input.GetKey(KeyCode.RightArrow) && num_channel == 0)
        {
            return 1f;
        }
        if (Input.GetKey(KeyCode.UpArrow) && num_channel == 1)
        {
            return 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow) && num_channel == 1)
        {
            return -1f;
        }
        return 0;
        //if (Input.GetKey(KeyCode.A) && num_channel == 0)
        //{
        //    return -1f;
        //}
        //if (Input.GetKey(KeyCode.D) && num_channel == 0)
        //{
        //    return 1f;
        //}
        //if (Input.GetKey(KeyCode.W) && num_channel == 1)
        //{
        //    return 1f;
        //}
        //if (Input.GetKey(KeyCode.S) && num_channel == 1)
        //{
        //    return -1f;
        //}
        //return 0;
    }
    public float GetAnalogArrow(int num_channel)
    {
        if (Input.GetKey(KeyCode.LeftArrow) && num_channel == 0)
        {
            return -1f;
        }
        if (Input.GetKey(KeyCode.RightArrow) && num_channel == 0)
        {
            return 1f;
        }
        if (Input.GetKey(KeyCode.UpArrow) && num_channel == 1)
        {
            return 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow) && num_channel == 1)
        {
            return -1f;
        }
        return 0;
      
    }


        public bool GetButton(int num_button)
    {
        //if (buttons.ContainsKey(num_button))
        //{
        //    return buttons[num_button];
        //}
        //return false;
        bool flag=false;
        switch (num_button)
        { 
            case 0:
                if (Input.GetKey(KeyCode.Alpha1) )
                {
                    flag= true;
                };
                break;
            case 1:
                if (Input.GetKey(KeyCode.Alpha2))
                {
                    flag = true;
                };
                break;
            case 2:
                if (Input.GetKey(KeyCode.RightArrow) )
                {
                    flag = true;
                };
                break;
            case 3:
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    flag = true;
                };
                break;
            case 4:
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    flag = true;
                };
                break;
            case 5:
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    flag = true;
                };
                break;
            case 6:
                if (Input.GetKey(KeyCode.Alpha3))
                {
                    flag = true;
                };
                break;
            case 7:
                if (Input.GetKey(KeyCode.Alpha4))
                {
                    flag = true;
                };
                break;
            case 8:
                if (Input.GetKey(KeyCode.Alpha5))
                {
                    flag = true;
                };
                break;
            default:
                flag = false;
                break;      
        }
        return flag;
    }

    public bool GetButtonDown(int num_button)
    {
        //if (buttonsDown.ContainsKey(num_button))
        //{
        //    return buttonsDown[num_button];
        //}
        //return false;
        bool flag = false;
        switch (num_button)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    flag = true;
                };
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    flag = true;
                };
                break;
            case 2:
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    flag = true;
                };
                break;
            case 3:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    flag = true;
                };
                break;
            case 4:
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    flag = true;
                };
                break;
            case 5:
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    flag = true;
                };
                break;
            case 6:
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    flag = true;
                };
                break;
            case 7:
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    flag = true;
                };
                break;
            case 8:
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    flag = true;
                };
                break;
            default:
                flag = false;
                break;
        }
        return flag;
    }

    public bool GetButtonUp(int num_button)
    {
        //if (buttonsUp.ContainsKey(num_button))
        //{
        //    return buttonsUp[num_button];
        //}
        //return false;

        bool flag = false;
        switch (num_button)
        {
            case 0:
                if (Input.GetKeyUp(KeyCode.Alpha1))
                {
                    flag = true;
                };
                break;
            case 1:
                if (Input.GetKeyUp(KeyCode.Alpha2))
                {
                    flag = true;
                };
                break;
            case 2:
                if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    flag = true;
                };
                break;
            case 3:
                if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    flag = true;
                };
                break;
            case 4:
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    flag = true;
                };
                break;
            case 5:
                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    flag = true;
                };
                break;
            case 6:
                if (Input.GetKeyUp(KeyCode.Alpha3))
                {
                    flag = true;
                };
                break;
            case 7:
                if (Input.GetKeyUp(KeyCode.Alpha4))
                {
                    flag = true;
                };
                break;
            case 8:
                if (Input.GetKeyUp(KeyCode.Alpha5))
                {
                    flag = true;
                };
                break;
            default:
                flag = false;
                break;
        }
        return flag;
    }

    private void Update()
    {
        //if (ButtonAddress != null)
        //{
        //    for (int i = 0; i < ConfigManager.config.ButtonNum; i++)
        //    {
        //        if (buttonsUp.ContainsKey(i)) buttonsUp[i] = false;
        //        if (buttonsDown.ContainsKey(i)) buttonsDown[i] = false;

        //        bool value = VRCN.vrpnButton(ButtonAddress, i);

        //        if (instance.buttons.ContainsKey(i))
        //        {
        //            if (value != instance.buttons[i])
        //            {
        //                if (OnButtonChanged != null)
        //                {
        //                    OnButtonChanged(i, value);
        //                }
        //                instance.buttonsDown[i] = value;
        //                instance.buttonsUp[i] = !value;
        //                instance.buttons[i] = value;
        //            }
        //            else
        //            {
        //                instance.buttonsDown[i] = false;
        //                instance.buttonsUp[i] = false;
        //            }
        //        }
        //        else
        //        {
        //            if (OnButtonChanged != null)
        //            {
        //                OnButtonChanged(i, value);
        //            }
        //            instance.buttons.Add(i, value);
        //            instance.buttonsDown.Add(i, value);
        //            instance.buttonsUp.Add(i, false);
        //        }
        //    }
        //}

        //if (AxisAddress != null)
        //{
        //    for (int i = 0; i < ConfigManager.config.AxisNum; i++)
        //    {
        //        float value = (float)VRCN.vrpnAnalog(AxisAddress, i);
        //        if (instance.analogs.ContainsKey(i))
        //        {
        //            if (value != instance.analogs[i])
        //            {
        //                if (OnAnalogChanged != null) OnAnalogChanged(i, value);
        //            }
        //            instance.analogs[i] = value;
        //        }
        //        else
        //        {
        //            if (OnAnalogChanged != null) OnAnalogChanged(i, value);
        //            instance.analogs.Add(i, value);
        //        }
        //    }
        //}

        if (TrackerAddress != null)
        {
            //for (int i = 0; i < ConfigManager.config.TrackerNum; i++)
            //{
            //    Vector3 pos = VRCN.vrpnTrackerPos(TrackerAddress, i);
            //    Quaternion rot = VRCN.vrpnTrackerQuat(TrackerAddress, i);
            //    if (instance.trackers.ContainsKey(i))
            //    {
            //        instance.trackers[i] = new GmotionTracker(pos, rot);
            //    }
            //    else
            //    {
            //        instance.trackers.Add(i, new GmotionTracker(pos, rot));
            //    }
            //}
            Vector3 pos = VRCN.vrpnTrackerPos(TrackerAddress, 0);
            Quaternion rot = VRCN.vrpnTrackerQuat(TrackerAddress, 0);
           
            if (instance.trackers.ContainsKey(0))
            {
                instance.trackers[0] = new AlphaTracker(pos, rot);
            }
            else
            {
                instance.trackers.Add(0, new AlphaTracker(pos, rot));
            }
          
        }
        if (ButtonAddress!=null)
        {
            Vector3 pos = VRCN.vrpnTrackerPos(ButtonAddress, 0);
            Quaternion rot = VRCN.vrpnTrackerQuat(ButtonAddress, 0);
           // Quaternion totTemp = rot;
           
            if (instance.trackers.ContainsKey(1))
            {
                instance.trackers[1] = new AlphaTracker(pos, rot);
            }
            else
            {
                instance.trackers.Add(1, new AlphaTracker(pos, rot));
            }
          
        }
    }

    private void LateUpdate()
    {
        //for (int i = 0; i < 5; i++)
        //{
        //    if (buttonsUp.ContainsKey(i)) buttonsUp[i] = false;
        //    if (buttonsDown.ContainsKey(i)) buttonsDown[i] = false;
        //}
    }

    public AlphaTracker GetTracker(int sensor_id)
    {
        if (trackers.ContainsKey(sensor_id))
        {
            return trackers[sensor_id];
        }
        return AlphaTracker.Zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class VRCN : MonoBehaviour {
    [DllImport("unityVrpn")]
    private static extern double vrpnAnalogExtern(string address, int channel, int frameCount);

    [DllImport("unityVrpn")]
    private static extern bool vrpnButtonExtern(string address, int channel, int frameCount);

    [DllImport("unityVrpn")]
    private static extern double vrpnTrackerExtern(string address, int channel, int component, int frameCount);

    private static float ErrorCode = -505;
    private static bool _isConnected = false;
    public static bool isConnected
    {
        get
        {
            return _isConnected;
        }
        set
        {
            if (_isConnected != value)
            {
                _isConnected = value;
                if (value)
                {
                    if (OnConnected != null) OnConnected();
                }
                else
                {
                    if (OnDisConnect != null) OnDisConnect();
                }
            }
        }
    }
    public delegate void VoidDelegate();
    public static VoidDelegate OnConnected;
    public static VoidDelegate OnDisConnect;

    public static double vrpnAnalog(string address, int channel)
    {
        return vrpnAnalogExtern(address, channel, Time.frameCount);
    }

    public static bool vrpnButton(string address, int channel)
    {
        return vrpnButtonExtern(address, channel, Time.frameCount);
    }

    //todo - need to allow different transforms here...
    //need to adjust two below functions to match up with your own tracking system's transform
    public static Vector3 vrpnTrackerPos(string address, int channel)
    {
        float x = -(float)vrpnTrackerExtern(address, channel, 0, Time.frameCount);
        float y = (float)vrpnTrackerExtern(address, channel, 2, Time.frameCount);
        float z = -(float)vrpnTrackerExtern(address, channel, 1, Time.frameCount);
       
        if (x == -ErrorCode && y == ErrorCode && z == -ErrorCode)
        {
            isConnected = false;
            return Vector3.zero;
        }
        else
        {
            isConnected = true;
            return new Vector3(x, y, z);
        }
       
        //return new Vector3(
        //  -(float)vrpnTrackerExtern(address, channel, 0, Time.frameCount),
        //  (float)vrpnTrackerExtern(address, channel, 2, Time.frameCount),
        //  -(float)vrpnTrackerExtern(address, channel, 1, Time.frameCount));
    }

    public static Quaternion vrpnTrackerQuat(string address, int channel)
    {
        //float x = (float)vrpnTrackerExtern(address, channel, 3, Time.frameCount);
        //float y = (float)vrpnTrackerExtern(address, channel, 5, Time.frameCount);
        //float z = (float)vrpnTrackerExtern(address, channel, 4, Time.frameCount);
        //float w = -(float)vrpnTrackerExtern(address, channel, 6, Time.frameCount);
        //if (x == ErrorCode && y == ErrorCode && z == ErrorCode)
        //{
        //    return Quaternion.identity;
        //}
        //return new Quaternion(x, y, z, w);

        float x = (float)vrpnTrackerExtern(address, channel, 3, Time.frameCount);
        float y = (float)vrpnTrackerExtern(address, channel, 5, Time.frameCount);
        float z = (float)vrpnTrackerExtern(address, channel, 4, Time.frameCount);
        float w = -(float)vrpnTrackerExtern(address, channel, 6, Time.frameCount);
        Quaternion a = new Quaternion(x, y, z, w);
        Quaternion b;
        b = a;
        b.eulerAngles = new Vector3(-a.eulerAngles.x, a.eulerAngles.y, -a.eulerAngles.z);
       
        return b;
    }

    public static Vector3 StepVrpnTrackerPos(string address, int channel)
    {
        float x = -(float)vrpnTrackerExtern(address, channel, 0, Time.frameCount);
        float y = (float)vrpnTrackerExtern(address, channel, 1, Time.frameCount);
        float z =- (float)vrpnTrackerExtern(address, channel, 2, Time.frameCount);
        if (x == ErrorCode && y == ErrorCode && z == ErrorCode)
        {
            isConnected = false;
            return Vector3.zero;
        }
        else
        {
            isConnected = true;
            return new Vector3(x, y, z);
        }
       
        //return new Vector3(
        //   -(float)vrpnTrackerExtern(address, channel, 0, Time.frameCount),
        //   (float)vrpnTrackerExtern(address, channel, 2, Time.frameCount),
        //   -(float)vrpnTrackerExtern(address, channel, 1, Time.frameCount));
    }

    public static Quaternion StepVrpnTrackerQuat(string address, int channel)
    {
        //float x = (float)vrpnTrackerExtern(address, channel, 3, Time.frameCount);
        //float y = (float)vrpnTrackerExtern(address, channel, 4, Time.frameCount);
        //float z = (float)vrpnTrackerExtern(address, channel, 5, Time.frameCount);
        //float w = (float)vrpnTrackerExtern(address, channel, 6, Time.frameCount);
        //if (x == ErrorCode && y == ErrorCode && z == ErrorCode)
        //{
        //    return Quaternion.identity;
        //}
        //return new Quaternion(x, y, z, w);

        float x = (float)vrpnTrackerExtern(address, channel, 3, Time.frameCount);
        float y = (float)vrpnTrackerExtern(address, channel, 5, Time.frameCount);
        float z = (float)vrpnTrackerExtern(address, channel, 4, Time.frameCount);
        float w = -(float)vrpnTrackerExtern(address, channel, 6, Time.frameCount);
        Quaternion a = new Quaternion(x, y, z, w);
        Quaternion b;
        b = a;
        b.eulerAngles = new Vector3(-a.eulerAngles.x, a.eulerAngles.y, -a.eulerAngles.z);
        return b;
    }
}

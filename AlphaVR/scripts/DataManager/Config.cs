
using UnityEngine;

public class Config
{
    public RenderPC renderPC;
    public VRScreen screen;
    public HardWare hardWare;
    public Config()
    {

    }
    public Config(RenderPC _renderPC, VRScreen _screen, HardWare _hardware)
    {
        renderPC = _renderPC;
        screen = _screen;
        hardWare = _hardware;
    }
}
public class RenderPC
{
    public string pcName;
    public string ip;
    public float eyeSeparation;
    public RenderPC()
    {

    }
    public RenderPC(string _pcName, string _ip, float _eyeSeparation)
    {
        pcName = _pcName;
        ip = _ip;
        eyeSeparation = _eyeSeparation;
    }
}
public class VRScreen
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
    public float sizeWidth;
    public float sizeHeight;
    public float resoWidth;
    public float resoHeight;
    public VRScreen()
    {

    }
    public VRScreen(string _name, Vector3 _position, Vector3 _rotationP, float _sizeWidth, float _sizeHeight, float _resoWidth, float _resoHeight)
    {
        name = _name;
        position = _position;
        rotation = _rotationP;
        sizeWidth = _sizeWidth;
        sizeHeight = _sizeHeight;
        resoHeight = _resoHeight;
        resoWidth = _resoWidth;
    }
}
public class HardWare
{
    public string pcName;
    public string trackerName;
    public string trackerIP;
    public Head head;
    public Hand hand;
    public HardWare()
    {

    }
    public HardWare(string _pcName, string _trackerName, string _trackerIP, Head _head, Hand _hand)
    {
        pcName = _pcName;
        trackerName = _trackerName;
        trackerIP = _trackerIP;
        head = _head;
        hand = _hand;
    }
}
public class Head
{
    public string name;
    public string type;
    public Head()
    {

    }
    public Head(string _name, string _type)
    {
        name = _name;
        type = _type;
    }
}
public class Hand
{
    public string name;
    public string type;
    public int joystickNum;
    public int buttonNum;
    public Hand()
    {

    }
    public Hand(string _name, string _type, int _joystickNum, int _buttonNum)
    {
        name = _name;
        type = _type;
        joystickNum = _joystickNum;
        buttonNum = _buttonNum;
    }
}


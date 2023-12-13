using Microsoft.Win32;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
public class MenuPanel : MonoBehaviour
{
    private GameObject MenuUI;
    private GameObject PanelMenuBox;        //退出 或 复位按钮
    private GameObject PanelQuitCheckBox;   //确认退出 或 取消退出

    private Button ButtonMenuQuit;          //退出
    private Button ButtonMenuReset;         //复位
    private Button ButtonMenuClose;         //关闭菜单
    private FlutterTextTips TextMenuTips;              //错误提示

    private Button ButtonQuitOk;            //确认退出
    private Button ButtonQiutCancel;        //取消退出

    private bool MenuState;

    private GameObject VRCamera;
    private Vector3 VRCameraPos;
    private Quaternion VRCameraRot;

    // Use this for initialization
    void Start()
    {
        MenuUI = transform.ZYFindChild("MenuUI").gameObject;
        MenuUI.gameObject.SetActive(false);

        PanelMenuBox = transform.ZYFindChild("PanelMenuBox").gameObject;
        PanelMenuBox.gameObject.SetActive(false);

        PanelQuitCheckBox = transform.ZYFindChild("PanelQuitCheckBox").gameObject;
        PanelQuitCheckBox.gameObject.SetActive(false);

        ButtonMenuQuit = transform.ZYFindChild("ButtonMenuQuit").GetComponent<Button>();
        ButtonMenuQuit.onClick.AddListener(() => { SetPanelQuitCheckBox(true); });

        ButtonMenuReset = transform.ZYFindChild("ButtonMenuReset").GetComponent<Button>();
        ButtonMenuReset.onClick.AddListener(() =>
        {
            if (VRCamera == null)
            {
                TextMenuTips.ShowFlutterTips("VR相机节点异常");
                return;
            }
            VRCamera.transform.position = VRCameraPos;
            VRCamera.transform.rotation = VRCameraRot;
            SetPanelMenuBox(false);
        }); //添加复位操作

        ButtonMenuClose = transform.ZYFindChild("ButtonMenuClose").GetComponent<Button>();
        ButtonMenuClose.onClick.AddListener(() => 
        { 
            SetPanelMenuBox(false); 
        });

        TextMenuTips = transform.ZYFindChild("TextMenuTips").GetComponent<FlutterTextTips>();
        TextMenuTips.gameObject.SetActive(false);

        ButtonQuitOk = transform.ZYFindChild("ButtonQuitOk").GetComponent<Button>();
        ButtonQuitOk.onClick.AddListener(() => 
        { 
            Application.Quit(); 
        });

        ButtonQiutCancel = transform.ZYFindChild("ButtonQiutCancel").GetComponent<Button>();
        ButtonQiutCancel.onClick.AddListener(() => 
        {
            SetPanelMenuBox(false);
        });

        VRCamera = GameObject.Find("VRController564");
        VRCameraPos = VRCamera.transform.position;
        VRCameraRot = VRCamera.transform.rotation;
    }

    void Update()
    {
        if (AlphaMotion.instance.GetButtonUp(1))
        {
            SetPanelMenuBox(!MenuState);
        }
    }


    private void SetPanelMenuBox(bool value)
    {
        MenuState = value;
        MenuUI.gameObject.SetActive(value);
        PanelMenuBox.gameObject.SetActive(value);
        if (!value)
        {
            PanelQuitCheckBox.gameObject.SetActive(value);
        }
    }

    private void SetPanelQuitCheckBox(bool value)
    {
        PanelMenuBox.gameObject.SetActive(!value);
        PanelQuitCheckBox.gameObject.SetActive(value);
    }
}
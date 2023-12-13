using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Realis.Input
{
    public class Handle : ExternalDevices
    {
        public WindowsKey KeyUp = WindowsKey.Alpha1;
        public WindowsKey KeyCenter = WindowsKey.Alpha3;
        public WindowsKey KeyDown = WindowsKey.Alpha2;


        public float X { get; private set; }
        public float Y { get; private set; }

        public override int GetButtonCount
        {
            get
            {
                return 3;
            }
        }
        /// <summary>
        /// LED灯设置
        /// </summary>
        /// <param name="on"></param>
        /// <param name="off"></param>
        /// <param name="time"></param>
        public override void LEDSeting(sbyte on, sbyte off, sbyte time)
        {
            Plugin.setHandleLed(on, off, time);
        }
        /// <summary>
        /// 震动设置
        /// </summary>
        /// <param name="on"></param>
        /// <param name="off"></param>
        /// <param name="time"></param>
        public override void ShockSeting(sbyte on, sbyte off, sbyte time)
        {
            Plugin.setHandleMotor(on, off, time);
        }
        
        /// <summary>
        /// Input.GetKey
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool GetKey(WindowsKey key)
        {
            return UnityEngine.Input.GetKey(WindoKeyToKeycode(key));
        }
        /// <summary>
        /// Input.GetKeyDown
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool GetKeyDown(WindowsKey key)
        {
            return UnityEngine.Input.GetKeyDown(WindoKeyToKeycode(key));
        }
        /// <summary>
        /// Input.GetKeyUp
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool GetKeyUp(WindowsKey key)
        {
            return UnityEngine.Input.GetKeyUp(WindoKeyToKeycode(key));
        }

        private void Awake()
        {
            try
            {
                //初始化imu  并注册按键
                Plugin.initHandle();

            }
            catch (Exception e)
            {
                Log.Warning("IMU笔初始化失败" + e);
            }
        }
        void Start()
        {
            try
            {
                Plugin.RegisteredHandleKeyboard(0, KeyManager.GetWindowsKeyToCppName(KeyDown));
                Plugin.RegisteredHandleKeyboard(1, KeyManager.GetWindowsKeyToCppName(KeyCenter));
                Plugin.RegisteredHandleKeyboard(2, KeyManager.GetWindowsKeyToCppName(KeyUp));
                Thread th = new Thread(new ThreadStart(StartDeviceThread));
                th.Start();
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void OnDestroy()
        {
            isUseIMU = false;
        }
        void StartDeviceThread()
        {
            while (isUseIMU)
            {
                Plugin.UpDataKeyState();
                X = Plugin.GetRockerX();
                Y = Plugin.GetRockerY();
                Thread.Sleep(1);
            }
        }
        private bool isUseIMU = true;
    }

}


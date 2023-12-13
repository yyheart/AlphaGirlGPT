using UnityEngine;
using System;

namespace Realis.Input
{
    public enum WindowsKey
    {
        None,
        /// <summary>
        /// The Left (or primary) mouse button.
        /// </summary>
        Mouse0,
        /// <summary>
        /// Right mouse button (or secondary mouse button).
        /// </summary>
        Mouse1,
        /// <summary>
        /// Middle mouse button (or third button).
        /// </summary>
        Mouse2,

        /// <summary>
        /// 方向键上下左右
        /// </summary>
        UpArrow, DownArrow, RightArrow, LeftArrow,

        /// <summary>
        /// 26个大写英文字母
        /// </summary>
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,

        /// <summary>
        /// 字母上方的数字键
        /// </summary>
        Alpha0, Alpha1, Alpha2, Alpha3, Alpha4, Alpha5, Alpha6, Alpha7, Alpha8, Alpha9,

        /// <summary>
        /// 右侧小键盘的数字键
        /// </summary>
        Keypad0, Keypad1, Keypad2, Keypad3, Keypad4, Keypad5, Keypad6, Keypad7, Keypad8, Keypad9,
        /// <summary>
        /// Numeric keypad '+'.
        /// </summary>
        KeypadPlus,
        /// <summary>
        /// Numeric keypad '-'.
        /// </summary>
        KeypadMinus,
        /// <summary>
        /// Numeric keypad '*'.
        /// </summary>
        KeypadMultiply,
        /// <summary>
        /// Numeric keypad '/'.
        /// </summary>
        KeypadDivide,
        /// <summary>
        /// Numeric keypad '.'.
        /// </summary>
        KeypadPeriod,

        LeftShift, LeftControl, LeftAlt, Escape,
        /// <summary>
        /// Return or Enter 回车键
        /// </summary>
        Return, Space
   ,


    }
    public static class KeyManager
    {

        public static System.String GetWindowsKeyToCppName(WindowsKey windowsKey)
        {
            System.String KeyString = "";

            if (windowsKey >= WindowsKey.A && windowsKey <= WindowsKey.F12)
            {
                KeyString = windowsKey.ToString();
            }
            else
            {
                switch (windowsKey)
                {
                    case WindowsKey.None:
                        break;
                    case WindowsKey.Mouse0:
                        KeyString = "MouseLeft";
                        break;
                    case WindowsKey.Mouse1:
                        KeyString = "MouseRight";
                        break;
                    case WindowsKey.Mouse2:
                        KeyString = "MouseMiddle";
                        break;
                    case WindowsKey.UpArrow:
                        KeyString = "Key_Up";
                        break;
                    case WindowsKey.DownArrow:
                        KeyString = "Key_Down";
                        break;
                    case WindowsKey.RightArrow:
                        KeyString = "Key_Right";
                        break;
                    case WindowsKey.LeftArrow:
                        KeyString = "Key_Left";
                        break;
                    case WindowsKey.Alpha0:
                        KeyString = "Key_0";
                        break;
                    case WindowsKey.Alpha1:
                        KeyString = "Key_1";
                        break;
                    case WindowsKey.Alpha2:
                        KeyString = "Key_2";
                        break;
                    case WindowsKey.Alpha3:
                        KeyString = "Key_3";
                        break;
                    case WindowsKey.Alpha4:
                        KeyString = "Key_4";
                        break;
                    case WindowsKey.Alpha5:
                        KeyString = "Key_5";
                        break;
                    case WindowsKey.Alpha6:
                        KeyString = "Key_6";
                        break;
                    case WindowsKey.Alpha7:
                        KeyString = "Key_7";
                        break;
                    case WindowsKey.Alpha8:
                        KeyString = "Key_8";
                        break;
                    case WindowsKey.Alpha9:
                        KeyString = "Key_9";
                        break;
                    case WindowsKey.Keypad0:
                        KeyString = "Key_num0";
                        break;
                    case WindowsKey.Keypad1:
                        KeyString = "Key_num1";
                        break;
                    case WindowsKey.Keypad2:
                        KeyString = "Key_num2";
                        break;
                    case WindowsKey.Keypad3:
                        KeyString = "Key_num3";
                        break;
                    case WindowsKey.Keypad4:
                        KeyString = "Key_num4";
                        break;
                    case WindowsKey.Keypad5:
                        KeyString = "Key_num5";
                        break;
                    case WindowsKey.Keypad6:
                        KeyString = "Key_num6";
                        break;
                    case WindowsKey.Keypad7:
                        KeyString = "Key_num7";
                        break;
                    case WindowsKey.Keypad8:
                        KeyString = "Key_num8";
                        break;
                    case WindowsKey.Keypad9:
                        KeyString = "Key_num9";
                        break;
                    case WindowsKey.KeypadPlus:
                        KeyString = "Key_add";
                        break;
                    case WindowsKey.KeypadMinus:
                        KeyString = "Key_subtract";
                        break;
                    case WindowsKey.KeypadMultiply:
                        KeyString = "Key_multiply";
                        break;
                    case WindowsKey.KeypadDivide:
                        KeyString = "Key_divide";
                        break;
                    case WindowsKey.KeypadPeriod:
                        KeyString = "Key_decimal";
                        break;
                    case WindowsKey.LeftShift:
                        KeyString = "Key_LeftShift";
                        break;
                    case WindowsKey.LeftControl:
                        KeyString = "Key_LeftCtrl";
                        break;
                    case WindowsKey.LeftAlt:
                        KeyString = "Key_LeftAlt";
                        break;
                    case WindowsKey.Escape:
                        KeyString = "Key_escape";
                        break;
                    case WindowsKey.Return:
                        KeyString = "Key_return";
                        break;
                    case WindowsKey.Space:
                        KeyString = "Key_space";
                        break;
                    default:
                        break;
                }
            }
            return KeyString;
        }


        public static int GetWindowsKeyToUnityKey(WindowsKey key)
        {

            int keyCodeInt = 0;

            if (key >= WindowsKey.A && key <= WindowsKey.Z)
            {
                keyCodeInt = (int)KeyCode.A + (int)(key - WindowsKey.A);
            }
            else if (key >= WindowsKey.F1 && key <= WindowsKey.F12)
            {
                keyCodeInt = (int)KeyCode.F1 + (int)(key - WindowsKey.F1);
            }
            else if (key >= WindowsKey.Alpha0 && key <= WindowsKey.Alpha9)
            {
                keyCodeInt = (int)KeyCode.Alpha0 + (int)(key - WindowsKey.Alpha0);
            }
            else if (key >= WindowsKey.Keypad0 && key <= WindowsKey.Keypad9)
            {
                keyCodeInt = (int)KeyCode.Keypad0 + (int)(key - WindowsKey.Keypad0);
            }
            else
            {
                switch (key)
                {
                    case WindowsKey.None:
                        keyCodeInt = 0;
                        break;
                    case WindowsKey.Mouse0:
                        keyCodeInt = 323;
                        break;
                    case WindowsKey.Mouse1:
                        keyCodeInt = 324;
                        break;
                    case WindowsKey.Mouse2:
                        keyCodeInt = 325;
                        break;
                    case WindowsKey.UpArrow:
                        keyCodeInt = 273;
                        break;
                    case WindowsKey.DownArrow:
                        keyCodeInt = 274;
                        break;
                    case WindowsKey.RightArrow:
                        keyCodeInt = 275;
                        break;
                    case WindowsKey.LeftArrow:
                        keyCodeInt = 276;
                        break;
                    case WindowsKey.KeypadPlus:
                        keyCodeInt = 270;
                        break;
                    case WindowsKey.KeypadMinus:
                        keyCodeInt = 269;
                        break;
                    case WindowsKey.KeypadMultiply:
                        keyCodeInt = 268;
                        break;
                    case WindowsKey.KeypadDivide:
                        keyCodeInt = 267;
                        break;
                    case WindowsKey.KeypadPeriod:
                        keyCodeInt = 266;
                        break;
                    case WindowsKey.LeftShift:
                        keyCodeInt = 304;
                        break;
                    case WindowsKey.LeftControl:
                        keyCodeInt = 306;
                        break;
                    case WindowsKey.LeftAlt:
                        keyCodeInt = 308;
                        break;
                    case WindowsKey.Escape:
                        keyCodeInt = 27;
                        break;
                    case WindowsKey.Return:
                        keyCodeInt = 13;
                        break;
                    case WindowsKey.Space:
                        keyCodeInt = 32;
                        break;
                }
            }
            return keyCodeInt;

        }
    }

}


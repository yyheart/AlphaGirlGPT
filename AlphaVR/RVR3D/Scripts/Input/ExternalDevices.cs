using System;
using UnityEngine;

namespace Realis.Input
{
    public abstract class ExternalDevices : MonoBehaviour
    {
        public abstract int GetButtonCount { get; }
        public abstract void LEDSeting(sbyte on, sbyte off, sbyte time);
        public abstract void ShockSeting(sbyte on, sbyte off, sbyte time);
        public abstract bool GetKey(WindowsKey key);
        public abstract bool GetKeyDown(WindowsKey key);
        public abstract bool GetKeyUp(WindowsKey key);

        /// <summary>
        /// WindowsKey转KeyCode
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected KeyCode WindoKeyToKeycode(WindowsKey key)
        {
            return (KeyCode)Enum.ToObject(typeof(KeyCode), KeyManager.GetWindowsKeyToUnityKey(key));
        }
    }
}


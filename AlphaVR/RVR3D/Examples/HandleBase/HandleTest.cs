using Realis.Input;
using UnityEngine;

namespace Realis.Examples
{
    public class HandleTest : MonoBehaviour
    {
        public Handle handle;

        void Update()
        {
            if (handle)
            {
                //IMUKeyUp
                if (handle.GetKey(handle.KeyUp))
                {
                    Log.Info("持续按下"+ handle.KeyUp);
                }
                if (handle.GetKeyDown(handle.KeyUp))
                {
                    Log.Info("按下" + handle.KeyUp);
                }
                if (handle.GetKeyUp(handle.KeyUp))
                {
                    Log.Info("抬起" + handle.KeyUp);
                }

                //IMUKeyCenter
                if (handle.GetKey(handle.KeyCenter))
                {
                    Log.Info("持续按下" + handle.KeyCenter);
                }
                if (handle.GetKeyDown(handle.KeyCenter))
                {
                    Log.Info("按下" + handle.KeyCenter);
                }
                if (handle.GetKeyUp(handle.KeyCenter))
                {
                    Log.Info("抬起" + handle.KeyCenter);
                }


                //IMUKeyDown
                if (handle.GetKey(handle.KeyDown))
                {
                    Log.Info("持续按下" + handle.KeyDown);
                }
                if (handle.GetKeyDown(handle.KeyDown))
                {
                    Log.Info("按下" + handle.KeyDown);
                }
                if (handle.GetKeyUp(handle.KeyDown))
                {
                    Log.Info("抬起" + handle.KeyDown);
                }
               
                //震动  LED闪烁
                if(handle.GetKeyDown(handle.KeyDown) && handle.GetKeyDown(handle.KeyUp))
                {
                    //imu.LEDSeting(10, 10, 10);

                    handle.ShockSeting(10, 10, 10);
                    
                }
            }

        }
    }
}


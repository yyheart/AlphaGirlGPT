using System.Runtime.InteropServices;

namespace Realis 
{
    public static class Plugin
    {
        //初始化IMU
        [DllImport("imuclientkey_dll")]
        public static extern void initWinSock();
        [DllImport("imuclientkey_dll")]
        //更新按键状态给windows
        public static extern void SuperviseKeyState();

        [DllImport("imuclientkey_dll")]
        //LED灯闪烁接口
        //flash_on_duration：闪烁持续时间（单位10毫秒）
        //flash_off_duration：停止闪烁持续时间（单位10毫秒）
        //duration：循环时间（单位100毫秒）
        // public static extern int setImuLed(System.SByte flash_on_duration,System.SByte flash_off_duration,System.SByte duration);
        public static extern int setImuLed(System.SByte flash_on_duration, System.SByte flash_off_duration, System.SByte duration);
        //震动接口
        //flash_on_duration：震动持续时间（单位10毫秒）
        //flash_off_duration：停止震动持续时间（单位10毫秒）
        //duration：循环时间（单位100毫秒）
        [DllImport("imuclientkey_dll")]
        public static extern int setImuMotor(System.SByte flash_on_duration, System.SByte flash_off_duration, System.SByte duration);

        [DllImport("imuclientkey_dll")]
        //将IMU的按键和Windows的按键绑定
        //KeyNum：IMU按键，0为down键，1为center键，2为up键
        //key_bdName：Window的按键
        public static extern void RegisteredKeyboard(int KeyNum, System.String key_bdName);

        //初始化IMU
        [DllImport("handle_clientkey_dll")]
        public static extern void initHandle();
        [DllImport("handle_clientkey_dll")]
        //将IMU的按键和Windows的按键绑定
        //KeyNum：IMU按键，0为down键，1为center键，2为up键
        //key_bdName：Window的按键
        public static extern void RegisteredHandleKeyboard(int KeyNum, System.String key_bdName);

        [DllImport("handle_clientkey_dll")]
        public static extern void UpDataKeyState();

        [DllImport("handle_clientkey_dll")]
        public static extern float GetRockerX();

        [DllImport("handle_clientkey_dll")]
        public static extern float GetRockerY();

        [DllImport("handle_clientkey_dll")]
        public static extern int setHandleLed(System.SByte flash_on_duration, System.SByte flash_off_duration, System.SByte duration);

        [DllImport("handle_clientkey_dll")]
        public static extern int setHandleMotor(System.SByte flash_on_duration, System.SByte flash_off_duration, System.SByte duration);
    }
}



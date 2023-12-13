using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTools
{
    public static long ConvertDateTimeToInt(int bits = 10)
    {
        //  System.DateTime time = System.DateTime.Now;
        // System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
        // long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      

        System.TimeSpan ts = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        long t = System.Convert.ToInt64(ts.TotalMilliseconds);
        if (bits == 10)
        {
            t /= 1000;
        }
        // print(t);
        return t;
    }


    /// <summary>
    /// 获得13位的时间戳
    /// </summary>
    /// <returns></returns>
    public static string GetTimeStamp()
    {
        System.DateTime time = System.DateTime.Now;
        long ts = ConvertDateTimeToInt(time);
        return ts.ToString();
    }
    /// <summary>  
    /// 将c# DateTime时间格式转换为Unix时间戳格式  
    /// </summary>  
    /// <param name="time">时间</param>  
    /// <returns>long</returns>  
    private static long ConvertDateTimeToInt(System.DateTime time)
    {
        System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
        long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
        return t;
    }
    //public static long ConvertDateTimeToInt()
    //{

    //    return ConvertDateTimeToInt(System.DateTime.Now);
    //}
    public static string TimeFormatBase(float nTotalTime)
    {
        string time = string.Empty;
        nTotalTime = Mathf.Floor(nTotalTime);
        float hour = Mathf.Floor(nTotalTime / 3600);
        float min = Mathf.Floor(nTotalTime % 3600 / 60);
        float sec = nTotalTime % 60;
        //if(hour >= 0 && hour<10)
        //    time = string.Concat("0",hour,":");
        //else
        //    time = string.Concat(hour, ":");

        if (min >= 0 && min < 10)
            time = string.Concat(time, "0", min, ":");
        else
            time = string.Concat(time, min, ":");

        if (sec >= 0 && sec < 10)
            time = string.Concat(time, "0", sec);
        else
            time = string.Concat(time, sec);
        return time;
    }

   public static string CurrentTime()
    {

        int hour;
        int minute;
        int second;
        int year;
        int month;
        int day;
        //获取当前时间
        hour = DateTime.Now.Hour;
        minute = DateTime.Now.Minute;
        second = DateTime.Now.Second;
        year = DateTime.Now.Year;
        month = DateTime.Now.Month;
        day = DateTime.Now.Day;

        //格式化显示当前时间
        //   return string.Format("{0:D2}:{1:D2}:{2:D2} " + "{3:D4}/{4:D2}/{5:D2}", hour, minute, second, year, month, day);
        return string.Format("{0:D4}/{1:D2}/{2:D2}", year, month, day);

    }
}

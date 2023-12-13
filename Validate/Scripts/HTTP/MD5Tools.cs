using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


public class MD5Tools 
{
   
    //获取字符串的MD5码
    public static string GetMD5(string input,int count=1)
    {
        string result = "";
        while (count>0)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
                // To force the hex string to lower-case letters instead of
                // upper-case, use he following line instead:
                // sb.Append(hashBytes[i].ToString("x2")); 
            }
            result = sb.ToString();
            input = result;
            count--;
        }
       
        return result;
    }

    public static bool Check(string str,string key,int count=1)
    {
        bool result = false;
        string md5str;
      
        while (count>0)
        {
           
            md5str = GetMD5(str);
            result = md5str.CompareTo(key) == 0;
            str = md5str;
            count--;
           
        }
       
        return result;
    }

  
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;

public class SocketServer : MonoBehaviour
{
    private IPEndPoint remoteEP;

    public void Start()
    {
        // 服务器的IP地址和端口
        IPAddress ipAddress = IPAddress.Parse("192.168.0.25"); // IP
        int port = 8888; // 端口

        remoteEP = new IPEndPoint(ipAddress, port);
    }

    public IEnumerator Chat(string wavPath) 
    {
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        bool connected = false;
        try
        {
            sock.Connect(remoteEP);
            // 连接成功
            Debug.Log("连接成功！");
            connected = true;
        }
        catch (System.Exception e)
        {
            // 处理发送异常
            Debug.LogError("发送失败：" + e.Message);
            connected = false;
        }

        if (connected)
        {
            byte[] byteSendArray  = File.ReadAllBytes(wavPath);
            int bytesSent = sock.Send(byteSendArray);
            // 数据发送成功
           
            if (bytesSent > 0)
            {
                Debug.Log("数据发送成功"+bytesSent);
                byte[] byteBufferArray = new byte[50 * 1024 * 1024];//50M空间接受数据
                sock.Blocking = false;
                int byteRecv = 0;
                while (byteRecv <= 0)
                {
                    try
                    {
                        byteRecv = sock.Receive(byteBufferArray);
                    }
                    catch { 
                        
                    }
                    yield return null;
                }
                Debug.Log("数据接受成功"+byteRecv);
                byte[] byteRecvArray = new byte[byteRecv];
                Array.Copy(byteBufferArray, byteRecvArray, byteRecv);
                // 释放Socket资源
                BaiDuAI.Instance.OnRecvData?.Invoke(byteRecvArray);
            }
            else
            {
                Debug.Log("数据发送失败");
            }
        }
        sock.Shutdown(SocketShutdown.Both);
        sock.Close();
    }


    public IEnumerator Chat(byte[] byteSendArray)
    {
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        bool connected = false;
        try
        {
            sock.Connect(remoteEP);
            // 连接成功
            Debug.Log("连接成功！");
            connected = true;
        }
        catch (System.Exception e)
        {
            // 处理发送异常
            Debug.LogError("发送失败：" + e.Message);
            connected = false;
        }

        if (connected)
        {
            int bytesSent = sock.Send(byteSendArray);
            // 数据发送成功

            if (bytesSent > 0)
            {
                Debug.Log($"数据发送成功,共发送了{bytesSent}个字节" + bytesSent);


                byte[] byteBufferArray = new byte[50 * 1024 * 1024];//50M空间接受数据
                int bufferSize = byteBufferArray.Length;
                sock.Blocking = false;
                int totalBytesReceived = 0;
                while (totalBytesReceived <= 0)
                {
                    int byteRecv = 0;
                    try
                    {
                        byteRecv = sock.Receive(byteBufferArray);
                        totalBytesReceived += byteRecv;
                    }
                    catch
                    {

                    }
                    // 确保字节数组至少有四个字节
                    if (byteRecv >= 4)
                    {
                        byte[] byteTotalLenght = new byte[4];
                        Array.Copy(byteBufferArray, byteTotalLenght, 4);

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(byteTotalLenght);
                        }
                        int totalLength = BitConverter.ToInt32(byteTotalLenght, 0) + 4; // 从数组的第0个位置开始转换
                        Debug.Log($"即将接受的音频数据总长度{totalLength - 4}");
                        while (totalBytesReceived < totalLength)
                        {
                            try
                            {
                                byteRecv = sock.Receive(byteBufferArray, totalBytesReceived, bufferSize - totalBytesReceived, SocketFlags.None);
                                totalBytesReceived += byteRecv;
                            }
                            catch
                            {

                            }
                            yield return null;
                        }
                    }
                    else
                    {
                        Console.WriteLine("字节数组长度不足以进行转换");
                    }
         
                    yield return null;
                }
                Debug.Log("数据接受成功" + (totalBytesReceived-4));
                byte[] byteRecvArray = new byte[totalBytesReceived-4];
                Array.Copy(byteBufferArray,4, byteRecvArray,0, totalBytesReceived-4);
                byteBufferArray = null;
                // 释放Socket资源
                BaiDuAI.Instance.OnRecvData?.Invoke(byteRecvArray);
            }
            else
            {
                Debug.Log("数据发送失败");
            }
        }
        sock.Shutdown(SocketShutdown.Both);
        sock.Close();
    }
}

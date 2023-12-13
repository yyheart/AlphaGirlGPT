using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;
public class XmlManager
{
    byte[] _keyArray = UTF8Encoding.UTF8.GetBytes("12345678909876543210123456789012");
    public XmlManager()
    {

    }
    public string serializeObject(object pObject, System.Type ty)
    {
        MemoryStream mStream = new MemoryStream();
        XmlSerializer xs = new XmlSerializer(ty);
        XmlTextWriter xmlTextWriter = new XmlTextWriter(mStream, Encoding.UTF8);
        xs.Serialize(xmlTextWriter, pObject);
        mStream = (MemoryStream)xmlTextWriter.BaseStream;
        return UTF8ByteArrayToString(mStream.ToArray());
    }
    public string UTF8ByteArrayToString(byte[] bytes)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        return encoding.GetString(bytes);
    }
    public void createXML(String fileName, string dataString)
    {
        StreamWriter writer;
        writer = File.CreateText(fileName);
        writer.Write(encrypt(dataString));
        writer.Close();
    }
    public T deserializeObject<T>(string serializedString, System.Type ty)
    {
        XmlSerializer xs = new XmlSerializer(ty);
        MemoryStream mStream = new MemoryStream(stringToUTF8ByteArray(serializedString));
        return (T)xs.Deserialize(mStream);
    }
    byte[] stringToUTF8ByteArray(string dataString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        return encoding.GetBytes(dataString);
    }
    public string LoadXML(string fileName)
    {
        StreamReader reader = File.OpenText(fileName);
        string dataString = reader.ReadToEnd();
        reader.Close();
        return decrypt(dataString) ;
    }
    public bool hasFile(string fileName)
    {
        return File.Exists(fileName);
    }
    public string encrypt(string toEncrypt)
    {
        ICryptoTransform cTransform = getRijndaelManaged().CreateEncryptor();
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
    public string decrypt(string toDecrypt)
    {
        ICryptoTransform cTransform = getRijndaelManaged().CreateDecryptor();
        byte[] toDecryptArray = Convert.FromBase64String(toDecrypt);
        byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
        return UTF8Encoding.UTF8.GetString(resultArray);
    }
    RijndaelManaged getRijndaelManaged()
    {
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = _keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        return rDel;
    }
}

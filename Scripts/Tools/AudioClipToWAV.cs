using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AudioClipToWAV : MonoBehaviour
{
    const int HEADER_SIZE = 44;//ͷ���ֶγ���
    int trueLength; //��Ƶ��ʵ����
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ��clip�д洢����Ƶ����תΪwav�ֽ���������Ϊwav�ļ��������ļ���ַ
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="trueLength"></param>
    /// <returns></returns>
    public string ConvertToWAVStreamAndSaveAsFile(AudioClip clip, int trueLength)
    {
        string fileName = "temp.wav";
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        this.trueLength = trueLength;
        using (var fileStream = CreateEmpty(filePath))
        {
            ConvertAndWrite(fileStream, clip);
            WriteHeader(fileStream, clip);
            
        }
        Debug.Log(filePath);
        return filePath;
    }

    /// <summary>
    /// ��clip�д洢����Ƶ����תΪwav�ֽ��������ش洢�ֽڵ�����
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="trueLength"></param>
    /// <returns></returns>
    public byte[] ConvertToWAVStream(AudioClip clip, int trueLength)
    {
        byte[] wavStream = new byte[44 + (clip.frequency * trueLength * clip.channels)*2];
        using (MemoryStream memoryStream = new MemoryStream(wavStream))
        {
            BinaryWriter writer = new BinaryWriter(memoryStream);

            writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
            writer.Write(BitConverter.GetBytes(wavStream.Length - 8));
            writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
            writer.Write(BitConverter.GetBytes(16));
            writer.Write(BitConverter.GetBytes((UInt16)1)); // ��Ƶ��ʽ��PCM = 1��
            writer.Write(BitConverter.GetBytes((UInt16)clip.channels));
            writer.Write(BitConverter.GetBytes(clip.frequency));
            writer.Write(BitConverter.GetBytes(clip.frequency * clip.channels * 2)); // byteRate
            writer.Write(BitConverter.GetBytes((UInt16)(clip.channels * 2))); // blockAlign
            writer.Write(BitConverter.GetBytes((UInt16)16)); // bitsPerSample
            writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
            writer.Write(BitConverter.GetBytes((clip.frequency * trueLength * clip.channels) * 2));

            float[] samples = new float[clip.frequency * trueLength * clip.channels];
            clip.GetData(samples, 0);
            int rescaleFactor = 32767; //to convert float to Int16
            UInt16 temp;
            for (int i = 0; i < samples.Length; i++)
            {
                temp = (UInt16)(samples[i] * rescaleFactor);
                writer.Write(BitConverter.GetBytes(temp)); // bitsPerSample
            }
        }
        return wavStream;
    }

    private void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        var samples = new float[clip.frequency * trueLength * clip.channels];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];
        //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

        Byte[] bytesData = new Byte[samples.Length * 2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }

    private FileStream CreateEmpty(string filepath)
    {
        var fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
        {
            fileStream.WriteByte(emptyByte);
        }

        return fileStream;
    }

    private void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        fileStream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        UInt16 two = 2;
        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        fileStream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes((clip.frequency * trueLength * clip.channels) * 2);//��Ҫ��֤
        fileStream.Write(subChunk2, 0, 4);

        //		fileStream.Close();
    }

    public  AudioClip ToAudioClip(byte[] wavBytes, string clipName = "MyAudioClip")
    {
        // ��ȡ�����ʣ�λ���ֽ�����24��27��
        int sampleRate = BitConverter.ToInt32(wavBytes, 24);

        // ��ȡͨ������λ���ֽ�����22��23��
        int channels = BitConverter.ToInt16(wavBytes, 22);

        // ��ȡλ��ȣ�λ���ֽ�����34��35��
        int bitDepth = BitConverter.ToInt16(wavBytes, 34);

        // ���ݿ�ʼ��λ�ã�ͨ����44�ֽڵ�ͷ��֮��
        int headerSize = 44;

        // �������������
        int sampleCount = (wavBytes.Length - headerSize) / (bitDepth / 8);

        // ����һ���µ� AudioClip
        AudioClip clip = AudioClip.Create(clipName, sampleCount, channels, sampleRate, false);

        // ת���ֽڵ�������
        float[] floatData = ConvertBytesToFloats(wavBytes, headerSize, bitDepth);

        // ��������
        clip.SetData(floatData, 0);

        return clip;
    }

    private  float[] ConvertBytesToFloats(byte[] bytes, int startIndex, int bitDepth)
    {
        int floatCount = (bytes.Length - startIndex) / (bitDepth / 8);
        float[] floatArr = new float[floatCount];

        for (int i = 0; i < floatCount; i++)
        {
            if (bitDepth == 16)
            {
                short sample = BitConverter.ToInt16(bytes, startIndex + i * 2);
                floatArr[i] = sample / 32768f; // 16-bit PCM �ķ�Χ�� -32768 �� 32767
            }
            else if (bitDepth == 32)
            {
                floatArr[i] = BitConverter.ToSingle(bytes, startIndex + i * 4); // 32-bit PCM
            }
            // ����λ��ȵĴ�������ڴ����
        }

        return floatArr;
    }
}

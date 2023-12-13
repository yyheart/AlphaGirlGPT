using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSouceManager : MonoBehaviour
{
    public static AudioSouceManager instance;

    public AudioSource saveAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySaveAudioSource(AudioClip _saveAudioClip)
    {
        saveAudioSource.clip = _saveAudioClip;
        saveAudioSource.Play();
    }
    public void StopSaveAudioSource()
    {
        saveAudioSource.Stop();
    }
}

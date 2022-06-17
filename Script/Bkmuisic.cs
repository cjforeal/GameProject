using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bkmuisic : MonoBehaviour
{
    private static Bkmuisic instance;
    public static Bkmuisic Instance => instance;
    public AudioClip playingAudioClip;
    public AudioClip prepareAudioClip;
    private float volumeBkmusic;
    [HideInInspector]
    public float volumeXiao;
    public AudioSource[] audioSources;
    public AudioClip[] uiAudio;
    public AudioClip[] playersAuido;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        volumeBkmusic = 1;
        volumeXiao = 1;
    }
    // Update is called once per frame
    void Update()
    {
    }
    //音乐大小改变
    public void OnBkMusicChange(float volume)
    {
        volumeBkmusic=volume;
        audioSources[0].volume = volumeBkmusic;
    }
    //音效大小改变
    public void OnXiaoMusicChange(float volume)
    {
        volumeXiao=volume;
        audioSources[1].volume = volumeXiao;
        audioSources[2].volume = volumeXiao;
        audioSources[3].volume = volumeXiao;
        audioSources[4].volume = volumeXiao;
    }
}

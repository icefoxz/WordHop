using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmSource; // 用于背景音乐
    public AudioSource sfxSource; // 用于音效

    public AudioData[] bgmData; // 用于在 Inspector 中设置背景音乐和其音量
    public AudioData[] sfxData; // 用于在 Inspector 中设置音效和其音量

    private static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static void PlayBGM(int index)=> instance.PlayMusic(index);
    public void PlayMusic(int index)
    {
        if (bgmSource.isPlaying) StopBGM();
        var bgm = bgmData[index];
        bgmSource.clip = bgm.clip;
        bgmSource.volume = bgm.volume;
        bgmSource.Play();
    }

    public static void StopBGM()=> instance.StopMusic();
    public void StopMusic()
    {
        bgmSource.Stop();
    }

    public static void PlaySFX(int index)=> instance.PlaySound(index);
    public void PlaySound(int index) // 使用索引来播放特定的音效
    {
        if (index >= 0 && index < sfxData.Length)
        {
            sfxSource.PlayOneShot(sfxData[index].clip, sfxData[index].volume);
        }
        else
        {
            Debug.LogWarning("AudioManager: Invalid SFX index!");
        }
    }
}
[Serializable]
public class AudioData
{
    public AudioClip clip; // 音效或背景音乐文件
    public float volume = 1.0f; // 预设音量
}

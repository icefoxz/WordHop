using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmSource; // 用于背景音乐
    public AudioSource sfxSource; // 用于音效

    public AudioData[] bgmData; // 用于在 Inspector 中设置背景音乐和其音量
    public AudioSfxData sfxData; // 用于在 Inspector 中设置音效和其音量

    public AudioMixer mixer; // 用于在 Inspector 中设置音量

    private static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Init()
    {
        BgmInit();
        EffectsInit();
    }

    private void EffectsInit()
    {
        RegSfx(GameEvents.Level_Alphabet_Add, sfxData.Level_Alphabet_Add);
        RegSfx(GameEvents.Level_Hints_add, sfxData.Level_Hints_add);
        RegSfx(GameEvents.Level_Item_Clear, sfxData.Level_Item_Clear);
    }

    private void RegSfx(string gameEvent, AudioData aud) => Game.MessagingManager.RegEvent(gameEvent, _ => PlaySound(aud));

    private void BgmInit()
    {
        Game.MessagingManager.RegEvent(GameEvents.Game_Start, _ =>
        {
            randomPlayBgm = false;
            PlayMusic(0);
        });
        Game.MessagingManager.RegEvent(GameEvents.Stage_Start, _ =>
        {
            StopMusic();
            randomPlayBgm = true;
            ResetBgmList();
        });
    }

    public static void PlayBGM(int index)=> instance.PlayMusic(index);
    [Button]public void PlayMusic(int index)
    {
        if (bgmSource.isPlaying) StopMusic();
        var bgm = bgmData[index];
        bgmSource.clip = bgm.clip;
        bgmSource.volume = bgm.volume;
        bgmSource.Play();
    }

    public static void StopBGM() => instance.StopMusic();
    [Button]public void StopMusic()
    {
        bgmSource.Stop();
    }

    public static void PlaySound(AudioData aud) => instance.sfxSource.PlayOneShot(aud.clip, aud.volume);// 使用索引来播放特定的音效

    public bool randomPlayBgm;
    private int currentBGMIndex = 0;  // 用于跟踪当前播放的 BGM 的索引
    private int[] bgmList;
    private void PlayNextBGM()
    {
        currentBGMIndex++;
        if (currentBGMIndex >= bgmList.Length)  // 如果已到达播放列表的末尾，则从头开始
        {
            ResetBgmList();
        }

        PlayMusic(currentBGMIndex);
    }

    private void ResetBgmList()
    {
        currentBGMIndex = 0;
        bgmList = Enumerable.Range(0, bgmData.Length).OrderByDescending(_ => Random.Range(0, bgmData.Length)).ToArray();
    }

    private void Update()
    {
        if(!randomPlayBgm)return;
        if (bgmList == null) ResetBgmList();
        if (bgmSource.isPlaying == false && bgmData.Length > 0)  // 如果音轨已结束，并且有 BGM 列表
            PlayNextBGM();
    }
}
[Serializable]
public class AudioData
{
    public AudioClip clip; // 音效或背景音乐文件
    [Range(0,1)]public float volume = 1.0f; // 预设音量
}

[Serializable]public class AudioSfxData
{
    public AudioData Level_Hints_add;
    public AudioData Level_Alphabet_Add;
    public AudioData Level_Item_Clear;
}
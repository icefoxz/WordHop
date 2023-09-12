using AOT.Utl;
using UnityEngine;

public class Pref
{
    private const string BGM_MUTE = "BGM_MUTE";
    private const string SFX_MUTE = "SFX_MUTE";
    private const string BGM_VOLUME = "BGM_VOLUME";
    private const string SFX_VOLUME = "SFX_VOLUME";
    private const string Highestlevel = "HighestLevel";
    private const string Playerlevel = "PlayerLevel";

    private static int CheckOrDefault(string key, int defaultValue)
    {
        if(!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetInt(key, defaultValue);
        return PlayerPrefs.GetInt(key);
    }

    private static bool CheckOrDefault(string key, bool defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetInt(key, defaultValue ? 1 : 0);
        return PlayerPrefs.GetInt(key) > 0;
    }

    private static float CheckOrDefault(string key, float defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetFloat(key, defaultValue);
        return PlayerPrefs.GetFloat(key);
    }
    private static string CheckOrDefault(string key, string defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetString(key, defaultValue);
        return PlayerPrefs.GetString(key);
    }

    public static bool GetBgmMute() => CheckOrDefault(BGM_MUTE, false);

    public static void SetBgmMute(bool value) => PlayerPrefs.SetInt(BGM_MUTE, value ? 1 : 0);
    public static bool GetSfxMute() => CheckOrDefault(SFX_MUTE, false);

    public static void SetSfxMute(bool value) => PlayerPrefs.SetInt(SFX_MUTE, value ? 1 : 0);
    public static float GetBgmVolume() => CheckOrDefault(BGM_VOLUME, 1);
    public static void SetBgmVolume(float value) => PlayerPrefs.SetFloat(BGM_VOLUME, value);
    public static float GetSfxVolume() => CheckOrDefault(SFX_VOLUME, 1);
    public static void SetSfxVolume(float value) => PlayerPrefs.SetFloat(SFX_VOLUME, value);

    public static PlayerLevel GetHighestLevel()
    {
        var json = CheckOrDefault(Highestlevel, string.Empty);
        return Json.Deserialize<PlayerLevel>(json);
    }

    public static void SetHighestLevel(PlayerLevel highestLevel)
    {
        var json = Json.Serialize(highestLevel);
        PlayerPrefs.SetString(Highestlevel, json);
    }

    public static PlayerLevel GetPlayerLevel()
    {
        var json = CheckOrDefault(Playerlevel, string.Empty);
        return Json.Deserialize<PlayerLevel>(json);
    }

    public static void SetPlayerLevel(PlayerLevel level)
    {
        var json = Json.Serialize(level);
        PlayerPrefs.SetString(Playerlevel, json);
    }
}
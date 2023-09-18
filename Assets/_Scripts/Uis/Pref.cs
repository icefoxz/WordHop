using AOT.Utl;
using System.Collections.Generic;
using UnityEngine;

public class Pref
{
    private const string BGM_MUTE = "BGM_MUTE";
    private const string SFX_MUTE = "SFX_MUTE";
    private const string BGM_VOLUME = "BGM_VOLUME";
    private const string SFX_VOLUME = "SFX_VOLUME";
    private const string Highestlevel = "HighestLevel";
    private const string Playerlevel = "PlayerLevel";

    private const string VillagersSet = "VillagersSet";
    private const string WarriorsSet = "WarriorSet";
    private const string MysteriousSet = "MysteriousSet";
    private const string MagesSet = "MagesSet";
    private const string ElvesSet = "ElvesSet";
    private const string NecromancerSet = "NecromancerSet";

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
    public static float GetBgmVolume() => CheckOrDefault(BGM_VOLUME, 0.5f);
    public static void SetBgmVolume(float value) => PlayerPrefs.SetFloat(BGM_VOLUME, value);
    public static float GetSfxVolume() => CheckOrDefault(SFX_VOLUME, 0.5f);
    public static void SetSfxVolume(float value) => PlayerPrefs.SetFloat(SFX_VOLUME, value);

    public static PlayerRec GetHighestLevel()
    {
        var json = CheckOrDefault(Highestlevel, string.Empty);
        return Json.Deserialize<PlayerRec>(json);
    }

    public static void SetHighestLevel(PlayerRec highestRec)
    {
        var json = Json.Serialize(highestRec);
        PlayerPrefs.SetString(Highestlevel, json);
    }

    public static PlayerRec GetPlayerLevel()
    {
        var json = CheckOrDefault(Playerlevel, string.Empty);
        return Json.Deserialize<PlayerRec>(json);
    }

    public static void SetPlayerLevel(PlayerRec current)
    {
        var json = Json.Serialize(current);
        PlayerPrefs.SetString(Playerlevel, json);
    }

    public static int[] GetCardData(JobTypes jobType)
    {
        string key = string.Empty;
        switch (jobType)
        {
            case JobTypes.Villagers:
                key = VillagersSet;
                break;
            case JobTypes.Warriors:
                key = WarriorsSet;
                break;
            case JobTypes.Mysterious:
                key = MysteriousSet;
                break;
            case JobTypes.Mages:
                key = MagesSet;
                break;
            case JobTypes.Elves:
                key = ElvesSet;
                break;
            case JobTypes.Necromancer:
                key = NecromancerSet;
                break;
        }
        var json = PlayerPrefs.GetString(key, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            var list = Json.Deserialize<int[]>(json);
            return list;
        }
        return null;
    }

    public static void SetCardData(JobTypes jobType, int level)
    {
        string key = string.Empty;
        switch (jobType)
        {
            case JobTypes.Villagers:
                key = VillagersSet;
                break;
            case JobTypes.Warriors:
                key = WarriorsSet;
                break;
            case JobTypes.Mysterious:
                key = MysteriousSet;
                break;
            case JobTypes.Mages:
                key = MagesSet;
                break;
            case JobTypes.Elves:
                key = ElvesSet;
                break;
            case JobTypes.Necromancer:
                key = NecromancerSet;
                break;
        }
        SetCardData(key, level);
    }

    private static void SetCardData(string key, int level)
    {
        List<int> card = new List<int>();
        var json = PlayerPrefs.GetString(key, string.Empty);
        var data = Json.Deserialize<int[]>(json);
        for (int i = 0; i < data.Length; i++)
            card.Add(data[i]);
        card.Add(level);
        int[] lists = card.ToArray();
        json = Json.Serialize(lists);
        PlayerPrefs.SetString(key, json);
    }
}
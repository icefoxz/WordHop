using System;
using AOT.Utl;
using System.Collections.Generic;
using System.Linq;
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
        var key = GetJobTypeKey(jobType);
        var list = GetCardList(key);
        if (list == null)
        {
            foreach (var type in Enum.GetValues(typeof(JobTypes)).Cast<JobTypes>())
            {
                var typeKey = GetJobTypeKey(type);
                if (type == JobTypes.Villagers)
                {
                    SetCardData(typeKey, new[] { 1 });
                    continue;
                }

                SetCardData(typeKey, Array.Empty<int>());
            }

            list = GetCardList(key);
        }

        return list;

        int[] GetCardList(string k)
        {
            var json = PlayerPrefs.GetString(k, string.Empty);
            var l = Json.Deserialize<int[]>(json);
            return l;
        }

    }

    private static string GetJobTypeKey(JobTypes jobType)
    {
        var key = jobType switch
        {
            JobTypes.Villagers => VillagersSet,
            JobTypes.Warriors => WarriorsSet,
            JobTypes.Mysterious => MysteriousSet,
            JobTypes.Mages => MagesSet,
            JobTypes.Elves => ElvesSet,
            JobTypes.Necromancer => NecromancerSet,
            _ => throw new ArgumentOutOfRangeException(nameof(jobType), jobType, null)
        };

        return key;
    }

    public static void UnlockCard(JobTypes jobType, int level)
    {
        var key = GetJobTypeKey(jobType);
        var json = PlayerPrefs.GetString(key, string.Empty);
        var data = Json.Deserialize<int[]>(json);
        if (data.Contains(level)) return;
        var card = data.ToList();
        card.Add(level);
        var lists = card.ToArray();
        SetCardData(key, lists);
    }

    private static void SetCardData(string key, int[] lists)
    {
        var json = Json.Serialize(lists);
        PlayerPrefs.SetString(key, json);
    }
}
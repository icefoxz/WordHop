#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AOT.Utl;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class Test_WordArranger : MonoBehaviour
{
    private const string ConfigsWordsPath = "/Configs/Words/";

    [Button]public void ConvertWordsToGroup(TextAsset asset)
    {
        var text = TextAssetProcessToRows(asset);
        var group = text.Select(s => new{key=new string(s.OrderBy(c=>c).ToArray()), text= s}).GroupBy(a=>a.key,a=>a.text).ToList();

        var wordGroup = new List<WordGroup>();
        group.ForEach(g =>
        {
            var wg = new WordGroup
            {
                Key = g.Key,
                Words = g.ToArray()
            };
            wordGroup.Add(wg);
        });
        //foreach (var wg in wordGroup)
        //{
        //    print($"{wg.Key}:[" + string.Join(",", wg.Words) + "]");
        //}
        var json = Json.Serialize(wordGroup);
        //write to file /Configs/Words/
        var path = GetFilePath(asset.name + "Group.json");
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();
    }

    private static string GetFilePath(string fileName) => Application.dataPath + ConfigsWordsPath + fileName;

    private static string[] TextAssetProcessToRows(TextAsset asset)
    {
        return asset.text.Split('\n')
            .Where(s=>!s.IsNullOrWhitespace())
            .Select(w=>w.Trim('\r').ToLower())
            .Distinct()
            .Where(c=>!WordUtl.IsShortForm(c) && !WordUtl.HasTripleConsecutiveChars(c))
            .ToArray();
    }

    [Button]public void SplitWords(TextAsset asset, string filePrefix = "common")
    {
        var text = TextAssetProcessToRows(asset);
        var lettersGroup = text.GroupBy(w => w.Length, w => w).ToArray();
        SplitWord(filePrefix, lettersGroup);
    }

    private static void SplitWord(string filePrefix, IGrouping<int, string>[] lettersGroup)
    {
        foreach (var letters in lettersGroup)
        {
            var wgList = new List<WordGroup>();
            foreach (var w in letters.GroupBy(l => new string(l.OrderBy(c => c).ToArray()), l => l))
            {
                var wg = new WordGroup
                {
                    Key = w.Key,
                    Words = w.ToArray()
                };
                wgList.Add(wg);
            }

            //print(string.Join('\n', wgList));
            var json = Json.Serialize(wgList);
            var path = GetFilePath(filePrefix + "_" + letters.Key + ".txt");
            File.WriteAllText(path, json);
        }

        AssetDatabase.Refresh();
    }

    [Button]public void GroupTexts(TextAsset asset, string filePrefix = "Filtered")
    {
        var text = TextAssetProcessToRows(asset);
        var lettersGroup = text.GroupBy(w => w.Length, w => w).ToArray();
        GroupText(filePrefix, lettersGroup);
    }

    private static void GroupText(string filePrefix, IGrouping<int, string>[] lettersGroup)
    {
        foreach (var letters in lettersGroup)
        {
            //print(string.Join('\n', wgList));
            var list = letters.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            var json = string.Join(" ", list);
            var path = GetFilePath(filePrefix + "_" + letters.Key + ".txt");
            File.WriteAllText(path, json);
        }

        AssetDatabase.Refresh();
    }

    [Button]public void DeserializeFromCsv(TextAsset asset,TextAsset dirtyText ,string filePrefix = "Filtered")
    {
        var rows = TextAssetProcessToRows(asset);
        var dirtyTexts = dirtyText?.text.Split('\n').Select(s=>s.Trim('\r').ToLower()).ToArray() ?? Array.Empty<string>();
        var letters = rows.Select(r =>
        {
            var a = r.Split(',');
            return a[0];
        }).Except(dirtyTexts).ToArray();
        var letterGroups = letters.GroupBy(l => l.Length, l => l).ToArray();
        SplitWord(filePrefix, letterGroups);
    }

    [Button]public void ResolveText(TextAsset asset,string fileName = null)
    {
        var words = asset.text.Split('\n').Select(s=>s.Split(' ')[0]).ToArray();
        var csv = string.Join("\n", words);
        var path = GetFilePath(fileName ?? asset.name + ".csv");
        File.WriteAllText(path, csv);
        AssetDatabase.Refresh();
    }
}
#endif
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
    [Button]public void ConvertWordsToGroup(TextAsset asset)
    {
        var text = asset.text.Split('\n').Where(s=>!s.IsNullOrWhitespace()).Select(w=>w.Trim('\r')).ToArray();
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
        var path = Application.dataPath + "/Configs/Words/" + asset.name + "Group.json";
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();
    }

    [Button]
    public void SplitWords(TextAsset asset, string filePrefix = "common")
    {
        var text = asset.text.Split('\n').Where(s => !s.IsNullOrWhitespace()).Select(w => w.Trim('\r')).ToArray();
        var letterGroup = text.GroupBy(w => w.Length, w => w).ToArray();
        foreach (var g in letterGroup)
        {
            var wgList = new List<WordGroup>();
            g.ForEach(w =>
            {
                var wg = new WordGroup
                {
                    Key = w,
                    Words = new[] {w}
                };
                wgList.Add(wg);
            });
            var json = Json.Serialize(wgList);
            var path = Application.dataPath + "/Configs/Words/" + filePrefix + "_" + g.Key + ".txt";
            File.WriteAllText(path, json);
        }
        AssetDatabase.Refresh();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AOT.Utl;
using UnityEngine;

[CreateAssetMenu(fileName = "WordConfigure", menuName = "配置/词语")]
public class WordConfigSo : ScriptableObject
{
    [SerializeField] private TextAsset translatedCsv;
    [SerializeField] private WordField[] 文字配置;

    private WordField[] Fields => 文字配置;

    public WordGroup[] GetWords(int characters)
    {
        var field = Fields.FirstOrDefault(c => c.Characters == characters);
        return Json.Deserialize<WordGroup[]>(field.Json);
    }

    public IDictionary<string, string> GetTranslateDictionary()
    {
        var dic = translatedCsv.text.Split('\n')
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(w =>
            {
                var array = w.Trim('\r').ToLower().Split(',');
                return new { word = array[0], cn = array[1] };
            })
            .GroupBy(c=>c.word,c=>new{c.word,c.cn})
            .Select(c => new { c.Key, c.ToArray()[0].cn })
            .ToDictionary(c => c.Key, c => c.cn);
        return dic;
    }

    public WordGroup GetRandomWords(int characters)
    {
        var array = GetWords(characters).ToArray();
        if (array.Length == 0)
            throw new NullReferenceException($"找不到[{characters}]的词语!");
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public string[] GetTranslatedWords()
    {
        return translatedCsv.text.Split('\n')
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(w => w.Trim('\r').ToLower())
            .Distinct()
            .Where(c => !WordUtl.IsShortForm(c) && !WordUtl.HasTripleConsecutiveChars(c))
            .ToArray();
    }

    [Serializable]private class WordField
    {
        [SerializeField] private int 字数;
        [SerializeField] private TextAsset 文件;

        public int Characters => 字数;
        public string Json => 文件.text;
    }
}
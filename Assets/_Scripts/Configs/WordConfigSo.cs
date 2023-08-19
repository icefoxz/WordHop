using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WordConfigure", menuName = "配置/词语")]
public class WordConfigSo : ScriptableObject
{
    [SerializeField] private WordField[] 文字配置;

    private WordField[] Fields => 文字配置;

    public IEnumerable<string> GetWords(int characters)
    {
        var field = Fields.FirstOrDefault(c => c.Characters == characters);
        return field?.Words.Split('\n').Where(s=>!string.IsNullOrWhiteSpace(s));
    }

    public string GetRandomWords(int characters)
    {
        var array = GetWords(characters).ToArray();
        if (array.Length == 0)
            throw new NullReferenceException($"找不到[{characters}]的词语!");
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    [Serializable]private class WordField
    {
        [SerializeField] private int 字数;
        [SerializeField] private TextAsset 文件;

        public int Characters => 字数;
        public string Words => 文件.text;
    }
}
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_WordsConfig : MonoBehaviour
{
    public WordConfigSo wordConfigSo;
    // Start is called before the first frame update
    [Button(ButtonSizes.Large)]public void GetRandomWords(int characters = 3)
    {
        var word = wordConfigSo.GetRandomWords(characters);
        print(word);
    }
}
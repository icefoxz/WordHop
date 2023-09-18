using AOT.Utl;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_SaveAndLoadIntList : MonoBehaviour
{
    public enum RaceType
    {
        Villagers,
        Warriors
    }
    [SerializeField] private int[] numVill = { 1, 3, 5, 7, 9, 10, 11, 12, 13 };
    [SerializeField] private int[] numWarr = { 2, 4, 6, 8, 10, 12, 14 };
    private const string Villagers = "Villagers";
    private const string Warriors = "Warriors";
    [SerializeField]private RaceType type;

    [Button] public void SaveJson()
    {
        var json = string.Empty;
        var key = string.Empty;
        switch (type)
        {
            case RaceType.Villagers:
                json = Json.Serialize(numVill);
                key = Villagers;
                break;
            case RaceType.Warriors:
                json = Json.Serialize(numWarr);
                key = Warriors;
                break;
        }
        PlayerPrefs.SetString(key, json);
    }

    [Button]public void LoadFromJson()
    {
        var txt = string.Join(", ", LoadFromTheDict(type));
        Debug.Log(txt);
        for(int i =0; i < LoadFromTheDict(type).Length; i++)
        {
            Debug.Log(LoadFromTheDict(type)[i]);
        }
    }
    private int[] LoadFromTheDict(RaceType race)
    {
        string key = string.Empty;
        switch (race)
        {
            case RaceType.Villagers:
                key = Villagers;
                break;
            case RaceType.Warriors:
                key = Warriors;
                break;
        }
        var json = PlayerPrefs.GetString(key, string.Empty);
        var vil = Json.Deserialize<int[]>(json);
        return vil;
   }
}

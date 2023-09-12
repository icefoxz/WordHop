using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Test_PlayerLevelConfig : MonoBehaviour
{
    [SerializeField] private JobTreeSo JobTree;

    [SerializeField] private int JobType = 1;
    [Button]public void LevelSet(string jobName,int level = 1)
    {
        var levelName = JobTree.GetJobInfo(jobName, level);
        if (levelName == null)
        {
            print($"Title is not found");
        }
        else
        {
            print($"Title: {levelName}");
        }
    }
}

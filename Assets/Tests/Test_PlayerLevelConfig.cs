using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Test_PlayerLevelConfig : MonoBehaviour
{
    [SerializeField] private JobTreeSo JobTree;

    [SerializeField] private int JobType = 1;
    [Button]public void LevelSet(int level = 1)
    {
        var job = JobTree.GetJobType(JobType -1);
        if(job == null)
        {
            print($"Class is not found");
        }
        else
        {
            var levelName = JobTree.GetJobLevelTitle(level - 1);
            if(levelName == null)
            {
                print($"Title is not found");
            }
            else
            {
                print($"Class: {job}, Title: {levelName}");
            }
        }
    }
}

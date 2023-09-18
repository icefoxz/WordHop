using Sirenix.OdinInspector;
using UnityEngine;

public class Test_PlayerLevelConfig : MonoBehaviour
{
    [SerializeField] private JobConfigSo _jobConfig;

    [SerializeField] private int JobType = 1;
    [Button]public void LevelSet(string jobName,int level = 1)
    {
        var levelName = _jobConfig.GetJobInfo(jobName, level);
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

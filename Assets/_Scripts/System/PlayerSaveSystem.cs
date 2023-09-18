using System.Collections.Generic;
using AOT.Utls;
using GamePlay;

public class PlayerSaveSystem
{
    private PlayerModel Player => Game.Model.Player;


    public void Init()
    {
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Win, SaveCurrentPlayerRecord);
        Game.MessagingManager.RegEvent(GameEvents.Player_Level_Up, CheckPlayerUnlockedCards);
    }

    private void CheckPlayerUnlockedCards(ObjectBag b)
    {
        var job = Player.Current.Job;
        var jobLevels = new List<int> { 1, 2, 3 }; //Pref.GetJobLevels(job.JobType);
        if (jobLevels.Contains(job.Level)) return;
        jobLevels.Add(job.Level);
        //Pref.SetJobLevels(job.JobType, jobLevels);
    }

    private void SaveCurrentPlayerRecord(ObjectBag b)
    {
        Pref.SetHighestLevel(Player.HighestRec);
        Pref.SetPlayerLevel(Player.Current);
    }
}
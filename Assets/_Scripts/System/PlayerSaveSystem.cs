using System.Collections.Generic;
using System.Linq;
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
        Pref.UnlockCard(job.JobType, job.Level);
    }

    private void SaveCurrentPlayerRecord(ObjectBag b)
    {
        Pref.SetHighestLevel(Player.HighestRec);
        Pref.SetPlayerLevel(Player.Current);
    }

    public void SaveAll() => SaveCurrentPlayerRecord(null);

    public void LoadHighestRecord()
    {
        var highestLevel = Pref.GetHighestLevel();
        Player.SetHighestLevel(highestLevel);
    }

    public void StartGame(JobTypes jobType)
    {
        var job = Game.ConfigureSo.JobConfig.GetPlayerJob(jobType, 1);
        var levelFields = Game.ConfigureSo.UpgradeConfigSo.GetLevels();
        var player = new PlayerModel(new PlayerRec(1, 0, 0, 0, job), levelFields);
        Game.Model.InitPlayer(player);
        Game.PlayerSave.LoadHighestRecord();
    }

    public JobTypes[] GetUnlockedJobs()
    {
        var jobs = new List<JobTypes> { JobTypes.Villagers };
        var hasWarrior = Pref.GetCardData(JobTypes.Warriors).Contains(1);
        if(hasWarrior) jobs.Add(JobTypes.Warriors);
        var hasMage = Pref.GetCardData(JobTypes.Mages).Contains(1);
        if(hasMage) jobs.Add(JobTypes.Mages);
        var hasNecromancer = Pref.GetCardData(JobTypes.Necromancers).Contains(1);
        if(hasNecromancer) jobs.Add(JobTypes.Necromancers);
        var hasElves = Pref.GetCardData(JobTypes.Elves).Contains(1);
        if(hasElves) jobs.Add(JobTypes.Elves);
        var hasMysterious = Pref.GetCardData(JobTypes.Mysterious).Contains(1);
        if(hasMysterious) jobs.Add(JobTypes.Mysterious);
        return jobs.ToArray();
    }
}
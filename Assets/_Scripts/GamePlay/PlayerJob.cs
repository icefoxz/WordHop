using System;

public enum JobTypes
{
    Villagers,
    Warriors,
    Mysterious,
    Mages,
    Elves,
    Necromancers
}

public record PlayerJob
{
    public string Title { get; set; }
    public int Level { get; set; }
    public JobTypes JobType { get; set; }

    public PlayerJob(string title, int level, JobTypes jobType)
    {
        Title = title;
        Level = level;
        JobType = jobType;
    }
}

public static class JobExtension
{
    public static string ToText(this JobTypes job) => job switch
    {
        JobTypes.Villagers => "Villager",
        JobTypes.Warriors => "Warrior",
        JobTypes.Mysterious => "Mysterious",
        JobTypes.Mages => "Mage",
        JobTypes.Elves => "Elf",
        JobTypes.Necromancers => "Necromancer",
        _ => throw new ArgumentOutOfRangeException(nameof(job), job, null)
    };
}
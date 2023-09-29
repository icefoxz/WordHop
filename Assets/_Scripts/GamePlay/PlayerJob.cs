using System;

public enum JobTypes
{
    Villagers = 0,
    Warriors = 1,
    Mysterious = 2,
    Mages = 3,
    Elves = 4,
    Necromancers = 5
}

public record PlayerJob
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Level { get; set; }
    public int Quality { get; set; }
    public JobTypes JobType { get; set; }

    public PlayerJob(int id,string title, int level, JobTypes jobType, int quality)
    {
        Id = id;
        Title = title;
        Level = level;
        JobType = jobType;
        Quality = quality;
    }

    public virtual bool Equals(PlayerJob other) => other != null && Id == other.Id;
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
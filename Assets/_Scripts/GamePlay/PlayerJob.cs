public enum JobTypes
{
    Warriors,
    Mages,
    Elves,
    Rouges
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
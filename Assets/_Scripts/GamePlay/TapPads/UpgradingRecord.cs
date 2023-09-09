using System.Collections.Generic;

public record UpgradingRecord
{
    private readonly List<LevelRecord> _levels = new List<LevelRecord>();
    public int UpgradeExp { get; }
    public IList<LevelRecord> Levels => _levels;

    public UpgradingRecord(int upgradeExp)
    {
        UpgradeExp = upgradeExp;
    }

    public void AddRecord(int from, int to, int level, int maxExp) =>
        _levels.Add(new LevelRecord(level, from, to, maxExp));

    public record LevelRecord
    {
        public int Level { get; }
        public int FromExp { get; }
        public int ToExp { get; }
        public int MaxExp { get; }

        public LevelRecord(int level, int fromExp, int toExp, int maxExp)
        {
            Level = level;
            ToExp = toExp;
            MaxExp = maxExp;
            FromExp = fromExp;
        }
    }
}
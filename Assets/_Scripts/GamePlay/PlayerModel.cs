public class PlayerModel : ModelBase
{
    public int Level { get; set; }
    public int Exp { get; set; }
    public PlayerRank Rank { get; set; }
    public RankConfigSo RankConfig { get;set; }
    public PlayerModel(int level, int exp, PlayerRank rank, RankConfigSo rankConfig)
    {
        Level = level;
        Exp = exp;
        Rank = rank;
        RankConfig = rankConfig;
    }
    public PlayerModel()
    {
        Level = 1;
        Exp = 0;
    }
    public void AddExp(int exp)
    {
        Exp += exp;
        if (Rank.IsMax) return;
        if(Exp >= Rank.NextLevelExp)
        {
            Level++;
            Exp -= Rank.NextLevelExp;
            Rank = RankConfig.GetNextRank(Rank);
            Rank.NextLevelExp = Rank.GetNextLevelExp();
        }
    }
}

public class PlayerRank
{
    public bool IsMax;
    public int NextLevelExp;

    internal int GetNextLevelExp()
    {
        return NextLevelExp;
    }
}
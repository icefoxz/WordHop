public record PlayerRec
{
    public int Level { get; set; }
    public int Exp { get; set; }
    public int Coin { get; set; }
    public int Score { get; set; }
    public PlayerJob Job { get; set; }

    public PlayerRec(int level, int exp, int coin, int score, PlayerJob job)
    {
        Level = level;
        Exp = exp;
        Coin = coin;
        Score = score;
        Job = job;
    }

    public void SetLevel(int level, int exp)
    {
        Level = level;
        Exp = exp;
    }
    public void AddScore(int score) => Score += score;
    public void AddCoin(int coin) => Coin += coin;
    public void SetScore(int score) => Score = score;
    public void SetCoin(int coin) => Coin = coin;
    public void UpdateJob(PlayerJob job) => Job = job;
}
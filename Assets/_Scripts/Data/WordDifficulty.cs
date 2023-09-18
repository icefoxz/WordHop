public readonly struct TapDifficulty
{
    public float Outline { get; }
    public int Item { get; }

    public TapDifficulty(float outline, int item)
    {
        Outline = outline;
        Item = item;
    }
}

public readonly struct LevelDifficultyWeight
{
    public float MaxWord { get; }
    public float MaxTime { get; }
    public float Total => MaxWord + MaxTime;
    public float WordWeight => MaxWord / Total;
    public float TimeWeight => MaxTime / Total;

    public LevelDifficultyWeight(float maxWord, float time)
    {
        MaxWord = maxWord;
        MaxTime = time;
    }

    public float GetWordDifficulty(float difficultyValue) => WordWeight * difficultyValue;
    public float GetTimeDifficulty(float difficultyValue) => TimeWeight * difficultyValue;

    public override string ToString() => $"{{ MaxWord = {MaxWord}, MaxTime = {MaxTime} }}";
}
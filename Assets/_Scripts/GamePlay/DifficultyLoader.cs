using UnityEngine;

/// <summary>
/// 关卡加载器
/// </summary>
public class DifficultyLoader
{
    public LevelDifficultySo LevelDifficultySo { get; }
    public TapPadDifficultySo TapPadDifficultySo { get; }

    public DifficultyLoader(LevelDifficultySo levelDifficultySo, TapPadDifficultySo tapPadDifficultySo)
    {
        LevelDifficultySo = levelDifficultySo;
        TapPadDifficultySo = tapPadDifficultySo;
    }

    // 获取随机关卡配置
    public (TapDifficulty[]words, int countdown, float difficulty) GetChallengeStageLevelConfig(int gameCount)
    {
        var difficulty = new GameDifficulty(gameCount, LevelDifficultySo); // 获取当前的难度系数
        var difficultyValue = difficulty.GetCurrentDifficulty(); // 获取难度值
        var extraSecs = difficulty.GetExtraTime(); // 获取额外时间
        var wordLength = difficulty.GetWordLength();// 获取文字长度, 如果有指定文字长度，则使用指定的文字长度
        var wds = GetTapPadSettings(TapPadDifficultySo, difficultyValue, wordLength); // 获取文字难度配置
        return (wds, extraSecs, difficultyValue);
    }

    // 获取文字难度配置
    public static TapDifficulty[] GetTapPadSettings(TapPadDifficultySo tapPadDifficultySo, float difficulty, int wordLength)
    {
        //根据最大字数分配难度权重
        var wordArray = new TapDifficulty[wordLength];

        float avgDifficulty = difficulty / wordLength;
        float variance = avgDifficulty * 0.1f; // 你可以调整这个值来控制难度的变化幅度

        for (var i = 0; i < wordLength; i++)
        {
            // 难度值小于0时，难度值为0
            var difficultValue = difficulty <= 0 ? 0 : Random.Range(avgDifficulty - variance, avgDifficulty + variance);
            var wd = tapPadDifficultySo.GetDifficultyValue(difficultValue, 0.5f);
            wordArray[i] = wd;
            difficulty -= difficultValue;
        }
        return wordArray;
    }


    // 获取关卡难度权重
    public static LevelDifficultyWeight GetDifficultyWeight(LevelDifficultySo levelDifficultySo,
        TapPadDifficultySo tapPadDifficultySo)
    {
        var maxWords = levelDifficultySo.GetMaxWords(); // 获取最大字数
        var maxTimeDifficulty = levelDifficultySo.GetMaxTimeDifficulty(); // 获取最大加时难度
        var tapPadMaxDifficulty = tapPadDifficultySo.Difficulty.MaxDifficulty(); // 获取最大点击难度
        var padMaxDifficulty = maxWords * tapPadMaxDifficulty; // 字数 * 点击难度 = 最大点击难度
        var w = new LevelDifficultyWeight(padMaxDifficulty, maxTimeDifficulty); // 点击难度权重，加时难度权重
        return w;
    }
}

public class GameDifficulty
{
    public LevelDifficultySo LevelConfig;
    private int gameCount;

    private const float K = 0.001f; // 这是我们提到的k值，你可以根据需要调整它


    public GameDifficulty(int gameCount, LevelDifficultySo levelConfig)
    {
        this.gameCount = gameCount;
        this.LevelConfig = levelConfig;
    }

    public float GetCurrentDifficulty()
    {
        // 使用新的难度曲线函数
        return 1f - Mathf.Exp(-KValue() * gameCount);
    }

    // 难度曲线函数, k值 如果基于 400局的游戏
    // 0.004f 为适中, 0.002f 为难度上升更平缓, 0.006f 就会比较陡峭也比较有挑战性
    private static float KValue()
    {
        return 0.004f;
    }

    public int GetExtraTime()
    {
        var currentDifficulty = GetCurrentDifficulty();
        return gameCount % 10 == 0 ? // 每10关额外加时
            // 考虑为7字数的关卡提供更多的额外时间，可以调整这里的逻辑
            LevelConfig.GetCountdownSecsByDifficulty(1.0f).ExtraSecs : LevelConfig.GetCountdownSecsByDifficulty(currentDifficulty).ExtraSecs;
    }

    public int GetWordLength()
    {
        var currentDifficulty = GetCurrentDifficulty();
        var wordLength = LevelConfig.GetWordLengthByDifficulty(currentDifficulty);
        if (gameCount % 10 == 0) // 每10关7字数
        {
            wordLength += Random.Range(0, 2);
            return wordLength > 7 ? 7 : wordLength;
        }
        return wordLength;
    }
}
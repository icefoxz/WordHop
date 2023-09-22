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

    // 获取文字难度配置
    public TapDifficulty[] GetTapPadSettings(float difficulty, int wordLength)
    {
        //根据最大字数分配难度权重
        var wordArray = new TapDifficulty[wordLength];

        float avgDifficulty = difficulty / wordLength;
        float variance = avgDifficulty * 0.1f; // 你可以调整这个值来控制难度的变化幅度

        for (var i = 0; i < wordLength; i++)
        {
            // 难度值小于0时，难度值为0
            var difficultValue = difficulty <= 0 ? 0 : Random.Range(avgDifficulty - variance, avgDifficulty + variance);
            var wd = TapPadDifficultySo.GetDifficultyValue(difficultValue, 0.5f);
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
        // 对于前5关，固定为3个字。
        if (gameCount <= 5)
        {
            return 3;
        }
        var currentDifficulty = GetCurrentDifficulty();
        // 依据当前的难度和一些随机性来决定单词的长度。
        var rand = Random.Range(0f, 1f);
        //var wordLength = LevelConfig.GetWordLengthByDifficulty(currentDifficulty);
        var wordLength = 3 + (int)(rand * (currentDifficulty * 4f)); // 这里的4f代表最大增加4个字，即最大7个字
        if (gameCount % 10 == 0) 
        {
            wordLength += Random.Range(0, 2); //难度加2字
        }
        wordLength = Mathf.Clamp(wordLength, 3, 7); // 确保单词长度在3到7之间
        return wordLength;
    }
}
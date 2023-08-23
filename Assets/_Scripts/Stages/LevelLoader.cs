using UnityEngine;

/// <summary>
/// 关卡加载器
/// </summary>
public class LevelLoader
{
    public LevelDifficultySo LevelDifficultySo { get; }
    public TapPadDifficultySo TapPadDifficultySo { get; }

    public LevelLoader(LevelDifficultySo levelDifficultySo, TapPadDifficultySo tapPadDifficultySo)
    {
        LevelDifficultySo = levelDifficultySo;
        TapPadDifficultySo = tapPadDifficultySo;
    }

    // 获取随机关卡配置
    public (WordDifficulty[]words, int countdown) GetChallengeStageLevelConfig(float difficulty, int word = 0)
    {
        var levelDiff = GetDifficultyWeight(LevelDifficultySo, TapPadDifficultySo); // 获取难度权重
        var totalDiff = difficulty +
                        //levelDiff.Total / 7; // 计算总难度
                        Random.Range(0, 3);
        var timeDiff = levelDiff.GetTimeDifficulty(totalDiff); // 分配时间难度
        var wordDiff = levelDiff.GetWordDifficulty(totalDiff); // 分配文字难度
        var secSet = LevelDifficultySo.GetCountdownSecsByDifficulty(timeDiff); // 获取倒计时配置
        var countdown = secSet.ExtraSecs; // 计算倒计时
        var wds = GetTapPadSettings(LevelDifficultySo, TapPadDifficultySo, wordDiff, word); // 获取文字难度配置
        return (wds, countdown);
    }

    // 获取文字难度配置
    public static WordDifficulty[] GetTapPadSettings(LevelDifficultySo levelDifficultySo,
        TapPadDifficultySo tapPadDifficultySo, float difficultyValue, int words = 0)
    {
        // 先决定最大字数
        var set = words > 0
            ? levelDifficultySo.GetRandomDifficultyByLength(words)
            : levelDifficultySo.GetRandomWordLengthByDifficulty(difficultyValue);
        //根据最大字数分配难度权重
        var wordArray = new WordDifficulty[set.WordLength];
        for (var i = 0; i < set.WordLength; i++)
        {
            // 难度值小于0时，难度值为0
            var difficultValue = difficultyValue <= 0 ? 0 : Random.Range(0, set.DifficultyValue);
            var wd = tapPadDifficultySo.GetDifficultyValue(difficultValue, 0.5f);
            wordArray[i] = wd;
            difficultyValue -= set.DifficultyValue;
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
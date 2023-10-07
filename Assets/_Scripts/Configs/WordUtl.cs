using System.Linq;

public static class WordUtl
{
    public static bool IsShortForm(string word)
    {
        // 创建一个元音集合
        char[] vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };

        // 如果word中不存在任何元音，则判定为简写
        return !word.ToLower().Any(c => vowels.Contains(c));
    }

    public static bool HasTripleConsecutiveChars(string word)
    {
        if (word.Length < 3) return false;
        for (int i = 0; i < word.Length - 2; i++)
        {
            if (word[i] == word[i + 1] && word[i] == word[i + 2])
            {
                return true;
            }
        }
        return false;
    }
}
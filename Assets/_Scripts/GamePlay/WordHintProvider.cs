using System;
using UnityEngine;

public static class WordHintProvider
{
    public static int CalculateHints(int elapsedSeconds,int totalSeconds,int totalHints)
    {
        var lastHintedIndex = -1; // -1表示没有提示
        int startSecondForHints; // 从第几秒开始提示
        int hintInterval; // 每隔多少秒提示一次
        
        if (totalSeconds > totalHints) // 如果总时间大于提示的次数
        {
            startSecondForHints = totalSeconds / 2;// 从一半开始提示
            hintInterval = (totalSeconds - startSecondForHints) / totalHints;// 每隔多少秒提示一次
        }
        else
        {
            startSecondForHints = 2; // 从第二秒开始提示
            hintInterval = Math.Max(1, (totalSeconds - 1) / totalHints); // 每隔多少秒提示一次
        }

        if (elapsedSeconds < startSecondForHints) // 如果还没到提示的时间
        {
            return lastHintedIndex; // 返回-1
        }

        var currentIndex = (elapsedSeconds - startSecondForHints) / hintInterval; // 当前提示的索引

        if (currentIndex > lastHintedIndex) // 如果当前提示的索引大于上次提示的索引
        {
            lastHintedIndex = currentIndex; // 更新上次提示的索引
        }

        return lastHintedIndex; // 返回当前提示的索引
    }
}

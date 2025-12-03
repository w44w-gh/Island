using System;
using UnityEngine;

/// <summary>
/// 天候の種類
/// </summary>
public enum WeatherType
{
    Sunny,      // 晴れ
    Cloudy,     // 曇り
    Rainy,      // 雨
    Stormy      // 嵐
}

/// <summary>
/// 天候システム
/// 前日の天候に応じて次の日の天候を決定
/// </summary>
public class Weather
{
    private WeatherType currentWeather;
    private System.Random random;

    // 天候遷移の確率テーブル（仮の値、後で調整可能）
    // [前日の天候][次の天候] = 確率(%)
    private static readonly int[,] weatherTransitionTable = new int[,]
    {
        // 前日: 晴れ → 次の日: 晴れ, 曇り, 雨, 嵐
        { 70, 25, 5, 0 },

        // 前日: 曇り → 次の日: 晴れ, 曇り, 雨, 嵐
        { 30, 40, 25, 5 },

        // 前日: 雨 → 次の日: 晴れ, 曇り, 雨, 嵐
        { 20, 50, 25, 5 },

        // 前日: 嵐 → 次の日: 晴れ, 曇り, 雨, 嵐
        { 10, 30, 40, 20 }
    };

    /// <summary>
    /// 現在の天候
    /// </summary>
    public WeatherType CurrentWeather => currentWeather;

    /// <summary>
    /// コンストラクタ（初期天候を指定）
    /// </summary>
    public Weather(WeatherType initialWeather = WeatherType.Sunny, int? seed = null)
    {
        currentWeather = initialWeather;
        random = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
    }

    /// <summary>
    /// 次の日の天候を決定
    /// </summary>
    public void AdvanceToNextDay()
    {
        currentWeather = DetermineNextWeather(currentWeather);
        Debug.Log($"天候更新: {currentWeather.ToJapaneseString()}");
    }

    /// <summary>
    /// 前日の天候から次の日の天候を決定
    /// </summary>
    private WeatherType DetermineNextWeather(WeatherType previousWeather)
    {
        int previousIndex = (int)previousWeather;
        int roll = random.Next(0, 100); // 0-99のランダム値

        int cumulativeProbability = 0;
        for (int i = 0; i < 4; i++)
        {
            cumulativeProbability += weatherTransitionTable[previousIndex, i];
            if (roll < cumulativeProbability)
            {
                return (WeatherType)i;
            }
        }

        // フォールバック（通常ここには到達しない）
        return WeatherType.Sunny;
    }

    /// <summary>
    /// 天候を強制的に設定（デバッグ用）
    /// </summary>
    public void SetWeather(WeatherType weather)
    {
        currentWeather = weather;
        Debug.Log($"天候を強制設定: {currentWeather.ToJapaneseString()}");
    }

    /// <summary>
    /// 天候遷移確率を取得（デバッグ/UI用）
    /// </summary>
    public static int GetTransitionProbability(WeatherType from, WeatherType to)
    {
        return weatherTransitionTable[(int)from, (int)to];
    }
}

/// <summary>
/// WeatherType拡張メソッド
/// </summary>
public static class WeatherTypeExtensions
{
    /// <summary>
    /// 天候を日本語文字列に変換
    /// </summary>
    public static string ToJapaneseString(this WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Sunny:
                return "晴れ";
            case WeatherType.Cloudy:
                return "曇り";
            case WeatherType.Rainy:
                return "雨";
            case WeatherType.Stormy:
                return "嵐";
            default:
                return "不明";
        }
    }

    /// <summary>
    /// 天候の説明を取得
    /// </summary>
    public static string GetDescription(this WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Sunny:
                return "晴れ - 活動しやすい天候";
            case WeatherType.Cloudy:
                return "曇り - 少し動きづらい";
            case WeatherType.Rainy:
                return "雨 - 屋外活動が困難";
            case WeatherType.Stormy:
                return "嵐 - 危険な天候";
            default:
                return "不明";
        }
    }
}

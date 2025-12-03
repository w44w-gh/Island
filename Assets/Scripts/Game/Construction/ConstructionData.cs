using System;
using System.Collections.Generic;

/// <summary>
/// 建築物の定義データ
/// </summary>
[Serializable]
public class ConstructionData
{
    /// <summary>
    /// 建築物ID
    /// </summary>
    public string id;

    /// <summary>
    /// 建築物名
    /// </summary>
    public string name;

    /// <summary>
    /// 説明
    /// </summary>
    public string description;

    /// <summary>
    /// 建築に必要な素材（アイテムID, 必要数）
    /// </summary>
    public Dictionary<string, int> requiredMaterials;

    /// <summary>
    /// 建築に必要な時間（秒）
    /// </summary>
    public int requiredSeconds;

    /// <summary>
    /// 建築場所
    /// </summary>
    public MapLocation location;

    public ConstructionData(
        string id,
        string name,
        string description,
        Dictionary<string, int> requiredMaterials,
        int requiredSeconds,
        MapLocation location)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.requiredMaterials = requiredMaterials;
        this.requiredSeconds = requiredSeconds;
        this.location = location;
    }

    /// <summary>
    /// 必要時間を日時分秒で取得
    /// </summary>
    public string GetRequiredTimeText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(requiredSeconds);
        if (timeSpan.TotalDays >= 1)
        {
            return $"{(int)timeSpan.TotalDays}日{timeSpan.Hours}時間";
        }
        else if (timeSpan.TotalHours >= 1)
        {
            return $"{(int)timeSpan.TotalHours}時間{timeSpan.Minutes}分";
        }
        else
        {
            return $"{timeSpan.Minutes}分";
        }
    }
}

/// <summary>
/// 建築中の状態データ
/// </summary>
[Serializable]
public class OngoingConstruction
{
    /// <summary>
    /// 建築物ID
    /// </summary>
    public string constructionId;

    /// <summary>
    /// 建築開始時刻（UnixTime ミリ秒）
    /// </summary>
    public long startTimeMillis;

    /// <summary>
    /// 建築完了予定時刻（UnixTime ミリ秒）
    /// </summary>
    public long completeTimeMillis;

    public OngoingConstruction(string constructionId, DateTime startTime, int durationSeconds)
    {
        this.constructionId = constructionId;
        this.startTimeMillis = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
        this.completeTimeMillis = this.startTimeMillis + (durationSeconds * 1000L);
    }

    /// <summary>
    /// 建築開始時刻
    /// </summary>
    public DateTime StartTime => DateTimeOffset.FromUnixTimeMilliseconds(startTimeMillis).DateTime;

    /// <summary>
    /// 建築完了時刻
    /// </summary>
    public DateTime CompleteTime => DateTimeOffset.FromUnixTimeMilliseconds(completeTimeMillis).DateTime;

    /// <summary>
    /// 建築が完了しているか
    /// </summary>
    public bool IsCompleted(DateTime currentTime)
    {
        return currentTime >= CompleteTime;
    }

    /// <summary>
    /// 残り時間を取得
    /// </summary>
    public TimeSpan GetRemainingTime(DateTime currentTime)
    {
        if (IsCompleted(currentTime))
        {
            return TimeSpan.Zero;
        }
        return CompleteTime - currentTime;
    }

    /// <summary>
    /// 残り時間をテキストで取得
    /// </summary>
    public string GetRemainingTimeText(DateTime currentTime)
    {
        TimeSpan remaining = GetRemainingTime(currentTime);
        if (remaining.TotalDays >= 1)
        {
            return $"残り{(int)remaining.TotalDays}日{remaining.Hours}時間";
        }
        else if (remaining.TotalHours >= 1)
        {
            return $"残り{(int)remaining.TotalHours}時間{remaining.Minutes}分";
        }
        else if (remaining.TotalMinutes >= 1)
        {
            return $"残り{remaining.Minutes}分";
        }
        else
        {
            return "まもなく完成";
        }
    }

    /// <summary>
    /// 進捗率を取得（0.0～1.0）
    /// </summary>
    public float GetProgress(DateTime currentTime)
    {
        long totalDuration = completeTimeMillis - startTimeMillis;
        long elapsed = new DateTimeOffset(currentTime).ToUnixTimeMilliseconds() - startTimeMillis;
        float progress = (float)elapsed / totalDuration;
        return UnityEngine.Mathf.Clamp01(progress);
    }
}

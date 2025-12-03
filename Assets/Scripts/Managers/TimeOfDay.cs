using System;

/// <summary>
/// 時間帯の種類
/// </summary>
public enum TimeOfDay
{
    EarlyMorning,   // 早朝
    Morning,        // 朝
    Noon,           // 昼
    Evening,        // 夜
    Midnight        // 深夜
}

/// <summary>
/// 時間帯判定のユーティリティクラス
/// </summary>
public static class TimeOfDayUtility
{
    // 時間帯の区切り（時）
    private const int MIDNIGHT_END = 5;          // 深夜終了: 5時
    private const int EARLY_MORNING_START = 5;   // 早朝開始: 5時
    private const int MORNING_START = 8;         // 朝開始: 8時
    private const int NOON_START = 11;           // 昼開始: 11時
    private const int EVENING_START = 17;        // 夜開始: 17時

    /// <summary>
    /// 指定された時刻から時間帯を取得
    /// </summary>
    public static TimeOfDay GetTimeOfDay(DateTime dateTime)
    {
        int hour = dateTime.Hour;

        if (hour < MIDNIGHT_END)
        {
            return TimeOfDay.Midnight;      // 0:00 - 4:59
        }
        else if (hour >= EARLY_MORNING_START && hour < MORNING_START)
        {
            return TimeOfDay.EarlyMorning;  // 5:00 - 7:59
        }
        else if (hour >= MORNING_START && hour < NOON_START)
        {
            return TimeOfDay.Morning;       // 8:00 - 10:59
        }
        else if (hour >= NOON_START && hour < EVENING_START)
        {
            return TimeOfDay.Noon;          // 11:00 - 16:59
        }
        else
        {
            return TimeOfDay.Evening;       // 17:00 - 23:59
        }
    }

    /// <summary>
    /// 時間帯を日本語文字列に変換
    /// </summary>
    public static string ToJapaneseString(this TimeOfDay timeOfDay)
    {
        switch (timeOfDay)
        {
            case TimeOfDay.EarlyMorning:
                return "早朝";
            case TimeOfDay.Morning:
                return "朝";
            case TimeOfDay.Noon:
                return "昼";
            case TimeOfDay.Evening:
                return "夜";
            case TimeOfDay.Midnight:
                return "深夜";
            default:
                return "不明";
        }
    }

    /// <summary>
    /// 時間帯の詳細説明を取得
    /// </summary>
    public static string GetDescription(this TimeOfDay timeOfDay)
    {
        switch (timeOfDay)
        {
            case TimeOfDay.Midnight:
                return "深夜 (0:00 - 4:59)";
            case TimeOfDay.EarlyMorning:
                return "早朝 (5:00 - 7:59)";
            case TimeOfDay.Morning:
                return "朝 (8:00 - 10:59)";
            case TimeOfDay.Noon:
                return "昼 (11:00 - 16:59)";
            case TimeOfDay.Evening:
                return "夜 (17:00 - 23:59)";
            default:
                return "不明";
        }
    }

    /// <summary>
    /// 次の時間帯に切り替わるまでの残り時間を取得
    /// </summary>
    public static TimeSpan GetTimeUntilNextTimeOfDay(DateTime dateTime)
    {
        int hour = dateTime.Hour;
        int nextHour;

        if (hour < EARLY_MORNING_START)
        {
            nextHour = EARLY_MORNING_START;  // 深夜 → 早朝 (5:00)
        }
        else if (hour < MORNING_START)
        {
            nextHour = MORNING_START;        // 早朝 → 朝 (8:00)
        }
        else if (hour < NOON_START)
        {
            nextHour = NOON_START;           // 朝 → 昼 (11:00)
        }
        else if (hour < EVENING_START)
        {
            nextHour = EVENING_START;        // 昼 → 夜 (17:00)
        }
        else
        {
            nextHour = 0;                    // 夜 → 深夜 (0:00, 翌日)
        }

        DateTime nextTime;
        if (nextHour == 0 || nextHour <= hour)
        {
            // 翌日の時間（夜→深夜は必ず翌日）
            nextTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, nextHour, 0, 0).AddDays(1);
        }
        else
        {
            // 今日の時間
            nextTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, nextHour, 0, 0);
        }

        return nextTime - dateTime;
    }
}

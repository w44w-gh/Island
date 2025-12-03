using UnityEngine;

/// <summary>
/// NovelBridge - 天候・時間関連メソッド
/// </summary>
public partial class NovelBridge
{
    // ========== 天候・時間関連 ==========

    /// <summary>
    /// 現在の天候を取得（宴の分岐条件などで使用）
    /// </summary>
    public string GetCurrentWeather()
    {
        if (GameManager.Instance.State == null) return "Unknown";

        return GameManager.Instance.State.CurrentWeather.ToString();
    }

    /// <summary>
    /// 現在の天候（日本語）を取得（宴の表示などで使用）
    /// </summary>
    public string GetCurrentWeatherJapanese()
    {
        if (GameManager.Instance.State == null) return "不明";

        return GameManager.Instance.State.CurrentWeather.ToJapaneseString();
    }

    /// <summary>
    /// 現在の時間帯を取得（宴の分岐条件などで使用）
    /// </summary>
    public string GetCurrentTimeOfDay()
    {
        if (GameManager.Instance.GlobalGameTime == null) return "Unknown";

        return GameManager.Instance.GlobalGameTime.CurrentTimeOfDay.ToString();
    }

    /// <summary>
    /// 現在の時間帯（日本語）を取得（宴の表示などで使用）
    /// </summary>
    public string GetCurrentTimeOfDayJapanese()
    {
        if (GameManager.Instance.GlobalGameTime == null) return "不明";

        return GameManager.Instance.GlobalGameTime.CurrentTimeOfDay.ToJapaneseString();
    }

    /// <summary>
    /// 指定された天候かどうか確認（宴の分岐条件で使用）
    /// weatherType: "Sunny", "Cloudy", "Rainy", "Stormy"
    /// </summary>
    public bool IsWeather(string weatherType)
    {
        if (GameManager.Instance.State == null) return false;

        return GameManager.Instance.State.CurrentWeather.ToString() == weatherType;
    }

    /// <summary>
    /// 指定された時間帯かどうか確認（宴の分岐条件で使用）
    /// timeOfDay: "Midnight", "EarlyMorning", "Morning", "Noon", "Evening"
    /// </summary>
    public bool IsTimeOfDay(string timeOfDay)
    {
        if (GameManager.Instance.GlobalGameTime == null) return false;

        return GameManager.Instance.GlobalGameTime.CurrentTimeOfDay.ToString() == timeOfDay;
    }

    /// <summary>
    /// 晴れかどうか
    /// </summary>
    public bool IsSunny()
    {
        return IsWeather("Sunny");
    }

    /// <summary>
    /// 曇りかどうか
    /// </summary>
    public bool IsCloudy()
    {
        return IsWeather("Cloudy");
    }

    /// <summary>
    /// 雨かどうか
    /// </summary>
    public bool IsRainy()
    {
        return IsWeather("Rainy");
    }

    /// <summary>
    /// 嵐かどうか
    /// </summary>
    public bool IsStormy()
    {
        return IsWeather("Stormy");
    }

    /// <summary>
    /// 深夜かどうか（0:00-4:59）
    /// </summary>
    public bool IsMidnight()
    {
        return IsTimeOfDay("Midnight");
    }

    /// <summary>
    /// 早朝かどうか（5:00-7:59）
    /// </summary>
    public bool IsEarlyMorning()
    {
        return IsTimeOfDay("EarlyMorning");
    }

    /// <summary>
    /// 朝かどうか（8:00-10:59）
    /// </summary>
    public bool IsMorning()
    {
        return IsTimeOfDay("Morning");
    }

    /// <summary>
    /// 昼かどうか（11:00-16:59）
    /// </summary>
    public bool IsNoon()
    {
        return IsTimeOfDay("Noon");
    }

    /// <summary>
    /// 夕方かどうか（17:00-23:59）
    /// </summary>
    public bool IsEvening()
    {
        return IsTimeOfDay("Evening");
    }

    /// <summary>
    /// 指定されたパッケージIDのアプリがインストールされているか確認（宴の分岐条件などで使用）
    /// </summary>
    public bool IsPackageInstalled(string packageId)
    {
        return AndroidPackageChecker.IsPackageInstalled(packageId);
    }

    /// <summary>
    /// 指定されたパッケージIDのアプリを起動（宴のコマンドから呼び出し）
    /// </summary>
    public void LaunchPackage(string packageId)
    {
        bool success = AndroidPackageChecker.LaunchPackage(packageId);
        if (success)
        {
            Debug.Log($"[NovelBridge] パッケージ '{packageId}' を起動しました");
        }
        else
        {
            Debug.LogWarning($"[NovelBridge] パッケージ '{packageId}' の起動に失敗しました");
        }
    }

    /// <summary>
    /// 特定のActivityを起動（宴のコマンドから呼び出し）
    /// </summary>
    public void LaunchActivity(string packageId, string activityClassName)
    {
        bool success = AndroidPackageChecker.LaunchActivity(packageId, activityClassName);
        if (success)
        {
            Debug.Log($"[NovelBridge] Activity '{packageId}/{activityClassName}' を起動しました");
        }
        else
        {
            Debug.LogWarning($"[NovelBridge] Activity '{packageId}/{activityClassName}' の起動に失敗しました");
        }
    }
}

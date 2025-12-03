using System;

/// <summary>
/// ゲームプレイ中の時間ステータス
/// 起動時/復帰時にNTPから時刻を取得し、その後はゲーム内で経過時間を計測
/// </summary>
public class GameTime
{
    private DateTime startTime;      // NTPから取得した基準時刻
    private float elapsedSeconds;    // 基準時刻からの経過時間（秒）

    /// <summary>
    /// コンストラクタ - NTPから現在時刻をキャプチャ
    /// </summary>
    public GameTime()
    {
        Capture();
    }

    /// <summary>
    /// NTPから現在時刻をキャプチャして基準時刻を更新
    /// 起動時・復帰時に呼ぶ
    /// </summary>
    public void Capture()
    {
        if (!NTPTimeManager.Instance.IsReady)
        {
            UnityEngine.Debug.LogWarning("NTP not ready. Using local time.");
            startTime = DateTime.Now;
        }
        else
        {
            startTime = NTPTimeManager.Instance.ServerTime;
        }

        elapsedSeconds = 0f;
        UnityEngine.Debug.Log($"GameTime captured: {startTime:yyyy/MM/dd HH:mm:ss}");
    }

    /// <summary>
    /// 経過時間を更新（毎フレーム呼ぶ）
    /// </summary>
    public void Update(float deltaTime)
    {
        elapsedSeconds += deltaTime;
    }

    /// <summary>
    /// 現在のゲーム内時刻（基準時刻 + 経過時間）
    /// </summary>
    public DateTime CurrentTime
    {
        get
        {
            return startTime.AddSeconds(elapsedSeconds);
        }
    }

    /// <summary>
    /// 現在の時間帯（早朝、朝、昼、夜、深夜）
    /// </summary>
    public TimeOfDay CurrentTimeOfDay
    {
        get
        {
            return TimeOfDayUtility.GetTimeOfDay(CurrentTime);
        }
    }

    /// <summary>
    /// 現在の時間帯の日本語表記
    /// </summary>
    public string CurrentTimeOfDayText
    {
        get
        {
            return CurrentTimeOfDay.ToJapaneseString();
        }
    }

    /// <summary>
    /// 次の時間帯に切り替わるまでの残り時間
    /// </summary>
    public TimeSpan TimeUntilNextTimeOfDay
    {
        get
        {
            return TimeOfDayUtility.GetTimeUntilNextTimeOfDay(CurrentTime);
        }
    }

    /// <summary>
    /// 指定した時間帯かどうかを判定
    /// </summary>
    public bool IsTimeOfDay(TimeOfDay timeOfDay)
    {
        return CurrentTimeOfDay == timeOfDay;
    }

    /// <summary>
    /// 早朝かどうか
    /// </summary>
    public bool IsEarlyMorning => CurrentTimeOfDay == TimeOfDay.EarlyMorning;

    /// <summary>
    /// 朝かどうか
    /// </summary>
    public bool IsMorning => CurrentTimeOfDay == TimeOfDay.Morning;

    /// <summary>
    /// 昼かどうか
    /// </summary>
    public bool IsNoon => CurrentTimeOfDay == TimeOfDay.Noon;

    /// <summary>
    /// 夜かどうか
    /// </summary>
    public bool IsEvening => CurrentTimeOfDay == TimeOfDay.Evening;

    /// <summary>
    /// 深夜かどうか
    /// </summary>
    public bool IsMidnight => CurrentTimeOfDay == TimeOfDay.Midnight;

    /// <summary>
    /// 指定した時刻が現在時刻より前かどうか（過去かどうか）
    /// </summary>
    public bool IsPast(DateTime targetTime)
    {
        return CurrentTime > targetTime;
    }

    /// <summary>
    /// 指定した時刻が現在時刻より後かどうか（未来かどうか）
    /// </summary>
    public bool IsFuture(DateTime targetTime)
    {
        return CurrentTime < targetTime;
    }

    /// <summary>
    /// 指定した時刻までの残り時間を取得
    /// </summary>
    public TimeSpan GetTimeUntil(DateTime targetTime)
    {
        return targetTime - CurrentTime;
    }

    /// <summary>
    /// 今日の日付（年月日のみ）
    /// </summary>
    public DateTime Today
    {
        get
        {
            return CurrentTime.Date;
        }
    }

    /// <summary>
    /// 現在の時刻（時:分:秒）
    /// </summary>
    public TimeSpan CurrentTimeOfClock
    {
        get
        {
            return CurrentTime.TimeOfDay;
        }
    }

    /// <summary>
    /// 時間帯の詳細説明
    /// </summary>
    public string GetTimeOfDayDescription()
    {
        return CurrentTimeOfDay.GetDescription();
    }
}

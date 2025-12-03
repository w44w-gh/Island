using System;
using System.Collections.Generic;

/// <summary>
/// RemoteConfigから読み込むイベント報酬リストのJSON構造
/// </summary>
[Serializable]
public class EventRewardList
{
    public EventRewardData[] events;
}

/// <summary>
/// RemoteConfigから読み込むイベント報酬データのJSON構造
/// </summary>
[Serializable]
public class EventRewardData
{
    public string eventId;
    public string eventName;
    public string ssid;
    public string description;
    public RewardItemData[] rewards;
}

/// <summary>
/// RemoteConfigから読み込む報酬アイテムのJSON構造
/// </summary>
[Serializable]
public class RewardItemData
{
    public string itemId;
    public int quantity;
}

/// <summary>
/// リアルイベント配布報酬の定義
/// 特定のSSIDを検出したら報酬を付与
/// </summary>
[Serializable]
public class EventReward
{
    public string eventId;              // イベントID（一意）
    public string eventName;            // イベント名
    public string ssid;                 // 検出対象のSSID
    public List<RewardItem> rewards;    // 報酬アイテムリスト
    public string description;          // イベント説明

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EventReward(string eventId, string eventName, string ssid, string description = "")
    {
        this.eventId = eventId;
        this.eventName = eventName;
        this.ssid = ssid;
        this.description = description;
        this.rewards = new List<RewardItem>();
    }

    /// <summary>
    /// 報酬アイテムを追加
    /// </summary>
    public void AddReward(string itemId, int quantity)
    {
        rewards.Add(new RewardItem(itemId, quantity));
    }
}

/// <summary>
/// 報酬アイテム
/// </summary>
[Serializable]
public class RewardItem
{
    public string itemId;
    public int quantity;

    public RewardItem(string itemId, int quantity)
    {
        this.itemId = itemId;
        this.quantity = quantity;
    }
}

/// <summary>
/// イベント報酬データベース
/// Firebase RemoteConfigからイベントデータを取得
/// </summary>
public static class EventRewardDatabase
{
    private static List<EventReward> eventRewards;
    private static bool isInitialized = false;

    /// <summary>
    /// データベース初期化（RemoteConfigから読み込み）
    /// </summary>
    public static void Initialize()
    {
        if (isInitialized) return;

        eventRewards = new List<EventReward>();

        // RemoteConfigからイベント報酬データを取得
        if (RemoteConfigManager.Instance != null && RemoteConfigManager.Instance.IsInitialized)
        {
            LoadFromRemoteConfig();
        }
        else
        {
            // RemoteConfigが初期化されていない場合はデフォルト値を使用
            UnityEngine.Debug.LogWarning("EventRewardDatabase: RemoteConfigが初期化されていません。デフォルト値を使用します");
            LoadFromHardcodedData();
        }

        isInitialized = true;
        UnityEngine.Debug.Log($"EventRewardDatabase initialized: {eventRewards.Count} events registered");
    }

    /// <summary>
    /// RemoteConfigからイベントデータを読み込み
    /// </summary>
    private static void LoadFromRemoteConfig()
    {
        try
        {
            string json = RemoteConfigManager.Instance.GetEventRewardsJson();
            EventRewardList rewardList = UnityEngine.JsonUtility.FromJson<EventRewardList>(json);

            if (rewardList != null && rewardList.events != null)
            {
                foreach (var eventData in rewardList.events)
                {
                    var reward = new EventReward(
                        eventData.eventId,
                        eventData.eventName,
                        eventData.ssid,
                        eventData.description
                    );

                    if (eventData.rewards != null)
                    {
                        foreach (var rewardItem in eventData.rewards)
                        {
                            reward.AddReward(rewardItem.itemId, rewardItem.quantity);
                        }
                    }

                    eventRewards.Add(reward);
                }

                UnityEngine.Debug.Log($"EventRewardDatabase: RemoteConfigから{eventRewards.Count}件のイベントを読み込みました");
            }
            else
            {
                UnityEngine.Debug.LogWarning("EventRewardDatabase: RemoteConfigのJSONパースに失敗しました。デフォルト値を使用します");
                LoadFromHardcodedData();
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"EventRewardDatabase: RemoteConfigからの読み込みに失敗しました: {e.Message}。デフォルト値を使用します");
            LoadFromHardcodedData();
        }
    }

    /// <summary>
    /// ハードコードされたデフォルトデータを読み込み（フォールバック用）
    /// </summary>
    private static void LoadFromHardcodedData()
    {
        eventRewards.Clear();

        // デフォルトイベント1: アニメジャパン2025
        var animeJapan = new EventReward(
            "event_animejapan2025",
            "アニメジャパン2025",
            "AnimeJapan2025_Island",
            "アニメジャパン2025会場限定配布"
        );
        animeJapan.AddReward("wood", 100);
        animeJapan.AddReward("stone", 100);
        animeJapan.AddReward("fish", 50);
        eventRewards.Add(animeJapan);

        // デフォルトイベント2: コミケ2025
        var comiket = new EventReward(
            "event_comiket2025",
            "コミケ2025",
            "Comiket2025_Island",
            "コミケ2025会場限定配布"
        );
        comiket.AddReward("berry", 100);
        comiket.AddReward("coconut", 50);
        eventRewards.Add(comiket);

        UnityEngine.Debug.Log($"EventRewardDatabase: デフォルトデータから{eventRewards.Count}件のイベントを読み込みました");
    }

    /// <summary>
    /// RemoteConfigが更新された時にデータベースを再初期化
    /// </summary>
    public static void Reload()
    {
        isInitialized = false;
        Initialize();
    }

    /// <summary>
    /// 全イベント報酬を取得
    /// </summary>
    public static List<EventReward> GetAllRewards()
    {
        if (!isInitialized) Initialize();
        return eventRewards;
    }

    /// <summary>
    /// SSIDからイベント報酬を検索
    /// </summary>
    public static EventReward FindBySSID(string ssid)
    {
        if (!isInitialized) Initialize();

        foreach (var reward in eventRewards)
        {
            if (reward.ssid.Equals(ssid, StringComparison.OrdinalIgnoreCase))
            {
                return reward;
            }
        }

        return null;
    }

    /// <summary>
    /// イベントIDからイベント報酬を取得
    /// </summary>
    public static EventReward GetById(string eventId)
    {
        if (!isInitialized) Initialize();

        foreach (var reward in eventRewards)
        {
            if (reward.eventId == eventId)
            {
                return reward;
            }
        }

        return null;
    }
}

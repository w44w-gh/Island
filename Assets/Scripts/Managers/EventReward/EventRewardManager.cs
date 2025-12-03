using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// イベント報酬管理クラス（Singleton）
/// リアルイベント会場でのWiFiスキャンによる限定配布システム
/// </summary>
public class EventRewardManager : MonoBehaviour
{
    private static EventRewardManager _instance;
    public static EventRewardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("EventRewardManager");
                _instance = go.AddComponent<EventRewardManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private WiFiScanManager wifiScanner;
    private HashSet<string> claimedEventIds; // 取得済みイベントID

    // イベント
    public Action<EventReward> OnRewardClaimed;         // 報酬取得時
    public Action<string> OnScanComplete;               // スキャン完了時（メッセージ）
    public Action<string> OnError;                      // エラー時

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        wifiScanner = new WiFiScanManager();
        claimedEventIds = new HashSet<string>();

        // イベント報酬データベース初期化
        EventRewardDatabase.Initialize();

        Debug.Log("EventRewardManager initialized");
    }

    /// <summary>
    /// 取得済みイベントIDをロード
    /// </summary>
    public void LoadClaimedEvents(List<string> eventIds)
    {
        claimedEventIds.Clear();
        if (eventIds != null)
        {
            foreach (var id in eventIds)
            {
                claimedEventIds.Add(id);
            }
        }
        Debug.Log($"取得済みイベント: {claimedEventIds.Count}件");
    }

    /// <summary>
    /// 取得済みイベントIDを取得
    /// </summary>
    public List<string> GetClaimedEventIds()
    {
        return new List<string>(claimedEventIds);
    }

    /// <summary>
    /// イベントスキャンを開始（ユーザーがボタンを押した時）
    /// </summary>
    public void StartEventScan()
    {
        Debug.Log("イベントスキャン開始");

        // 1. WiFiが有効かチェック
        if (!wifiScanner.IsWiFiEnabled())
        {
            Debug.LogWarning("WiFiが無効です");
            ShowWiFiDisabledDialog();
            return;
        }

        // 2. パーミッションチェック
        if (!wifiScanner.HasLocationPermission())
        {
            Debug.Log("位置情報パーミッションを要求します");
            ShowPermissionRequestDialog();
            return;
        }

        // 3. スキャン実行
        ExecuteScan();
    }

    /// <summary>
    /// WiFi無効ダイアログ表示
    /// </summary>
    private void ShowWiFiDisabledDialog()
    {
        // TODO: UI実装時に差し替え
        Debug.Log("[ダイアログ] WiFiが無効です。設定画面を開きますか？");

        // 仮の処理：自動で設定画面を開く
        wifiScanner.OpenWiFiSettings();
        OnError?.Invoke("WiFiが無効です。設定画面からWiFiを有効にしてください。");
    }

    /// <summary>
    /// パーミッション要求ダイアログ表示
    /// </summary>
    private void ShowPermissionRequestDialog()
    {
        // TODO: UI実装時に差し替え
        Debug.Log("[ダイアログ] イベント報酬を受け取るには位置情報の許可が必要です");

        // パーミッション要求
        wifiScanner.RequestLocationPermission((granted) =>
        {
            if (granted)
            {
                Debug.Log("パーミッション許可されました");
                ExecuteScan();
            }
            else
            {
                Debug.LogWarning("パーミッションが拒否されました");
                ShowPermissionDeniedDialog();
            }
        });
    }

    /// <summary>
    /// パーミッション拒否ダイアログ表示
    /// </summary>
    private void ShowPermissionDeniedDialog()
    {
        // TODO: UI実装時に差し替え
        Debug.Log("[ダイアログ] 位置情報の許可が拒否されました。イベント報酬を受け取るには設定から許可してください。");
        OnError?.Invoke("位置情報の許可が必要です。設定から許可してください。");
    }

    /// <summary>
    /// スキャン実行
    /// </summary>
    private void ExecuteScan()
    {
        Debug.Log("WiFiスキャン実行中...");

        wifiScanner.OnScanComplete = OnWiFiScanComplete;
        wifiScanner.OnScanError = (error) =>
        {
            Debug.LogError($"スキャンエラー: {error}");
            OnError?.Invoke($"スキャンエラー: {error}");
        };

        wifiScanner.ScanWiFiNetworks();
    }

    /// <summary>
    /// WiFiスキャン完了時の処理
    /// </summary>
    private void OnWiFiScanComplete(List<string> ssidList)
    {
        Debug.Log($"WiFiスキャン完了: {ssidList.Count}件のネットワークを検出");

        bool foundNewReward = false;

        // 検出されたSSIDをチェック
        foreach (string ssid in ssidList)
        {
            EventReward reward = EventRewardDatabase.FindBySSID(ssid);

            if (reward != null)
            {
                // 既に取得済みかチェック
                if (claimedEventIds.Contains(reward.eventId))
                {
                    Debug.Log($"イベント「{reward.eventName}」は既に取得済みです");
                    continue;
                }

                // 報酬付与
                GrantReward(reward);
                foundNewReward = true;
            }
        }

        // 結果表示
        if (foundNewReward)
        {
            OnScanComplete?.Invoke("イベント報酬を取得しました！");
        }
        else
        {
            OnScanComplete?.Invoke("イベント報酬は見つかりませんでした。");
        }
    }

    /// <summary>
    /// 報酬を付与
    /// </summary>
    private void GrantReward(EventReward reward)
    {
        Debug.Log($"イベント報酬取得: {reward.eventName}");

        // 取得済みフラグを立てる
        claimedEventIds.Add(reward.eventId);

        // GameStateにアイテムを追加（実際のゲームロジックと連携）
        GameState gameState = GameManager.Instance?.GetGameState();
        if (gameState != null)
        {
            foreach (var item in reward.rewards)
            {
                Item itemData = ItemDatabase.GetItem(item.itemId);
                if (itemData != null)
                {
                    gameState.Inventory.AddItem(itemData, item.quantity);
                    Debug.Log($"アイテム追加: {item.itemId} x{item.quantity}");
                }
                else
                {
                    Debug.LogError($"Item '{item.itemId}' not found in ItemDatabase");
                }
            }
        }
        else
        {
            Debug.LogWarning("GameStateが取得できませんでした");
        }

        // イベント発火
        OnRewardClaimed?.Invoke(reward);

        // セーブデータに保存
        SaveClaimedEvents();
    }

    /// <summary>
    /// 取得済みイベントをセーブ
    /// </summary>
    private void SaveClaimedEvents()
    {
        // GameManager経由でセーブデータに保存
        // GameState.ToSaveData()内でGetClaimedEventIds()が呼ばれ、SaveDataに含まれる
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.SaveGame();
            Debug.Log($"取得済みイベントを保存: {claimedEventIds.Count}件");
        }
        else
        {
            Debug.LogWarning("GameManagerが見つかりません。イベント報酬の保存に失敗しました。");
        }
    }

    /// <summary>
    /// イベントが取得済みかチェック
    /// </summary>
    public bool IsEventClaimed(string eventId)
    {
        return claimedEventIds.Contains(eventId);
    }

    /// <summary>
    /// 取得可能なイベント一覧を取得
    /// </summary>
    public List<EventReward> GetAvailableEvents()
    {
        List<EventReward> available = new List<EventReward>();
        foreach (var reward in EventRewardDatabase.GetAllRewards())
        {
            if (!claimedEventIds.Contains(reward.eventId))
            {
                available.Add(reward);
            }
        }
        return available;
    }

    /// <summary>
    /// 取得済みイベント一覧を取得
    /// </summary>
    public List<EventReward> GetClaimedEvents()
    {
        List<EventReward> claimed = new List<EventReward>();
        foreach (var eventId in claimedEventIds)
        {
            EventReward reward = EventRewardDatabase.GetById(eventId);
            if (reward != null)
            {
                claimed.Add(reward);
            }
        }
        return claimed;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID || UNITY_IOS
using Firebase;
using Firebase.RemoteConfig;
using Firebase.Extensions;
#endif

/// <summary>
/// Firebase RemoteConfigの管理クラス（Singleton）
/// ゲーム起動時にサーバーから設定を取得
/// </summary>
public class RemoteConfigManager : MonoBehaviour
{
    private static RemoteConfigManager _instance;
    public static RemoteConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("RemoteConfigManager");
                _instance = go.AddComponent<RemoteConfigManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // RemoteConfigのキー定義
    public const string KEY_EVENT_REWARDS = "event_rewards";

    // 初期化完了フラグ
    private bool isInitialized = false;
    private bool isFetching = false;

    // イベント
    public Action OnConfigFetched;      // 設定取得完了時
    public Action<string> OnConfigError; // エラー時

    /// <summary>
    /// 初期化完了しているか
    /// </summary>
    public bool IsInitialized => isInitialized;

    /// <summary>
    /// フェッチ中かどうか
    /// </summary>
    public bool IsFetching => isFetching;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Firebase RemoteConfigを初期化してフェッチ
    /// </summary>
    public void InitializeAndFetch()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (isFetching)
        {
            Debug.LogWarning("RemoteConfig: 既にフェッチ中です");
            return;
        }

        isFetching = true;
        Debug.Log("RemoteConfig: Firebase初期化を開始");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("RemoteConfig: Firebase初期化成功");
                FetchRemoteConfig();
            }
            else
            {
                Debug.LogError($"RemoteConfig: Firebaseの初期化に失敗しました: {dependencyStatus}");
                isFetching = false;
                isInitialized = false;
                OnConfigError?.Invoke($"Firebase初期化失敗: {dependencyStatus}");
            }
        });
#else
        Debug.LogWarning("RemoteConfig: Firebase RemoteConfigはAndroid/iOSビルドでのみ動作します（エディタでは動作しません）");

        // エディタではデフォルト値を使用
        isInitialized = true;
        isFetching = false;
        OnConfigFetched?.Invoke();
#endif
    }

#if UNITY_ANDROID || UNITY_IOS
    /// <summary>
    /// RemoteConfigをフェッチして有効化
    /// </summary>
    private void FetchRemoteConfig()
    {
        Debug.Log("RemoteConfig: データフェッチ開始");

        // デフォルト値を設定
        var defaults = new Dictionary<string, object>
        {
            { KEY_EVENT_REWARDS, GetDefaultEventRewardsJson() }
        };

        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(defaultsTask =>
        {
            if (defaultsTask.IsCompleted)
            {
                Debug.Log("RemoteConfig: デフォルト値設定完了");

                // サーバーからフェッチ（開発中は0秒、本番では3600秒などに設定）
                var fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);

                fetchTask.ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("RemoteConfig: フェッチ完了");

                        // フェッチしたデータを有効化
                        var activateTask = FirebaseRemoteConfig.DefaultInstance.ActivateAsync();

                        activateTask.ContinueWithOnMainThread(activateTask =>
                        {
                            if (activateTask.IsCompleted)
                            {
                                Debug.Log("RemoteConfig: アクティベート完了");
                                isInitialized = true;
                                isFetching = false;
                                OnConfigFetched?.Invoke();
                            }
                            else
                            {
                                Debug.LogError("RemoteConfig: アクティベート失敗");
                                isFetching = false;
                                OnConfigError?.Invoke("アクティベート失敗");
                            }
                        });
                    }
                    else
                    {
                        Debug.LogError("RemoteConfig: フェッチ失敗");
                        isFetching = false;
                        OnConfigError?.Invoke("フェッチ失敗");
                    }
                });
            }
            else
            {
                Debug.LogError("RemoteConfig: デフォルト値設定失敗");
                isFetching = false;
                OnConfigError?.Invoke("デフォルト値設定失敗");
            }
        });
    }
#endif

    /// <summary>
    /// デフォルトのイベント報酬JSON（フォールバック用）
    /// </summary>
    private string GetDefaultEventRewardsJson()
    {
        // JSONフォーマット:
        // {
        //   "events": [
        //     {
        //       "eventId": "event_animejapan2025",
        //       "eventName": "アニメジャパン2025",
        //       "ssid": "AnimeJapan2025_Island",
        //       "description": "アニメジャパン2025会場限定配布",
        //       "rewards": [
        //         { "itemId": "wood", "quantity": 100 },
        //         { "itemId": "stone", "quantity": 100 },
        //         { "itemId": "fish", "quantity": 50 }
        //       ]
        //     }
        //   ]
        // }

        return @"{
  ""events"": [
    {
      ""eventId"": ""event_animejapan2025"",
      ""eventName"": ""アニメジャパン2025"",
      ""ssid"": ""AnimeJapan2025_Island"",
      ""description"": ""アニメジャパン2025会場限定配布"",
      ""rewards"": [
        { ""itemId"": ""wood"", ""quantity"": 100 },
        { ""itemId"": ""stone"", ""quantity"": 100 },
        { ""itemId"": ""fish"", ""quantity"": 50 }
      ]
    },
    {
      ""eventId"": ""event_comiket2025"",
      ""eventName"": ""コミケ2025"",
      ""ssid"": ""Comiket2025_Island"",
      ""description"": ""コミケ2025会場限定配布"",
      ""rewards"": [
        { ""itemId"": ""berry"", ""quantity"": 100 },
        { ""itemId"": ""coconut"", ""quantity"": 50 }
      ]
    }
  ]
}";
    }

    /// <summary>
    /// イベント報酬のJSON文字列を取得
    /// </summary>
    public string GetEventRewardsJson()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!isInitialized)
        {
            Debug.LogWarning("RemoteConfig: 初期化が完了していません。デフォルト値を返します");
            return GetDefaultEventRewardsJson();
        }

        try
        {
            string json = FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_EVENT_REWARDS).StringValue;

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("RemoteConfig: イベント報酬データが空です。デフォルト値を返します");
                return GetDefaultEventRewardsJson();
            }

            Debug.Log($"RemoteConfig: イベント報酬データ取得成功 ({json.Length}文字)");
            return json;
        }
        catch (Exception e)
        {
            Debug.LogError($"RemoteConfig: イベント報酬データの取得に失敗しました: {e.Message}");
            return GetDefaultEventRewardsJson();
        }
#else
        // エディタではデフォルト値を返す
        return GetDefaultEventRewardsJson();
#endif
    }

    /// <summary>
    /// 文字列値を取得
    /// </summary>
    public string GetString(string key, string defaultValue = "")
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!isInitialized)
        {
            Debug.LogWarning($"RemoteConfig: 初期化が完了していません。デフォルト値を返します: {key}");
            return defaultValue;
        }

        return FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
#else
        return defaultValue;
#endif
    }

    /// <summary>
    /// 整数値を取得
    /// </summary>
    public long GetLong(string key, long defaultValue = 0)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!isInitialized)
        {
            Debug.LogWarning($"RemoteConfig: 初期化が完了していません。デフォルト値を返します: {key}");
            return defaultValue;
        }

        return FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
#else
        return defaultValue;
#endif
    }

    /// <summary>
    /// 真偽値を取得
    /// </summary>
    public bool GetBool(string key, bool defaultValue = false)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!isInitialized)
        {
            Debug.LogWarning($"RemoteConfig: 初期化が完了していません。デフォルト値を返します: {key}");
            return defaultValue;
        }

        return FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
#else
        return defaultValue;
#endif
    }

    /// <summary>
    /// 浮動小数点値を取得
    /// </summary>
    public double GetDouble(string key, double defaultValue = 0.0)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!isInitialized)
        {
            Debug.LogWarning($"RemoteConfig: 初期化が完了していません。デフォルト値を返します: {key}");
            return defaultValue;
        }

        return FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue;
#else
        return defaultValue;
#endif
    }
}

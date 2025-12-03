using UnityEngine;

/// <summary>
/// ゲーム全体を管理するシングルトンマネージャー
/// シーン間で永続化され、アプリのライフサイクルを監視する
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();
            }
            return _instance;
        }
    }

    // グローバルなゲーム時間（全シーンで共有）
    private GameTime globalGameTime;
    public GameTime GlobalGameTime => globalGameTime;

    // ゲーム状態（日数、天候など）
    private GameState gameState;
    public GameState State => gameState;

    // 時間帯変化イベント
    public event System.Action<TimeOfDay, TimeOfDay> OnTimeOfDayChanged;
    private TimeOfDay previousTimeOfDay;

    private void Awake()
    {
        // シングルトンパターンの実装
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // NTP時刻同期を初期化
        InitializeTimeSync();
    }

    private bool isRefreshingTime = false;  // 時刻同期中フラグ
    private long initialSavedTimestamp = 0; // 起動時のセーブデータタイムスタンプ（オフライン処理用）

    private void Update()
    {
        if (globalGameTime == null) return;

        // グローバルなゲーム時間を更新
        globalGameTime.Update(Time.deltaTime);

        // 時間帯の変化を監視
        TimeOfDay current = globalGameTime.CurrentTimeOfDay;
        if (current != previousTimeOfDay)
        {
            Debug.Log($"[GameManager] 時間帯変化: {previousTimeOfDay.ToJapaneseString()} → {current.ToJapaneseString()}");
            OnTimeOfDayChanged?.Invoke(previousTimeOfDay, current);
            previousTimeOfDay = current;
        }

        // 建築完了チェック（1秒に1回程度で十分）
        if (gameState != null && Time.frameCount % 60 == 0)
        {
            gameState.CheckConstructionCompletion();
        }
    }

    /// <summary>
    /// アプリがバックグラウンドから復帰した際に呼ばれる
    /// </summary>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // アプリが一時停止された（画面OFFになった）
            Debug.Log("App paused - Auto saving");
            AutoSave();
        }
        else
        {
            // アプリが再開された（画面ONになった）
            Debug.Log("App resumed - Refreshing server time");
            RefreshTimeWithFade();
        }
    }

    /// <summary>
    /// アプリのフォーカス状態が変化した際に呼ばれる
    /// </summary>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // アプリがフォーカスを失った
            Debug.Log("App lost focus - Auto saving");
            AutoSave();
        }
        else
        {
            Debug.Log("App gained focus - Refreshing server time");
            RefreshTimeWithFade();
        }
    }

    /// <summary>
    /// 画面をフェードして時刻同期（復帰時）
    /// </summary>
    private void RefreshTimeWithFade()
    {
        // 既に同期中の場合はスキップ
        if (isRefreshingTime)
        {
            Debug.Log("Time sync already in progress, skipping");
            return;
        }

        isRefreshingTime = true;

        // DayTransitionUIを使って復帰処理
        if (DayTransitionManager.Instance.IsReady() && gameState != null)
        {
            DayTransitionManager.Instance.ShowWithInitialization(
                gameState,
                async () =>
                {
                    // NTP同期実行
                    bool success = await NTPTimeManager.Instance.RefreshServerTimeAsync();

                    if (success)
                    {
                        Debug.Log("Time sync completed successfully on resume");

                        // ゲーム時間を再キャプチャ（復帰時に時刻を更新）
                        globalGameTime?.Capture();
                    }
                    else
                    {
                        Debug.LogWarning("Time sync failed on resume");
                    }

                    isRefreshingTime = false;
                }
            );
        }
        else
        {
            // DayTransitionUIが使えない場合は従来通りSceneFadeManagerを使用
            _ = RefreshTimeWithSceneFadeAsync();
        }
    }

    /// <summary>
    /// SceneFadeManagerを使った復帰処理（フォールバック）
    /// </summary>
    private async System.Threading.Tasks.Task RefreshTimeWithSceneFadeAsync()
    {
        // 画面を暗くする
        SceneFadeManager.Instance.FadeIn();

        // NTP同期実行
        bool success = await NTPTimeManager.Instance.RefreshServerTimeAsync();

        if (success)
        {
            Debug.Log("Time sync completed successfully on resume");

            // ゲーム時間を再キャプチャ
            globalGameTime?.Capture();
        }
        else
        {
            Debug.LogWarning("Time sync failed on resume");
        }

        // 少し待ってから画面を明るくする
        await System.Threading.Tasks.Task.Delay(300);

        // 画面を明るくする
        SceneFadeManager.Instance.FadeOut();

        isRefreshingTime = false;
    }

    private void InitializeTimeSync()
    {
        // アイテムデータベースを初期化（静的データ）
        ItemDatabase.Initialize();

        // 建築データベースを初期化（静的データ）
        ConstructionDatabase.Initialize();

        // イベントデータベースを初期化（静的データ）
        EventDatabase.Initialize();

        // グローバルなゲーム時間を初期化（仮の時刻で開始、NTP同期後に更新）
        globalGameTime = new GameTime();
        previousTimeOfDay = globalGameTime.CurrentTimeOfDay;

        // セーブデータがあればロード、なければ新規作成
        if (SaveManager.Instance.HasSaveData())
        {
            Debug.Log("セーブデータを検出しました。ロード中...");
            SaveData saveData = SaveManager.Instance.LoadGame();

            if (saveData != null)
            {
                // タイムスタンプを保存（オフライン処理用）
                initialSavedTimestamp = saveData.savedTimestamp;

                // ゲーム状態を初期化してからセーブデータを復元
                gameState = new GameState(globalGameTime, WeatherType.Sunny);
                gameState.LoadFromSaveData(saveData);
                Debug.Log($"セーブデータからゲームを復元: {gameState.GetSummary()}");
            }
            else
            {
                // ロード失敗 - 新規ゲーム開始
                Debug.LogWarning("セーブデータの読み込みに失敗しました。新規ゲームを開始します。");
                gameState = new GameState(globalGameTime, WeatherType.Sunny);
            }
        }
        else
        {
            // セーブデータなし - 新規ゲーム開始
            Debug.Log("セーブデータが見つかりません。新規ゲームを開始します。");
            gameState = new GameState(globalGameTime, WeatherType.Sunny);
        }

        Debug.Log($"GameState initialized - {gameState.GetSummary()}");

        // DayTransitionUIを表示しながらNTP同期とRemoteConfigを初期化
        ShowInitializationScreen();
    }

    /// <summary>
    /// 初期化画面を表示してNTP同期とRemoteConfigをフェッチ
    /// </summary>
    private void ShowInitializationScreen()
    {
        if (DayTransitionManager.Instance.IsReady())
        {
            // DayTransitionUIを表示しながら初期化
            DayTransitionManager.Instance.ShowWithInitialization(
                gameState.CurrentDay,
                gameState.Time.CurrentTimeOfDay,
                gameState.CurrentWeather,
                async () =>
                {
                    // NTP時刻同期
                    Debug.Log("Initializing NTP time sync...");
                    await NTPTimeManager.Instance.Initialize();
                    Debug.Log("NTP time sync initialized");

                    // ゲーム時間を再キャプチャ（NTP同期後）
                    globalGameTime?.Capture();

                    // オフライン経過時間を処理（起動時のみ）
                    if (initialSavedTimestamp > 0)
                    {
                        gameState.ProcessOfflineTime(initialSavedTimestamp);
                    }

                    // Firebase RemoteConfigを初期化とフェッチ
                    Debug.Log("Initializing Firebase RemoteConfig...");
                    await InitializeRemoteConfigAsync();
                    Debug.Log("Firebase RemoteConfig initialized");
                }
            );
        }
        else
        {
            Debug.LogWarning("DayTransitionUI is not ready. Initializing without UI...");
            // UIなしで初期化
            _ = InitializeWithoutUIAsync();
        }
    }

    /// <summary>
    /// UIなしで初期化（フォールバック）
    /// </summary>
    private async System.Threading.Tasks.Task InitializeWithoutUIAsync()
    {
        // NTP時刻同期
        Debug.Log("Initializing NTP time sync...");
        await NTPTimeManager.Instance.Initialize();
        Debug.Log("NTP time sync initialized");

        // ゲーム時間を再キャプチャ
        globalGameTime?.Capture();

        // オフライン経過時間を処理（起動時のみ）
        if (initialSavedTimestamp > 0)
        {
            gameState.ProcessOfflineTime(initialSavedTimestamp);
        }

        // Firebase RemoteConfigを初期化とフェッチ
        Debug.Log("Initializing Firebase RemoteConfig...");
        await InitializeRemoteConfigAsync();
        Debug.Log("Firebase RemoteConfig initialized");
    }

    /// <summary>
    /// オートセーブ
    /// </summary>
    private void AutoSave()
    {
        if (gameState == null)
        {
            Debug.LogWarning("GameStateが初期化されていないため、セーブをスキップします");
            return;
        }

        SaveData saveData = gameState.ToSaveData();
        bool success = SaveManager.Instance.SaveGame(saveData);

        if (success)
        {
            Debug.Log("オートセーブ完了");
        }
        else
        {
            Debug.LogError("オートセーブ失敗");
        }
    }

    /// <summary>
    /// Firebase RemoteConfigを初期化（非同期）
    /// </summary>
    private async System.Threading.Tasks.Task InitializeRemoteConfigAsync()
    {
        // RemoteConfigManagerのインスタンスを取得
        var remoteConfigManager = RemoteConfigManager.Instance;

        // 完了フラグ
        bool isCompleted = false;
        bool hasError = false;

        // 設定取得完了時のコールバック
        System.Action onFetchedHandler = null;
        System.Action<string> onErrorHandler = null;

        onFetchedHandler = () =>
        {
            Debug.Log("RemoteConfig: 設定取得完了");

            // EventRewardDatabaseをリロード（RemoteConfigから最新データを取得）
            EventRewardDatabase.Reload();

            isCompleted = true;

            // イベントハンドラを解除
            remoteConfigManager.OnConfigFetched -= onFetchedHandler;
            remoteConfigManager.OnConfigError -= onErrorHandler;
        };

        onErrorHandler = (error) =>
        {
            Debug.LogWarning($"RemoteConfig: エラーが発生しました - {error}");
            Debug.LogWarning("RemoteConfig: デフォルト値を使用してゲームを続行します");

            // エラーが発生してもデフォルト値でEventRewardDatabaseを初期化
            EventRewardDatabase.Reload();

            hasError = true;
            isCompleted = true;

            // イベントハンドラを解除
            remoteConfigManager.OnConfigFetched -= onFetchedHandler;
            remoteConfigManager.OnConfigError -= onErrorHandler;
        };

        remoteConfigManager.OnConfigFetched += onFetchedHandler;
        remoteConfigManager.OnConfigError += onErrorHandler;

        // RemoteConfigの初期化とフェッチを開始
        remoteConfigManager.InitializeAndFetch();

        // 完了を待つ（最大10秒）
        float timeout = 10f;
        float elapsed = 0f;
        while (!isCompleted && elapsed < timeout)
        {
            await System.Threading.Tasks.Task.Delay(100);
            elapsed += 0.1f;
        }

        if (!isCompleted)
        {
            Debug.LogWarning("RemoteConfig: タイムアウトしました。デフォルト値を使用します");
            EventRewardDatabase.Reload();

            // イベントハンドラを解除
            remoteConfigManager.OnConfigFetched -= onFetchedHandler;
            remoteConfigManager.OnConfigError -= onErrorHandler;
        }
    }

    /// <summary>
    /// ゲームデータを保存（公開メソッド）
    /// </summary>
    public void SaveGame()
    {
        AutoSave();
    }

    /// <summary>
    /// GameStateを取得（公開メソッド）
    /// </summary>
    public GameState GetGameState()
    {
        return gameState;
    }
}

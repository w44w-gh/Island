using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ゲームシーンを制御するクラス
/// </summary>
public class GameSceneController : MonoBehaviour
{
    public static GameSceneController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI serverTimeText;
    [SerializeField] private TextMeshProUGUI timeOfDayText;
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI playerStatusText;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Button settingsButton;

    // ゲーム時間ステータス（グローバルを使用）
    private GameTime gameTime => GameManager.Instance.GlobalGameTime;

    // 現在の場所（場所によってBGMを変更）
    private string currentLocation = "default";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // NTP同期チェック（念のため）
        if (!NTPTimeManager.Instance.IsReady || gameTime == null)
        {
            Debug.LogError("GameScene started without server time sync! Returning to title.");
            ShowError();
            return;
        }

        Debug.Log($"GameScene started at game time: {gameTime.CurrentTime}");
        Debug.Log($"Current time of day: {gameTime.CurrentTimeOfDayText}");

        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);
        }

        // ゲームBGMを再生（時間帯に応じたBGM）
        PlayBGMForTimeOfDay(gameTime.CurrentTimeOfDay);

        // 時間帯変化イベントを購読
        GameManager.Instance.OnTimeOfDayChanged += OnTimeOfDayChanged;

        // 日付変化イベントを購読
        if (GameManager.Instance.State != null)
        {
            GameManager.Instance.State.OnDayChanged += OnDayChanged;

            // プレイヤーステータス変化イベントを購読
            GameManager.Instance.State.Player.OnHPChanged += OnHPChanged;
            GameManager.Instance.State.Player.OnStaminaChanged += OnStaminaChanged;
            GameManager.Instance.State.Player.OnDeath += OnPlayerDeath;
        }

        // 設定ボタンのリスナーを設定
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }
    }

    private void OnDestroy()
    {
        // イベント購読解除
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimeOfDayChanged -= OnTimeOfDayChanged;

            if (GameManager.Instance.State != null)
            {
                GameManager.Instance.State.OnDayChanged -= OnDayChanged;

                if (GameManager.Instance.State.Player != null)
                {
                    GameManager.Instance.State.Player.OnHPChanged -= OnHPChanged;
                    GameManager.Instance.State.Player.OnStaminaChanged -= OnStaminaChanged;
                    GameManager.Instance.State.Player.OnDeath -= OnPlayerDeath;
                }
            }
        }

        // ボタンリスナーを解除
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
        }
    }

    private void Update()
    {
        if (gameTime == null) return;

        // サーバー時刻を表示（デバッグ用）
        if (serverTimeText != null)
        {
            serverTimeText.text = $"サーバー時刻:\n{gameTime.CurrentTime:yyyy/MM/dd HH:mm:ss}";
        }

        // 時間帯を表示
        if (timeOfDayText != null)
        {
            TimeOfDay timeOfDay = gameTime.CurrentTimeOfDay;
            TimeSpan remaining = gameTime.TimeUntilNextTimeOfDay;

            timeOfDayText.text = $"時間帯: {timeOfDay.ToJapaneseString()}\n" +
                                 $"{timeOfDay.GetDescription()}\n" +
                                 $"次の時間帯まで: {remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
        }

        // ゲーム状態を表示
        if (gameStateText != null && GameManager.Instance.State != null)
        {
            GameState state = GameManager.Instance.State;
            gameStateText.text = $"無人島サバイバル\n" +
                                 $"{state.GetDayText()}\n" +
                                 $"天候: {state.GetWeatherText()}\n" +
                                 $"{state.CurrentWeather.GetDescription()}";
        }

        // プレイヤーステータスを表示
        if (playerStatusText != null && GameManager.Instance.State != null)
        {
            PlayerStatus player = GameManager.Instance.State.Player;
            playerStatusText.text = $"【主人公】\n" +
                                    $"状態: {player.GetConditionText()}\n" +
                                    $"HP: {player.CurrentHP}/{player.MaxHP}\n" +
                                    $"スタミナ: {player.CurrentStamina}/{player.MaxStamina}\n" +
                                    $"疲労度: {player.Fatigue}%";
        }
    }

    /// <summary>
    /// 時間帯が変化した時の処理（GameManagerから通知）
    /// </summary>
    private void OnTimeOfDayChanged(TimeOfDay previous, TimeOfDay current)
    {
        Debug.Log($"[GameScene] 時間帯変化: {previous.ToJapaneseString()} → {current.ToJapaneseString()}");

        // 時間帯に応じてBGMを変更
        PlayBGMForTimeOfDay(current);

        // 深夜になったら日付を進める例
        if (current == TimeOfDay.Midnight && previous != TimeOfDay.Midnight)
        {
            GameManager.Instance.State?.AdvanceDay();
        }
    }

    /// <summary>
    /// 時間帯に応じたBGMを再生（天候も考慮）
    /// </summary>
    private void PlayBGMForTimeOfDay(TimeOfDay timeOfDay)
    {
        // 天候を確認（雨や嵐の場合はBGMなし）
        if (GameManager.Instance.State != null)
        {
            WeatherType weather = GameManager.Instance.State.CurrentWeather;

            if (weather == WeatherType.Rainy || weather == WeatherType.Stormy)
            {
                // 雨や嵐の時はBGMを停止
                AudioManager.Instance.StopBGM(2f);
                Debug.Log($"[GameScene] BGM stopped due to weather: {weather.ToJapaneseString()}");
                return;
            }
        }

        // 場所と時間帯に応じたBGMを取得
        string bgmName = GetBGMForLocation(currentLocation, timeOfDay);

        AudioManager.Instance.PlayBGM(bgmName, 2f);
        Debug.Log($"[GameScene] BGM changed to: {bgmName} (Location: {currentLocation}, Time: {timeOfDay.ToJapaneseString()})");
    }

    /// <summary>
    /// 場所と時間帯に応じたBGM名を取得
    /// </summary>
    private string GetBGMForLocation(string location, TimeOfDay timeOfDay)
    {
        // 場所別のBGMがある場合は優先
        switch (location)
        {
            case "beach":
                // ビーチは常に同じBGM（波の音などがある想定）
                return "game_beach";

            case "forest":
                // 森は時間帯によって変わる
                return timeOfDay == TimeOfDay.Midnight || timeOfDay == TimeOfDay.Evening
                    ? "game_forest_night"
                    : "game_forest_day";

            case "cave":
                // 洞窟は常に同じBGM（暗い雰囲気）
                return "game_cave";

            case "camp":
                // キャンプ地は時間帯によって変わる
                return timeOfDay == TimeOfDay.Midnight || timeOfDay == TimeOfDay.Evening
                    ? "game_camp_night"
                    : "game_camp_day";

            case "default":
            default:
                // デフォルト（島の一般エリア）- 時間帯に応じたBGM
                switch (timeOfDay)
                {
                    case TimeOfDay.EarlyMorning:
                        return "game_early_morning";
                    case TimeOfDay.Morning:
                        return "game_morning";
                    case TimeOfDay.Noon:
                        return "game_noon";
                    case TimeOfDay.Evening:
                        return "game_evening";
                    case TimeOfDay.Midnight:
                        return "game_midnight";
                    default:
                        return "game_default";
                }
        }
    }

    /// <summary>
    /// 場所を変更してBGMを切り替える（MapLocation版）
    /// </summary>
    public void SetLocation(MapLocation location)
    {
        string locationName = location switch
        {
            MapLocation.Beach => "beach",
            MapLocation.Forest => "forest",
            MapLocation.Mountain => "cave",  // 山は洞窟扱い
            MapLocation.River => "default",
            _ => "default"
        };
        SetLocation(locationName);
    }

    /// <summary>
    /// 場所を変更してBGMを切り替える（公開メソッド）
    /// </summary>
    /// <param name="locationName">場所名（"beach", "forest", "cave", "camp", "default"）</param>
    public void SetLocation(string locationName)
    {
        if (currentLocation == locationName)
        {
            // 既に同じ場所にいる場合はスキップ
            return;
        }

        Debug.Log($"[GameScene] Location changed: {currentLocation} → {locationName}");
        currentLocation = locationName;

        // 場所SEを再生
        AudioManager.Instance.PlaySE("location_change");

        // BGMを切り替え
        if (gameTime != null)
        {
            PlayBGMForTimeOfDay(gameTime.CurrentTimeOfDay);
        }
    }

    /// <summary>
    /// 日付が変化した時の処理（GameStateから通知）
    /// </summary>
    private void OnDayChanged(int day, WeatherType weather)
    {
        Debug.Log($"[GameScene] 日付変化: {day}日目, 新しい天候: {weather.ToJapaneseString()}");

        // 天候が変わったらBGMを再チェック（雨/嵐なら停止、晴れ/曇りなら再生）
        if (gameTime != null)
        {
            PlayBGMForTimeOfDay(gameTime.CurrentTimeOfDay);
        }

        // ここで日付変化時の処理を実行
        // 例: イベント発生、リソース回復、新しい発見など
    }

    /// <summary>
    /// HPが変化した時の処理
    /// </summary>
    private void OnHPChanged(int current, int change)
    {
        if (change > 0)
        {
            Debug.Log($"[GameScene] HP回復: +{change} (現在: {current})");
            AudioManager.Instance.PlaySE("heal");
        }
        else if (change < 0)
        {
            Debug.Log($"[GameScene] ダメージ: {change} (現在: {current})");
            AudioManager.Instance.PlaySE("damage");
        }
    }

    /// <summary>
    /// スタミナが変化した時の処理
    /// </summary>
    private void OnStaminaChanged(int current, int change)
    {
        if (change > 0)
        {
            Debug.Log($"[GameScene] スタミナ回復: +{change} (現在: {current})");
        }
        else
        {
            Debug.Log($"[GameScene] スタミナ消費: {change} (現在: {current})");
        }
    }

    /// <summary>
    /// 主人公が死亡した時の処理
    /// </summary>
    private void OnPlayerDeath()
    {
        Debug.Log("[GameScene] 主人公が死亡しました - ゲームオーバー");

        // ゲームオーバーSEを再生
        AudioManager.Instance.PlaySE("game_over");

        // BGMを停止
        AudioManager.Instance.StopBGM(1f);

        // ゲームオーバー処理
        // 例: ゲームオーバー画面表示、タイトルに戻る、など
    }

    /// <summary>
    /// 時刻同期エラー表示とタイトルに戻る
    /// </summary>
    private void ShowError()
    {
        if (errorText != null)
        {
            errorText.text = "時刻同期エラー\n\nタイトルに戻ります...";
            errorText.gameObject.SetActive(true);
        }

        // 3秒後にタイトルシーンに戻る
        Invoke(nameof(ReturnToTitle), 3f);
    }

    private void ReturnToTitle()
    {
        SceneLoader.Instance.LoadTitleScene();
    }

    /// <summary>
    /// 設定ボタンがクリックされた時の処理
    /// </summary>
    private void OnSettingsButtonClicked()
    {
        Debug.Log("Settings button clicked - Loading SettingsScene");

        // ボタンSEを再生
        AudioManager.Instance.PlaySE("button_tap");

        // 設定シーンへ遷移
        SceneLoader.Instance.LoadSettingsScene();
    }
}

using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 日付遷移UIの管理クラス（Singleton）
/// ゲーム中いつでも日付・時間帯・天候の遷移演出を呼び出せる
/// </summary>
public class DayTransitionManager : MonoBehaviour
{
    private static DayTransitionManager _instance;
    public static DayTransitionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // シーンに存在しない場合は作成
                GameObject go = new GameObject("DayTransitionManager");
                _instance = go.AddComponent<DayTransitionManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Header("UI Reference")]
    [SerializeField] private DayTransitionUI transitionUI;

    [Header("Prefab")]
    [SerializeField] private GameObject transitionUIPrefab;

    private void Awake()
    {
        // Singleton設定
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // UIが未設定の場合、Prefabからロード
        if (transitionUI == null)
        {
            LoadTransitionUI();
        }
    }

    /// <summary>
    /// Transition UIをロード
    /// </summary>
    private void LoadTransitionUI()
    {
        // Prefabが設定されている場合
        if (transitionUIPrefab != null)
        {
            GameObject uiObject = Instantiate(transitionUIPrefab);
            transitionUI = uiObject.GetComponent<DayTransitionUI>();

            if (transitionUI != null)
            {
                DontDestroyOnLoad(uiObject);
                Debug.Log("DayTransitionUI loaded from prefab");
            }
            else
            {
                Debug.LogError("DayTransitionUI component not found in prefab");
                Destroy(uiObject);
            }
        }
        else
        {
            Debug.LogWarning("DayTransitionUI prefab not assigned. Please set it in the inspector or create UI manually.");
        }
    }

    /// <summary>
    /// UIを手動設定（エディタやコードから）
    /// </summary>
    public void SetTransitionUI(DayTransitionUI ui)
    {
        transitionUI = ui;
    }

    /// <summary>
    /// 日付遷移を表示（個別指定）
    /// </summary>
    public void Show(int day, TimeOfDay timeOfDay, WeatherType weather)
    {
        if (transitionUI == null)
        {
            Debug.LogWarning("DayTransitionUI is not set. Cannot show transition.");
            return;
        }

        transitionUI.ShowTransition(day, timeOfDay, weather);
        Debug.Log($"Day transition shown: Day {day}, {timeOfDay.ToJapaneseString()}, {weather.ToJapaneseString()}");
    }

    /// <summary>
    /// 日付遷移を表示（GameStateから自動取得）
    /// </summary>
    public void Show(GameState gameState)
    {
        if (gameState == null)
        {
            Debug.LogWarning("GameState is null. Cannot show transition.");
            return;
        }

        Show(gameState.CurrentDay, gameState.Time.CurrentTimeOfDay, gameState.CurrentWeather);
    }

    /// <summary>
    /// 遷移UIを強制的に非表示
    /// </summary>
    public void Hide()
    {
        if (transitionUI != null)
        {
            transitionUI.Hide();
        }
    }

    /// <summary>
    /// UIが使用可能かチェック
    /// </summary>
    public bool IsReady()
    {
        return transitionUI != null;
    }

    /// <summary>
    /// 日付遷移を表示して初期化処理を待機（GameStateから自動取得）
    /// </summary>
    /// <param name="gameState">ゲーム状態</param>
    /// <param name="onInitialization">初期化処理（非同期）</param>
    public void ShowWithInitialization(GameState gameState, Func<Task> onInitialization)
    {
        if (gameState == null)
        {
            Debug.LogWarning("GameState is null. Cannot show transition.");
            return;
        }

        ShowWithInitialization(gameState.CurrentDay, gameState.Time.CurrentTimeOfDay, gameState.CurrentWeather, onInitialization);
    }

    /// <summary>
    /// 日付遷移を表示して初期化処理を待機（個別指定）
    /// </summary>
    /// <param name="day">日数</param>
    /// <param name="timeOfDay">時間帯</param>
    /// <param name="weather">天候</param>
    /// <param name="onInitialization">初期化処理（非同期）</param>
    public void ShowWithInitialization(int day, TimeOfDay timeOfDay, WeatherType weather, Func<Task> onInitialization)
    {
        if (transitionUI == null)
        {
            Debug.LogWarning("DayTransitionUI is not set. Cannot show transition.");

            // UIがなくても初期化処理は実行
            if (onInitialization != null)
            {
                _ = onInitialization();
            }
            return;
        }

        transitionUI.ShowTransitionWithInitialization(day, timeOfDay, weather, onInitialization);
        Debug.Log($"Day transition shown with initialization: Day {day}, {timeOfDay.ToJapaneseString()}, {weather.ToJapaneseString()}");
    }
}

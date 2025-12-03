using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン遷移を管理するクラス
/// 非同期ロードとローディング画面の表示をサポート
/// </summary>
public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    public static SceneLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SceneLoader");
                _instance = go.AddComponent<SceneLoader>();
            }
            return _instance;
        }
    }

    private bool isLoading = false;

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
    /// 指定したシーンを同期的にロード
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (isLoading)
        {
            Debug.LogWarning("Scene is already loading!");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 指定したシーンを非同期でロード（フェード付き）
    /// </summary>
    public void LoadSceneAsync(string sceneName)
    {
        if (isLoading)
        {
            Debug.LogWarning("Scene is already loading!");
            return;
        }

        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        isLoading = true;

        // フェードイン（画面を暗くする）
        SceneFadeManager.Instance.FadeIn();
        yield return new WaitForSeconds(0.5f); // フェード完了を待つ

        // 非同期でシーンをロード
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // ロード進捗を監視
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"Loading progress: {progress * 100}%");

            // 90%まで達したら、シーンを有効化
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // シーンロード完了後、少し待ってからフェードアウト
        yield return new WaitForSeconds(0.3f);

        // フェードアウト（画面を明るくする）
        SceneFadeManager.Instance.FadeOut();

        isLoading = false;
        Debug.Log($"Scene {sceneName} loaded successfully");
    }

    /// <summary>
    /// TitleSceneをロード
    /// </summary>
    public void LoadTitleScene()
    {
        LoadScene("TitleScene");
    }

    /// <summary>
    /// GameSceneをロード（NTP同期と日付表示付き）
    /// </summary>
    public void LoadGameScene()
    {
        StartCoroutine(LoadGameSceneWithTransition());
    }

    /// <summary>
    /// ゲームシーンロードの特別な遷移処理
    /// フェード → NTP同期 → 日付表示 → シーンロード → フェードアウト
    /// </summary>
    private IEnumerator LoadGameSceneWithTransition()
    {
        if (isLoading)
        {
            Debug.LogWarning("Scene is already loading!");
            yield break;
        }

        isLoading = true;

        // 1. フェードイン（画面を真っ黒にする）
        Debug.Log("Starting fade to black...");
        SceneFadeManager.Instance.FadeIn();
        yield return new WaitForSeconds(1.0f); // フェード完了を待つ

        // 2. NTP時刻同期を開始（非同期）
        Debug.Log("Starting NTP time synchronization...");
        var ntpTask = NTPTimeManager.Instance.RefreshServerTimeAsync();

        // 3. GameStateから現在の日数を取得
        int currentDay = 1; // デフォルト値
        if (GameManager.Instance != null && GameManager.Instance.State != null)
        {
            currentDay = GameManager.Instance.State.CurrentDay;
        }

        // 4. 日付遷移画面を表示（"〇日目"）
        Debug.Log($"Displaying day transition: Day {currentDay}");
        bool dayTransitionComplete = false;
        DayTransitionScreen.Instance.ShowDay(currentDay, () => dayTransitionComplete = true);

        // 5. シーンの非同期ロード開始
        Debug.Log("Starting async scene load...");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        asyncLoad.allowSceneActivation = false;

        // 6. NTP同期とシーンロードの完了を待つ
        while (!ntpTask.IsCompleted || asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // NTP同期結果をログ出力
        bool ntpSuccess = ntpTask.Result;
        if (ntpSuccess)
        {
            Debug.Log($"NTP sync successful. Server time: {NTPTimeManager.Instance.ServerTime:yyyy/MM/dd HH:mm:ss}");
        }
        else
        {
            Debug.LogWarning("NTP sync failed, but continuing with local time");
        }

        // 7. 日付表示が完了するまで待つ
        while (!dayTransitionComplete)
        {
            yield return null;
        }

        // 8. シーンをアクティブ化（先に切り替えておく）
        Debug.Log("Activating scene...");
        asyncLoad.allowSceneActivation = true;

        // シーンロード完了を待つ
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log("Scene loaded, now showing time of day image");

        // 9. 時間帯の画像を表示（2秒間）
        TimeOfDay currentTimeOfDay = TimeOfDayUtility.GetTimeOfDay(NTPTimeManager.Instance.ServerTime);
        bool timeOfDayComplete = false;
        DayTransitionScreen.Instance.ShowTimeOfDayImage(currentTimeOfDay, () => timeOfDayComplete = true);

        // 10. 時間帯画像を表示している間に、裏でFadePanelをフェードアウト（黒→透明）
        Debug.Log("Fading out black panel behind time of day image...");
        SceneFadeManager.Instance.FadeOut(2.0f);

        // 11. 時間帯画像表示が完了するまで待つ
        while (!timeOfDayComplete)
        {
            yield return null;
        }

        isLoading = false;
        Debug.Log("Game scene loaded successfully with full transition");
    }

    /// <summary>
    /// SettingsSceneをロード
    /// </summary>
    public void LoadSettingsScene()
    {
        LoadScene("SettingsScene");
    }

    /// <summary>
    /// ノベルシーンをロード
    /// </summary>
    public void LoadNovelScene(string scenarioLabel, string returnSceneName = "GameScene", string targetCharacterId = null)
    {
        // ノベルシーンに渡すデータを設定
        NovelSceneData.Instance.Setup(scenarioLabel, returnSceneName, targetCharacterId);

        // ノベルシーンをロード
        LoadSceneAsync("NovelScene");
    }
}

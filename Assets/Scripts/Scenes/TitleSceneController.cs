using UnityEngine;
using TMPro;

/// <summary>
/// タイトルシーンを制御するクラス
/// タップでゲームシーンへ遷移
/// </summary>
public class TitleSceneController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI tapToStartText;
    [SerializeField] private TextMeshProUGUI versionText;
    [SerializeField] private TextMeshProUGUI patchStatusText;

    [Header("Font Settings")]
    [SerializeField] private TMP_FontAsset dayTransitionFont;  // 日付遷移画面用フォント

    [Header("Time of Day Images")]
    [SerializeField] private Sprite earlyMorningImage;  // 早朝の画像
    [SerializeField] private Sprite morningImage;       // 朝の画像
    [SerializeField] private Sprite noonImage;          // 昼の画像
    [SerializeField] private Sprite eveningImage;       // 夜の画像
    [SerializeField] private Sprite midnightImage;      // 深夜の画像

    [Header("Patch Settings")]
    [SerializeField] private string patchAppPackageName = "com.example.patchapp";  // パッチアプリのパッケージ名

#if UNITY_EDITOR
    // エディタでパッチ適用済みとしてテスト
    [Header("Editor Debug")]
    [SerializeField] private bool debugPatchInstalled = false; 
#endif

    private void Start()
    {
        // GameManagerが存在しない場合は作成
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
        }

        // バージョン情報を表示
        DisplayVersionInfo();

        // パッチアプリの状態をチェック
        CheckPatchStatus();

        // DayTransitionScreenにフォントと画像を設定
        if (dayTransitionFont != null)
        {
            DayTransitionScreen.Instance.SetJapaneseFont(dayTransitionFont);
            Debug.Log($"Day transition font set: {dayTransitionFont.name}");
        }

        // 時間帯画像を設定
        DayTransitionScreen.Instance.SetTimeOfDayImages(
            earlyMorningImage,
            morningImage,
            noonImage,
            eveningImage,
            midnightImage
        );
        Debug.Log("Time of day images set for DayTransitionScreen");

        // タップテキストを表示して点滅開始
        if (tapToStartText != null)
        {
            tapToStartText.gameObject.SetActive(true);
            StartCoroutine(BlinkText(tapToStartText));
        }

        // タイトルBGMを再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM("title_theme", 2f);
        }

        Debug.Log("TitleScene ready - Waiting for player input");
    }

    /// <summary>
    /// バージョン情報を表示
    /// </summary>
    private void DisplayVersionInfo()
    {
        if (versionText != null)
        {
            // Unityのバージョン情報を取得（Project Settings > Player > Versionで設定）
            string version = Application.version;
            versionText.text = $"Version {version}";

            Debug.Log($"App Version: {version}");
        }
    }

    /// <summary>
    /// パッチアプリの適用状態をチェック
    /// </summary>
    private void CheckPatchStatus()
    {
        if (patchStatusText == null) return;

        bool isPatchInstalled = false;

#if UNITY_EDITOR
        // エディタではデバッグ設定を使用
        isPatchInstalled = debugPatchInstalled;
        if (isPatchInstalled)
        {
            Debug.Log("[Debug] Patch app is installed (editor debug mode)");
        }
#else
        // 実機では既存のAndroidPackageCheckerを使用
        isPatchInstalled = AndroidPackageChecker.IsPackageInstalled(patchAppPackageName);
#endif

        if (isPatchInstalled)
        {
            patchStatusText.gameObject.SetActive(true);
            patchStatusText.text = "パッチ適用済み";
        }
        else
        {
            patchStatusText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // タップ/クリック検出（Android対応）
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            OnScreenTapped();
        }

        // エディタ用のマウスクリック検出
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            OnScreenTapped();
        }
        #endif
    }

    private System.Collections.IEnumerator BlinkText(TextMeshProUGUI text)
    {
        float fadeDuration = 1.5f;  // フェード時間（秒）
        float minAlpha = 0.3f;      // 最小透明度
        float maxAlpha = 1f;        // 最大透明度

        while (true)
        {
            // フェードアウト（濃い → 薄い）
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                text.alpha = Mathf.Lerp(maxAlpha, minAlpha, t);
                yield return null;
            }

            // フェードイン（薄い → 濃い）
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                text.alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
                yield return null;
            }
        }
    }

    private void OnScreenTapped()
    {
        // タップSEを再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE("button_tap");
        }

        // ゲームシーンへ遷移
        SceneLoader.Instance.LoadGameScene();
    }
}

using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 日付遷移画面 - シーン切り替え時に"〇日目"を表示
/// </summary>
public class DayTransitionScreen : MonoBehaviour
{
    private static DayTransitionScreen _instance;
    public static DayTransitionScreen Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("DayTransitionScreen");
                _instance = go.AddComponent<DayTransitionScreen>();
                _instance.CreateTransitionUI();
            }
            return _instance;
        }
    }

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI dayText;
    private UnityEngine.UI.Image timeOfDayImage;  // 時間帯画像表示用
    private Coroutine fadeCoroutine;

    // 静的フィールドでフォントと画像をキャッシュ（TitleSceneで設定したものを使い回す）
    private static TMP_FontAsset cachedFont;
    private static Sprite cachedEarlyMorningImage;
    private static Sprite cachedMorningImage;
    private static Sprite cachedNoonImage;
    private static Sprite cachedEveningImage;
    private static Sprite cachedMidnightImage;

    [Header("Display Settings")]
    [SerializeField] private float displayDuration = 2.0f;  // 表示時間（秒）
    [SerializeField] private float fadeDuration = 0.5f;     // フェード時間（秒）

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
    /// 遷移UIを動的に作成
    /// </summary>
    private void CreateTransitionUI()
    {
        // Canvas作成
        GameObject canvasObj = new GameObject("DayTransitionCanvas");
        canvasObj.transform.SetParent(transform);

        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000; // FadePanelより前面に表示

        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // CanvasGroup追加（フェード制御用）
        canvasGroup = canvasObj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        // TextMeshPro作成
        GameObject textObj = new GameObject("DayText");
        textObj.transform.SetParent(canvasObj.transform, false);

        dayText = textObj.AddComponent<TextMeshProUGUI>();
        dayText.fontSize = 72;
        dayText.alignment = TextAlignmentOptions.Center;
        dayText.color = Color.white;

        // フォントを自動ロード（どのシーンから起動しても使える）
        LoadDefaultFont();

        // RectTransformの設定（画面中央）
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        // 時間帯画像用のImage作成（全画面）
        GameObject imageObj = new GameObject("TimeOfDayImage");
        imageObj.transform.SetParent(canvasObj.transform, false);

        timeOfDayImage = imageObj.AddComponent<UnityEngine.UI.Image>();
        timeOfDayImage.color = Color.white;

        // RectTransformの設定（全画面）
        RectTransform imageRectTransform = imageObj.GetComponent<RectTransform>();
        imageRectTransform.anchorMin = Vector2.zero;
        imageRectTransform.anchorMax = Vector2.one;
        imageRectTransform.sizeDelta = Vector2.zero;
        imageRectTransform.anchoredPosition = Vector2.zero;

        // 初期状態は非表示
        imageObj.SetActive(false);

        Debug.Log("DayTransitionScreen UI created");
    }

    /// <summary>
    /// 日付を表示（フェードイン → 表示 → フェードアウト）
    /// </summary>
    public void ShowDay(int dayNumber, Action onComplete = null)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // テキストを設定
        dayText.text = $"{dayNumber}日目";

        // 表示アニメーション開始
        fadeCoroutine = StartCoroutine(ShowDayCoroutine(onComplete));
    }

    /// <summary>
    /// 日付表示コルーチン
    /// </summary>
    private IEnumerator ShowDayCoroutine(Action onComplete)
    {
        // フェードイン
        yield return StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));

        // 表示維持
        yield return new WaitForSecondsRealtime(displayDuration);

        // フェードアウトせず、そのまま続く

        fadeCoroutine = null;
        onComplete?.Invoke();
    }

    /// <summary>
    /// CanvasGroupのフェードアニメーション
    /// </summary>
    private IEnumerator FadeCanvasGroup(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    /// <summary>
    /// 即座に非表示
    /// </summary>
    public void HideImmediate()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// 時間帯の画像を表示（2秒間）
    /// </summary>
    public void ShowTimeOfDayImage(TimeOfDay timeOfDay, Action onComplete = null)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // 時間帯に応じた画像を設定（未設定の場合は白い画像）
        Sprite sprite = GetTimeOfDaySprite(timeOfDay);
        timeOfDayImage.sprite = sprite;

        Debug.Log($"ShowTimeOfDayImage called: {timeOfDay}, sprite: {sprite != null}");

        fadeCoroutine = StartCoroutine(ShowTimeOfDayCoroutine(onComplete));
    }

    /// <summary>
    /// 時間帯画像表示コルーチン（パッと表示 → 2秒待機 → パッと消える）
    /// </summary>
    private IEnumerator ShowTimeOfDayCoroutine(Action onComplete)
    {
        Debug.Log("ShowTimeOfDayCoroutine started");

        // テキストを非表示
        dayText.gameObject.SetActive(false);

        // 画像を即座に表示
        timeOfDayImage.gameObject.SetActive(true);
        Debug.Log("Time of day image activated");

        // 2秒間表示
        yield return new WaitForSecondsRealtime(2.0f);

        // 画像を即座に非表示
        timeOfDayImage.gameObject.SetActive(false);
        Debug.Log("Time of day image deactivated");

        // テキストを再表示
        dayText.gameObject.SetActive(true);

        // CanvasGroupのalphaを0に戻す（次のフェードアウトのため）
        canvasGroup.alpha = 0f;

        fadeCoroutine = null;
        onComplete?.Invoke();
        Debug.Log("ShowTimeOfDayCoroutine completed");
    }

    /// <summary>
    /// 時間帯に応じたSpriteを取得（キャッシュから）
    /// </summary>
    private Sprite GetTimeOfDaySprite(TimeOfDay timeOfDay)
    {
        Sprite sprite = null;

        switch (timeOfDay)
        {
            case TimeOfDay.EarlyMorning:
                sprite = cachedEarlyMorningImage;
                break;
            case TimeOfDay.Morning:
                sprite = cachedMorningImage;
                break;
            case TimeOfDay.Noon:
                sprite = cachedNoonImage;
                break;
            case TimeOfDay.Evening:
                sprite = cachedEveningImage;
                break;
            case TimeOfDay.Midnight:
                sprite = cachedMidnightImage;
                break;
        }

        // 画像が設定されていない場合は白いSpriteを生成
        if (sprite == null)
        {
            sprite = CreateWhiteSprite();
        }

        return sprite;
    }

    /// <summary>
    /// 白いSpriteを生成（仮画像用）
    /// </summary>
    private Sprite CreateWhiteSprite()
    {
        // 1x1の白いテクスチャを作成
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        // SpriteとしてTextureを作成
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        return sprite;
    }

    /// <summary>
    /// 時間帯画像を設定（TitleSceneから呼び出される）
    /// </summary>
    public void SetTimeOfDayImages(Sprite earlyMorning, Sprite morning, Sprite noon, Sprite evening, Sprite midnight)
    {
        // 静的フィールドにキャッシュ
        cachedEarlyMorningImage = earlyMorning;
        cachedMorningImage = morning;
        cachedNoonImage = noon;
        cachedEveningImage = evening;
        cachedMidnightImage = midnight;

        Debug.Log("Time of day images cached successfully");
    }

    /// <summary>
    /// デフォルトフォントをロード
    /// </summary>
    private void LoadDefaultFont()
    {
        // キャッシュされたフォントがあれば使用（TitleSceneで設定されたもの）
        if (cachedFont != null)
        {
            dayText.font = cachedFont;
            Debug.Log($"Day transition font loaded from cache: {cachedFont.name}");
            return;
        }

#if UNITY_EDITOR
        // エディタでGameSceneから直接起動した場合、AssetDatabaseで検索
        string[] guids = UnityEditor.AssetDatabase.FindAssets("RampartOne-Regular SDF t:TMP_FontAsset");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            TMP_FontAsset font = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (font != null)
            {
                dayText.font = font;
                cachedFont = font; // キャッシュに保存
                Debug.Log($"Day transition font loaded from AssetDatabase: {font.name}");
                return;
            }
        }
#endif

        Debug.LogWarning("Day transition font not found. Please start from TitleScene or set font manually.");
    }

    /// <summary>
    /// 日本語フォントを設定（TitleSceneから呼び出される）
    /// </summary>
    public void SetJapaneseFont(TMP_FontAsset font)
    {
        if (font != null)
        {
            cachedFont = font; // 静的フィールドにキャッシュ
            if (dayText != null)
            {
                dayText.font = font;
            }
            Debug.Log($"Day transition font set: {font.name}");
        }
    }
}

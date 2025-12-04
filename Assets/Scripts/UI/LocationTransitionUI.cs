using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 場所移動時の演出UI
/// マップズームで移動感を演出
/// </summary>
public class LocationTransitionUI : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private CanvasGroup fadePanel;

    [Header("Map Zoom")]
    [SerializeField] private RectTransform mapViewTransform; // MapView全体
    [SerializeField] private float zoomScale = 3f; // ズーム倍率
    [SerializeField] private float zoomDuration = 0.6f; // ズーム時間

    [Header("Optional")]
    [SerializeField] private Text locationNameText; // 「海岸」など（オプション）

    private Vector3 originalMapPosition;
    private Vector3 originalMapScale;

    private void Awake()
    {
        // 元の状態を保存
        if (mapViewTransform != null)
        {
            originalMapPosition = mapViewTransform.localPosition;
            originalMapScale = mapViewTransform.localScale;
        }

        // フェードパネルを非表示
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// アイコンにズームして場所へ移動する演出
    /// </summary>
    /// <param name="iconTransform">タップされたアイコンのRectTransform</param>
    /// <param name="location">移動先の場所</param>
    public IEnumerator ZoomToLocation(RectTransform iconTransform, MapLocation location)
    {
        // 場所名を表示（オプション）
        if (locationNameText != null)
        {
            locationNameText.text = location.ToJapaneseString();
        }

        // アイコンの位置を取得（ワールド座標→ローカル座標）
        Vector3 targetPosition = GetTargetPositionForZoom(iconTransform);

        // ズームイン + フェードアウト（同時）
        yield return StartCoroutine(ZoomAndFade(targetPosition, zoomScale, true));

        // フェードが完了（画面が黒い）時点でMapViewをリセット
        // （この後すぐにShowLocationViewでMapViewが非表示になるので見えない）
        ResetMapViewImmediate();
    }

    /// <summary>
    /// フェードインのみ（画面切り替え後に呼び出す）
    /// </summary>
    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(FadeInOnly());
    }

    /// <summary>
    /// MapViewを即座にリセット（フェード中に呼び出す）
    /// </summary>
    private void ResetMapViewImmediate()
    {
        if (mapViewTransform != null)
        {
            mapViewTransform.localPosition = originalMapPosition;
            mapViewTransform.localScale = originalMapScale;
        }
    }

    /// <summary>
    /// マップに戻る演出（フェードアウト → ズームアウト）
    /// </summary>
    public IEnumerator ZoomOutToMap()
    {
        // フェードアウト（画面を黒くする）
        yield return StartCoroutine(FadeOutOnly());

        // ここで呼び出し元がMapViewに切り替える

        // MapViewを縮小状態からスタートしてズームアウト（拡大→等倍）
        yield return StartCoroutine(ZoomOutAndFadeIn());
    }

    /// <summary>
    /// フェードアウト（公開用）
    /// </summary>
    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(FadeOutOnly());
    }

    /// <summary>
    /// ズームアウトしながらフェードイン（公開用）
    /// </summary>
    public IEnumerator ZoomOutAndFadeInPublic()
    {
        yield return StartCoroutine(ZoomOutAndFadeIn());
    }

    /// <summary>
    /// フェードアウトのみ（画面を黒くする）
    /// </summary>
    private IEnumerator FadeOutOnly()
    {
        float elapsed = 0f;
        float fadeDuration = 0.3f;

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
        }
        else
        {
            yield break;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            fadePanel.alpha = t;

            yield return null;
        }

        fadePanel.alpha = 1f;
    }

    /// <summary>
    /// ズームアウトしながらフェードイン（縮小状態から等倍へ）
    /// </summary>
    private IEnumerator ZoomOutAndFadeIn()
    {
        float elapsed = 0f;
        float zoomOutDuration = zoomDuration * 0.6f;  // ズームアウトは速めに

        // 縮小状態からスタート
        Vector3 startScale = originalMapScale * zoomScale;
        Vector3 endScale = originalMapScale;

        mapViewTransform.localScale = startScale;
        mapViewTransform.localPosition = originalMapPosition;

        while (elapsed < zoomOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomOutDuration;

            // イージング（EaseInOut）
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // ズームアウト（大→小）
            mapViewTransform.localScale = Vector3.Lerp(startScale, endScale, smoothT);

            // フェードイン
            if (fadePanel != null)
            {
                fadePanel.alpha = 1f - smoothT;
            }

            yield return null;
        }

        // 最終状態を確定
        mapViewTransform.localScale = endScale;
        mapViewTransform.localPosition = originalMapPosition;

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// フェードインのみ
    /// </summary>
    private IEnumerator FadeInOnly()
    {
        float elapsed = 0f;
        float fadeDuration = 0.3f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            if (fadePanel != null)
            {
                fadePanel.alpha = 1f - t;
            }

            yield return null;
        }

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ズームインしながらフェードアウト
    /// </summary>
    private IEnumerator ZoomAndFade(Vector3 targetPosition, float targetScale, bool fadeOut)
    {
        float elapsed = 0f;
        Vector3 startPosition = mapViewTransform.localPosition;
        Vector3 startScale = mapViewTransform.localScale;

        // フェードパネルを有効化
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
        }

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;

            // イージング（EaseInOut）
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // ズーム
            mapViewTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, smoothT);
            mapViewTransform.localScale = Vector3.Lerp(startScale, Vector3.one * targetScale, smoothT);

            // フェード
            if (fadePanel != null && fadeOut)
            {
                fadePanel.alpha = smoothT;
            }

            yield return null;
        }

        // 最終状態を確定
        mapViewTransform.localPosition = targetPosition;
        mapViewTransform.localScale = Vector3.one * targetScale;

        if (fadePanel != null && fadeOut)
        {
            fadePanel.alpha = 1f;
        }
    }

    /// <summary>
    /// フェードインしながらズームをリセット
    /// </summary>
    private IEnumerator FadeInAndResetZoom()
    {
        float elapsed = 0f;

        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.3f;

            // フェードイン
            if (fadePanel != null)
            {
                fadePanel.alpha = 1f - t;
            }

            yield return null;
        }

        // マップビューをリセット（次回のために）
        mapViewTransform.localPosition = originalMapPosition;
        mapViewTransform.localScale = originalMapScale;

        // フェードパネルを非表示
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// アイコンの中心にズームするための目標位置を計算
    /// </summary>
    private Vector3 GetTargetPositionForZoom(RectTransform iconTransform)
    {
        // アイコンのMapView内でのローカル位置を取得
        Vector3 iconLocalPos = mapViewTransform.InverseTransformPoint(iconTransform.position);

        // ズーム倍率を考慮して、アイコンが画面中央に来るように移動
        // MapViewを逆方向に移動させる
        Vector3 offset = -new Vector3(iconLocalPos.x, iconLocalPos.y, 0f) * zoomScale;

        return originalMapPosition + offset;
    }

    /// <summary>
    /// 即座にリセット（デバッグ用）
    /// </summary>
    public void ResetZoom()
    {
        if (mapViewTransform != null)
        {
            mapViewTransform.localPosition = originalMapPosition;
            mapViewTransform.localScale = originalMapScale;
        }

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
        }
    }
}

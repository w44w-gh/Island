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

        // ここで呼び出し元が場所切り替えを行う

        // 少し待つ（オプション）
        yield return new WaitForSeconds(0.1f);

        // フェードイン + ズームリセット
        yield return StartCoroutine(FadeInAndResetZoom());
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
        // アイコンの中心位置（ワールド座標）
        Vector3 iconWorldPos = iconTransform.position;

        // MapViewの親（Canvas）
        Canvas canvas = mapViewTransform.GetComponentInParent<Canvas>();

        // ワールド座標をローカル座標に変換
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapViewTransform.parent as RectTransform,
            iconWorldPos,
            canvas.worldCamera,
            out localPoint
        );

        // アイコンが画面中央に来るように、MapViewを逆方向に移動
        // （アイコン位置 - 画面中央）の逆
        Vector3 offset = -new Vector3(localPoint.x, localPoint.y, 0f);

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

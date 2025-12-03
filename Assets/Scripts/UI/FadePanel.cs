using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// フェードイン/アウト機能を提供する汎用パネルクラス
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class FadePanel : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;  // フェードにかかる時間（秒）
    [SerializeField] private bool startVisible = false;  // 起動時に表示状態にするか

    private CanvasGroup canvasGroup;
    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // 初期状態の設定
        if (startVisible)
        {
            canvasGroup.alpha = 1f;
            gameObject.SetActive(true);
        }
        else
        {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// フェードインして表示
    /// </summary>
    public void FadeIn(Action onComplete = null)
    {
        FadeIn(fadeDuration, onComplete);
    }

    /// <summary>
    /// フェードインして表示（カスタム時間指定）
    /// </summary>
    public void FadeIn(float duration, Action onComplete = null)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        gameObject.SetActive(true);
        currentFadeCoroutine = StartCoroutine(FadeCoroutine(0f, 1f, duration, onComplete));
    }

    /// <summary>
    /// フェードアウトして非表示
    /// </summary>
    public void FadeOut(Action onComplete = null)
    {
        FadeOut(fadeDuration, onComplete);
    }

    /// <summary>
    /// フェードアウトして非表示（カスタム時間指定）
    /// </summary>
    public void FadeOut(float duration, Action onComplete = null)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeCoroutine(1f, 0f, duration, () =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }));
    }

    /// <summary>
    /// 即座に表示（フェードなし）
    /// </summary>
    public void ShowImmediate()
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }

        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 即座に非表示（フェードなし）
    /// </summary>
    public void HideImmediate()
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// フェードのコルーチン
    /// </summary>
    private IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration, Action onComplete)
    {
        float elapsed = 0f;
        canvasGroup.alpha = startAlpha;

        // フェード中はタップを無効化（FadeInの場合のみ）
        if (endAlpha > startAlpha)
        {
            canvasGroup.blocksRaycasts = true;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;  // タイムスケールの影響を受けない
            float t = Mathf.Clamp01(elapsed / duration);

            // イージング（滑らかな加速・減速）
            t = EaseInOutQuad(t);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        // フェードアウト完了時はレイキャストをブロックしない
        if (endAlpha == 0f)
        {
            canvasGroup.blocksRaycasts = false;
        }

        currentFadeCoroutine = null;

        onComplete?.Invoke();
    }

    /// <summary>
    /// イージング関数（加速して減速）
    /// </summary>
    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    /// <summary>
    /// 現在フェード中かどうか
    /// </summary>
    public bool IsFading => currentFadeCoroutine != null;

    /// <summary>
    /// 現在表示されているかどうか
    /// </summary>
    public bool IsVisible => gameObject.activeSelf && canvasGroup.alpha > 0f;
}

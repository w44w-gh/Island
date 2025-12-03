using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 日付・時間帯・天候の遷移を表示するUI
/// ムジュラの仮面風の演出
/// </summary>
public class DayTransitionUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text dayText;           // 「〇日目」
    [SerializeField] private Text timeWeatherText;   // 「朝 - 晴れ」

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private Coroutine currentTransition;

    private void Awake()
    {
        // 初期状態は非表示
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 日付遷移を表示
    /// </summary>
    public void ShowTransition(int day, TimeOfDay timeOfDay, WeatherType weather)
    {
        // 既に表示中の場合は停止
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        currentTransition = StartCoroutine(TransitionCoroutine(day, timeOfDay, weather));
    }

    /// <summary>
    /// 遷移アニメーションのコルーチン
    /// </summary>
    private IEnumerator TransitionCoroutine(int day, TimeOfDay timeOfDay, WeatherType weather)
    {
        // テキストを設定
        if (dayText != null)
        {
            dayText.text = $"{day}日目";
        }

        if (timeWeatherText != null)
        {
            string timeStr = timeOfDay.ToJapaneseString();
            string weatherStr = weather.ToJapaneseString();
            timeWeatherText.text = $"{timeStr} - {weatherStr}";
        }

        // UI表示
        gameObject.SetActive(true);

        // フェードイン
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeInDuration));

        // 表示維持
        yield return new WaitForSeconds(displayDuration);

        // フェードアウト
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, fadeOutDuration));

        // UI非表示
        gameObject.SetActive(false);
        currentTransition = null;
    }

    /// <summary>
    /// CanvasGroupのアルファをアニメーション
    /// </summary>
    private IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        if (group == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        group.alpha = endAlpha;
    }

    /// <summary>
    /// 強制的に非表示
    /// </summary>
    public void Hide()
    {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
            currentTransition = null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 日付遷移を表示して、初期化処理の完了を待つ
    /// </summary>
    /// <param name="day">日数</param>
    /// <param name="timeOfDay">時間帯</param>
    /// <param name="weather">天候</param>
    /// <param name="onInitialization">初期化処理のコールバック（非同期）</param>
    public void ShowTransitionWithInitialization(int day, TimeOfDay timeOfDay, WeatherType weather, Func<System.Threading.Tasks.Task> onInitialization)
    {
        // 既に表示中の場合は停止
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        currentTransition = StartCoroutine(TransitionWithInitializationCoroutine(day, timeOfDay, weather, onInitialization));
    }

    /// <summary>
    /// 初期化処理を待機する遷移アニメーション
    /// </summary>
    private IEnumerator TransitionWithInitializationCoroutine(int day, TimeOfDay timeOfDay, WeatherType weather, Func<System.Threading.Tasks.Task> onInitialization)
    {
        // 現在のBGMを停止（1秒フェードアウト）
        AudioManager.Instance.StopBGM(1f);

        // 遷移開始SEを再生
        AudioManager.Instance.PlaySE("transition_start");

        // テキストを設定
        if (dayText != null)
        {
            dayText.text = $"{day}日目";
        }

        if (timeWeatherText != null)
        {
            string timeStr = timeOfDay.ToJapaneseString();
            string weatherStr = weather.ToJapaneseString();
            timeWeatherText.text = $"{timeStr} - {weatherStr}";
        }

        // UI表示
        gameObject.SetActive(true);

        // フェードイン
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeInDuration));

        // 初期化処理を実行（非同期処理を待つ）
        if (onInitialization != null)
        {
            var task = onInitialization();
            yield return new WaitUntil(() => task.IsCompleted);

            // エラーチェック
            if (task.IsFaulted)
            {
                Debug.LogError($"初期化処理中にエラーが発生しました: {task.Exception}");
            }
        }

        // 少し待ってからフェードアウト（最低表示時間を確保）
        yield return new WaitForSeconds(0.5f);

        // 遷移完了SEを再生
        AudioManager.Instance.PlaySE("transition_end");

        // フェードアウト
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, fadeOutDuration));

        // UI非表示
        gameObject.SetActive(false);
        currentTransition = null;
    }
}

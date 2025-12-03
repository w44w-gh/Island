using System;
using UnityEngine;

/// <summary>
/// シーン遷移時のフェード管理（シングルトン）
/// DontDestroyOnLoadで全シーンで使用可能
/// </summary>
public class SceneFadeManager : MonoBehaviour
{
    private static SceneFadeManager _instance;
    public static SceneFadeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Resourcesフォルダからプレハブをロード、または動的に生成
                GameObject prefab = Resources.Load<GameObject>("SceneFadeCanvas");

                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab);
                    _instance = go.GetComponent<SceneFadeManager>();
                }
                else
                {
                    // プレハブがない場合は動的に生成
                    GameObject go = new GameObject("SceneFadeManager");
                    _instance = go.AddComponent<SceneFadeManager>();
                    _instance.CreateFadePanel();
                }
            }
            return _instance;
        }
    }

    private FadePanel fadePanel;

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
    /// フェードパネルを動的に作成
    /// </summary>
    private void CreateFadePanel()
    {
        // Canvas作成
        GameObject canvasObj = new GameObject("SceneFadeCanvas");
        canvasObj.transform.SetParent(transform);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // 最前面に表示

        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // FadePanel作成
        GameObject panelObj = new GameObject("FadePanel");
        panelObj.transform.SetParent(canvasObj.transform, false);

        // RectTransformの設定（画面全体に広げる）
        RectTransform rectTransform = panelObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        // 真っ黒な画像（完全不透明）
        UnityEngine.UI.Image image = panelObj.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0f, 0f, 0f, 1f);  // 黒、100%不透明

        // CanvasGroupとFadePanelコンポーネント
        panelObj.AddComponent<CanvasGroup>();
        fadePanel = panelObj.AddComponent<FadePanel>();

        Debug.Log("SceneFadeManager: FadePanel created dynamically");
    }

    /// <summary>
    /// フェードインして表示（デフォルト1.0秒）
    /// </summary>
    public void FadeIn(float duration = 1.0f, Action onComplete = null)
    {
        if (fadePanel != null)
        {
            fadePanel.FadeIn(duration, onComplete);
        }
    }

    /// <summary>
    /// フェードアウトして非表示（デフォルト1.0秒）
    /// </summary>
    public void FadeOut(float duration = 1.0f, Action onComplete = null)
    {
        if (fadePanel != null)
        {
            fadePanel.FadeOut(duration, onComplete);
        }
    }

    /// <summary>
    /// 即座に表示
    /// </summary>
    public void ShowImmediate()
    {
        if (fadePanel != null)
        {
            fadePanel.ShowImmediate();
        }
    }

    /// <summary>
    /// 即座に非表示
    /// </summary>
    public void HideImmediate()
    {
        if (fadePanel != null)
        {
            fadePanel.HideImmediate();
        }
    }
}

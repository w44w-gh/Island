using System;
using UnityEngine;
using UnityEngine.UI;
using Utage;

/// <summary>
/// ノベルシーンを制御するクラス
/// </summary>
public class NovelSceneController : MonoBehaviour
{
    [Header("宴の設定")]
    [SerializeField] private AdvEngine advEngine;

    [Header("UI References")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image messageWindowBackground;

    [Header("メッセージウィンドウのスタイル")]
    [SerializeField] private Color messageWindowColor = new Color(0, 0, 0, 0.7f);  // 黒の半透明

    private string scenarioLabel;
    private string returnSceneName = "GameScene";

    private void Start()
    {
        // 宴のエンジンが設定されているか確認
        if (advEngine == null)
        {
           Debug.LogError("AdvEngineが設定されていません！");
           ReturnToGameScene();
           return;
        }

        // GameManagerからシナリオラベルを取得
        scenarioLabel = NovelSceneData.Instance.ScenarioLabel;
        returnSceneName = NovelSceneData.Instance.ReturnSceneName;

        if (string.IsNullOrEmpty(scenarioLabel))
        {
            Debug.LogError("シナリオラベルが設定されていません！");
            ReturnToGameScene();
            return;
        }

        Debug.Log($"NovelScene開始: シナリオ '{scenarioLabel}'");

        // ローディングパネルを非表示
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        // メッセージウィンドウの背景色を設定
        SetupMessageWindowStyle();

        // シナリオ開始
        StartScenario();

        // 宴のシナリオ終了イベントを購読
        advEngine.Page.OnEndPage.AddListener(OnPageEnd);
    }

    private void OnDestroy()
    {
        // イベント購読解除
        if (advEngine != null && advEngine.Page != null)
        {
           advEngine.Page.OnEndPage.RemoveListener(OnPageEnd);
        }
    }

    /// <summary>
    /// メッセージウィンドウのスタイルを設定
    /// </summary>
    private void SetupMessageWindowStyle()
    {
        if (messageWindowBackground != null)
        {
            messageWindowBackground.color = messageWindowColor;
            Debug.Log("メッセージウィンドウの背景色を設定しました");
        }
    }

    /// <summary>
    /// シナリオを開始
    /// </summary>
    private void StartScenario()
    {
        try
        {
            advEngine.JumpScenario(scenarioLabel);
            Debug.Log($"シナリオ '{scenarioLabel}' を開始しました");
        }
        catch (Exception e)
        {
            Debug.LogError($"シナリオの開始に失敗: {e.Message}");
            ReturnToGameScene();
        }
    }

    /// <summary>
    /// ページ終了時の処理
    /// </summary>
    private void OnPageEnd(AdvPage page)
    {
       // シナリオが完全に終了したかチェック
       if (advEngine.IsEndScenario)
       {
           Debug.Log("シナリオが終了しました");
           OnScenarioEnd();
       }
    }

    /// <summary>
    /// シナリオ終了時の処理
    /// </summary>
    private void OnScenarioEnd()
    {
        Debug.Log("ノベルパート終了 - GameSceneに戻ります");

        // 少し待ってから戻る
        Invoke(nameof(ReturnToGameScene), 0.5f);
    }

    /// <summary>
    /// GameSceneに戻る
    /// </summary>
    private void ReturnToGameScene()
    {
        // ノベルシーンデータをクリア
        NovelSceneData.Instance.Clear();

        // GameSceneに戻る
        SceneLoader.Instance.LoadScene(returnSceneName);
    }

    /// <summary>
    /// 強制終了（デバッグ用）
    /// </summary>
    public void ForceQuit()
    {
        Debug.Log("ノベルパートを強制終了");
        OnScenarioEnd();
    }
}

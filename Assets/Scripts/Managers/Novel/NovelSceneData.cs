using UnityEngine;

/// <summary>
/// NovelSceneに渡すデータを保持するクラス（DontDestroyOnLoad）
/// </summary>
public class NovelSceneData : MonoBehaviour
{
    private static NovelSceneData _instance;
    public static NovelSceneData Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("NovelSceneData");
                _instance = go.AddComponent<NovelSceneData>();
            }
            return _instance;
        }
    }

    /// <summary>
    /// 再生するシナリオのラベル
    /// </summary>
    public string ScenarioLabel { get; private set; }

    /// <summary>
    /// ノベル終了後に戻るシーン名
    /// </summary>
    public string ReturnSceneName { get; private set; }

    /// <summary>
    /// ノベルに関連するキャラクターID（友好度変化などに使用）
    /// </summary>
    public string TargetCharacterId { get; private set; }

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
    /// ノベルシーンデータを設定
    /// </summary>
    public void Setup(string scenarioLabel, string returnSceneName = "GameScene", string targetCharacterId = null)
    {
        this.ScenarioLabel = scenarioLabel;
        this.ReturnSceneName = returnSceneName;
        this.TargetCharacterId = targetCharacterId;

        Debug.Log($"NovelSceneData設定: シナリオ '{scenarioLabel}', 戻り先 '{returnSceneName}', キャラクター '{targetCharacterId}'");
    }

    /// <summary>
    /// データをクリア
    /// </summary>
    public void Clear()
    {
        ScenarioLabel = null;
        ReturnSceneName = null;
        TargetCharacterId = null;
    }
}

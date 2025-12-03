using UnityEngine;

/// <summary>
/// NovelBridge - 建築関連メソッド
/// </summary>
public partial class NovelBridge
{
    // ========== 建築関連 ==========

    /// <summary>
    /// 建築を開始（宴のコマンドから呼び出し）
    /// </summary>
    public bool StartConstruction(string constructionId)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return false;
        }

        // 既に建築中か確認
        if (GameManager.Instance.State.Construction.IsConstructing)
        {
            Debug.LogWarning("[NovelBridge] 既に建築中です");
            return false;
        }

        // 建築データを取得
        ConstructionData data = ConstructionDatabase.GetConstruction(constructionId);
        if (data == null)
        {
            Debug.LogWarning($"[NovelBridge] 建築物が見つかりません: {constructionId}");
            return false;
        }

        // 現在時刻を取得
        System.DateTime currentTime = GameManager.Instance.GlobalGameTime.CurrentTime;

        // 建築開始
        bool success = GameManager.Instance.State.Construction.StartConstruction(
            constructionId,
            currentTime,
            GameManager.Instance.State.Inventory
        );

        if (success)
        {
            Debug.Log($"[NovelBridge] 建築開始: {data.name} (完了まで {data.GetRequiredTimeText()})");
        }

        return success;
    }

    /// <summary>
    /// 建築中かどうか確認（宴の分岐条件で使用）
    /// </summary>
    public bool IsConstructing()
    {
        if (GameManager.Instance.State == null) return false;
        return GameManager.Instance.State.Construction.IsConstructing;
    }

    /// <summary>
    /// 建築をキャンセル（宴のコマンドから呼び出し）
    /// </summary>
    public void CancelConstruction()
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        GameManager.Instance.State.Construction.CancelConstruction();
        Debug.Log("[NovelBridge] 建築をキャンセルしました");
    }

    /// <summary>
    /// 現在の建築状況を取得（宴で表示）
    /// </summary>
    public string GetCurrentConstructionStatus()
    {
        if (GameManager.Instance.State == null) return "不明";

        System.DateTime currentTime = GameManager.Instance.GlobalGameTime.CurrentTime;
        return GameManager.Instance.State.Construction.GetCurrentConstructionSummary(currentTime);
    }

    /// <summary>
    /// 特定の建築物が既にマップに存在するか確認（宴の分岐条件で使用）
    /// </summary>
    public bool HasConstruction(string constructionId)
    {
        if (GameManager.Instance.State == null) return false;
        return GameManager.Instance.State.Map.HasConstruction(constructionId);
    }

    /// <summary>
    /// 建築に必要な素材を持っているか確認（宴の分岐条件で使用）
    /// </summary>
    public bool HasConstructionMaterials(string constructionId)
    {
        if (GameManager.Instance.State == null) return false;

        ConstructionData data = ConstructionDatabase.GetConstruction(constructionId);
        if (data == null) return false;

        // すべての素材を持っているか確認
        foreach (var material in data.requiredMaterials)
        {
            if (!GameManager.Instance.State.Inventory.HasItem(material.Key, material.Value))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 建築物の必要素材を取得（宴で表示）
    /// </summary>
    public string GetConstructionMaterials(string constructionId)
    {
        ConstructionData data = ConstructionDatabase.GetConstruction(constructionId);
        if (data == null) return "不明な建築物";

        string materials = "";
        foreach (var material in data.requiredMaterials)
        {
            Item item = ItemDatabase.GetItem(material.Key);
            string itemName = item != null ? item.name : material.Key;
            materials += $"{itemName} x{material.Value}, ";
        }

        return materials.TrimEnd(',', ' ');
    }

    /// <summary>
    /// 建築物の必要時間を取得（宴で表示）
    /// </summary>
    public string GetConstructionRequiredTime(string constructionId)
    {
        ConstructionData data = ConstructionDatabase.GetConstruction(constructionId);
        if (data == null) return "不明";

        return data.GetRequiredTimeText();
    }
}

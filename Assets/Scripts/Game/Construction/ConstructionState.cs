using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建築の進行状態を管理するクラス
/// </summary>
[Serializable]
public class ConstructionState
{
    /// <summary>
    /// 現在建築中の建築物（1つのみ）
    /// </summary>
    public OngoingConstruction currentConstruction;

    /// <summary>
    /// 建築中かどうか
    /// </summary>
    public bool IsConstructing => currentConstruction != null;

    /// <summary>
    /// 建築を開始
    /// </summary>
    /// <returns>建築開始に成功したか</returns>
    public bool StartConstruction(string constructionId, DateTime startTime, Inventory inventory)
    {
        // 既に建築中の場合は開始できない
        if (IsConstructing)
        {
            Debug.LogWarning("既に建築中です。完了するまで新しい建築は開始できません。");
            return false;
        }

        // 建築データを取得
        ConstructionData data = ConstructionDatabase.GetConstruction(constructionId);
        if (data == null)
        {
            Debug.LogWarning($"建築物 '{constructionId}' が見つかりません。");
            return false;
        }

        // 必要な素材があるか確認
        foreach (var material in data.requiredMaterials)
        {
            if (!inventory.HasItem(material.Key, material.Value))
            {
                Debug.LogWarning($"素材が足りません: {material.Key} x{material.Value}");
                return false;
            }
        }

        // 素材を消費
        foreach (var material in data.requiredMaterials)
        {
            inventory.RemoveItem(material.Key, material.Value);
        }

        // 建築開始
        currentConstruction = new OngoingConstruction(constructionId, startTime, data.requiredSeconds);
        Debug.Log($"建築開始: {data.name} (完了予定: {currentConstruction.CompleteTime})");
        return true;
    }

    /// <summary>
    /// 建築が完了しているかチェックし、完了していれば建築物IDを返す
    /// </summary>
    public string CheckAndCompleteConstruction(DateTime currentTime)
    {
        if (!IsConstructing)
        {
            return null;
        }

        if (currentConstruction.IsCompleted(currentTime))
        {
            string completedId = currentConstruction.constructionId;
            ConstructionData data = ConstructionDatabase.GetConstruction(completedId);
            Debug.Log($"建築完了: {data?.name ?? completedId}");

            currentConstruction = null;
            return completedId;
        }

        return null;
    }

    /// <summary>
    /// 建築をキャンセル（素材は返却されない）
    /// </summary>
    public void CancelConstruction()
    {
        if (IsConstructing)
        {
            ConstructionData data = ConstructionDatabase.GetConstruction(currentConstruction.constructionId);
            Debug.Log($"建築キャンセル: {data?.name ?? currentConstruction.constructionId}");
            currentConstruction = null;
        }
    }

    /// <summary>
    /// 現在の建築状況を取得
    /// </summary>
    public string GetCurrentConstructionSummary(DateTime currentTime)
    {
        if (!IsConstructing)
        {
            return "建築中の建築物はありません";
        }

        ConstructionData data = ConstructionDatabase.GetConstruction(currentConstruction.constructionId);
        if (data == null)
        {
            return "不明な建築物";
        }

        string summary = $"【建築中】{data.name}\n";
        summary += $"  {currentConstruction.GetRemainingTimeText(currentTime)}\n";
        summary += $"  進捗: {(currentConstruction.GetProgress(currentTime) * 100):F1}%";

        return summary;
    }

    /// <summary>
    /// セーブ用データに変換
    /// </summary>
    public SaveData.ConstructionStateData ToSaveData()
    {
        return new SaveData.ConstructionStateData
        {
            currentConstruction = currentConstruction
        };
    }

    /// <summary>
    /// セーブデータから復元
    /// </summary>
    public static ConstructionState FromSaveData(SaveData.ConstructionStateData data)
    {
        if (data == null)
        {
            return new ConstructionState();
        }

        return new ConstructionState
        {
            currentConstruction = data.currentConstruction
        };
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ライバルイベントの管理クラス
/// 時間経過で自動的にライバルイベントを発生させる
/// </summary>
public class RivalEventManager
{
    private Dictionary<string, RivalPair> rivalPairs; // key: mainCharacterId
    private List<RivalEvent> pendingEvents;            // 発生待ちのライバルイベント

    public RivalEventManager()
    {
        rivalPairs = new Dictionary<string, RivalPair>();
        pendingEvents = new List<RivalEvent>();
        InitializeRivalPairs();
    }

    /// <summary>
    /// ライバルペアを初期化
    /// </summary>
    private void InitializeRivalPairs()
    {
        // 3組のライバルペアを登録
        RegisterRivalPair("char_01", "rival_01"); // 職人 - ライバル1
        RegisterRivalPair("char_02", "rival_02"); // 料理人 - ライバル2
        RegisterRivalPair("char_03", "rival_03"); // 医者 - ライバル3
        // 注: char_04（科学者）はライバルキャラだがペアを組まない独立キャラ
    }

    /// <summary>
    /// ライバルペアを登録
    /// </summary>
    private void RegisterRivalPair(string mainCharacterId, string rivalCharacterId)
    {
        if (!rivalPairs.ContainsKey(mainCharacterId))
        {
            rivalPairs.Add(mainCharacterId, new RivalPair(mainCharacterId, rivalCharacterId));
        }
    }

    /// <summary>
    /// ライバルペアを取得
    /// </summary>
    public RivalPair GetRivalPair(string mainCharacterId)
    {
        if (rivalPairs.TryGetValue(mainCharacterId, out RivalPair pair))
        {
            return pair;
        }
        return null;
    }

    /// <summary>
    /// 全ライバルペアを取得
    /// </summary>
    public List<RivalPair> GetAllRivalPairs()
    {
        return rivalPairs.Values.ToList();
    }

    /// <summary>
    /// ライバルイベントをチェック（日付変更時に呼び出し）
    /// </summary>
    public List<RivalEvent> CheckForRivalEvents(GameState gameState)
    {
        List<RivalEvent> triggeredEvents = new List<RivalEvent>();

        // 全ライバルイベントをチェック
        List<RivalEvent> allRivalEvents = EventDatabase.GetAllRivalEvents();

        foreach (RivalEvent rivalEvent in allRivalEvents)
        {
            RivalPair pair = GetRivalPair(rivalEvent.mainCharacterId);
            if (pair == null) continue;

            if (rivalEvent.CanTrigger(gameState, pair))
            {
                triggeredEvents.Add(rivalEvent);
            }
        }

        return triggeredEvents;
    }

    /// <summary>
    /// ライバルイベントを進行させる
    /// </summary>
    public void AdvanceRivalEvent(string mainCharacterId, int stage)
    {
        RivalPair pair = GetRivalPair(mainCharacterId);
        if (pair == null) return;

        // イベント段階を更新
        pair.currentEventStage = stage;

        // 結婚段階の場合
        if (stage == 5)
        {
            pair.Marry();
            Debug.Log($"[RivalEventManager] {mainCharacterId} と {pair.rivalCharacterId} が結婚しました");
        }
        else
        {
            Debug.Log($"[RivalEventManager] {mainCharacterId} のライバルイベント段階 {stage} に進行");
        }
    }

    /// <summary>
    /// ライバルペアが結婚済みか
    /// </summary>
    public bool IsRivalMarried(string mainCharacterId)
    {
        RivalPair pair = GetRivalPair(mainCharacterId);
        return pair != null && pair.isMarried;
    }

    /// <summary>
    /// プレイヤーが結婚した時にライバルイベントを停止
    /// </summary>
    public void OnPlayerMarried(string mainCharacterId)
    {
        RivalPair pair = GetRivalPair(mainCharacterId);
        if (pair != null)
        {
            Debug.Log($"[RivalEventManager] プレイヤーが{mainCharacterId}と結婚したため、ライバルイベント停止");
        }
    }

    /// <summary>
    /// セーブデータからロード
    /// </summary>
    public void LoadFromSaveData(List<SaveData.RivalPairData> savedPairs)
    {
        if (savedPairs == null) return;

        foreach (var savedPair in savedPairs)
        {
            if (rivalPairs.TryGetValue(savedPair.mainCharacterId, out RivalPair pair))
            {
                pair.currentEventStage = savedPair.currentEventStage;
                pair.isMarried = savedPair.isMarried;
            }
        }
    }

    /// <summary>
    /// セーブデータに変換
    /// </summary>
    public List<SaveData.RivalPairData> ToSaveData()
    {
        List<SaveData.RivalPairData> savedPairs = new List<SaveData.RivalPairData>();

        foreach (var pair in rivalPairs.Values)
        {
            savedPairs.Add(new SaveData.RivalPairData(
                pair.mainCharacterId,
                pair.rivalCharacterId,
                pair.currentEventStage,
                pair.isMarried
            ));
        }

        return savedPairs;
    }

    /// <summary>
    /// サマリーを取得
    /// </summary>
    public string GetSummary()
    {
        string summary = "=== ライバル状況 ===\n";
        foreach (var pair in rivalPairs.Values)
        {
            summary += $"{pair.mainCharacterId}: {pair.GetStageDescription()}";
            if (pair.isMarried)
            {
                summary += " [ライバルと結婚済み]";
            }
            summary += "\n";
        }
        return summary;
    }
}

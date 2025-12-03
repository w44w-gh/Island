using UnityEngine;

/// <summary>
/// NovelBridge - ライバル関連メソッド
/// </summary>
public partial class NovelBridge
{
    // ========== ライバル関連 ==========

    /// <summary>
    /// ライバルキャラクターの友好度を増加（宴のコマンドから呼び出し）
    /// </summary>
    public void IncreaseRivalFriendship(string rivalCharacterId, int amount)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        var rivalCharacter = GameManager.Instance.State.Characters.GetRivalCharacter(rivalCharacterId);
        if (rivalCharacter != null)
        {
            rivalCharacter.IncreaseFriendship(amount);
            Debug.Log($"[NovelBridge] {rivalCharacter.name}の友好度を{amount}増加");
        }
        else
        {
            Debug.LogWarning($"ライバルキャラクター '{rivalCharacterId}' が見つかりません");
        }
    }

    /// <summary>
    /// ライバルキャラクターの友好度を取得（宴の分岐条件などで使用）
    /// </summary>
    public int GetRivalFriendship(string rivalCharacterId)
    {
        if (GameManager.Instance.State == null) return 0;

        var rivalCharacter = GameManager.Instance.State.Characters.GetRivalCharacter(rivalCharacterId);
        return rivalCharacter != null ? rivalCharacter.Friendship : 0;
    }

    /// <summary>
    /// ライバルペアが結婚済みか確認（宴の分岐条件で使用）
    /// </summary>
    public bool IsRivalMarried(string mainCharacterId)
    {
        if (GameManager.Instance.State == null) return false;

        return GameManager.Instance.State.RivalEvents.IsRivalMarried(mainCharacterId);
    }

    /// <summary>
    /// ライバルイベントの現在段階を取得（宴の分岐条件で使用）
    /// 0-5の数値で返す（0=未開始、1=ハート1、2=ハート2、3=ハート3、4=告白、5=結婚）
    /// </summary>
    public int GetRivalEventStage(string mainCharacterId)
    {
        if (GameManager.Instance.State == null) return 0;

        RivalPair pair = GameManager.Instance.State.RivalEvents.GetRivalPair(mainCharacterId);
        return pair != null ? pair.currentEventStage : 0;
    }

    /// <summary>
    /// ライバルイベントの段階説明を取得（宴で表示用）
    /// </summary>
    public string GetRivalEventStageDescription(string mainCharacterId)
    {
        if (GameManager.Instance.State == null) return "不明";

        RivalPair pair = GameManager.Instance.State.RivalEvents.GetRivalPair(mainCharacterId);
        return pair != null ? pair.GetStageDescription() : "不明";
    }

    /// <summary>
    /// 攻略キャラクターが結婚可能か（プレイヤーまたはライバルと結婚していない）
    /// </summary>
    public bool IsCharacterAvailableForMarriage(string characterId)
    {
        if (GameManager.Instance.State == null) return false;

        // プレイヤーと結婚済みか
        if (GameManager.Instance.State.IsMarried && GameManager.Instance.State.MarriedCharacterId == characterId)
        {
            return false;
        }

        // ライバルと結婚済みか
        if (IsRivalMarried(characterId))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// ライバルイベントを手動で進行（デバッグ用）
    /// </summary>
    public void AdvanceRivalEventStage(string mainCharacterId, int stage)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        GameManager.Instance.State.RivalEvents.AdvanceRivalEvent(mainCharacterId, stage);
        Debug.Log($"[NovelBridge] ライバルイベント段階を{stage}に設定");
    }

    /// <summary>
    /// ライバルペアの名前を取得（宴で表示用）
    /// </summary>
    public string GetRivalCharacterName(string mainCharacterId)
    {
        if (GameManager.Instance.State == null) return "";

        RivalPair pair = GameManager.Instance.State.RivalEvents.GetRivalPair(mainCharacterId);
        if (pair == null) return "";

        var rivalCharacter = GameManager.Instance.State.Characters.GetRivalCharacter(pair.rivalCharacterId);
        return rivalCharacter != null ? rivalCharacter.name : "";
    }
}

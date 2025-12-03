using System;

/// <summary>
/// ライバルペアの定義
/// 攻略キャラとライバルキャラのペアを管理
/// </summary>
[Serializable]
public class RivalPair
{
    public string mainCharacterId;      // 攻略可能キャラクターID
    public string rivalCharacterId;     // ライバルキャラクターID
    public int currentEventStage;       // 現在のライバルイベント段階（0-5）
    public bool isMarried;              // ライバルペアが結婚済みか

    public RivalPair(string mainCharacterId, string rivalCharacterId)
    {
        this.mainCharacterId = mainCharacterId;
        this.rivalCharacterId = rivalCharacterId;
        this.currentEventStage = 0;
        this.isMarried = false;
    }

    /// <summary>
    /// ライバルイベントを進行
    /// </summary>
    public void AdvanceEventStage()
    {
        if (currentEventStage < 5 && !isMarried)
        {
            currentEventStage++;
        }
    }

    /// <summary>
    /// ライバルペアを結婚させる
    /// </summary>
    public void Marry()
    {
        isMarried = true;
        currentEventStage = 5;
    }

    /// <summary>
    /// イベント進行可能か（攻略キャラがプレイヤーと結婚していない場合のみ）
    /// </summary>
    public bool CanAdvance(GameState gameState)
    {
        // 既に結婚済み
        if (isMarried) return false;

        // プレイヤーが攻略キャラと結婚している場合は進行不可
        if (gameState.IsMarried && gameState.MarriedCharacterId == mainCharacterId)
        {
            return false;
        }

        // 既に最終段階
        if (currentEventStage >= 5) return false;

        return true;
    }

    /// <summary>
    /// 現在の段階の説明を取得
    /// </summary>
    public string GetStageDescription()
    {
        switch (currentEventStage)
        {
            case 0: return "未開始";
            case 1: return "ハート1";
            case 2: return "ハート2";
            case 3: return "ハート3";
            case 4: return "告白";
            case 5: return isMarried ? "結婚" : "結婚直前";
            default: return "不明";
        }
    }
}

using System;

/// <summary>
/// ライバルイベントの定義
/// 時間経過で自動的に発生するイベント
/// </summary>
[Serializable]
public class RivalEvent
{
    public string eventId;              // イベントID
    public string eventName;            // イベント名
    public string description;          // イベント説明
    public string scenarioLabel;        // 宴のシナリオラベル
    public string mainCharacterId;      // 攻略キャラクターID
    public string rivalCharacterId;     // ライバルキャラクターID
    public int eventStage;              // イベント段階（1-5: ハート1/2/3/告白/結婚）
    public int triggerDay;              // 発生日（ゲーム開始からの経過日数）

    public RivalEvent(
        string eventId,
        string eventName,
        string description,
        string scenarioLabel,
        string mainCharacterId,
        string rivalCharacterId,
        int eventStage,
        int triggerDay)
    {
        this.eventId = eventId;
        this.eventName = eventName;
        this.description = description;
        this.scenarioLabel = scenarioLabel;
        this.mainCharacterId = mainCharacterId;
        this.rivalCharacterId = rivalCharacterId;
        this.eventStage = eventStage;
        this.triggerDay = triggerDay;
    }

    /// <summary>
    /// イベントが発生可能かチェック
    /// </summary>
    public bool CanTrigger(GameState gameState, RivalPair pair)
    {
        // 現在の日数が発生日以上
        if (gameState.CurrentDay < triggerDay) return false;

        // プレイヤーが攻略キャラと結婚している場合は発生しない
        if (gameState.IsMarried && gameState.MarriedCharacterId == mainCharacterId)
        {
            return false;
        }

        // ライバルペアが既に結婚済みの場合は発生しない
        if (pair.isMarried) return false;

        // 前のイベントが完了しているか
        if (pair.currentEventStage != eventStage - 1) return false;

        return true;
    }
}

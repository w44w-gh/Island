using System;
using UnityEngine;

/// <summary>
/// 特別な会話イベントの定義
/// </summary>
[Serializable]
public class SpecialEvent
{
    /// <summary>
    /// イベントID（一意）
    /// </summary>
    public string eventId;

    /// <summary>
    /// イベント名
    /// </summary>
    public string eventName;

    /// <summary>
    /// イベントの説明
    /// </summary>
    public string description;

    /// <summary>
    /// 優先度（高いほど優先、同じ優先度の場合は登録順）
    /// </summary>
    public int priority;

    /// <summary>
    /// 宴のシナリオラベル
    /// </summary>
    public string scenarioLabel;

    /// <summary>
    /// イベント発生条件
    /// </summary>
    public EventCondition condition;

    /// <summary>
    /// 一度だけ発生するか（現状は全て一度だけなのでtrue固定）
    /// </summary>
    public bool onceOnly = true;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SpecialEvent(
        string eventId,
        string eventName,
        string description,
        string scenarioLabel,
        int priority,
        EventCondition condition)
    {
        this.eventId = eventId;
        this.eventName = eventName;
        this.description = description;
        this.scenarioLabel = scenarioLabel;
        this.priority = priority;
        this.condition = condition ?? new EventCondition();
        this.onceOnly = true;
    }

    /// <summary>
    /// このイベントが発生可能かチェック
    /// </summary>
    public bool CanTrigger(GameState gameState, string characterId = null, MapLocation? location = null, EventManager eventManager = null)
    {
        // 既に発生済みかチェック
        if (onceOnly && eventManager != null && eventManager.HasTriggered(eventId))
        {
            return false;
        }

        // 前提イベントをチェック
        if (condition.prerequisiteEvents != null && eventManager != null)
        {
            foreach (var prereqEventId in condition.prerequisiteEvents)
            {
                if (!eventManager.HasTriggered(prereqEventId))
                {
                    return false;
                }
            }
        }

        // 条件チェック
        return condition.CheckCondition(gameState, characterId, location);
    }

    /// <summary>
    /// イベント情報のサマリー
    /// </summary>
    public string GetSummary()
    {
        return $"[{eventId}] {eventName} (優先度: {priority})\n  条件: {condition.GetConditionDescription()}\n  シナリオ: {scenarioLabel}";
    }
}

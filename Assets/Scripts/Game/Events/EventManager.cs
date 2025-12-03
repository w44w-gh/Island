using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// イベント管理クラス
/// 発生済みイベントの記録と、条件チェックを行う
/// </summary>
[Serializable]
public class EventManager
{
    /// <summary>
    /// 発生済みイベントIDのセット
    /// </summary>
    private HashSet<string> triggeredEvents;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EventManager()
    {
        triggeredEvents = new HashSet<string>();
        Debug.Log("EventManager初期化完了");
    }

    /// <summary>
    /// イベントが発生済みかチェック
    /// </summary>
    public bool HasTriggered(string eventId)
    {
        return triggeredEvents.Contains(eventId);
    }

    /// <summary>
    /// イベントを発生済みとしてマーク
    /// </summary>
    public void MarkAsTriggered(string eventId)
    {
        if (!triggeredEvents.Contains(eventId))
        {
            triggeredEvents.Add(eventId);
            Debug.Log($"[EventManager] イベント発生済みとしてマーク: {eventId}");
        }
    }

    /// <summary>
    /// 発生可能なイベントをチェック（キャラクター会話時）
    /// 優先度順に1つだけ返す
    /// </summary>
    public SpecialEvent CheckForCharacterEvent(GameState gameState, string characterId)
    {
        List<SpecialEvent> allEvents = EventDatabase.GetEventsForCharacter(characterId);

        foreach (var ev in allEvents)
        {
            if (ev.CanTrigger(gameState, characterId, null, this))
            {
                Debug.Log($"[EventManager] キャラクターイベント発生: {ev.eventName} ({ev.eventId})");
                return ev;
            }
        }

        return null;
    }

    /// <summary>
    /// 発生可能なイベントをチェック（場所移動時）
    /// 優先度順に1つだけ返す
    /// </summary>
    public SpecialEvent CheckForLocationEvent(GameState gameState, MapLocation location)
    {
        List<SpecialEvent> allEvents = EventDatabase.GetEventsForLocation(location);

        foreach (var ev in allEvents)
        {
            if (ev.CanTrigger(gameState, null, location, this))
            {
                Debug.Log($"[EventManager] 場所イベント発生: {ev.eventName} ({ev.eventId}) at {location.ToJapaneseString()}");
                return ev;
            }
        }

        return null;
    }

    /// <summary>
    /// 全イベントから条件を満たすものをチェック
    /// 優先度順に1つだけ返す（汎用チェック用）
    /// </summary>
    public SpecialEvent CheckForAnyEvent(GameState gameState, string characterId = null, MapLocation? location = null)
    {
        List<SpecialEvent> allEvents = EventDatabase.GetAllEvents();

        foreach (var ev in allEvents)
        {
            if (ev.CanTrigger(gameState, characterId, location, this))
            {
                Debug.Log($"[EventManager] イベント発生: {ev.eventName} ({ev.eventId})");
                return ev;
            }
        }

        return null;
    }

    /// <summary>
    /// 発生済みイベントの数を取得
    /// </summary>
    public int GetTriggeredEventCount()
    {
        return triggeredEvents.Count;
    }

    /// <summary>
    /// 発生済みイベントのリストを取得
    /// </summary>
    public List<string> GetTriggeredEventIds()
    {
        return new List<string>(triggeredEvents);
    }

    /// <summary>
    /// デバッグ用のサマリー
    /// </summary>
    public string GetSummary()
    {
        string summary = $"【イベント管理】\n";
        summary += $"  発生済みイベント: {triggeredEvents.Count}件\n";

        if (triggeredEvents.Count > 0)
        {
            summary += "  発生済みイベント一覧:\n";
            foreach (var eventId in triggeredEvents)
            {
                SpecialEvent ev = EventDatabase.GetEvent(eventId);
                string eventName = ev != null ? ev.eventName : eventId;
                summary += $"    - {eventName} ({eventId})\n";
            }
        }

        return summary;
    }

    /// <summary>
    /// セーブ用データに変換
    /// </summary>
    public SaveData.EventManagerData ToSaveData()
    {
        return new SaveData.EventManagerData
        {
            triggeredEvents = new List<string>(triggeredEvents)
        };
    }

    /// <summary>
    /// セーブデータから復元
    /// </summary>
    public static EventManager FromSaveData(SaveData.EventManagerData data)
    {
        EventManager manager = new EventManager();

        if (data != null && data.triggeredEvents != null)
        {
            foreach (var eventId in data.triggeredEvents)
            {
                manager.triggeredEvents.Add(eventId);
            }
            Debug.Log($"EventManager: {data.triggeredEvents.Count}件の発生済みイベントを復元");
        }

        return manager;
    }

    /// <summary>
    /// 発生済みフラグをクリア（デバッグ用）
    /// </summary>
    public void ClearAllTriggeredEvents()
    {
        triggeredEvents.Clear();
        Debug.Log("[EventManager] 全ての発生済みフラグをクリアしました");
    }
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特別イベントのデータベース（メイン）
/// partial classでキャラクター別に分割されています
/// </summary>
public static partial class EventDatabase
{
    private static Dictionary<string, SpecialEvent> events = new Dictionary<string, SpecialEvent>();
    private static List<SpecialEvent> eventList = new List<SpecialEvent>(); // 優先度順にソートされたリスト

    private static Dictionary<string, RivalEvent> rivalEvents = new Dictionary<string, RivalEvent>();
    private static List<RivalEvent> rivalEventList = new List<RivalEvent>(); // 発生日順にソートされたリスト

    /// <summary>
    /// イベントデータベースの初期化
    /// </summary>
    public static void Initialize()
    {
        if (events.Count > 0) return; // 既に初期化済み

        // 各キャラクターのイベントを登録（partial classのメソッドを呼び出し）
        RegisterCraftsmanEvents();
        RegisterCookEvents();
        RegisterDoctorEvents();
        RegisterScientistEvents(); // 科学者はライバルキャラだが特別イベントあり
        RegisterLocationEvents();
        RegisterConstructionEvents();
        RegisterComplexEvents();

        Debug.Log($"EventDatabase initialized - {events.Count} events registered");

        // 優先度順にソート
        SortEventsByPriority();

        // ライバルイベントを登録
        RegisterRivalEvents();
    }

    /// <summary>
    /// イベントを登録
    /// </summary>
    private static void RegisterEvent(SpecialEvent specialEvent)
    {
        if (events.ContainsKey(specialEvent.eventId))
        {
            Debug.LogWarning($"Event ID '{specialEvent.eventId}' is already registered. Skipping.");
            return;
        }
        events.Add(specialEvent.eventId, specialEvent);
        eventList.Add(specialEvent);
    }

    /// <summary>
    /// ライバルイベントを登録
    /// </summary>
    private static void RegisterRivalEvent(RivalEvent rivalEvent)
    {
        if (rivalEvents.ContainsKey(rivalEvent.eventId))
        {
            Debug.LogWarning($"Rival Event ID '{rivalEvent.eventId}' is already registered. Skipping.");
            return;
        }
        rivalEvents.Add(rivalEvent.eventId, rivalEvent);
        rivalEventList.Add(rivalEvent);
    }

    /// <summary>
    /// イベントを優先度順にソート
    /// </summary>
    private static void SortEventsByPriority()
    {
        eventList.Sort((a, b) => b.priority.CompareTo(a.priority)); // 降順（優先度が高い順）
    }

    /// <summary>
    /// イベントIDからイベントを取得
    /// </summary>
    public static SpecialEvent GetEvent(string eventId)
    {
        if (events.TryGetValue(eventId, out SpecialEvent specialEvent))
        {
            return specialEvent;
        }

        Debug.LogWarning($"Event '{eventId}' not found in database.");
        return null;
    }

    /// <summary>
    /// 全イベントを取得（優先度順）
    /// </summary>
    public static List<SpecialEvent> GetAllEvents()
    {
        return eventList;
    }

    /// <summary>
    /// 特定キャラクター向けのイベントを取得
    /// </summary>
    public static List<SpecialEvent> GetEventsForCharacter(string characterId)
    {
        List<SpecialEvent> result = new List<SpecialEvent>();
        foreach (var ev in eventList)
        {
            if (ev.condition.characterId == characterId || string.IsNullOrEmpty(ev.condition.characterId))
            {
                result.Add(ev);
            }
        }
        return result;
    }

    /// <summary>
    /// 特定の場所向けのイベントを取得
    /// </summary>
    public static List<SpecialEvent> GetEventsForLocation(MapLocation location)
    {
        List<SpecialEvent> result = new List<SpecialEvent>();
        foreach (var ev in eventList)
        {
            if (ev.condition.requiredLocation == location || !ev.condition.requiredLocation.HasValue)
            {
                result.Add(ev);
            }
        }
        return result;
    }

    /// <summary>
    /// ライバルイベントIDからライバルイベントを取得
    /// </summary>
    public static RivalEvent GetRivalEvent(string eventId)
    {
        if (rivalEvents.TryGetValue(eventId, out RivalEvent rivalEvent))
        {
            return rivalEvent;
        }

        Debug.LogWarning($"Rival Event '{eventId}' not found in database.");
        return null;
    }

    /// <summary>
    /// 全ライバルイベントを取得
    /// </summary>
    public static List<RivalEvent> GetAllRivalEvents()
    {
        return rivalEventList;
    }

    /// <summary>
    /// 特定キャラクター向けのライバルイベントを取得
    /// </summary>
    public static List<RivalEvent> GetRivalEventsForCharacter(string mainCharacterId)
    {
        List<RivalEvent> result = new List<RivalEvent>();
        foreach (var rivalEvent in rivalEventList)
        {
            if (rivalEvent.mainCharacterId == mainCharacterId)
            {
                result.Add(rivalEvent);
            }
        }
        return result;
    }
}

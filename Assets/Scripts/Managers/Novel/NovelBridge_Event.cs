using UnityEngine;

/// <summary>
/// NovelBridge - イベント関連メソッド
/// </summary>
public partial class NovelBridge
{
    // ========== イベント関連 ==========

    /// <summary>
    /// キャラクターとの会話時に特別イベントをチェック（宴のコマンドから呼び出し）
    /// イベントが発生する場合はシナリオラベルを返す
    /// </summary>
    public string CheckCharacterEvent(string characterId)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return null;
        }

        SpecialEvent ev = GameManager.Instance.State.Events.CheckForCharacterEvent(
            GameManager.Instance.State,
            characterId
        );

        if (ev != null)
        {
            Debug.Log($"[NovelBridge] キャラクターイベント: {ev.eventName} ({ev.scenarioLabel})");
            return ev.scenarioLabel;
        }

        return null;
    }

    /// <summary>
    /// 場所移動時に特別イベントをチェック（宴のコマンドから呼び出し）
    /// イベントが発生する場合はシナリオラベルを返す
    /// </summary>
    public string CheckLocationEvent(string locationName)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return null;
        }

        // 場所名をMapLocationに変換
        if (!System.Enum.TryParse(locationName, out MapLocation location))
        {
            Debug.LogWarning($"[NovelBridge] 不明な場所: {locationName}");
            return null;
        }

        SpecialEvent ev = GameManager.Instance.State.Events.CheckForLocationEvent(
            GameManager.Instance.State,
            location
        );

        if (ev != null)
        {
            Debug.Log($"[NovelBridge] 場所イベント: {ev.eventName} ({ev.scenarioLabel})");
            return ev.scenarioLabel;
        }

        return null;
    }

    /// <summary>
    /// イベントを発生済みとしてマーク（宴のシナリオ終了時に呼び出し）
    /// </summary>
    public void MarkEventAsTriggered(string eventId)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        GameManager.Instance.State.Events.MarkAsTriggered(eventId);
        Debug.Log($"[NovelBridge] イベント発生済みとしてマーク: {eventId}");
    }

    /// <summary>
    /// イベントが発生済みか確認（宴の分岐条件で使用）
    /// </summary>
    public bool HasTriggeredEvent(string eventId)
    {
        if (GameManager.Instance.State == null) return false;

        return GameManager.Instance.State.Events.HasTriggered(eventId);
    }

    /// <summary>
    /// 発生可能なイベントをチェックしてシナリオラベルとイベントIDを取得
    /// キャラクター会話時に使用（イベントIDも返すことで、シナリオ側で自動マーク可能に）
    /// </summary>
    public (string scenarioLabel, string eventId) CheckAndGetCharacterEvent(string characterId)
    {
        if (GameManager.Instance.State == null)
        {
            return (null, null);
        }

        SpecialEvent ev = GameManager.Instance.State.Events.CheckForCharacterEvent(
            GameManager.Instance.State,
            characterId
        );

        if (ev != null)
        {
            Debug.Log($"[NovelBridge] キャラクターイベント: {ev.eventName} (ID: {ev.eventId}, Label: {ev.scenarioLabel})");
            return (ev.scenarioLabel, ev.eventId);
        }

        return (null, null);
    }

    /// <summary>
    /// 発生可能なイベントをチェックしてシナリオラベルとイベントIDを取得
    /// 場所移動時に使用
    /// </summary>
    public (string scenarioLabel, string eventId) CheckAndGetLocationEvent(string locationName)
    {
        if (GameManager.Instance.State == null)
        {
            return (null, null);
        }

        // 場所名をMapLocationに変換
        if (!System.Enum.TryParse(locationName, out MapLocation location))
        {
            Debug.LogWarning($"[NovelBridge] 不明な場所: {locationName}");
            return (null, null);
        }

        SpecialEvent ev = GameManager.Instance.State.Events.CheckForLocationEvent(
            GameManager.Instance.State,
            location
        );

        if (ev != null)
        {
            Debug.Log($"[NovelBridge] 場所イベント: {ev.eventName} (ID: {ev.eventId}, Label: {ev.scenarioLabel})");
            return (ev.scenarioLabel, ev.eventId);
        }

        return (null, null);
    }
}

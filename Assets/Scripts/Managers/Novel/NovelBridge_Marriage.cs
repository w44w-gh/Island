using UnityEngine;

/// <summary>
/// NovelBridge - 結婚関連メソッド
/// </summary>
public partial class NovelBridge
{
    // ========== 結婚関連 ==========

    /// <summary>
    /// キャラクターと結婚する（宴のコマンドから呼び出し）
    /// プロポーズイベント内で使用
    /// </summary>
    public bool MarryCharacter(string characterId)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return false;
        }

        bool success = GameManager.Instance.State.MarryCharacter(characterId);
        if (success)
        {
            // 青い羽を消費
            GameManager.Instance.State.Inventory.RemoveItem("blue_feather", 1);
            Debug.Log($"[NovelBridge] {characterId}と結婚しました");
        }

        return success;
    }

    /// <summary>
    /// 結婚しているか確認（宴の分岐条件で使用）
    /// </summary>
    public bool IsMarried()
    {
        if (GameManager.Instance.State == null) return false;

        return GameManager.Instance.State.IsMarried;
    }

    /// <summary>
    /// 特定のキャラクターと結婚しているか確認（宴の分岐条件で使用）
    /// </summary>
    public bool IsMarriedTo(string characterId)
    {
        if (GameManager.Instance.State == null) return false;

        return GameManager.Instance.State.MarriedCharacterId == characterId;
    }

    /// <summary>
    /// 結婚相手のキャラクターIDを取得（宴で使用）
    /// </summary>
    public string GetMarriedCharacterId()
    {
        if (GameManager.Instance.State == null) return null;

        return GameManager.Instance.State.MarriedCharacterId;
    }

    /// <summary>
    /// 結婚相手の名前を取得（宴で表示）
    /// </summary>
    public string GetMarriedCharacterName()
    {
        if (GameManager.Instance.State == null || !GameManager.Instance.State.IsMarried)
        {
            return "";
        }

        Character marriedCharacter = GameManager.Instance.State.MarriedCharacter;
        return marriedCharacter != null ? marriedCharacter.name : "";
    }

    /// <summary>
    /// キャラクターが結婚可能か確認（宴の分岐条件で使用）
    /// </summary>
    public bool CanMarryCharacter(string characterId)
    {
        if (GameManager.Instance.State == null) return false;

        // 既に誰かと結婚している場合は不可
        if (GameManager.Instance.State.IsMarried) return false;

        Character character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        if (character == null) return false;

        return character.CanMarry();
    }

    /// <summary>
    /// プロポーズイベントが発生可能か詳細にチェック（宴の分岐条件で使用）
    /// 全てのハートイベント、告白イベント、その他条件をチェック
    /// </summary>
    public bool CanProposeToCharacter(string characterId)
    {
        if (GameManager.Instance.State == null) return false;

        // 既に誰かと結婚している場合は不可
        if (GameManager.Instance.State.IsMarried) return false;

        // キャラクター取得
        Character character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        if (character == null) return false;

        // 恋愛度100必須
        if (character.Romance < 100) return false;

        // 青い羽を持っているか
        if (!GameManager.Instance.State.Inventory.HasItem("blue_feather", 1)) return false;

        // 大きな家を建てているか
        if (!GameManager.Instance.State.Map.HasConstruction("large_house")) return false;

        // 全てのハートイベントと告白イベントが発生済みか
        string[] requiredEvents = new string[]
        {
            $"{characterId}_heart_1",
            $"{characterId}_heart_2",
            $"{characterId}_heart_3",
            $"{characterId}_confession"
        };

        foreach (string eventId in requiredEvents)
        {
            if (!GameManager.Instance.State.Events.HasTriggered(eventId))
            {
                Debug.Log($"[NovelBridge] プロポーズ不可: {eventId} が未発生");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 次に見るべきハートイベントを取得（宴で表示用）
    /// </summary>
    public string GetNextHeartEventName(string characterId)
    {
        if (GameManager.Instance.State == null) return "不明";

        // ハート1が未発生
        if (!GameManager.Instance.State.Events.HasTriggered($"{characterId}_heart_1"))
        {
            return "ハート1イベント";
        }
        // ハート2が未発生
        if (!GameManager.Instance.State.Events.HasTriggered($"{characterId}_heart_2"))
        {
            return "ハート2イベント";
        }
        // ハート3が未発生
        if (!GameManager.Instance.State.Events.HasTriggered($"{characterId}_heart_3"))
        {
            return "ハート3イベント";
        }
        // 告白が未発生
        if (!GameManager.Instance.State.Events.HasTriggered($"{characterId}_confession"))
        {
            return "告白イベント";
        }

        return "全イベント完了";
    }

    /// <summary>
    /// ハートイベントの進行状況を取得（宴で表示用）
    /// 0-4の数値で返す（0=未開始、1=ハート1完了、2=ハート2完了、3=ハート3完了、4=告白完了）
    /// </summary>
    public int GetHeartEventProgress(string characterId)
    {
        if (GameManager.Instance.State == null) return 0;

        if (GameManager.Instance.State.Events.HasTriggered($"{characterId}_confession"))
        {
            return 4; // 告白完了
        }
        if (GameManager.Instance.State.Events.HasTriggered($"{characterId}_heart_3"))
        {
            return 3; // ハート3完了
        }
        if (GameManager.Instance.State.Events.HasTriggered($"{characterId}_heart_2"))
        {
            return 2; // ハート2完了
        }
        if (GameManager.Instance.State.Events.HasTriggered($"{characterId}_heart_1"))
        {
            return 1; // ハート1完了
        }

        return 0; // 未開始
    }
}

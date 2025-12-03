using System;
using System.Collections.Generic;

/// <summary>
/// イベント発生条件
/// </summary>
[Serializable]
public class EventCondition
{
    // キャラクター条件
    public string characterId;              // 対象キャラクターID（nullなら全キャラ対象）
    public int minFriendship = -1;          // 最小友好度（-1なら条件なし）
    public int minRomance = -1;             // 最小恋愛度（-1なら条件なし）

    // 天候条件
    public WeatherType? requiredWeather;    // 必要な天候（nullなら条件なし）

    // 時間帯条件
    public TimeOfDay? requiredTimeOfDay;    // 必要な時間帯（nullなら条件なし）

    // 日数条件
    public int minDay = -1;                 // 最小日数（-1なら条件なし）

    // アイテム条件
    public List<ItemRequirement> requiredItems;     // 必要なアイテム

    // 建築物条件
    public List<string> requiredConstructions;      // 必要な建築物ID

    // 場所条件
    public MapLocation? requiredLocation;   // 必要な場所（nullなら条件なし）

    // 前提イベント条件
    public List<string> prerequisiteEvents; // 前提となるイベントID（これらが全て発生済みである必要がある）

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EventCondition()
    {
        requiredItems = new List<ItemRequirement>();
        requiredConstructions = new List<string>();
        prerequisiteEvents = new List<string>();
    }

    /// <summary>
    /// 条件を満たしているかチェック
    /// </summary>
    public bool CheckCondition(GameState gameState, string targetCharacterId = null, MapLocation? currentLocation = null)
    {
        // キャラクター指定がある場合、一致するかチェック
        if (!string.IsNullOrEmpty(characterId) && characterId != targetCharacterId)
        {
            return false;
        }

        // キャラクター関連の条件チェック
        if (!string.IsNullOrEmpty(targetCharacterId))
        {
            Character character = gameState.Characters.GetCharacter(targetCharacterId);
            if (character == null) return false;

            // 友好度チェック
            if (minFriendship >= 0 && character.Friendship < minFriendship)
            {
                return false;
            }

            // 恋愛度チェック
            if (minRomance >= 0 && character.Romance < minRomance)
            {
                return false;
            }
        }

        // 天候チェック
        if (requiredWeather.HasValue && gameState.CurrentWeather != requiredWeather.Value)
        {
            return false;
        }

        // 時間帯チェック
        if (requiredTimeOfDay.HasValue && gameState.Time.CurrentTimeOfDay != requiredTimeOfDay.Value)
        {
            return false;
        }

        // 日数チェック
        if (minDay >= 0 && gameState.CurrentDay < minDay)
        {
            return false;
        }

        // アイテムチェック
        foreach (var itemReq in requiredItems)
        {
            if (!gameState.Inventory.HasItem(itemReq.itemId, itemReq.quantity))
            {
                return false;
            }
        }

        // 建築物チェック
        foreach (var constructionId in requiredConstructions)
        {
            if (!gameState.Map.HasConstruction(constructionId))
            {
                return false;
            }
        }

        // 場所チェック
        if (requiredLocation.HasValue && currentLocation.HasValue && currentLocation.Value != requiredLocation.Value)
        {
            return false;
        }

        // 前提イベントチェック（EventManagerで実施）
        // ここではチェックせず、EventManager側で確認

        return true;
    }

    /// <summary>
    /// 条件の説明文を生成（デバッグ用）
    /// </summary>
    public string GetConditionDescription()
    {
        List<string> conditions = new List<string>();

        if (!string.IsNullOrEmpty(characterId))
        {
            conditions.Add($"キャラクター: {characterId}");
        }

        if (minFriendship >= 0)
        {
            conditions.Add($"友好度{minFriendship}以上");
        }

        if (minRomance >= 0)
        {
            conditions.Add($"恋愛度{minRomance}以上");
        }

        if (requiredWeather.HasValue)
        {
            conditions.Add($"天候: {requiredWeather.Value.ToJapaneseString()}");
        }

        if (requiredTimeOfDay.HasValue)
        {
            conditions.Add($"時間帯: {requiredTimeOfDay.Value.ToJapaneseString()}");
        }

        if (minDay >= 0)
        {
            conditions.Add($"{minDay}日目以降");
        }

        if (requiredItems.Count > 0)
        {
            foreach (var item in requiredItems)
            {
                conditions.Add($"アイテム: {item.itemId} x{item.quantity}");
            }
        }

        if (requiredConstructions.Count > 0)
        {
            foreach (var construction in requiredConstructions)
            {
                conditions.Add($"建築物: {construction}");
            }
        }

        if (requiredLocation.HasValue)
        {
            conditions.Add($"場所: {requiredLocation.Value.ToJapaneseString()}");
        }

        if (prerequisiteEvents.Count > 0)
        {
            conditions.Add($"前提イベント: {prerequisiteEvents.Count}件");
        }

        return string.Join(", ", conditions);
    }
}

/// <summary>
/// アイテム必要条件
/// </summary>
[Serializable]
public class ItemRequirement
{
    public string itemId;
    public int quantity;

    public ItemRequirement(string itemId, int quantity = 1)
    {
        this.itemId = itemId;
        this.quantity = quantity;
    }
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムのデータベース（食料を中心とした定義）
/// </summary>
public static class ItemDatabase
{
    private static Dictionary<string, Item> items = new Dictionary<string, Item>();

    /// <summary>
    /// アイテムデータベースの初期化
    /// </summary>
    public static void Initialize()
    {
        if (items.Count > 0) return; // 既に初期化済み

        // 各カテゴリのアイテムを登録
        FoodItems.Register(items);
        CookingItems.Register(items);
        MaterialItems.Register(items);
        ToolItems.Register(items);
        RecipeItems.Register(items);
        EquipmentItems.Register(items);
        KeyItems.Register(items);

        Debug.Log($"ItemDatabase initialized - {items.Count} items registered");
    }

    /// <summary>
    /// アイテムIDからアイテムを取得
    /// </summary>
    public static Item GetItem(string itemId)
    {
        if (items.TryGetValue(itemId, out Item item))
        {
            return item;
        }

        Debug.LogWarning($"Item '{itemId}' not found in database.");
        return null;
    }

    /// <summary>
    /// 全アイテムを取得
    /// </summary>
    public static IEnumerable<Item> GetAllItems()
    {
        return items.Values;
    }

    /// <summary>
    /// 特定タイプのアイテムを取得
    /// </summary>
    public static IEnumerable<Item> GetItemsByType(ItemType type)
    {
        List<Item> result = new List<Item>();
        foreach (var item in items.Values)
        {
            if (item.type == type)
            {
                result.Add(item);
            }
        }
        return result;
    }
}

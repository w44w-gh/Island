using System.Collections.Generic;

/// <summary>
/// 道具アイテムの定義
/// </summary>
public static class ToolItems
{
    public static void Register(Dictionary<string, Item> items)
    {
        // 釣竿
        RegisterItem(items, new Item(
            "fishing_rod",
            "釣竿",
            "魚を釣るための道具。職人が作ってくれる。",
            ItemType.Tool,
            maxStack: 1
        ));

        // フライパン
        RegisterItem(items, new Item(
            "frying_pan",
            "フライパン",
            "料理に使うフライパン。職人が作ってくれる。",
            ItemType.Tool,
            maxStack: 1
        ));
    }

    private static void RegisterItem(Dictionary<string, Item> items, Item item)
    {
        if (items.ContainsKey(item.id))
        {
            UnityEngine.Debug.LogWarning($"Item ID '{item.id}' is already registered. Skipping.");
            return;
        }
        items.Add(item.id, item);
    }
}

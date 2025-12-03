using System.Collections.Generic;

/// <summary>
/// 料理アイテムの定義
/// </summary>
public static class CookingItems
{
    public static void Register(Dictionary<string, Item> items)
    {
        // 焼き魚
        RegisterItem(items, new Consumable(
            "grilled_fish",
            "焼き魚",
            "焼いた魚。香ばしくて美味しい。",
            ItemType.Cooking,
            hpRestore: 20,
            staminaRestore: 10,
            maxStack: 99
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

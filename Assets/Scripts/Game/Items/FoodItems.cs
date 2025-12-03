using System.Collections.Generic;

/// <summary>
/// 食料アイテムの定義
/// </summary>
public static class FoodItems
{
    public static void Register(Dictionary<string, Item> items)
    {
        // 魚
        RegisterItem(items, new Consumable(
            "fish",
            "さかな",
            "海で釣れた新鮮な魚。そのまま食べるか、焼いて食べる。",
            ItemType.Food,
            hpRestore: 10,
            staminaRestore: 5,
            maxStack: 99
        ));

        // きのみ
        RegisterItem(items, new Consumable(
            "berry",
            "きのみ",
            "島で採れる甘酸っぱい実。少し空腹を満たす。",
            ItemType.Food,
            hpRestore: 5,
            staminaRestore: 3,
            maxStack: 99
        ));

        // 水
        RegisterItem(items, new Consumable(
            "water",
            "水",
            "清潔な飲み水。疲労回復に効果的。",
            ItemType.Food,
            hpRestore: 0,
            staminaRestore: 15,
            maxStack: 99
        ));

        // ココナッツ
        RegisterItem(items, new Consumable(
            "coconut",
            "ココナッツ",
            "ヤシの実。中の水分と果肉が栄養満点。",
            ItemType.Food,
            hpRestore: 15,
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

using System.Collections.Generic;

/// <summary>
/// 大切なものアイテムの定義
/// </summary>
public static class KeyItems
{
    public static void Register(Dictionary<string, Item> items)
    {
        // 小さな鞄
        RegisterItem(items, new KeyItem(
            "small_bag",
            "小さな鞄",
            "ちょっとした物を入れられる鞄。",
            KeyItemEffect.InventoryExpansion,
            effectValue: 5
        ));

        // 大きな鞄
        RegisterItem(items, new KeyItem(
            "large_bag",
            "大きな鞄",
            "たくさんの物を持ち運べる大きな鞄。",
            KeyItemEffect.InventoryExpansion,
            effectValue: 10
        ));

        // 巨大なバックパック
        RegisterItem(items, new KeyItem(
            "huge_backpack",
            "巨大なバックパック",
            "驚くほど大容量のバックパック。",
            KeyItemEffect.InventoryExpansion,
            effectValue: 20
        ));

        // 青い羽（結婚用アイテム）
        RegisterItem(items, new KeyItem(
            "blue_feather",
            "青い羽",
            "プロポーズに使う特別な青い羽。結婚したい相手に渡すことで想いを伝えられる。",
            KeyItemEffect.None,
            effectValue: 0
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

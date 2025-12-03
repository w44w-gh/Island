using System.Collections.Generic;

/// <summary>
/// 素材アイテムの定義
/// </summary>
public static class MaterialItems
{
    public static void Register(Dictionary<string, Item> items)
    {
        // 木材
        RegisterItem(items, new Item(
            "wood",
            "木材",
            "加工に使える木の素材。",
            ItemType.Material,
            maxStack: 99
        ));

        // 石
        RegisterItem(items, new Item(
            "stone",
            "石",
            "硬い石。道具作りに使える。",
            ItemType.Material,
            maxStack: 99
        ));

        // つる
        RegisterItem(items, new Item(
            "vine",
            "つる",
            "丈夫なつる。ロープの代わりになる。",
            ItemType.Material,
            maxStack: 99
        ));

        // 糸
        RegisterItem(items, new Item(
            "thread",
            "糸",
            "釣竿を作るために必要。科学者から購入できる。",
            ItemType.Material,
            maxStack: 99
        ));

        // ガラス
        RegisterItem(items, new Item(
            "glass",
            "ガラス",
            "科学実験などに使える。科学者から購入できる。",
            ItemType.Material,
            maxStack: 99
        ));

        // 鉄
        RegisterItem(items, new Item(
            "iron",
            "鉄",
            "フライパンを作るために必要。科学者から購入できる。",
            ItemType.Material,
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

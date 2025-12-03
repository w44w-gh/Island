using System.Collections.Generic;

/// <summary>
/// レシピアイテムの定義
/// </summary>
public static class RecipeItems
{
    public static void Register(Dictionary<string, Item> items)
    {
        // 焼き魚のレシピ
        RegisterItem(items, new Item(
            "recipe_grilled_fish",
            "レシピ：焼き魚",
            "焼き魚の作り方が書かれたレシピ。料理人から購入できる。",
            ItemType.Recipe,
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

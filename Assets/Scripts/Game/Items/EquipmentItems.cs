using System.Collections.Generic;

/// <summary>
/// アクセサリーアイテムの定義
/// </summary>
public static class EquipmentItems
{
    public static void Register(Dictionary<string, Item> items)
    {
        // 貝殻のお守り
        RegisterItem(items, new Equipment(
            "shell_charm",
            "貝殻のお守り",
            "美しい貝殻で作ったお守り。少し元気が出る。",
            EquipmentSlot.Accessory1,
            hpBonus: 10,
            staminaBonus: 5
        ));

        // 木彫りのペンダント
        RegisterItem(items, new Equipment(
            "wood_pendant",
            "木彫りのペンダント",
            "島の木で作った手作りペンダント。",
            EquipmentSlot.Accessory2,
            hpBonus: 5,
            staminaBonus: 10
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

using System;

/// <summary>
/// 装備スロットの種類
/// </summary>
public enum EquipmentSlot
{
    Accessory1,     // アクセサリー1
    Accessory2      // アクセサリー2
}

/// <summary>
/// 装備品のクラス（Itemを継承）
/// </summary>
[Serializable]
public class Equipment : Item
{
    public EquipmentSlot slot;      // 装備スロット
    public int hpBonus;             // HPボーナス
    public int staminaBonus;        // スタミナボーナス

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Equipment(
        string id,
        string name,
        string description,
        EquipmentSlot slot,
        int hpBonus = 0,
        int staminaBonus = 0
    ) : base(id, name, description, ItemType.Accessory, 1)
    {
        this.slot = slot;
        this.hpBonus = hpBonus;
        this.staminaBonus = staminaBonus;
        this.isEquippable = true;
    }

    /// <summary>
    /// 装備スロットを日本語文字列に変換
    /// </summary>
    public string GetSlotText()
    {
        switch (slot)
        {
            case EquipmentSlot.Accessory1:
                return "アクセサリー1";
            case EquipmentSlot.Accessory2:
                return "アクセサリー2";
            default:
                return "不明";
        }
    }

    /// <summary>
    /// 装備のステータスサマリー
    /// </summary>
    public string GetStatsText()
    {
        string stats = "";

        if (hpBonus > 0) stats += $"HP+{hpBonus} ";
        if (staminaBonus > 0) stats += $"スタミナ+{staminaBonus} ";

        return stats.Trim();
    }
}

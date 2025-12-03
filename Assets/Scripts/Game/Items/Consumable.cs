using System;

/// <summary>
/// 消費アイテムのクラス（Itemを継承）
/// </summary>
[Serializable]
public class Consumable : Item
{
    public int hpRestore;           // HP回復量
    public int staminaRestore;      // スタミナ回復量

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Consumable(
        string id,
        string name,
        string description,
        ItemType type,
        int hpRestore = 0,
        int staminaRestore = 0,
        int maxStack = 99
    ) : base(id, name, description, type, maxStack)
    {
        this.hpRestore = hpRestore;
        this.staminaRestore = staminaRestore;
    }

    /// <summary>
    /// 消費アイテムの効果テキスト
    /// </summary>
    public string GetEffectText()
    {
        string effect = "";

        if (hpRestore > 0) effect += $"HP+{hpRestore} ";
        if (staminaRestore > 0) effect += $"スタミナ+{staminaRestore} ";

        return effect.Trim();
    }
}

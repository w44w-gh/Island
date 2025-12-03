using System;

/// <summary>
/// 大切なものの種類
/// </summary>
public enum KeyItemEffect
{
    None,               // 効果なし
    InventoryExpansion  // インベントリ拡張
}

/// <summary>
/// 大切なもののクラス（Itemを継承）
/// </summary>
[Serializable]
public class KeyItem : Item
{
    public KeyItemEffect effect;        // 特殊効果
    public int effectValue;             // 効果の値（容量追加数など）

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public KeyItem(
        string id,
        string name,
        string description,
        KeyItemEffect effect = KeyItemEffect.None,
        int effectValue = 0
    ) : base(id, name, description, ItemType.KeyItem, 1)
    {
        this.effect = effect;
        this.effectValue = effectValue;
    }

    /// <summary>
    /// 効果の説明テキスト
    /// </summary>
    public string GetEffectText()
    {
        switch (effect)
        {
            case KeyItemEffect.InventoryExpansion:
                return $"持ち物の容量+{effectValue}";
            case KeyItemEffect.None:
            default:
                return "";
        }
    }
}

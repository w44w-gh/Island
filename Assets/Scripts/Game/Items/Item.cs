using System;

/// <summary>
/// アイテムの種類
/// </summary>
public enum ItemType
{
    Accessory,      // アクセサリー
    Food,           // 食料（通貨として使用可能）
    Cooking,        // 料理（回復のみ、通貨不可）
    Material,       // 素材
    Tool,           // 道具（釣竿、フライパンなど）
    Recipe,         // レシピ
    KeyItem         // 重要アイテム
}

/// <summary>
/// アイテムの基本クラス
/// </summary>
[Serializable]
public class Item
{
    public string id;               // アイテムID（一意）
    public string name;             // アイテム名
    public string description;      // 説明
    public ItemType type;           // 種類
    public int maxStack;            // 最大スタック数（0 = スタック不可）
    public bool isEquippable;       // 装備可能か
    public string iconPath;         // アイコン画像のパス

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Item(string id, string name, string description, ItemType type, int maxStack = 1)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.type = type;
        this.maxStack = maxStack;
        this.isEquippable = false;
    }

    /// <summary>
    /// アイテムタイプを日本語文字列に変換
    /// </summary>
    public string GetTypeText()
    {
        switch (type)
        {
            case ItemType.Accessory:
                return "アクセサリー";
            case ItemType.Food:
                return "食料";
            case ItemType.Cooking:
                return "料理";
            case ItemType.Material:
                return "素材";
            case ItemType.Tool:
                return "道具";
            case ItemType.Recipe:
                return "レシピ";
            case ItemType.KeyItem:
                return "重要アイテム";
            default:
                return "不明";
        }
    }

    /// <summary>
    /// 通貨として使用可能か
    /// </summary>
    public bool IsUsableAsCurrency => type == ItemType.Food;
}

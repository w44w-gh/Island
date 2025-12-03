using System;

/// <summary>
/// 一般キャラクタークラス
/// 攻略不可だが、店や特別なイベントを持つキャラクター
/// プレイヤーとは友好度のみ上げられる
/// </summary>
[Serializable]
public class GeneralCharacter : BaseCharacter
{
    public ShopType shopType;       // 店の種類

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public GeneralCharacter(string id, string name, string description, ShopType shopType = ShopType.None)
        : base(id, name, description)
    {
        this.shopType = shopType;
    }

    /// <summary>
    /// 店の種類を日本語で取得
    /// </summary>
    public string GetShopTypeName()
    {
        switch (shopType)
        {
            case ShopType.Craftsman:
                return "職人";
            case ShopType.Doctor:
                return "医者";
            case ShopType.Scientist:
                return "科学者";
            case ShopType.Cook:
                return "料理人";
            case ShopType.Undefined:
                return "未定";
            case ShopType.None:
            default:
                return "";
        }
    }

    /// <summary>
    /// サマリー取得
    /// </summary>
    public override string GetSummary()
    {
        string shopInfo = shopType != ShopType.None ? $" [{GetShopTypeName()}]" : "";
        return $"{name}{shopInfo} - 友好度: {friendship} ({GetFriendshipLevel()})";
    }
}

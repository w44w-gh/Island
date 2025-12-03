using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターの店の種類
/// </summary>
public enum ShopType
{
    None,           // お店なし
    Craftsman,      // 職人（家や道具を作る）
    Doctor,         // 医者（体力回復）
    Scientist,      // 科学者（素材販売）
    Cook,           // 料理人（レシピ販売）
    Undefined       // 未定（5人目）
}

/// <summary>
/// 攻略可能キャラクタークラス
/// 友好度、恋愛度、結婚機能を持つ
/// </summary>
[Serializable]
public class Character : BaseCharacter
{
    public ShopType shopType;       // 店の種類

    private int romance;            // 恋愛度 (0-100)
    private bool isMarried;         // 結婚しているか

    // 好きなアイテムID（友好度+10）
    public List<string> favoriteItems;
    // 嫌いなアイテムID（友好度-5）
    public List<string> dislikedItems;

    /// <summary>
    /// 恋愛度（0-100）
    /// </summary>
    public int Romance => romance;

    /// <summary>
    /// 恋愛度の割合（0.0-1.0）
    /// </summary>
    public float RomanceRatio => romance / 100f;

    /// <summary>
    /// 結婚しているか
    /// </summary>
    public bool IsMarried => isMarried;

    /// <summary>
    /// 恋愛度が変化した時のイベント
    /// </summary>
    public event Action<int, int> OnRomanceChanged;  // (current, change)

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Character(string id, string name, string description, ShopType shopType = ShopType.None)
        : base(id, name, description)
    {
        this.shopType = shopType;
        this.romance = 0;
        this.isMarried = false;
        this.favoriteItems = new List<string>();
        this.dislikedItems = new List<string>();
    }

    /// <summary>
    /// 好きなアイテムを追加
    /// </summary>
    public void AddFavoriteItem(params string[] itemIds)
    {
        favoriteItems.AddRange(itemIds);
    }

    /// <summary>
    /// 嫌いなアイテムを追加
    /// </summary>
    public void AddDislikedItem(params string[] itemIds)
    {
        dislikedItems.AddRange(itemIds);
    }

    /// <summary>
    /// アイテムを渡した時の反応を判定
    /// </summary>
    public int GetItemReaction(string itemId)
    {
        if (favoriteItems.Contains(itemId))
        {
            return 10; // 好きなアイテム: +10
        }
        else if (dislikedItems.Contains(itemId))
        {
            return -5; // 嫌いなアイテム: -5
        }
        else
        {
            return 2; // 普通のアイテム: +2
        }
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
    /// 恋愛度を設定（ロード時のみ使用）
    /// </summary>
    public void SetRomance(int value)
    {
        romance = Mathf.Clamp(value, 0, 100);
    }

    /// <summary>
    /// 恋愛度を増加
    /// </summary>
    public void IncreaseRomance(int amount)
    {
        if (amount <= 0) return;

        int previousRomance = romance;
        romance = Mathf.Min(100, romance + amount);

        int actualIncrease = romance - previousRomance;
        if (actualIncrease > 0)
        {
            Debug.Log($"{name}の恋愛度が上昇: +{actualIncrease} (現在: {romance})");
            OnRomanceChanged?.Invoke(romance, actualIncrease);
        }
    }

    /// <summary>
    /// 恋愛度を減少
    /// </summary>
    public void DecreaseRomance(int amount)
    {
        if (amount <= 0) return;

        int previousRomance = romance;
        romance = Mathf.Max(0, romance - amount);

        int actualDecrease = previousRomance - romance;
        if (actualDecrease > 0)
        {
            Debug.Log($"{name}の恋愛度が低下: -{actualDecrease} (現在: {romance})");
            OnRomanceChanged?.Invoke(romance, -actualDecrease);
        }
    }

    /// <summary>
    /// 恋愛度のレベルを取得
    /// </summary>
    public string GetRomanceLevel()
    {
        if (romance >= 80) return "恋人";
        if (romance >= 60) return "好意的";
        if (romance >= 40) return "興味あり";
        if (romance >= 20) return "ほんのり";
        return "なし";
    }

    /// <summary>
    /// 結婚する
    /// </summary>
    public void Marry()
    {
        isMarried = true;
        Debug.Log($"{name}と結婚しました");
    }

    /// <summary>
    /// 結婚状態を設定（ロード時のみ使用）
    /// </summary>
    public void SetMarried(bool married)
    {
        isMarried = married;
    }

    /// <summary>
    /// 結婚可能かチェック
    /// </summary>
    public bool CanMarry()
    {
        // 既に結婚している場合は不可
        if (isMarried) return false;

        // 恋愛度が100でないと結婚できない
        if (romance < 100) return false;

        return true;
    }

    /// <summary>
    /// キャラクターのサマリー
    /// </summary>
    public override string GetSummary()
    {
        string marriageStatus = isMarried ? " [既婚]" : "";
        return $"{name}{marriageStatus} - 友好度: {friendship} ({GetFriendshipLevel()}), 恋愛度: {romance} ({GetRomanceLevel()})";
    }
}

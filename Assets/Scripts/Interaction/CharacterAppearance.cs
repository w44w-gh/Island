using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターの外見（立ち絵）を管理するクラス
/// 表情、衣装、ポーズなどを動的に変更できる
/// </summary>
[System.Serializable]
public class CharacterAppearance
{
    [System.Serializable]
    public class AppearanceVariation
    {
        public string variationName;  // "normal", "happy", "sad", "angry", "swimsuit"など
        public Sprite sprite;         // 立ち絵のSprite
    }

    [Header("Appearance Variations")]
    [SerializeField] private List<AppearanceVariation> variations = new List<AppearanceVariation>();

    [Header("Current State")]
    [SerializeField] private string currentVariation = "normal";

    /// <summary>
    /// 指定したバリエーションのSpriteを取得
    /// </summary>
    public Sprite GetSprite(string variationName)
    {
        foreach (var variation in variations)
        {
            if (variation.variationName == variationName)
            {
                return variation.sprite;
            }
        }

        // 見つからない場合はデフォルト（normal）を返す
        foreach (var variation in variations)
        {
            if (variation.variationName == "normal")
            {
                return variation.sprite;
            }
        }

        // それでもない場合は最初の要素
        if (variations.Count > 0)
        {
            return variations[0].sprite;
        }

        return null;
    }

    /// <summary>
    /// 現在のSpriteを取得
    /// </summary>
    public Sprite GetCurrentSprite()
    {
        return GetSprite(currentVariation);
    }

    /// <summary>
    /// バリエーションを変更
    /// </summary>
    public void SetVariation(string variationName)
    {
        currentVariation = variationName;
    }

    /// <summary>
    /// 現在のバリエーション名を取得
    /// </summary>
    public string GetCurrentVariation()
    {
        return currentVariation;
    }

    /// <summary>
    /// バリエーションを追加（動的）
    /// </summary>
    public void AddVariation(string variationName, Sprite sprite)
    {
        // 既存の場合は上書き
        foreach (var variation in variations)
        {
            if (variation.variationName == variationName)
            {
                variation.sprite = sprite;
                return;
            }
        }

        // 新規追加
        variations.Add(new AppearanceVariation
        {
            variationName = variationName,
            sprite = sprite
        });
    }

    /// <summary>
    /// 全てのバリエーション名を取得
    /// </summary>
    public List<string> GetAllVariationNames()
    {
        List<string> names = new List<string>();
        foreach (var variation in variations)
        {
            names.Add(variation.variationName);
        }
        return names;
    }
}

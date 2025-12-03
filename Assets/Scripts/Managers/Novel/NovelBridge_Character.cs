using UnityEngine;

/// <summary>
/// NovelBridge - キャラクター関連メソッド
/// </summary>
public partial class NovelBridge
{
    // ========== 友好度・恋愛度関連 ==========

    /// <summary>
    /// 友好度を増加（宴のコマンドから呼び出し）
    /// </summary>
    public void IncreaseFriendship(string characterId, int amount)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        var character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        if (character != null)
        {
            character.IncreaseFriendship(amount);
            Debug.Log($"[NovelBridge] {character.name}の友好度を{amount}増加");
        }
        else
        {
            Debug.LogWarning($"キャラクター '{characterId}' が見つかりません");
        }
    }

    /// <summary>
    /// 友好度を減少（宴のコマンドから呼び出し）
    /// </summary>
    public void DecreaseFriendship(string characterId, int amount)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        var character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        if (character != null)
        {
            character.DecreaseFriendship(amount);
            Debug.Log($"[NovelBridge] {character.name}の友好度を{amount}減少");
        }
        else
        {
            Debug.LogWarning($"キャラクター '{characterId}' が見つかりません");
        }
    }

    /// <summary>
    /// 恋愛度を増加（宴のコマンドから呼び出し）
    /// </summary>
    public void IncreaseRomance(string characterId, int amount)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        var character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        if (character != null)
        {
            character.IncreaseRomance(amount);
            Debug.Log($"[NovelBridge] {character.name}の恋愛度を{amount}増加");
        }
        else
        {
            Debug.LogWarning($"キャラクター '{characterId}' が見つかりません");
        }
    }

    /// <summary>
    /// 恋愛度を減少（宴のコマンドから呼び出し）
    /// </summary>
    public void DecreaseRomance(string characterId, int amount)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        var character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        if (character != null)
        {
            character.DecreaseRomance(amount);
            Debug.Log($"[NovelBridge] {character.name}の恋愛度を{amount}減少");
        }
        else
        {
            Debug.LogWarning($"キャラクター '{characterId}' が見つかりません");
        }
    }

    /// <summary>
    /// 友好度を取得（宴の分岐条件などで使用）
    /// </summary>
    public int GetFriendship(string characterId)
    {
        if (GameManager.Instance.State == null) return 0;

        var character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        return character != null ? character.Friendship : 0;
    }

    /// <summary>
    /// 恋愛度を取得（宴の分岐条件などで使用）
    /// </summary>
    public int GetRomance(string characterId)
    {
        if (GameManager.Instance.State == null) return 0;

        var character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        return character != null ? character.Romance : 0;
    }

    /// <summary>
    /// キャラクターに物を渡す（宴のコマンドから呼び出し）
    /// </summary>
    public void GiveItemToCharacter(string characterId, string itemId, int quantity = 1)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        // アイテムを所持しているか確認
        if (!GameManager.Instance.State.Inventory.HasItem(itemId, quantity))
        {
            Debug.LogWarning($"[NovelBridge] アイテムが足りません: {itemId} x{quantity}");
            return;
        }

        // キャラクターを取得
        var character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        if (character == null)
        {
            Debug.LogWarning($"[NovelBridge] キャラクターが見つかりません: {characterId}");
            return;
        }

        // アイテムをインベントリから削除
        bool removed = GameManager.Instance.State.Inventory.RemoveItem(itemId, quantity);
        if (!removed)
        {
            Debug.LogWarning($"[NovelBridge] アイテムの削除に失敗: {itemId}");
            return;
        }

        // キャラクターの反応を取得
        int reactionValue = character.GetItemReaction(itemId);

        // 友好度を変化
        if (reactionValue > 0)
        {
            character.IncreaseFriendship(reactionValue);
        }
        else if (reactionValue < 0)
        {
            character.DecreaseFriendship(-reactionValue);
        }

        Debug.Log($"[NovelBridge] {character.name}に{itemId}を渡しました。友好度変化: {reactionValue}");
    }

    /// <summary>
    /// キャラクターがアイテムを好きかどうか確認（宴の分岐条件で使用）
    /// </summary>
    public bool CharacterLikesItem(string characterId, string itemId)
    {
        if (GameManager.Instance.State == null) return false;

        var character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        if (character == null) return false;

        return character.favoriteItems.Contains(itemId);
    }

    /// <summary>
    /// キャラクターがアイテムを嫌いかどうか確認（宴の分岐条件で使用）
    /// </summary>
    public bool CharacterDislikesItem(string characterId, string itemId)
    {
        if (GameManager.Instance.State == null) return false;

        var character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        if (character == null) return false;

        return character.dislikedItems.Contains(itemId);
    }

    /// <summary>
    /// キャラクターの店の種類を取得（宴で使用）
    /// </summary>
    public string GetCharacterShopType(string characterId)
    {
        if (GameManager.Instance.State == null) return "None";

        var character = GameManager.Instance.State.Characters.GetCharacter(characterId);
        if (character == null) return "None";

        return character.shopType.ToString();
    }
}

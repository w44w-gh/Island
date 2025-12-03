using UnityEngine;

/// <summary>
/// NovelBridge - アイテム・インベントリ関連メソッド
/// </summary>
public partial class NovelBridge
{
    // ========== アイテム・インベントリ関連 ==========

    /// <summary>
    /// アイテムを取得（宴のコマンドから呼び出し）
    /// </summary>
    public void GiveItem(string itemId, int quantity)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        Item item = ItemDatabase.GetItem(itemId);
        if (item != null)
        {
            bool success = GameManager.Instance.State.Inventory.AddItem(item, quantity);
            if (success)
            {
                Debug.Log($"[NovelBridge] アイテム取得: {item.name} x{quantity}");
            }
            else
            {
                Debug.LogWarning($"[NovelBridge] アイテムの追加に失敗: {item.name} x{quantity} (インベントリが満杯)");
            }
        }
        else
        {
            Debug.LogWarning($"[NovelBridge] アイテムが見つかりません: {itemId}");
        }
    }

    /// <summary>
    /// アイテムを削除（宴のコマンドから呼び出し）
    /// </summary>
    public void RemoveItem(string itemId, int quantity)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        bool success = GameManager.Instance.State.Inventory.RemoveItem(itemId, quantity);
        if (success)
        {
            Debug.Log($"[NovelBridge] アイテム削除: {itemId} x{quantity}");
        }
        else
        {
            Debug.LogWarning($"[NovelBridge] アイテムの削除に失敗: {itemId} x{quantity} (アイテムが足りない)");
        }
    }

    /// <summary>
    /// アイテムを所持しているか確認（宴の分岐条件などで使用）
    /// </summary>
    public bool HasItem(string itemId, int quantity = 1)
    {
        if (GameManager.Instance.State == null) return false;

        return GameManager.Instance.State.Inventory.HasItem(itemId, quantity);
    }

    /// <summary>
    /// アイテムの所持数を取得（宴の分岐条件などで使用）
    /// </summary>
    public int GetItemCount(string itemId)
    {
        if (GameManager.Instance.State == null) return 0;

        return GameManager.Instance.State.Inventory.GetItemCount(itemId);
    }
}

using System;

/// <summary>
/// インベントリのスロット（アイテム + 数量）
/// </summary>
[Serializable]
public class InventorySlot
{
    public Item item;           // アイテム
    public int quantity;        // 数量

    public InventorySlot(Item item, int quantity = 1)
    {
        this.item = item;
        this.quantity = quantity;
    }

    /// <summary>
    /// スロットが空か
    /// </summary>
    public bool IsEmpty => item == null || quantity <= 0;

    /// <summary>
    /// 追加可能か
    /// </summary>
    public bool CanAddMore(int amount)
    {
        if (item == null) return false;
        if (item.maxStack == 0) return false;
        return quantity + amount <= item.maxStack;
    }

    /// <summary>
    /// アイテムを追加
    /// </summary>
    public int Add(int amount)
    {
        if (item == null) return 0;
        if (item.maxStack == 0) return 0;

        int canAdd = Math.Min(amount, item.maxStack - quantity);
        quantity += canAdd;
        return canAdd;
    }

    /// <summary>
    /// アイテムを削除
    /// </summary>
    public int Remove(int amount)
    {
        int removed = Math.Min(amount, quantity);
        quantity -= removed;

        if (quantity <= 0)
        {
            item = null;
            quantity = 0;
        }

        return removed;
    }

    /// <summary>
    /// スロットをクリア
    /// </summary>
    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// インベントリ（アイテム保管）
/// </summary>
public class Inventory
{
    private List<InventorySlot> slots;
    private int capacity;

    /// <summary>
    /// インベントリの基本容量
    /// </summary>
    public int BaseCapacity => capacity;

    /// <summary>
    /// インベントリの現在の容量（拡張込み）
    /// </summary>
    public int Capacity => capacity + GetCapacityBonus();

    /// <summary>
    /// 使用中のスロット数
    /// </summary>
    public int UsedSlots => slots.Count(s => !s.IsEmpty);

    /// <summary>
    /// 空きスロット数
    /// </summary>
    public int FreeSlots => capacity - UsedSlots;

    /// <summary>
    /// 全スロット
    /// </summary>
    public IReadOnlyList<InventorySlot> Slots => slots;

    /// <summary>
    /// アイテムが追加された時のイベント
    /// </summary>
    public event Action<Item, int> OnItemAdded;

    /// <summary>
    /// アイテムが削除された時のイベント
    /// </summary>
    public event Action<Item, int> OnItemRemoved;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Inventory(int capacity = 20)
    {
        this.capacity = capacity;
        this.slots = new List<InventorySlot>(capacity);

        for (int i = 0; i < capacity; i++)
        {
            slots.Add(new InventorySlot(null, 0));
        }

        Debug.Log($"Inventory initialized - Capacity: {capacity}");
    }

    /// <summary>
    /// アイテムを追加
    /// </summary>
    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;

        int remaining = quantity;

        // スタック可能な場合、既存のスロットに追加
        if (item.maxStack > 1)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.item.id == item.id && slot.CanAddMore(remaining))
                {
                    int added = slot.Add(remaining);
                    remaining -= added;

                    if (remaining <= 0)
                    {
                        Debug.Log($"アイテム追加: {item.name} x{quantity}");
                        OnItemAdded?.Invoke(item, quantity);
                        return true;
                    }
                }
            }
        }

        // 新しいスロットに追加
        while (remaining > 0)
        {
            // 容量拡張アイテムを追加した時は容量を再計算
            if (item is KeyItem keyItem && keyItem.effect == KeyItemEffect.InventoryExpansion)
            {
                ExpandSlotsIfNeeded();
            }

            var emptySlot = slots.FirstOrDefault(s => s.IsEmpty);
            if (emptySlot == null)
            {
                Debug.LogWarning($"インベントリが満杯です。{item.name}を{remaining}個追加できませんでした。");
                if (remaining < quantity)
                {
                    OnItemAdded?.Invoke(item, quantity - remaining);
                }
                return false;
            }

            int addAmount = item.maxStack > 1 ? Math.Min(remaining, item.maxStack) : 1;
            emptySlot.item = item;
            emptySlot.quantity = addAmount;
            remaining -= addAmount;
        }

        Debug.Log($"アイテム追加: {item.name} x{quantity}");
        OnItemAdded?.Invoke(item, quantity);

        // 容量拡張アイテムを追加した場合は容量を表示
        if (item is KeyItem keyItem2 && keyItem2.effect == KeyItemEffect.InventoryExpansion)
        {
            Debug.Log($"インベントリ容量が増加: {Capacity} (基本: {BaseCapacity}, ボーナス: +{GetCapacityBonus()})");
        }

        return true;
    }

    /// <summary>
    /// アイテムを削除
    /// </summary>
    public bool RemoveItem(string itemId, int quantity = 1)
    {
        if (quantity <= 0) return false;

        int remaining = quantity;

        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item.id == itemId)
            {
                int removed = slot.Remove(remaining);
                remaining -= removed;

                if (remaining <= 0)
                {
                    Debug.Log($"アイテム削除: {slot.item.name} x{quantity}");
                    OnItemRemoved?.Invoke(slot.item, quantity);
                    return true;
                }
            }
        }

        if (remaining < quantity)
        {
            Debug.LogWarning($"アイテムが足りません。{itemId}を{remaining}個削除できませんでした。");
        }

        return remaining == 0;
    }

    /// <summary>
    /// アイテムの所持数を取得
    /// </summary>
    public int GetItemCount(string itemId)
    {
        return slots.Where(s => !s.IsEmpty && s.item.id == itemId).Sum(s => s.quantity);
    }

    /// <summary>
    /// アイテムを所持しているか
    /// </summary>
    public bool HasItem(string itemId, int quantity = 1)
    {
        return GetItemCount(itemId) >= quantity;
    }

    /// <summary>
    /// 特定のスロットのアイテムを取得
    /// </summary>
    public Item GetItemAt(int index)
    {
        if (index < 0 || index >= slots.Count) return null;
        return slots[index].item;
    }

    /// <summary>
    /// インベントリをクリア
    /// </summary>
    public void Clear()
    {
        foreach (var slot in slots)
        {
            slot.Clear();
        }
        Debug.Log("インベントリをクリアしました");
    }

    /// <summary>
    /// インベントリのサマリー
    /// </summary>
    public string GetSummary()
    {
        return $"インベントリ: {UsedSlots}/{Capacity} 使用中";
    }

    /// <summary>
    /// 大切なものによる容量ボーナスを計算
    /// </summary>
    private int GetCapacityBonus()
    {
        int bonus = 0;

        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item is KeyItem keyItem)
            {
                if (keyItem.effect == KeyItemEffect.InventoryExpansion)
                {
                    bonus += keyItem.effectValue;
                }
            }
        }

        return bonus;
    }

    /// <summary>
    /// 必要に応じてスロットを拡張
    /// </summary>
    private void ExpandSlotsIfNeeded()
    {
        int currentCapacity = Capacity;
        while (slots.Count < currentCapacity)
        {
            slots.Add(new InventorySlot(null, 0));
        }
    }

    /// <summary>
    /// 容量ボーナスを再計算してスロットを調整
    /// </summary>
    public void RefreshCapacity()
    {
        ExpandSlotsIfNeeded();
        Debug.Log($"インベントリ容量を更新: {Capacity} (基本: {BaseCapacity}, ボーナス: +{GetCapacityBonus()})");
    }
}

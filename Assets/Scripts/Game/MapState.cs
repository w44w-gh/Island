using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// マップ上の場所
/// </summary>
public enum MapLocation
{
    Beach,      // 海岸
    Forest,     // 森
    Mountain,   // 山
    River       // 川
}

/// <summary>
/// マップ上のアイテムデータ
/// </summary>
[Serializable]
public class MapItemData
{
    public string itemId;       // アイテムID
    public int quantity;        // 数量

    public MapItemData(string itemId, int quantity)
    {
        this.itemId = itemId;
        this.quantity = quantity;
    }
}

/// <summary>
/// マップ上の建築物データ
/// </summary>
[Serializable]
public class MapConstructionData
{
    public string constructionId;   // 建築物ID
    public MapLocation location;    // 建築場所

    public MapConstructionData(string constructionId, MapLocation location)
    {
        this.constructionId = constructionId;
        this.location = location;
    }
}

/// <summary>
/// マップの状態管理（各場所のアイテムと建築物を管理）
/// </summary>
public class MapState
{
    // 各場所のアイテムリスト
    private Dictionary<MapLocation, List<MapItemData>> locationItems;

    // マップ上の建築物リスト
    private List<MapConstructionData> constructions;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MapState()
    {
        locationItems = new Dictionary<MapLocation, List<MapItemData>>();
        constructions = new List<MapConstructionData>();

        // 各場所を初期化
        foreach (MapLocation location in Enum.GetValues(typeof(MapLocation)))
        {
            locationItems[location] = new List<MapItemData>();
        }

        Debug.Log("MapState初期化完了");
    }

    /// <summary>
    /// 指定された場所のアイテムを取得
    /// </summary>
    public List<MapItemData> GetItemsAt(MapLocation location)
    {
        return locationItems[location];
    }

    /// <summary>
    /// 指定された場所にアイテムを追加
    /// </summary>
    public void AddItemAt(MapLocation location, string itemId, int quantity)
    {
        if (quantity <= 0) return;

        // 既に同じアイテムがあるか確認
        var existingItem = locationItems[location].FirstOrDefault(item => item.itemId == itemId);

        if (existingItem != null)
        {
            existingItem.quantity += quantity;
            Debug.Log($"{location.ToJapaneseString()}に{itemId}を{quantity}個追加（合計: {existingItem.quantity}）");
        }
        else
        {
            locationItems[location].Add(new MapItemData(itemId, quantity));
            Debug.Log($"{location.ToJapaneseString()}に{itemId}を{quantity}個追加（新規）");
        }
    }

    /// <summary>
    /// 指定された場所からアイテムを削除（拾った時）
    /// </summary>
    public bool RemoveItemAt(MapLocation location, string itemId, int quantity)
    {
        var item = locationItems[location].FirstOrDefault(i => i.itemId == itemId);

        if (item == null || item.quantity < quantity)
        {
            Debug.LogWarning($"{location.ToJapaneseString()}に{itemId}が{quantity}個ありません");
            return false;
        }

        item.quantity -= quantity;

        // 数量が0になったらリストから削除
        if (item.quantity <= 0)
        {
            locationItems[location].Remove(item);
        }

        Debug.Log($"{location.ToJapaneseString()}から{itemId}を{quantity}個削除");
        return true;
    }

    /// <summary>
    /// 指定された場所のアイテムをすべてクリア
    /// </summary>
    public void ClearLocation(MapLocation location)
    {
        locationItems[location].Clear();
        Debug.Log($"{location.ToJapaneseString()}のアイテムをクリアしました");
    }

    /// <summary>
    /// すべての場所のアイテム数を取得
    /// </summary>
    public int GetTotalItemCount()
    {
        int total = 0;
        foreach (var items in locationItems.Values)
        {
            total += items.Sum(item => item.quantity);
        }
        return total;
    }

    // ========== 建築物管理 ==========

    /// <summary>
    /// 建築物を追加
    /// </summary>
    public void AddConstruction(string constructionId, MapLocation location)
    {
        constructions.Add(new MapConstructionData(constructionId, location));
        Debug.Log($"{location.ToJapaneseString()}に{constructionId}を建設しました");
    }

    /// <summary>
    /// 建築物を削除
    /// </summary>
    public bool RemoveConstruction(string constructionId)
    {
        var construction = constructions.FirstOrDefault(c => c.constructionId == constructionId);
        if (construction != null)
        {
            constructions.Remove(construction);
            Debug.Log($"{constructionId}を削除しました");
            return true;
        }
        return false;
    }

    /// <summary>
    /// すべての建築物を取得
    /// </summary>
    public List<MapConstructionData> GetAllConstructions()
    {
        return constructions;
    }

    /// <summary>
    /// 特定の場所の建築物を取得
    /// </summary>
    public List<MapConstructionData> GetConstructionsAt(MapLocation location)
    {
        return constructions.Where(c => c.location == location).ToList();
    }

    /// <summary>
    /// 特定の建築物が存在するか確認
    /// </summary>
    public bool HasConstruction(string constructionId)
    {
        return constructions.Any(c => c.constructionId == constructionId);
    }

    /// <summary>
    /// 建築物の総数を取得
    /// </summary>
    public int GetConstructionCount()
    {
        return constructions.Count;
    }

    /// <summary>
    /// デバッグ用のサマリー
    /// </summary>
    public string GetSummary()
    {
        string summary = "【マップ上のアイテム】\n";
        foreach (var location in locationItems.Keys)
        {
            var items = locationItems[location];
            if (items.Count > 0)
            {
                summary += $"  {location.ToJapaneseString()}: ";
                summary += string.Join(", ", items.Select(item => $"{item.itemId} x{item.quantity}"));
                summary += "\n";
            }
        }

        summary += "\n【建築物】\n";
        if (constructions.Count > 0)
        {
            foreach (var construction in constructions)
            {
                ConstructionData data = ConstructionDatabase.GetConstruction(construction.constructionId);
                string name = data?.name ?? construction.constructionId;
                summary += $"  {construction.location.ToJapaneseString()}: {name}\n";
            }
        }
        else
        {
            summary += "  なし\n";
        }

        return summary;
    }
}

/// <summary>
/// MapLocation の拡張メソッド
/// </summary>
public static class MapLocationExtensions
{
    public static string ToJapaneseString(this MapLocation location)
    {
        switch (location)
        {
            case MapLocation.Beach:
                return "海岸";
            case MapLocation.Forest:
                return "森";
            case MapLocation.Mountain:
                return "山";
            case MapLocation.River:
                return "川";
            default:
                return "不明";
        }
    }
}

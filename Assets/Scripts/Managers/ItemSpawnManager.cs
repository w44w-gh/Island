using UnityEngine;

/// <summary>
/// アイテムスポーン管理クラス
/// 各ロケーションでのアイテム出現処理を担当
/// </summary>
public class ItemSpawnManager
{
    private MapState mapState;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ItemSpawnManager(MapState mapState)
    {
        this.mapState = mapState;
    }

    /// <summary>
    /// 海岸にアイテムをランダム配置
    /// </summary>
    public void SpawnItemsAtBeach()
    {
        // 30%の確率で木材が流れ着く
        if (Random.value <= 0.3f)
        {
            int woodCount = Random.Range(1, 4); // 1-3個
            mapState.AddItemAt(MapLocation.Beach, "wood", woodCount);
        }

        // 20%の確率で石が流れ着く
        if (Random.value <= 0.2f)
        {
            int stoneCount = Random.Range(1, 3); // 1-2個
            mapState.AddItemAt(MapLocation.Beach, "stone", stoneCount);
        }
    }

    /// <summary>
    /// 森にアイテムをランダム配置
    /// </summary>
    public void SpawnItemsAtForest()
    {
        // 50%の確率できのみができる
        if (Random.value <= 0.5f)
        {
            int berryCount = Random.Range(2, 6); // 2-5個
            mapState.AddItemAt(MapLocation.Forest, "berry", berryCount);
        }

        // 10%の確率でココナッツが落ちている
        if (Random.value <= 0.1f)
        {
            int coconutCount = Random.Range(1, 3); // 1-2個
            mapState.AddItemAt(MapLocation.Forest, "coconut", coconutCount);
        }
    }

    /// <summary>
    /// 山にアイテムをランダム配置
    /// </summary>
    public void SpawnItemsAtMountain()
    {
        // 40%の確率で石が出現
        if (Random.value <= 0.4f)
        {
            int stoneCount = Random.Range(2, 5); // 2-4個
            mapState.AddItemAt(MapLocation.Mountain, "stone", stoneCount);
        }

        // 15%の確率で鉄鉱石が出現
        if (Random.value <= 0.15f)
        {
            int ironCount = Random.Range(1, 3); // 1-2個
            mapState.AddItemAt(MapLocation.Mountain, "iron_ore", ironCount);
        }
    }

    /// <summary>
    /// 川にアイテムをランダム配置
    /// </summary>
    public void SpawnItemsAtRiver()
    {
        // 35%の確率で魚が出現
        if (Random.value <= 0.35f)
        {
            int fishCount = Random.Range(1, 4); // 1-3匹
            mapState.AddItemAt(MapLocation.River, "fish", fishCount);
        }

        // 25%の確率で蔓が出現
        if (Random.value <= 0.25f)
        {
            int vineCount = Random.Range(1, 3); // 1-2個
            mapState.AddItemAt(MapLocation.River, "vine", vineCount);
        }
    }

    /// <summary>
    /// 全ロケーションにアイテムをスポーン
    /// </summary>
    public void SpawnItemsAtAllLocations()
    {
        SpawnItemsAtBeach();
        SpawnItemsAtForest();
        SpawnItemsAtMountain();
        SpawnItemsAtRiver();
    }

    /// <summary>
    /// 特定のロケーションにアイテムをスポーン
    /// </summary>
    public void SpawnItemsAtLocation(MapLocation location)
    {
        switch (location)
        {
            case MapLocation.Beach:
                SpawnItemsAtBeach();
                break;
            case MapLocation.Forest:
                SpawnItemsAtForest();
                break;
            case MapLocation.Mountain:
                SpawnItemsAtMountain();
                break;
            case MapLocation.River:
                SpawnItemsAtRiver();
                break;
            default:
                Debug.LogWarning($"Unknown location: {location}");
                break;
        }
    }
}

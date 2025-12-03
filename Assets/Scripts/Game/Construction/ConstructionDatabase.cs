using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建築物のデータベース
/// </summary>
public static class ConstructionDatabase
{
    private static Dictionary<string, ConstructionData> constructions = new Dictionary<string, ConstructionData>();

    /// <summary>
    /// 建築データベースの初期化
    /// </summary>
    public static void Initialize()
    {
        if (constructions.Count > 0) return; // 既に初期化済み

        // ========== 住居系 ==========
        // 小さな小屋（30分）
        RegisterConstruction(new ConstructionData(
            "small_hut",
            "小さな小屋",
            "雨風をしのげる簡素な小屋。",
            new Dictionary<string, int> { { "wood", 20 }, { "vine", 10 } },
            1800, // 30分
            MapLocation.Beach
        ));

        // 普通の家（6時間）
        RegisterConstruction(new ConstructionData(
            "normal_house",
            "普通の家",
            "快適に過ごせる家。小さな小屋が必要。",
            new Dictionary<string, int> { { "wood", 50 }, { "stone", 30 }, { "vine", 20 } },
            21600, // 6時間
            MapLocation.Beach
        ));

        // 大きな家（2日）
        RegisterConstruction(new ConstructionData(
            "large_house",
            "大きな家",
            "広々とした立派な家。普通の家が必要。",
            new Dictionary<string, int> { { "wood", 100 }, { "stone", 80 }, { "iron", 10 } },
            172800, // 2日
            MapLocation.Beach
        ));

        // ========== 倉庫系 ==========
        // 小さな倉庫（1時間）
        RegisterConstruction(new ConstructionData(
            "small_storage",
            "小さな倉庫",
            "アイテムを保管できる倉庫。インベントリ+10。",
            new Dictionary<string, int> { { "wood", 30 }, { "stone", 10 } },
            3600, // 1時間
            MapLocation.Beach
        ));

        // 大きな倉庫（12時間）
        RegisterConstruction(new ConstructionData(
            "large_storage",
            "大きな倉庫",
            "多くのアイテムを保管できる倉庫。インベントリ+20。小さな倉庫が必要。",
            new Dictionary<string, int> { { "wood", 60 }, { "stone", 40 }, { "iron", 5 } },
            43200, // 12時間
            MapLocation.Beach
        ));

        // ========== 作業施設 ==========
        // 作業台（2時間）
        RegisterConstruction(new ConstructionData(
            "workbench",
            "作業台",
            "道具を作るための作業台。クラフトが可能になる。",
            new Dictionary<string, int> { { "wood", 25 }, { "stone", 15 } },
            7200, // 2時間
            MapLocation.Beach
        ));

        // かまど（4時間）
        RegisterConstruction(new ConstructionData(
            "furnace",
            "かまど",
            "焼き物ができるかまど。鉱石の精錬や焼き物料理が可能。",
            new Dictionary<string, int> { { "stone", 50 }, { "wood", 20 } },
            14400, // 4時間
            MapLocation.Beach
        ));

        // 調理場（8時間）
        RegisterConstruction(new ConstructionData(
            "cooking_station",
            "調理場",
            "本格的な料理ができる調理場。複雑な料理が作れる。",
            new Dictionary<string, int> { { "wood", 40 }, { "stone", 30 }, { "iron", 8 } },
            28800, // 8時間
            MapLocation.Beach
        ));

        // ========== 資源採取施設 ==========
        // 井戸（1日）
        RegisterConstruction(new ConstructionData(
            "well",
            "井戸",
            "きれいな水が汲める井戸。水を定期的に入手できる。",
            new Dictionary<string, int> { { "stone", 60 }, { "wood", 30 }, { "iron", 5 } },
            86400, // 1日
            MapLocation.Beach
        ));

        // 小さな畑（3時間）
        RegisterConstruction(new ConstructionData(
            "small_farm",
            "小さな畑",
            "作物を育てられる小さな畑。種を植えて栽培できる。",
            new Dictionary<string, int> { { "wood", 20 }, { "stone", 10 } },
            10800, // 3時間
            MapLocation.Forest
        ));

        // 大きな畑（1日）
        RegisterConstruction(new ConstructionData(
            "large_farm",
            "大きな畑",
            "多くの作物を育てられる畑。小さな畑が必要。",
            new Dictionary<string, int> { { "wood", 50 }, { "stone", 30 }, { "iron", 3 } },
            86400, // 1日
            MapLocation.Forest
        ));

        // 釣り場（6時間）
        RegisterConstruction(new ConstructionData(
            "fishing_spot",
            "釣り場",
            "魚が釣りやすい整備された釣り場。釣りの成功率アップ。",
            new Dictionary<string, int> { { "wood", 40 }, { "stone", 20 }, { "vine", 15 } },
            21600, // 6時間
            MapLocation.Beach
        ));

        // ========== 補助施設 ==========
        // 物干し場（1時間）
        RegisterConstruction(new ConstructionData(
            "drying_rack",
            "物干し場",
            "魚や肉を干すための物干し場。保存食を作れる。",
            new Dictionary<string, int> { { "wood", 15 }, { "vine", 20 } },
            3600, // 1時間
            MapLocation.Beach
        ));

        // 道具小屋（4時間）
        RegisterConstruction(new ConstructionData(
            "tool_shed",
            "道具小屋",
            "道具を保管する小屋。道具の耐久度が減りにくくなる。",
            new Dictionary<string, int> { { "wood", 35 }, { "stone", 15 } },
            14400, // 4時間
            MapLocation.Beach
        ));

        // 休憩所（5時間）
        RegisterConstruction(new ConstructionData(
            "rest_area",
            "休憩所",
            "快適に休憩できる場所。スタミナ回復速度がアップ。",
            new Dictionary<string, int> { { "wood", 40 }, { "vine", 25 } },
            18000, // 5時間
            MapLocation.Beach
        ));

        // 展望台（1.5日）
        RegisterConstruction(new ConstructionData(
            "watchtower",
            "展望台",
            "高い場所から周囲を見渡せる。マップの探索範囲が広がる。",
            new Dictionary<string, int> { { "wood", 80 }, { "stone", 50 }, { "vine", 30 } },
            129600, // 1.5日
            MapLocation.Mountain
        ));

        // 看板（15分）
        RegisterConstruction(new ConstructionData(
            "signboard",
            "看板",
            "メモを書き込める看板。目印やメモとして使える。",
            new Dictionary<string, int> { { "wood", 5 } },
            900, // 15分
            MapLocation.Beach
        ));

        // ========== 特殊施設 ==========
        // 雨水タンク（8時間）
        RegisterConstruction(new ConstructionData(
            "rainwater_tank",
            "雨水タンク",
            "雨水を貯めるタンク。雨の日に水を自動で貯める。",
            new Dictionary<string, int> { { "wood", 30 }, { "iron", 10 }, { "glass", 5 } },
            28800, // 8時間
            MapLocation.Beach
        ));

        // 温室（3日）
        RegisterConstruction(new ConstructionData(
            "greenhouse",
            "温室",
            "天候に左右されずに作物を育てられる。作物の成長が早い。",
            new Dictionary<string, int> { { "wood", 80 }, { "glass", 30 }, { "iron", 15 } },
            259200, // 3日
            MapLocation.Forest
        ));

        // 桟橋（1日）
        RegisterConstruction(new ConstructionData(
            "pier",
            "桟橋",
            "海に突き出た桟橋。沖の魚が釣りやすくなる。",
            new Dictionary<string, int> { { "wood", 70 }, { "stone", 40 }, { "vine", 20 } },
            86400, // 1日
            MapLocation.Beach
        ));

        Debug.Log($"ConstructionDatabase initialized - {constructions.Count} constructions registered");
    }

    private static void RegisterConstruction(ConstructionData construction)
    {
        if (constructions.ContainsKey(construction.id))
        {
            Debug.LogWarning($"Construction ID '{construction.id}' is already registered. Skipping.");
            return;
        }
        constructions.Add(construction.id, construction);
    }

    /// <summary>
    /// 建築物IDから建築物データを取得
    /// </summary>
    public static ConstructionData GetConstruction(string constructionId)
    {
        if (constructions.TryGetValue(constructionId, out ConstructionData construction))
        {
            return construction;
        }

        Debug.LogWarning($"Construction '{constructionId}' not found in database.");
        return null;
    }

    /// <summary>
    /// 全建築物を取得
    /// </summary>
    public static IEnumerable<ConstructionData> GetAllConstructions()
    {
        return constructions.Values;
    }

    /// <summary>
    /// 特定の場所に建設可能な建築物を取得
    /// </summary>
    public static IEnumerable<ConstructionData> GetConstructionsByLocation(MapLocation location)
    {
        List<ConstructionData> result = new List<ConstructionData>();
        foreach (var construction in constructions.Values)
        {
            if (construction.location == location)
            {
                result.Add(construction);
            }
        }
        return result;
    }
}

using UnityEngine;

/// <summary>
/// 建築完了イベント定義
/// </summary>
public static partial class EventDatabase
{
    private static void RegisterConstructionEvents()
    {
        // 小屋完成イベント
        RegisterEvent(new SpecialEvent(
            "construction_small_hut",
            "小屋の完成",
            "小屋が完成しました",
            "Construction_SmallHut",
            80,
            new EventCondition
            {
                requiredConstructions = { "small_hut" }
            }
        ));

        // 普通の家完成イベント
        RegisterEvent(new SpecialEvent(
            "construction_normal_house",
            "普通の家の完成",
            "普通の家が完成しました",
            "Construction_NormalHouse",
            80,
            new EventCondition
            {
                requiredConstructions = { "normal_house" },
                prerequisiteEvents = { "construction_small_hut" }
            }
        ));

        // 大きな家完成イベント
        RegisterEvent(new SpecialEvent(
            "construction_large_house",
            "大きな家の完成",
            "大きな家が完成しました",
            "Construction_LargeHouse",
            80,
            new EventCondition
            {
                requiredConstructions = { "large_house" },
                prerequisiteEvents = { "construction_normal_house" }
            }
        ));

        // 作業台完成イベント
        RegisterEvent(new SpecialEvent(
            "construction_workbench",
            "作業台の完成",
            "作業台が完成し、道具が作れるようになりました",
            "Construction_Workbench",
            85,
            new EventCondition
            {
                requiredConstructions = { "workbench" }
            }
        ));

        // 溶鉱炉完成イベント
        RegisterEvent(new SpecialEvent(
            "construction_furnace",
            "溶鉱炉の完成",
            "溶鉱炉が完成し、金属加工ができるようになりました",
            "Construction_Furnace",
            85,
            new EventCondition
            {
                requiredConstructions = { "furnace" }
            }
        ));

        // 調理場完成イベント
        RegisterEvent(new SpecialEvent(
            "construction_cooking_station",
            "調理場の完成",
            "調理場が完成し、本格的な料理ができるようになりました",
            "Construction_CookingStation",
            85,
            new EventCondition
            {
                requiredConstructions = { "cooking_station" }
            }
        ));

        // 井戸完成イベント
        RegisterEvent(new SpecialEvent(
            "construction_well",
            "井戸の完成",
            "井戸が完成し、新鮮な水が使えるようになりました",
            "Construction_Well",
            85,
            new EventCondition
            {
                requiredConstructions = { "well" }
            }
        ));

        // 小さな農場完成イベント
        RegisterEvent(new SpecialEvent(
            "construction_small_farm",
            "小さな農場の完成",
            "小さな農場が完成し、作物を育てられるようになりました",
            "Construction_SmallFarm",
            85,
            new EventCondition
            {
                requiredConstructions = { "small_farm" }
            }
        ));

        Debug.Log("Construction events registered");
    }
}

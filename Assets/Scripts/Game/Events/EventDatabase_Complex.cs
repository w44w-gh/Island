using UnityEngine;

/// <summary>
/// 複雑な条件を持つイベント定義
/// </summary>
public static partial class EventDatabase
{
    private static void RegisterComplexEvents()
    {
        // 全キャラクターと仲良くなった時のイベント
        RegisterEvent(new SpecialEvent(
            "complex_all_friends",
            "みんなと友達",
            "全てのキャラクターと友達になりました",
            "Complex_AllFriends",
            60,
            new EventCondition
            {
                minDay = 20
                // 注: 全キャラの友好度チェックはEventManagerで実装
            }
        ));

        // 初めての結婚イベント後の集会
        RegisterEvent(new SpecialEvent(
            "complex_first_marriage_party",
            "結婚パーティー",
            "結婚を祝う特別なパーティーが開かれます",
            "Complex_FirstMarriageParty",
            140,
            new EventCondition
            {
                minDay = 30
                // 注: 結婚状態のチェックはEventManagerで実装
            }
        ));

        // 雨の日の海岸イベント
        RegisterEvent(new SpecialEvent(
            "complex_rainy_beach",
            "雨の海岸",
            "雨の日に海岸で特別なイベントが発生",
            "Complex_RainyBeach",
            75,
            new EventCondition
            {
                minDay = 12,
                requiredWeather = WeatherType.Rainy,
                requiredLocation = MapLocation.Beach
            }
        ));

        // 夜の森イベント
        RegisterEvent(new SpecialEvent(
            "complex_night_forest",
            "夜の森",
            "夜の森で不思議な出来事が",
            "Complex_NightForest",
            75,
            new EventCondition
            {
                minDay = 15,
                requiredTimeOfDay = TimeOfDay.Evening,
                requiredLocation = MapLocation.Forest
            }
        ));

        // 晴れの日の山イベント - 特定アイテム所持
        RegisterEvent(new SpecialEvent(
            "complex_sunny_mountain",
            "山頂からの景色",
            "晴れた日の山頂で特別な景色を見る",
            "Complex_SunnyMountain",
            75,
            new EventCondition
            {
                minDay = 20,
                requiredWeather = WeatherType.Sunny,
                requiredLocation = MapLocation.Mountain,
                requiredItems = { new ItemRequirement("stone", 10) }
            }
        ));

        // 料理イベント - 調理場完成 + 魚所持
        RegisterEvent(new SpecialEvent(
            "complex_first_cooking",
            "初めての料理",
            "調理場で初めて料理を作る",
            "Complex_FirstCooking",
            85,
            new EventCondition
            {
                requiredConstructions = { "cooking_station" },
                requiredItems = { new ItemRequirement("fish", 1) }
            }
        ));

        // 作業台で道具作成イベント
        RegisterEvent(new SpecialEvent(
            "complex_first_tool",
            "初めての道具",
            "作業台で初めて道具を作る",
            "Complex_FirstTool",
            85,
            new EventCondition
            {
                requiredConstructions = { "workbench" },
                requiredItems = { new ItemRequirement("wood", 5), new ItemRequirement("stone", 3) }
            }
        ));

        // 井戸完成 + 農場完成の複合イベント
        RegisterEvent(new SpecialEvent(
            "complex_farming_ready",
            "農業の準備完了",
            "井戸と農場が完成し、本格的な農業が始められます",
            "Complex_FarmingReady",
            90,
            new EventCondition
            {
                minDay = 25,
                requiredConstructions = { "well", "small_farm" }
            }
        ));

        // 豊かな島イベント - 複数の建築物完成
        RegisterEvent(new SpecialEvent(
            "complex_prosperous_island",
            "豊かな島",
            "島が発展し、住みやすくなりました",
            "Complex_ProsperousIsland",
            70,
            new EventCondition
            {
                minDay = 40,
                requiredConstructions = { "normal_house", "workbench", "cooking_station", "small_farm" }
            }
        ));

        Debug.Log("Complex events registered");
    }
}

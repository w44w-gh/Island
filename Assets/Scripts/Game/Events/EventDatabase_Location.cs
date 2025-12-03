using UnityEngine;

/// <summary>
/// 場所関連のイベント定義
/// </summary>
public static partial class EventDatabase
{
    private static void RegisterLocationEvents()
    {
        // 海岸発見イベント - 初期から利用可能
        RegisterEvent(new SpecialEvent(
            "location_beach_discover",
            "海岸を発見",
            "海岸を発見し、魚やココナツが採れるようになる",
            "Location_BeachDiscover",
            90,
            new EventCondition
            {
                minDay = 1,
                requiredLocation = MapLocation.Beach
            }
        ));

        // 森発見イベント - 3日目以降
        RegisterEvent(new SpecialEvent(
            "location_forest_discover",
            "森を発見",
            "森を発見し、木材やベリーが採れるようになる",
            "Location_ForestDiscover",
            90,
            new EventCondition
            {
                minDay = 3,
                requiredLocation = MapLocation.Forest
            }
        ));

        // 山発見イベント - 7日目以降
        RegisterEvent(new SpecialEvent(
            "location_mountain_discover",
            "山を発見",
            "山を発見し、石材や鉱石が採れるようになる",
            "Location_MountainDiscover",
            90,
            new EventCondition
            {
                minDay = 7,
                requiredLocation = MapLocation.Mountain
            }
        ));

        // 川発見イベント - 5日目以降
        RegisterEvent(new SpecialEvent(
            "location_river_discover",
            "川を発見",
            "川を発見し、淡水魚が採れるようになる",
            "Location_RiverDiscover",
            90,
            new EventCondition
            {
                minDay = 5,
                requiredLocation = MapLocation.River
            }
        ));

        // 雨の日の特別イベント - 10日目以降、雨の日
        RegisterEvent(new SpecialEvent(
            "special_rainy_day",
            "雨の日の出会い",
            "雨の日に特別なイベントが発生",
            "Special_RainyDay",
            70,
            new EventCondition
            {
                minDay = 10,
                requiredWeather = WeatherType.Rainy
            }
        ));

        // 嵐の日の特別イベント - 15日目以降、嵐の日
        RegisterEvent(new SpecialEvent(
            "special_stormy_day",
            "嵐の日の出来事",
            "嵐の日に特別なイベントが発生",
            "Special_StormyDay",
            70,
            new EventCondition
            {
                minDay = 15,
                requiredWeather = WeatherType.Stormy
            }
        ));

        Debug.Log("Location events registered");
    }
}

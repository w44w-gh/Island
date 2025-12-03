using UnityEngine;

/// <summary>
/// 職人のイベント定義
/// </summary>
public static partial class EventDatabase
{
    private static void RegisterCraftsmanEvents()
    {
        // ハート1 - 恋愛度20以上
        RegisterEvent(new SpecialEvent(
            "craftsman_heart1",
            "職人ハートイベント1",
            "職人との初めてのハートイベント",
            "Craftsman_Heart1",
            100,
            new EventCondition
            {
                characterId = "char_01",
                minRomance = 20
            }
        ));

        // ハート2 - 恋愛度40以上、ハート1完了
        RegisterEvent(new SpecialEvent(
            "craftsman_heart2",
            "職人ハートイベント2",
            "職人との2回目のハートイベント",
            "Craftsman_Heart2",
            100,
            new EventCondition
            {
                characterId = "char_01",
                minRomance = 40,
                prerequisiteEvents = { "craftsman_heart1" }
            }
        ));

        // ハート3 - 恋愛度60以上、ハート2完了
        RegisterEvent(new SpecialEvent(
            "craftsman_heart3",
            "職人ハートイベント3",
            "職人との3回目のハートイベント",
            "Craftsman_Heart3",
            100,
            new EventCondition
            {
                characterId = "char_01",
                minRomance = 60,
                prerequisiteEvents = { "craftsman_heart2" }
            }
        ));

        // 告白イベント - 恋愛度80以上、ハート3完了
        RegisterEvent(new SpecialEvent(
            "craftsman_confession",
            "職人告白イベント",
            "職人への告白イベント",
            "Craftsman_Confession",
            100,
            new EventCondition
            {
                characterId = "char_01",
                minRomance = 80,
                prerequisiteEvents = { "craftsman_heart3" }
            }
        ));

        // 結婚イベント - 恋愛度100、告白完了
        RegisterEvent(new SpecialEvent(
            "craftsman_marriage",
            "職人結婚イベント",
            "職人との結婚イベント",
            "Craftsman_Marriage",
            150, // 結婚イベントは優先度を上げる
            new EventCondition
            {
                characterId = "char_01",
                minRomance = 100,
                prerequisiteEvents = { "craftsman_confession" }
            }
        ));

        // 職人の店オープンイベント - 友好度30以上
        RegisterEvent(new SpecialEvent(
            "craftsman_shop_open",
            "職人の店オープン",
            "職人の店が利用可能になる",
            "Craftsman_ShopOpen",
            80,
            new EventCondition
            {
                characterId = "char_01",
                minFriendship = 30
            }
        ));

        Debug.Log("Craftsman events registered");
    }
}

using UnityEngine;

/// <summary>
/// 料理人のイベント定義
/// </summary>
public static partial class EventDatabase
{
    private static void RegisterCookEvents()
    {
        // ハート1 - 恋愛度20以上
        RegisterEvent(new SpecialEvent(
            "cook_heart1",
            "料理人ハートイベント1",
            "料理人との初めてのハートイベント",
            "Cook_Heart1",
            100,
            new EventCondition
            {
                characterId = "char_02",
                minRomance = 20
            }
        ));

        // ハート2 - 恋愛度40以上、ハート1完了
        RegisterEvent(new SpecialEvent(
            "cook_heart2",
            "料理人ハートイベント2",
            "料理人との2回目のハートイベント",
            "Cook_Heart2",
            100,
            new EventCondition
            {
                characterId = "char_02",
                minRomance = 40,
                prerequisiteEvents = { "cook_heart1" }
            }
        ));

        // ハート3 - 恋愛度60以上、ハート2完了
        RegisterEvent(new SpecialEvent(
            "cook_heart3",
            "料理人ハートイベント3",
            "料理人との3回目のハートイベント",
            "Cook_Heart3",
            100,
            new EventCondition
            {
                characterId = "char_02",
                minRomance = 60,
                prerequisiteEvents = { "cook_heart2" }
            }
        ));

        // 告白イベント - 恋愛度80以上、ハート3完了
        RegisterEvent(new SpecialEvent(
            "cook_confession",
            "料理人告白イベント",
            "料理人への告白イベント",
            "Cook_Confession",
            100,
            new EventCondition
            {
                characterId = "char_02",
                minRomance = 80,
                prerequisiteEvents = { "cook_heart3" }
            }
        ));

        // 結婚イベント - 恋愛度100、告白完了
        RegisterEvent(new SpecialEvent(
            "cook_marriage",
            "料理人結婚イベント",
            "料理人との結婚イベント",
            "Cook_Marriage",
            150,
            new EventCondition
            {
                characterId = "char_02",
                minRomance = 100,
                prerequisiteEvents = { "cook_confession" }
            }
        ));

        // 料理人の店オープンイベント - 友好度30以上
        RegisterEvent(new SpecialEvent(
            "cook_shop_open",
            "料理人の店オープン",
            "料理人の店が利用可能になる",
            "Cook_ShopOpen",
            80,
            new EventCondition
            {
                characterId = "char_02",
                minFriendship = 30
            }
        ));

        Debug.Log("Cook events registered");
    }
}

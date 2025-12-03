using UnityEngine;

/// <summary>
/// 医者のイベント定義
/// </summary>
public static partial class EventDatabase
{
    private static void RegisterDoctorEvents()
    {
        // ハート1 - 恋愛度20以上
        RegisterEvent(new SpecialEvent(
            "doctor_heart1",
            "医者ハートイベント1",
            "医者との初めてのハートイベント",
            "Doctor_Heart1",
            100,
            new EventCondition
            {
                characterId = "char_03",
                minRomance = 20
            }
        ));

        // ハート2 - 恋愛度40以上、ハート1完了
        RegisterEvent(new SpecialEvent(
            "doctor_heart2",
            "医者ハートイベント2",
            "医者との2回目のハートイベント",
            "Doctor_Heart2",
            100,
            new EventCondition
            {
                characterId = "char_03",
                minRomance = 40,
                prerequisiteEvents = { "doctor_heart1" }
            }
        ));

        // ハート3 - 恋愛度60以上、ハート2完了
        RegisterEvent(new SpecialEvent(
            "doctor_heart3",
            "医者ハートイベント3",
            "医者との3回目のハートイベント",
            "Doctor_Heart3",
            100,
            new EventCondition
            {
                characterId = "char_03",
                minRomance = 60,
                prerequisiteEvents = { "doctor_heart2" }
            }
        ));

        // 告白イベント - 恋愛度80以上、ハート3完了
        RegisterEvent(new SpecialEvent(
            "doctor_confession",
            "医者告白イベント",
            "医者への告白イベント",
            "Doctor_Confession",
            100,
            new EventCondition
            {
                characterId = "char_03",
                minRomance = 80,
                prerequisiteEvents = { "doctor_heart3" }
            }
        ));

        // 結婚イベント - 恋愛度100、告白完了
        RegisterEvent(new SpecialEvent(
            "doctor_marriage",
            "医者結婚イベント",
            "医者との結婚イベント",
            "Doctor_Marriage",
            150,
            new EventCondition
            {
                characterId = "char_03",
                minRomance = 100,
                prerequisiteEvents = { "doctor_confession" }
            }
        ));

        // 医者の店オープンイベント - 友好度30以上
        RegisterEvent(new SpecialEvent(
            "doctor_shop_open",
            "医者の店オープン",
            "医者の店が利用可能になる",
            "Doctor_ShopOpen",
            80,
            new EventCondition
            {
                characterId = "char_03",
                minFriendship = 30
            }
        ));

        Debug.Log("Doctor events registered");
    }
}

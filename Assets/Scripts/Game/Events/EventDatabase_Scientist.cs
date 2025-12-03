using UnityEngine;

/// <summary>
/// 科学者のイベント定義（ライバルキャラ）
/// 恋愛相談などの特別なイベント
/// </summary>
public static partial class EventDatabase
{
    private static void RegisterScientistEvents()
    {
        // 科学者の店オープンイベント - 友好度30以上
        RegisterEvent(new SpecialEvent(
            "scientist_shop_open",
            "科学者の店オープン",
            "科学者の店が利用可能になる",
            "Scientist_ShopOpen",
            80,
            new EventCondition
            {
                characterId = "char_04",
                minFriendship = 30
            }
        ));

        // 科学者との友好イベント1 - 友好度40以上
        RegisterEvent(new SpecialEvent(
            "scientist_friendship1",
            "科学者との会話1",
            "科学者と科学について語り合う",
            "Scientist_Friendship1",
            75,
            new EventCondition
            {
                characterId = "char_04",
                minFriendship = 40
            }
        ));

        // 恋愛相談イベント1 - 友好度50以上
        RegisterEvent(new SpecialEvent(
            "scientist_advice1",
            "恋愛相談 - 初級",
            "科学者に恋愛について相談する",
            "Scientist_LoveAdvice1",
            70,
            new EventCondition
            {
                characterId = "char_04",
                minFriendship = 50,
                minDay = 20
            }
        ));

        // 科学者との友好イベント2 - 友好度60以上
        RegisterEvent(new SpecialEvent(
            "scientist_friendship2",
            "科学者との会話2",
            "科学者の研究について詳しく聞く",
            "Scientist_Friendship2",
            75,
            new EventCondition
            {
                characterId = "char_04",
                minFriendship = 60,
                prerequisiteEvents = { "scientist_friendship1" }
            }
        ));

        // 恋愛相談イベント2 - 友好度70以上
        RegisterEvent(new SpecialEvent(
            "scientist_advice2",
            "恋愛相談 - 中級",
            "科学者に本格的な恋愛相談をする",
            "Scientist_LoveAdvice2",
            70,
            new EventCondition
            {
                characterId = "char_04",
                minFriendship = 70,
                minDay = 40,
                prerequisiteEvents = { "scientist_advice1" }
            }
        ));

        // 科学者との深い友情イベント - 友好度80以上
        RegisterEvent(new SpecialEvent(
            "scientist_friendship3",
            "科学者との深い友情",
            "科学者と真の友達になる",
            "Scientist_Friendship3",
            75,
            new EventCondition
            {
                characterId = "char_04",
                minFriendship = 80,
                prerequisiteEvents = { "scientist_friendship2" }
            }
        ));

        // 特別な恋愛相談イベント - 友好度90以上
        RegisterEvent(new SpecialEvent(
            "scientist_advice3",
            "恋愛相談 - 上級",
            "科学者から恋愛についての深いアドバイスを受ける",
            "Scientist_LoveAdvice3",
            70,
            new EventCondition
            {
                characterId = "char_04",
                minFriendship = 90,
                minDay = 60,
                prerequisiteEvents = { "scientist_advice2" }
            }
        ));

        Debug.Log("Scientist events registered (Rival type)");
    }
}

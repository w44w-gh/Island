using UnityEngine;

/// <summary>
/// ライバルイベント定義
/// 時間経過で自動的に発生する恋愛イベント
/// </summary>
public static partial class EventDatabase
{
    private static void RegisterRivalEvents()
    {
        // ===== 職人ペア (char_01 + rival_01) =====
        RegisterRivalEvent(new RivalEvent(
            "rival_craftsman_heart1",
            "職人ライバル - ハート1",
            "職人とライバルのハートイベント1",
            "Rival_Craftsman_Heart1",
            "char_01",
            "rival_01",
            1,
            30
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_craftsman_heart2",
            "職人ライバル - ハート2",
            "職人とライバルのハートイベント2",
            "Rival_Craftsman_Heart2",
            "char_01",
            "rival_01",
            2,
            60
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_craftsman_heart3",
            "職人ライバル - ハート3",
            "職人とライバルのハートイベント3",
            "Rival_Craftsman_Heart3",
            "char_01",
            "rival_01",
            3,
            90
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_craftsman_confession",
            "職人ライバル - 告白",
            "職人とライバルの告白イベント",
            "Rival_Craftsman_Confession",
            "char_01",
            "rival_01",
            4,
            120
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_craftsman_marriage",
            "職人ライバル - 結婚",
            "職人とライバルの結婚イベント",
            "Rival_Craftsman_Marriage",
            "char_01",
            "rival_01",
            5,
            150
        ));

        // ===== 料理人ペア (char_02 + rival_02) =====
        RegisterRivalEvent(new RivalEvent(
            "rival_cook_heart1",
            "料理人ライバル - ハート1",
            "料理人とライバルのハートイベント1",
            "Rival_Cook_Heart1",
            "char_02",
            "rival_02",
            1,
            30
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_cook_heart2",
            "料理人ライバル - ハート2",
            "料理人とライバルのハートイベント2",
            "Rival_Cook_Heart2",
            "char_02",
            "rival_02",
            2,
            60
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_cook_heart3",
            "料理人ライバル - ハート3",
            "料理人とライバルのハートイベント3",
            "Rival_Cook_Heart3",
            "char_02",
            "rival_02",
            3,
            90
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_cook_confession",
            "料理人ライバル - 告白",
            "料理人とライバルの告白イベント",
            "Rival_Cook_Confession",
            "char_02",
            "rival_02",
            4,
            120
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_cook_marriage",
            "料理人ライバル - 結婚",
            "料理人とライバルの結婚イベント",
            "Rival_Cook_Marriage",
            "char_02",
            "rival_02",
            5,
            150
        ));

        // ===== 医者ペア (char_03 + rival_03) =====
        RegisterRivalEvent(new RivalEvent(
            "rival_doctor_heart1",
            "医者ライバル - ハート1",
            "医者とライバルのハートイベント1",
            "Rival_Doctor_Heart1",
            "char_03",
            "rival_03",
            1,
            30
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_doctor_heart2",
            "医者ライバル - ハート2",
            "医者とライバルのハートイベント2",
            "Rival_Doctor_Heart2",
            "char_03",
            "rival_03",
            2,
            60
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_doctor_heart3",
            "医者ライバル - ハート3",
            "医者とライバルのハートイベント3",
            "Rival_Doctor_Heart3",
            "char_03",
            "rival_03",
            3,
            90
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_doctor_confession",
            "医者ライバル - 告白",
            "医者とライバルの告白イベント",
            "Rival_Doctor_Confession",
            "char_03",
            "rival_03",
            4,
            120
        ));

        RegisterRivalEvent(new RivalEvent(
            "rival_doctor_marriage",
            "医者ライバル - 結婚",
            "医者とライバルの結婚イベント",
            "Rival_Doctor_Marriage",
            "char_03",
            "rival_03",
            5,
            150
        ));

        Debug.Log($"Rival events registered - {rivalEventList.Count} events");
    }
}

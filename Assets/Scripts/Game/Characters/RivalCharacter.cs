using System;

/// <summary>
/// ライバルキャラクタークラス
/// プレイヤーとは友好度のみ上げられる
/// 友好度が高い場合、恋愛相談などの特別なイベントが発生する
/// </summary>
[Serializable]
public class RivalCharacter : BaseCharacter
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public RivalCharacter(string id, string name, string description = "")
        : base(id, name, description)
    {
    }

    /// <summary>
    /// ライバルキャラクター用の友好度レベル（攻略キャラより1段階少ない）
    /// </summary>
    public override string GetFriendshipLevel()
    {
        if (friendship >= 80) return "親友";
        if (friendship >= 60) return "友達";
        if (friendship >= 40) return "知り合い";
        if (friendship >= 20) return "顔見知り";
        return "他人";
    }

    /// <summary>
    /// サマリー取得
    /// </summary>
    public override string GetSummary()
    {
        return $"{name} - 友好度: {friendship} ({GetFriendshipLevel()})";
    }
}

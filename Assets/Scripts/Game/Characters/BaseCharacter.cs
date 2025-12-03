using System;
using UnityEngine;

/// <summary>
/// キャラクターの基底クラス
/// Character（攻略可能）とRivalCharacter（ライバル）の共通機能を提供
/// </summary>
[Serializable]
public abstract class BaseCharacter
{
    public string id;               // キャラクターID
    public string name;             // 名前
    public string description;      // 説明

    protected int friendship;       // 友好度 (0-100)

    /// <summary>
    /// 友好度（0-100）
    /// </summary>
    public int Friendship => friendship;

    /// <summary>
    /// 友好度の割合（0.0-1.0）
    /// </summary>
    public float FriendshipRatio => friendship / 100f;

    /// <summary>
    /// 友好度が変化した時のイベント
    /// </summary>
    public event Action<int, int> OnFriendshipChanged;  // (current, change)

    /// <summary>
    /// コンストラクタ
    /// </summary>
    protected BaseCharacter(string id, string name, string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.friendship = 0;
    }

    /// <summary>
    /// 友好度を設定（ロード時のみ使用）
    /// </summary>
    public void SetFriendship(int value)
    {
        friendship = Mathf.Clamp(value, 0, 100);
    }

    /// <summary>
    /// 友好度を増加
    /// </summary>
    public void IncreaseFriendship(int amount)
    {
        if (amount <= 0) return;

        int previousFriendship = friendship;
        friendship = Mathf.Min(100, friendship + amount);

        int actualIncrease = friendship - previousFriendship;
        if (actualIncrease > 0)
        {
            Debug.Log($"{name}の友好度が上昇: +{actualIncrease} (現在: {friendship})");
            OnFriendshipChanged?.Invoke(friendship, actualIncrease);
        }
    }

    /// <summary>
    /// 友好度を減少
    /// </summary>
    public void DecreaseFriendship(int amount)
    {
        if (amount <= 0) return;

        int previousFriendship = friendship;
        friendship = Mathf.Max(0, friendship - amount);

        int actualDecrease = previousFriendship - friendship;
        if (actualDecrease > 0)
        {
            Debug.Log($"{name}の友好度が低下: -{actualDecrease} (現在: {friendship})");
            OnFriendshipChanged?.Invoke(friendship, -actualDecrease);
        }
    }

    /// <summary>
    /// 友好度のレベルを取得（サブクラスで独自実装可能）
    /// </summary>
    public virtual string GetFriendshipLevel()
    {
        if (friendship >= 80) return "親友";
        if (friendship >= 60) return "仲良し";
        if (friendship >= 40) return "友達";
        if (friendship >= 20) return "知り合い";
        return "他人";
    }

    /// <summary>
    /// キャラクターのサマリー（サブクラスでオーバーライド）
    /// </summary>
    public abstract string GetSummary();
}

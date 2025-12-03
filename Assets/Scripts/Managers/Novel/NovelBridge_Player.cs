using UnityEngine;

/// <summary>
/// NovelBridge - プレイヤーステータス関連メソッド
/// </summary>
public partial class NovelBridge
{
    // ========== プレイヤーステータス関連 ==========

    /// <summary>
    /// プレイヤーのHPを変更（宴のコマンドから呼び出し）
    /// </summary>
    public void ChangePlayerHP(int amount)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        if (amount > 0)
        {
            GameManager.Instance.State.Player.Heal(amount);
        }
        else if (amount < 0)
        {
            GameManager.Instance.State.Player.TakeDamage(-amount);
        }

        Debug.Log($"[NovelBridge] プレイヤーのHPを{amount}変更");
    }

    /// <summary>
    /// プレイヤーのスタミナを変更（宴のコマンドから呼び出し）
    /// </summary>
    public void ChangePlayerStamina(int amount)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return;
        }

        if (amount > 0)
        {
            GameManager.Instance.State.Player.RecoverStamina(amount);
        }
        else if (amount < 0)
        {
            GameManager.Instance.State.Player.ConsumeStamina(-amount);
        }

        Debug.Log($"[NovelBridge] プレイヤーのスタミナを{amount}変更");
    }
}

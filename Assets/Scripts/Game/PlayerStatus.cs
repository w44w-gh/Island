using System;
using UnityEngine;

/// <summary>
/// 主人公のステータス管理
/// 体力、疲労などを管理
/// </summary>
public class PlayerStatus
{
    // 最大値
    private const int MAX_HP = 100;
    private const int MAX_STAMINA = 100;

    // 現在値
    private int currentHP;
    private int currentStamina;

    /// <summary>
    /// 現在のHP（体力）
    /// </summary>
    public int CurrentHP => currentHP;

    /// <summary>
    /// 最大HP
    /// </summary>
    public int MaxHP => MAX_HP;

    /// <summary>
    /// 現在のスタミナ（疲労度の逆、高いほど元気）
    /// </summary>
    public int CurrentStamina => currentStamina;

    /// <summary>
    /// 最大スタミナ
    /// </summary>
    public int MaxStamina => MAX_STAMINA;

    /// <summary>
    /// 疲労度（0-100、スタミナの逆）
    /// </summary>
    public int Fatigue => MAX_STAMINA - currentStamina;

    /// <summary>
    /// HP割合（0.0 - 1.0）
    /// </summary>
    public float HPRatio => (float)currentHP / MAX_HP;

    /// <summary>
    /// スタミナ割合（0.0 - 1.0）
    /// </summary>
    public float StaminaRatio => (float)currentStamina / MAX_STAMINA;

    /// <summary>
    /// 疲労割合（0.0 - 1.0）
    /// </summary>
    public float FatigueRatio => (float)Fatigue / MAX_STAMINA;

    /// <summary>
    /// HPが変化した時のイベント
    /// </summary>
    public event Action<int, int> OnHPChanged;  // (current, change)

    /// <summary>
    /// スタミナが変化した時のイベント
    /// </summary>
    public event Action<int, int> OnStaminaChanged;  // (current, change)

    /// <summary>
    /// 死亡した時のイベント
    /// </summary>
    public event Action OnDeath;

    /// <summary>
    /// コンストラクタ（初期値は最大値）
    /// </summary>
    public PlayerStatus()
    {
        currentHP = MAX_HP;
        currentStamina = MAX_STAMINA;
        Debug.Log("PlayerStatus initialized - HP: 100/100, Stamina: 100/100");
    }

    #region HP操作

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        int previousHP = currentHP;
        currentHP = Mathf.Max(0, currentHP - damage);

        Debug.Log($"ダメージ: -{damage} HP ({previousHP} → {currentHP})");
        OnHPChanged?.Invoke(currentHP, -damage);

        if (currentHP == 0 && previousHP > 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 回復する
    /// </summary>
    public void Heal(int amount)
    {
        if (amount <= 0) return;

        int previousHP = currentHP;
        currentHP = Mathf.Min(MAX_HP, currentHP + amount);

        int actualHeal = currentHP - previousHP;
        if (actualHeal > 0)
        {
            Debug.Log($"HP回復: +{actualHeal} HP ({previousHP} → {currentHP})");
            OnHPChanged?.Invoke(currentHP, actualHeal);
        }
    }

    /// <summary>
    /// HPを完全回復
    /// </summary>
    public void FullHeal()
    {
        Heal(MAX_HP);
    }

    /// <summary>
    /// HPを直接設定（ロード時のみ使用）
    /// </summary>
    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, MAX_HP);
    }

    #endregion

    #region スタミナ操作

    /// <summary>
    /// スタミナを消費する（疲労が増える）
    /// </summary>
    public void ConsumeStamina(int amount)
    {
        if (amount <= 0) return;

        int previousStamina = currentStamina;
        currentStamina = Mathf.Max(0, currentStamina - amount);

        Debug.Log($"スタミナ消費: -{amount} ({previousStamina} → {currentStamina})");
        OnStaminaChanged?.Invoke(currentStamina, -amount);
    }

    /// <summary>
    /// スタミナを回復する（疲労が減る）
    /// </summary>
    public void RecoverStamina(int amount)
    {
        if (amount <= 0) return;

        int previousStamina = currentStamina;
        currentStamina = Mathf.Min(MAX_STAMINA, currentStamina + amount);

        int actualRecovery = currentStamina - previousStamina;
        if (actualRecovery > 0)
        {
            Debug.Log($"スタミナ回復: +{actualRecovery} ({previousStamina} → {currentStamina})");
            OnStaminaChanged?.Invoke(currentStamina, actualRecovery);
        }
    }

    /// <summary>
    /// スタミナを完全回復
    /// </summary>
    public void FullRecoverStamina()
    {
        RecoverStamina(MAX_STAMINA);
    }

    /// <summary>
    /// スタミナを直接設定（ロード時のみ使用）
    /// </summary>
    public void SetStamina(int stamina)
    {
        currentStamina = Mathf.Clamp(stamina, 0, MAX_STAMINA);
    }

    #endregion

    #region 状態判定

    /// <summary>
    /// 生きているか
    /// </summary>
    public bool IsAlive => currentHP > 0;

    /// <summary>
    /// 死亡しているか
    /// </summary>
    public bool IsDead => currentHP <= 0;

    /// <summary>
    /// HPが危険な状態か（30%以下）
    /// </summary>
    public bool IsHPCritical => HPRatio <= 0.3f;

    /// <summary>
    /// HPが警告状態か（50%以下）
    /// </summary>
    public bool IsHPLow => HPRatio <= 0.5f;

    /// <summary>
    /// 非常に疲れている（疲労度70%以上）
    /// </summary>
    public bool IsExhausted => FatigueRatio >= 0.7f;

    /// <summary>
    /// 疲れている（疲労度50%以上）
    /// </summary>
    public bool IsTired => FatigueRatio >= 0.5f;

    /// <summary>
    /// 元気な状態か（HP・スタミナ共に80%以上）
    /// </summary>
    public bool IsHealthy => HPRatio >= 0.8f && StaminaRatio >= 0.8f;

    /// <summary>
    /// 行動可能か（HPとスタミナが最低限ある）
    /// </summary>
    public bool CanAct => IsAlive && currentStamina > 0;

    #endregion

    #region 時間経過処理

    /// <summary>
    /// 時間経過によるスタミナ回復（睡眠時など）
    /// </summary>
    public void ApplyRest(int recoveryAmount)
    {
        RecoverStamina(recoveryAmount);
        Debug.Log($"休憩でスタミナ回復: +{recoveryAmount}");
    }

    /// <summary>
    /// 時間経過による自然な疲労（活動時）
    /// </summary>
    public void ApplyNaturalFatigue(int amount)
    {
        ConsumeStamina(amount);
    }

    #endregion

    /// <summary>
    /// 死亡処理
    /// </summary>
    private void Die()
    {
        Debug.Log("主人公が死亡しました");
        OnDeath?.Invoke();
    }

    /// <summary>
    /// ステータスのサマリー
    /// </summary>
    public string GetSummary()
    {
        return $"HP: {currentHP}/{MAX_HP} ({HPRatio:P0}), スタミナ: {currentStamina}/{MAX_STAMINA} ({StaminaRatio:P0})";
    }

    /// <summary>
    /// 状態を文字列で取得
    /// </summary>
    public string GetConditionText()
    {
        if (IsDead) return "死亡";
        if (IsHPCritical) return "瀕死";
        if (IsExhausted) return "過労";
        if (IsHPLow && IsTired) return "不調";
        if (IsHPLow) return "負傷";
        if (IsTired) return "疲労";
        if (IsHealthy) return "良好";
        return "普通";
    }
}

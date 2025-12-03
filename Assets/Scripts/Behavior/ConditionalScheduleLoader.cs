using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 条件付きでキャラクタースケジュールをロードする
/// 他のアプリの存在や、フラグ、好感度などの条件で追加スケジュールを読み込む
/// </summary>
[CreateAssetMenu(fileName = "ConditionalScheduleLoader", menuName = "Island/Conditional Schedule Loader", order = 2)]
public class ConditionalScheduleLoader : ScriptableObject
{
    public enum ScheduleMode
    {
        Replace,    // 完全置き換え（conditionalScheduleを全て使用）
        Override    // 上書き（conditionalScheduleで定義された時間帯のみ上書き）
    }

    [System.Serializable]
    public class ScheduleCondition
    {
        [Header("Condition Types (複数選択可能、全て満たす必要あり)")]
        public bool checkAppInstalled = false;
        public bool checkFlagEnabled = false;
        public bool checkAffectionLevel = false;
        public bool checkDayRange = false;
        public bool checkEquipment = false;

        [Header("App Check")]
        [Tooltip("チェックするアプリのパッケージ名（例: com.example.patchapp）")]
        public string requiredAppPackage = "";

        [Header("Flag Check")]
        [Tooltip("必要なフラグID")]
        public string requiredFlagId = "";

        [Header("Affection Check")]
        [Tooltip("必要な好感度")]
        public int requiredAffection = 0;
        [Tooltip("好感度チェック対象のキャラクターID")]
        public string targetCharacterId = "";

        [Header("Day Range Check")]
        [Tooltip("最小日数（0 = 無制限）")]
        public int minDay = 0;
        [Tooltip("最大日数（0 = 無制限）")]
        public int maxDay = 0;

        [Header("Equipment Check")]
        [Tooltip("装備が必要なアイテムのID")]
        public string requiredEquipmentId = "";

        [Header("Schedule Override")]
        [Tooltip("条件を満たした場合に使用するスケジュール")]
        public CharacterSchedule conditionalSchedule;

        [Header("Schedule Mode")]
        [Tooltip("Replace: 完全置き換え / Override: 定義された時間帯のみ上書き")]
        public ScheduleMode scheduleMode = ScheduleMode.Override;

        [Header("Priority")]
        [Tooltip("優先度（高いほど優先）")]
        public int priority = 0;
    }

    [Header("Conditions")]
    [SerializeField] private List<ScheduleCondition> conditions = new List<ScheduleCondition>();

    /// <summary>
    /// 条件を評価して、適用すべきスケジュールを返す
    /// </summary>
    /// <param name="baseSchedule">基本スケジュール</param>
    /// <returns>条件に応じたスケジュール（条件を満たさない場合はbaseSchedule）</returns>
    public CharacterSchedule EvaluateSchedule(CharacterSchedule baseSchedule)
    {
        // 優先度でソート
        conditions.Sort((a, b) => b.priority.CompareTo(a.priority));

        CharacterSchedule resultSchedule = baseSchedule;

        foreach (var condition in conditions)
        {
            if (EvaluateCondition(condition))
            {
                if (condition.scheduleMode == ScheduleMode.Replace)
                {
                    // 完全置き換え
                    Debug.Log($"Condition met: Replacing schedule with '{condition.conditionalSchedule.name}' (Priority: {condition.priority}, Mode: Replace)");
                    return condition.conditionalSchedule;
                }
                else // ScheduleMode.Override
                {
                    // 上書き（マージ）
                    Debug.Log($"Condition met: Overriding schedule with '{condition.conditionalSchedule.name}' (Priority: {condition.priority}, Mode: Override)");
                    resultSchedule = resultSchedule.MergeWith(condition.conditionalSchedule);
                }
            }
        }

        return resultSchedule;
    }

    /// <summary>
    /// 個別の条件を評価（複数条件を全て満たす必要がある）
    /// </summary>
    private bool EvaluateCondition(ScheduleCondition condition)
    {
        // チェックする条件がない場合はfalse
        bool hasAnyCondition = condition.checkAppInstalled || condition.checkFlagEnabled ||
                              condition.checkAffectionLevel || condition.checkDayRange ||
                              condition.checkEquipment;

        if (!hasAnyCondition)
        {
            Debug.LogWarning("No condition type is checked");
            return false;
        }

        // 全ての有効な条件をチェック（AND条件）
        // 1つでも条件を満たさなければfalse
        if (condition.checkAppInstalled && !EvaluateAppInstalled(condition)) return false;
        if (condition.checkFlagEnabled && !EvaluateFlagEnabled(condition)) return false;
        if (condition.checkAffectionLevel && !EvaluateAffectionLevel(condition)) return false;
        if (condition.checkDayRange && !EvaluateDayRange(condition)) return false;
        if (condition.checkEquipment && !EvaluateEquipment(condition)) return false;

        // 全ての条件を満たした
        return true;
    }

    /// <summary>
    /// アプリインストール条件を評価
    /// </summary>
    private bool EvaluateAppInstalled(ScheduleCondition condition)
    {
        if (string.IsNullOrEmpty(condition.requiredAppPackage))
        {
            Debug.LogWarning("Required app package is not set");
            return false;
        }

#if UNITY_EDITOR
        // エディタではデバッグ用の設定を使用
        return IsAppInstalledDebug(condition.requiredAppPackage);
#else
        // 既存のAndroidPackageCheckerを使用
        return AndroidPackageChecker.IsPackageInstalled(condition.requiredAppPackage);
#endif
    }

    /// <summary>
    /// エディタ用デバッグ: 特定アプリを「インストール済み」として扱う
    /// </summary>
#if UNITY_EDITOR
    [Header("Editor Debug")]
    [SerializeField] private string[] debugInstalledApps = new string[0];

    private bool IsAppInstalledDebug(string packageName)
    {
        foreach (string app in debugInstalledApps)
        {
            if (app == packageName)
            {
                Debug.Log($"[Debug] App '{packageName}' is installed (editor debug mode)");
                return true;
            }
        }
        return false;
    }
#endif

    /// <summary>
    /// フラグ条件を評価
    /// </summary>
    private bool EvaluateFlagEnabled(ScheduleCondition condition)
    {
        if (string.IsNullOrEmpty(condition.requiredFlagId))
        {
            Debug.LogWarning("Required flag ID is not set");
            return false;
        }

        // GameManagerからフラグ情報を取得
        if (GameManager.Instance != null && GameManager.Instance.State != null)
        {
            return GameManager.Instance.State.Flags.IsFlagEnabled(condition.requiredFlagId);
        }

        return false;
    }

    /// <summary>
    /// 好感度条件を評価
    /// </summary>
    private bool EvaluateAffectionLevel(ScheduleCondition condition)
    {
        if (string.IsNullOrEmpty(condition.targetCharacterId))
        {
            Debug.LogWarning("Target character ID is not set");
            return false;
        }

        // CharacterManagerから直接取得（既存システムを使用）
        if (GameManager.Instance != null && GameManager.Instance.State != null)
        {
            var character = GameManager.Instance.State.Characters.GetCharacter(condition.targetCharacterId);
            if (character != null)
            {
                // Romanceを好感度として使用
                return character.Romance >= condition.requiredAffection;
            }
        }

        return false;
    }

    /// <summary>
    /// 日数範囲条件を評価
    /// </summary>
    private bool EvaluateDayRange(ScheduleCondition condition)
    {
        if (GameManager.Instance == null || GameManager.Instance.State == null)
        {
            return false;
        }

        int currentDay = GameManager.Instance.State.CurrentDay;

        // minDayチェック（0なら無制限）
        if (condition.minDay > 0 && currentDay < condition.minDay)
        {
            return false;
        }

        // maxDayチェック（0なら無制限）
        if (condition.maxDay > 0 && currentDay > condition.maxDay)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 装備条件を評価
    /// </summary>
    private bool EvaluateEquipment(ScheduleCondition condition)
    {
        if (string.IsNullOrEmpty(condition.requiredEquipmentId))
        {
            Debug.LogWarning("Required equipment ID is not set");
            return false;
        }

        // GameManagerから装備情報を取得
        if (GameManager.Instance != null && GameManager.Instance.State != null)
        {
            return GameManager.Instance.State.IsEquipped(condition.requiredEquipmentId);
        }

        return false;
    }

    /// <summary>
    /// 全ての条件をチェックして、適用可能な条件のリストを取得（デバッグ用）
    /// </summary>
    public List<ScheduleCondition> GetMetConditions()
    {
        List<ScheduleCondition> metConditions = new List<ScheduleCondition>();

        foreach (var condition in conditions)
        {
            if (EvaluateCondition(condition))
            {
                metConditions.Add(condition);
            }
        }

        return metConditions;
    }

    /// <summary>
    /// 条件を追加（動的）
    /// </summary>
    public void AddCondition(ScheduleCondition condition)
    {
        conditions.Add(condition);
    }

    /// <summary>
    /// 条件をクリア
    /// </summary>
    public void ClearConditions()
    {
        conditions.Clear();
    }
}

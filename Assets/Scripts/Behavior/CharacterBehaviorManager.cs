using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全キャラクターの行動を一括管理するマネージャー
/// 時間帯変化を監視して、各キャラクターの行動を自動更新
/// </summary>
public class CharacterBehaviorManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterEntry
    {
        public InteractableCharacter character;       // キャラクター本体
        public CharacterSchedule schedule;            // 基本スケジュール定義
        public ConditionalScheduleLoader conditionalLoader;  // 条件付きスケジュールローダー（オプション）

        private CharacterSchedule _evaluatedSchedule;  // 評価済みスケジュール（キャッシュ）

        /// <summary>
        /// 条件を評価して適用すべきスケジュールを取得
        /// </summary>
        public CharacterSchedule GetEvaluatedSchedule()
        {
            if (conditionalLoader != null)
            {
                return conditionalLoader.EvaluateSchedule(schedule);
            }

            return schedule;
        }

        /// <summary>
        /// 条件付きスケジュールを使用しているか
        /// </summary>
        public bool IsUsingConditionalSchedule()
        {
            if (conditionalLoader == null)
            {
                return false;
            }

            CharacterSchedule evaluated = conditionalLoader.EvaluateSchedule(schedule);
            return evaluated != schedule;
        }
    }

    /// <summary>
    /// ScriptableObject ベースのキャラクターエントリ
    /// </summary>
    [System.Serializable]
    public class CharacterDataEntry
    {
        public CharacterData data;                    // キャラクターデータ（ScriptableObject）
        public ConditionalScheduleLoader conditionalLoader;  // 条件付きスケジュールローダー（オプション）

        /// <summary>
        /// 条件を評価して適用すべきスケジュールを取得
        /// </summary>
        public CharacterSchedule GetEvaluatedSchedule()
        {
            if (data == null) return null;

            if (conditionalLoader != null)
            {
                return conditionalLoader.EvaluateSchedule(data.defaultSchedule);
            }

            return data.defaultSchedule;
        }

        /// <summary>
        /// 条件付きスケジュールを使用しているか
        /// </summary>
        public bool IsUsingConditionalSchedule()
        {
            if (conditionalLoader == null || data == null)
            {
                return false;
            }

            CharacterSchedule evaluated = conditionalLoader.EvaluateSchedule(data.defaultSchedule);
            return evaluated != data.defaultSchedule;
        }
    }

    /// <summary>
    /// 場所にいるキャラクター情報（ScriptableObject版）
    /// </summary>
    public class CharacterDataLocationInfo
    {
        public CharacterData data;
        public bool isUsingConditionalSchedule;
    }

    [Header("Characters (ScriptableObject)")]
    [SerializeField] private List<CharacterDataEntry> characterDataList = new List<CharacterDataEntry>();

    [Header("Characters (Legacy)")]
    [SerializeField] private List<CharacterEntry> characters = new List<CharacterEntry>();

    [Header("Special Events")]
    [SerializeField] private string currentEventId = "";  // 現在のイベントID（空なら通常スケジュール）

    [Header("Auto Update")]
    [SerializeField] private bool autoUpdateOnTimeChange = true;  // 時間帯変化で自動更新

    private TimeOfDay lastTimeOfDay;

    private void Start()
    {
        // 初期化時に現在の時間帯の行動を適用
        if (GameManager.Instance.GlobalGameTime != null)
        {
            lastTimeOfDay = GameManager.Instance.GlobalGameTime.CurrentTimeOfDay;
            UpdateAllCharacters(lastTimeOfDay);
        }

        // 時間帯変化イベントを購読
        if (autoUpdateOnTimeChange && GameManager.Instance != null)
        {
            GameManager.Instance.OnTimeOfDayChanged += OnTimeOfDayChanged;
        }

        // スケジュール条件変更イベントを購読
        if (GameManager.Instance != null && GameManager.Instance.State != null)
        {
            GameManager.Instance.State.OnScheduleConditionChanged += OnScheduleConditionChanged;
        }

        Debug.Log("CharacterBehaviorManager initialized");
    }

    private void OnDestroy()
    {
        // イベント購読解除
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimeOfDayChanged -= OnTimeOfDayChanged;

            if (GameManager.Instance.State != null)
            {
                GameManager.Instance.State.OnScheduleConditionChanged -= OnScheduleConditionChanged;
            }
        }
    }

    /// <summary>
    /// 時間帯が変わった時の処理
    /// </summary>
    private void OnTimeOfDayChanged(TimeOfDay previous, TimeOfDay current)
    {
        Debug.Log($"[BehaviorManager] Time changed: {previous.ToJapaneseString()} → {current.ToJapaneseString()}");

        lastTimeOfDay = current;
        UpdateAllCharacters(current);
    }

    /// <summary>
    /// スケジュール条件が変更された時の処理
    /// （装備、好感度、フラグなどが変更された時に自動的に呼ばれる）
    /// </summary>
    private void OnScheduleConditionChanged()
    {
        Debug.Log("[BehaviorManager] Schedule condition changed, re-evaluating schedules...");
        ReevaluateConditionalSchedules();
    }

    /// <summary>
    /// 全キャラクターの行動を更新
    /// </summary>
    public void UpdateAllCharacters(TimeOfDay timeOfDay)
    {
        foreach (var entry in characters)
        {
            if (entry.character == null || entry.schedule == null)
            {
                Debug.LogWarning("Character or schedule is null. Skipping.");
                continue;
            }

            UpdateCharacter(entry, timeOfDay);
        }

        Debug.Log($"All characters updated for {timeOfDay.ToJapaneseString()}");
    }

    /// <summary>
    /// 特定のキャラクターの行動を更新
    /// </summary>
    private void UpdateCharacter(CharacterEntry entry, TimeOfDay timeOfDay)
    {
        CharacterAction action;

        // 条件を評価して適用すべきスケジュールを取得
        CharacterSchedule activeSchedule = entry.GetEvaluatedSchedule();

        // 特別イベント中の場合は、イベント行動を優先
        if (!string.IsNullOrEmpty(currentEventId))
        {
            action = activeSchedule.GetActionForEvent(currentEventId);

            if (action != null)
            {
                Debug.Log($"Applying special event action '{currentEventId}' to {entry.character.name}");
                action.ApplyToCharacter(entry.character);
                return;
            }
        }

        // 通常スケジュールの行動を適用
        action = activeSchedule.GetActionForTimeOfDay(timeOfDay);
        action.ApplyToCharacter(entry.character);
    }

    /// <summary>
    /// 特別イベントを開始
    /// </summary>
    public void StartEvent(string eventId)
    {
        if (string.IsNullOrEmpty(eventId))
        {
            Debug.LogWarning("Event ID is empty. Cannot start event.");
            return;
        }

        Debug.Log($"Starting special event: {eventId}");

        currentEventId = eventId;

        // 全キャラクターの行動を更新
        UpdateAllCharacters(lastTimeOfDay);
    }

    /// <summary>
    /// 特別イベントを終了
    /// </summary>
    public void EndEvent()
    {
        if (string.IsNullOrEmpty(currentEventId))
        {
            Debug.LogWarning("No event is currently running.");
            return;
        }

        Debug.Log($"Ending special event: {currentEventId}");

        currentEventId = "";

        // 通常スケジュールに戻す
        UpdateAllCharacters(lastTimeOfDay);
    }

    /// <summary>
    /// 現在のイベントIDを取得
    /// </summary>
    public string GetCurrentEventId()
    {
        return currentEventId;
    }

    /// <summary>
    /// イベント中かどうか
    /// </summary>
    public bool IsEventRunning()
    {
        return !string.IsNullOrEmpty(currentEventId);
    }

    /// <summary>
    /// キャラクターを追加（動的）
    /// </summary>
    public void AddCharacter(InteractableCharacter character, CharacterSchedule schedule)
    {
        if (character == null || schedule == null)
        {
            Debug.LogWarning("Character or schedule is null. Cannot add.");
            return;
        }

        characters.Add(new CharacterEntry
        {
            character = character,
            schedule = schedule
        });

        // 追加したキャラクターの行動を即座に適用
        UpdateCharacter(characters[characters.Count - 1], lastTimeOfDay);

        Debug.Log($"Character {character.name} added to behavior manager");
    }

    /// <summary>
    /// キャラクターを削除
    /// </summary>
    public void RemoveCharacter(InteractableCharacter character)
    {
        characters.RemoveAll(entry => entry.character == character);
        Debug.Log($"Character {character.name} removed from behavior manager");
    }

    /// <summary>
    /// 特定のキャラクターのスケジュールを取得（ScriptableObject版）
    /// </summary>
    public CharacterSchedule GetSchedule(string characterId)
    {
        // ScriptableObject版から検索
        foreach (var entry in characterDataList)
        {
            if (entry.data != null && entry.data.characterId == characterId)
            {
                return entry.GetEvaluatedSchedule();
            }
        }

        return null;
    }

    /// <summary>
    /// 手動で全キャラクターを更新（デバッグ用）
    /// </summary>
    [ContextMenu("Force Update All Characters")]
    public void ForceUpdateAllCharacters()
    {
        if (GameManager.Instance.GlobalGameTime != null)
        {
            TimeOfDay current = GameManager.Instance.GlobalGameTime.CurrentTimeOfDay;
            UpdateAllCharacters(current);
        }
        else
        {
            Debug.LogWarning("GameTime is not available");
        }
    }

    /// <summary>
    /// 全スケジュールの検証
    /// </summary>
    [ContextMenu("Validate All Schedules")]
    public void ValidateAllSchedules()
    {
        bool allValid = true;

        foreach (var entry in characters)
        {
            if (entry.schedule == null)
            {
                Debug.LogError($"Schedule is null for {entry.character.name}");
                allValid = false;
                continue;
            }

            if (!entry.schedule.ValidateSchedule())
            {
                allValid = false;
            }
        }

        if (allValid)
        {
            Debug.Log("All schedules are valid!");
        }
        else
        {
            Debug.LogWarning("Some schedules have issues. Check the console.");
        }
    }

    /// <summary>
    /// 条件付きスケジュールを再評価（アプリインストール状況が変わった時など）
    /// </summary>
    public void ReevaluateConditionalSchedules()
    {
        Debug.Log("Re-evaluating conditional schedules...");

        // 現在の時間帯で全員を更新（条件を再評価）
        UpdateAllCharacters(lastTimeOfDay);

        Debug.Log("Conditional schedules re-evaluated");
    }

    /// <summary>
    /// デバッグ: 現在適用されているスケジュールを表示
    /// </summary>
    [ContextMenu("Show Active Schedules")]
    public void ShowActiveSchedules()
    {
        foreach (var entry in characters)
        {
            CharacterSchedule activeSchedule = entry.GetEvaluatedSchedule();
            string scheduleName = activeSchedule != null ? activeSchedule.name : "null";
            Debug.Log($"{entry.character.name}: {scheduleName}");
        }
    }

    /// <summary>
    /// 指定した場所にいるキャラクターのリストを取得
    /// </summary>
    public List<CharacterLocationInfo> GetCharactersAtLocation(MapLocation location, TimeOfDay timeOfDay)
    {
        List<CharacterLocationInfo> result = new List<CharacterLocationInfo>();

        foreach (var entry in characters)
        {
            if (entry.character == null || entry.schedule == null)
            {
                continue;
            }

            // 評価済みスケジュールを取得
            CharacterSchedule activeSchedule = entry.GetEvaluatedSchedule();

            // 現在の時間帯の行動を取得
            CharacterAction action = activeSchedule.GetActionForTimeOfDay(timeOfDay);

            // この場所にいて、存在している場合
            if (action != null && action.isPresent && action.location == location)
            {
                result.Add(new CharacterLocationInfo
                {
                    character = entry.character,
                    isUsingConditionalSchedule = entry.IsUsingConditionalSchedule()
                });
            }
        }

        return result;
    }

    /// <summary>
    /// 場所にいるキャラクター情報
    /// </summary>
    [System.Serializable]
    public class CharacterLocationInfo
    {
        public InteractableCharacter character;
        public bool isUsingConditionalSchedule; // 条件付きスケジュールを使用中か
    }

    /// <summary>
    /// 全キャラクターEntryを取得（読み取り専用）
    /// </summary>
    public IReadOnlyList<CharacterEntry> AllCharacterEntries => characters;

    /// <summary>
    /// 指定した場所にいるキャラクターのリストを取得（ScriptableObject版）
    /// </summary>
    public List<CharacterDataLocationInfo> GetCharacterDataAtLocation(MapLocation location, TimeOfDay timeOfDay)
    {
        List<CharacterDataLocationInfo> result = new List<CharacterDataLocationInfo>();

        foreach (var entry in characterDataList)
        {
            if (entry.data == null || entry.data.defaultSchedule == null)
            {
                continue;
            }

            // 評価済みスケジュールを取得
            CharacterSchedule activeSchedule = entry.GetEvaluatedSchedule();

            // 現在の時間帯の行動を取得
            CharacterAction action = activeSchedule.GetActionForTimeOfDay(timeOfDay);

            // この場所にいて、存在している場合
            if (action != null && action.isPresent && action.location == location)
            {
                result.Add(new CharacterDataLocationInfo
                {
                    data = entry.data,
                    isUsingConditionalSchedule = entry.IsUsingConditionalSchedule()
                });
            }
        }

        return result;
    }
}

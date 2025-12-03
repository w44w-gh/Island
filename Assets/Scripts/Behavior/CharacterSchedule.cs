using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターの1日のスケジュールを定義するScriptableObject
/// エディタで編集可能なアセットとして保存
/// </summary>
[CreateAssetMenu(fileName = "CharacterSchedule", menuName = "Island/Character Schedule", order = 1)]
public class CharacterSchedule : ScriptableObject
{
    [Header("Character Info")]
    public string characterId = "emily";  // キャラクターID
    public string characterName = "Emily";  // キャラクター名（表示用）

    [Header("Daily Schedule")]
    [Tooltip("時間帯ごとの行動リスト")]
    public List<CharacterAction> actions = new List<CharacterAction>();

    [Header("Special Events")]
    [Tooltip("特別なイベント時の行動（優先度が高い）")]
    public List<SpecialEventAction> specialEvents = new List<SpecialEventAction>();

    [System.Serializable]
    public class SpecialEventAction
    {
        public string eventId;  // イベントID（"beach_party", "birthday"など）
        public CharacterAction action;  // イベント時の行動
    }

    /// <summary>
    /// 指定した時間帯の行動を取得
    /// </summary>
    public CharacterAction GetActionForTimeOfDay(TimeOfDay timeOfDay)
    {
        foreach (var action in actions)
        {
            if (action.timeOfDay == timeOfDay)
            {
                return action;
            }
        }

        // 見つからない場合はデフォルト行動を返す
        Debug.LogWarning($"No action defined for {characterId} at {timeOfDay.ToJapaneseString()}. Using default.");
        return GetDefaultAction(timeOfDay);
    }

    /// <summary>
    /// 特別イベントの行動を取得
    /// </summary>
    public CharacterAction GetActionForEvent(string eventId)
    {
        foreach (var specialEvent in specialEvents)
        {
            if (specialEvent.eventId == eventId)
            {
                return specialEvent.action;
            }
        }

        return null;
    }

    /// <summary>
    /// デフォルトの行動を返す
    /// </summary>
    private CharacterAction GetDefaultAction(TimeOfDay timeOfDay)
    {
        return new CharacterAction
        {
            timeOfDay = timeOfDay,
            isPresent = true,
            position = CharacterPositionPreset.PositionPreset.CenterNear,
            appearanceVariation = "normal",
            isInteractable = true,
            statusMessage = ""
        };
    }

    /// <summary>
    /// 全ての時間帯の行動を自動生成（初期設定用）
    /// </summary>
    [ContextMenu("Generate Default Schedule")]
    public void GenerateDefaultSchedule()
    {
        actions.Clear();

        // 各時間帯のデフォルト行動を生成
        actions.Add(new CharacterAction
        {
            timeOfDay = TimeOfDay.EarlyMorning,
            isPresent = false,  // 早朝は寝ている
            appearanceVariation = "sleep"
        });

        actions.Add(new CharacterAction
        {
            timeOfDay = TimeOfDay.Morning,
            isPresent = true,
            position = CharacterPositionPreset.PositionPreset.LeftNear,
            appearanceVariation = "normal",
            isInteractable = true,
            statusMessage = "おはよう！"
        });

        actions.Add(new CharacterAction
        {
            timeOfDay = TimeOfDay.Noon,
            isPresent = true,
            position = CharacterPositionPreset.PositionPreset.CenterMiddle,
            appearanceVariation = "normal",
            isInteractable = true,
            statusMessage = "お昼ごはんの時間！"
        });

        actions.Add(new CharacterAction
        {
            timeOfDay = TimeOfDay.Evening,
            isPresent = true,
            position = CharacterPositionPreset.PositionPreset.RightNear,
            appearanceVariation = "normal",
            isInteractable = true,
            statusMessage = "夕方だね"
        });

        actions.Add(new CharacterAction
        {
            timeOfDay = TimeOfDay.Midnight,
            isPresent = false,  // 深夜は寝ている
            appearanceVariation = "sleep"
        });

        Debug.Log($"Default schedule generated for {characterName}");
    }

    /// <summary>
    /// スケジュールの検証
    /// </summary>
    public bool ValidateSchedule()
    {
        // 全ての時間帯が定義されているかチェック
        TimeOfDay[] allTimes = new TimeOfDay[]
        {
            TimeOfDay.EarlyMorning,
            TimeOfDay.Morning,
            TimeOfDay.Noon,
            TimeOfDay.Evening,
            TimeOfDay.Midnight
        };

        foreach (var time in allTimes)
        {
            bool found = false;
            foreach (var action in actions)
            {
                if (action.timeOfDay == time)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.LogWarning($"[{characterName}] No action defined for {time.ToJapaneseString()}");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 別のスケジュールとマージして新しいスケジュールを返す
    /// overrideScheduleで定義されている時間帯は上書きされる
    /// </summary>
    public CharacterSchedule MergeWith(CharacterSchedule overrideSchedule)
    {
        // 新しいスケジュールインスタンスを作成
        CharacterSchedule merged = ScriptableObject.CreateInstance<CharacterSchedule>();
        merged.characterId = this.characterId;
        merged.characterName = this.characterName;

        // 基本スケジュールのactionsをコピー
        merged.actions = new List<CharacterAction>();
        foreach (var action in this.actions)
        {
            merged.actions.Add(action.Clone());
        }

        // overrideScheduleのactionsで上書き
        foreach (var overrideAction in overrideSchedule.actions)
        {
            bool found = false;
            for (int i = 0; i < merged.actions.Count; i++)
            {
                if (merged.actions[i].timeOfDay == overrideAction.timeOfDay)
                {
                    // 同じ時間帯があれば上書き
                    merged.actions[i] = overrideAction.Clone();
                    found = true;
                    break;
                }
            }

            // 同じ時間帯がなければ追加
            if (!found)
            {
                merged.actions.Add(overrideAction.Clone());
            }
        }

        // specialEventsもマージ
        merged.specialEvents = new List<SpecialEventAction>();
        foreach (var specialEvent in this.specialEvents)
        {
            merged.specialEvents.Add(new SpecialEventAction
            {
                eventId = specialEvent.eventId,
                action = specialEvent.action.Clone()
            });
        }

        // overrideScheduleのspecialEventsで上書き
        foreach (var overrideEvent in overrideSchedule.specialEvents)
        {
            bool found = false;
            for (int i = 0; i < merged.specialEvents.Count; i++)
            {
                if (merged.specialEvents[i].eventId == overrideEvent.eventId)
                {
                    merged.specialEvents[i].action = overrideEvent.action.Clone();
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                merged.specialEvents.Add(new SpecialEventAction
                {
                    eventId = overrideEvent.eventId,
                    action = overrideEvent.action.Clone()
                });
            }
        }

        return merged;
    }
}

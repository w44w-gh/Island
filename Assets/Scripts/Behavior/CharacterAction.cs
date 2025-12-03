using UnityEngine;

/// <summary>
/// キャラクターの時間帯ごとの行動を定義
/// 配置、表情、セリフ、有効/無効などを設定
/// </summary>
[System.Serializable]
public class CharacterAction
{
    [Header("Time Setting")]
    public TimeOfDay timeOfDay = TimeOfDay.Morning;  // この行動を実行する時間帯

    [Header("Presence")]
    public bool isPresent = true;  // この時間帯にキャラがいるか

    [Header("Location")]
    public MapLocation location = MapLocation.Beach;  // どの場所にいるか

    [Header("Position")]
    public CharacterPositionPreset.PositionPreset position = CharacterPositionPreset.PositionPreset.CenterNear;
    public bool useCustomPosition = false;  // カスタム位置を使うか
    public Vector2 customPosition = Vector2.zero;  // カスタム位置

    [Header("Appearance")]
    public string appearanceVariation = "normal";  // 表情・衣装

    [Header("Interaction")]
    public bool isInteractable = true;  // 話しかけられるか
    public string scenarioLabel = "";    // 会話シナリオのラベル（空なら変更なし）

    [Header("Message")]
    [TextArea(2, 4)]
    public string statusMessage = "";  // ステータスメッセージ（「料理中」など）

    /// <summary>
    /// この行動をキャラクターに適用
    /// </summary>
    public void ApplyToCharacter(InteractableCharacter character)
    {
        if (character == null)
        {
            Debug.LogWarning("Character is null. Cannot apply action.");
            return;
        }

        // 存在の有効/無効
        character.gameObject.SetActive(isPresent);

        if (!isPresent)
        {
            return;  // いない場合は以降の処理をスキップ
        }

        // 位置を設定
        if (useCustomPosition)
        {
            RectTransform rect = character.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = customPosition;
            }
        }
        else
        {
            character.ApplyPositionPreset(position);
        }

        // 表情・衣装を変更
        character.ChangeAppearance(appearanceVariation);

        // インタラクション可能状態を設定
        character.SetInteractable(isInteractable);

        // シナリオラベルを変更（空でない場合のみ）
        if (!string.IsNullOrEmpty(scenarioLabel))
        {
            character.SetScenarioLabel(scenarioLabel);
        }

        Debug.Log($"Action applied to {character.name}: Time={timeOfDay.ToJapaneseString()}, Present={isPresent}, Position={position}, Appearance={appearanceVariation}");
    }

    /// <summary>
    /// アクションのコピーを作成
    /// </summary>
    public CharacterAction Clone()
    {
        return new CharacterAction
        {
            timeOfDay = this.timeOfDay,
            isPresent = this.isPresent,
            location = this.location,
            position = this.position,
            useCustomPosition = this.useCustomPosition,
            customPosition = this.customPosition,
            appearanceVariation = this.appearanceVariation,
            isInteractable = this.isInteractable,
            scenarioLabel = this.scenarioLabel,
            statusMessage = this.statusMessage
        };
    }
}

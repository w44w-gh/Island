using UnityEngine;

/// <summary>
/// キャラクターのマスターデータ（ScriptableObject）
/// </summary>
[CreateAssetMenu(fileName = "NewCharacter", menuName = "Island/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("基本情報")]
    public string characterId;          // キャラクターID（"emily", "alex"など）
    public string displayName;          // 表示名

    [Header("立ち絵")]
    public Sprite defaultSprite;        // デフォルトの立ち絵
    public Sprite[] expressions;        // 表情差分（オプション）

    [Header("マップアイコン")]
    public Sprite mapIcon;              // マップ上に表示するアイコン

    [Header("会話")]
    public string defaultScenarioLabel; // デフォルトの会話シナリオラベル

    [Header("スケジュール")]
    public CharacterSchedule defaultSchedule;  // デフォルトの行動スケジュール
}

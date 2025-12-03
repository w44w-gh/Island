using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マップ上の場所アイコンUI
/// その場所にいるキャラクターのアイコンを表示
/// </summary>
public class LocationIconUI : MonoBehaviour
{
    [Header("Location")]
    [SerializeField] private MapLocation location; // この場所

    [Header("Character Icons")]
    [SerializeField] private Transform characterIconsContainer; // キャラアイコンを配置する親
    [SerializeField] private GameObject characterIconPrefab; // キャラアイコンのPrefab

    [Header("Icon Settings")]
    [SerializeField] private Color normalIconColor = Color.white; // 通常スケジュールの色
    [SerializeField] private Color conditionalIconColor = Color.yellow; // 条件付きスケジュールの色（光るイメージ）

    private List<GameObject> spawnedIcons = new List<GameObject>();

    /// <summary>
    /// キャラアイコンを更新
    /// </summary>
    public void UpdateCharacterIcons(List<CharacterBehaviorManager.CharacterLocationInfo> charactersHere)
    {
        // 既存のアイコンをクリア
        ClearCharacterIcons();

        // キャラがいない場合は終了
        if (charactersHere == null || charactersHere.Count == 0)
        {
            return;
        }

        // キャラごとにアイコンを生成
        for (int i = 0; i < charactersHere.Count; i++)
        {
            var charInfo = charactersHere[i];
            CreateCharacterIcon(charInfo, i, charactersHere.Count);
        }
    }

    /// <summary>
    /// キャラアイコンを生成
    /// </summary>
    private void CreateCharacterIcon(CharacterBehaviorManager.CharacterLocationInfo charInfo, int index, int totalCount)
    {
        if (characterIconPrefab == null || characterIconsContainer == null)
        {
            Debug.LogWarning("CharacterIconPrefab or Container is not set");
            return;
        }

        // アイコンを生成
        GameObject iconObj = Instantiate(characterIconPrefab, characterIconsContainer);
        spawnedIcons.Add(iconObj);

        // Image コンポーネントを取得
        Image iconImage = iconObj.GetComponent<Image>();
        if (iconImage != null)
        {
            // キャラクターの画像を設定（ここでは仮で色で表現）
            // 実際にはcharInfo.character.GetIconSprite()などで取得
            iconImage.color = charInfo.isUsingConditionalSchedule ? conditionalIconColor : normalIconColor;

            // TODO: 実際のキャラアイコン画像を設定
            // iconImage.sprite = charInfo.character.iconSprite;
        }

        // 複数キャラがいる場合は横に並べる
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        if (iconRect != null && totalCount > 1)
        {
            // 横に並べる（簡易的な配置）
            float spacing = 30f;
            float offsetX = (index - (totalCount - 1) * 0.5f) * spacing;
            iconRect.anchoredPosition = new Vector2(offsetX, 0f);
        }

        // Outline/Shadowで特殊感を演出（条件付きスケジュールの場合）
        if (charInfo.isUsingConditionalSchedule)
        {
            Outline outline = iconObj.GetComponent<Outline>();
            if (outline == null)
            {
                outline = iconObj.AddComponent<Outline>();
            }
            outline.effectColor = Color.yellow;
            outline.effectDistance = new Vector2(2f, -2f);
        }
    }

    /// <summary>
    /// キャラアイコンをクリア
    /// </summary>
    private void ClearCharacterIcons()
    {
        foreach (var icon in spawnedIcons)
        {
            if (icon != null)
            {
                Destroy(icon);
            }
        }
        spawnedIcons.Clear();
    }

    /// <summary>
    /// この場所を取得
    /// </summary>
    public MapLocation Location => location;

    /// <summary>
    /// RectTransformを取得（ズーム時に使用）
    /// </summary>
    public RectTransform GetRectTransform()
    {
        return GetComponent<RectTransform>();
    }

    private void OnDestroy()
    {
        ClearCharacterIcons();
    }
}

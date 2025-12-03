using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タップ可能なキャラクター
/// タップするとNovelSceneに遷移して会話イベントを開始
/// 立ち絵の動的変更、遠近感、影表示に対応
/// </summary>
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CharacterDepth))]
public class InteractableCharacter : MonoBehaviour, IInteractable
{
    [Header("Character Settings")]
    [SerializeField] private string characterId;        // キャラクターID（"emily", "alex"など）
    [SerializeField] private string scenarioLabel;      // 会話シナリオのラベル

    [Header("Appearance")]
    [SerializeField] private CharacterAppearance appearance = new CharacterAppearance();  // 外見管理

    [Header("Position & Depth")]
    [SerializeField] private CharacterPositionPreset.PositionPreset initialPosition = CharacterPositionPreset.PositionPreset.CenterNear;
    [SerializeField] private bool usePositionPreset = true;  // プリセットを使用するか

    [Header("Interaction Settings")]
    [SerializeField] private bool isInteractable = true;  // インタラクション可能か

    [Header("Visual Feedback")]
    [SerializeField] private GameObject highlightEffect;  // ハイライト表示（オプション）

    private Image characterImage;
    private Button button;
    private CharacterDepth depthComponent;

    private void Awake()
    {
        // Imageコンポーネントを取得
        characterImage = GetComponent<Image>();

        // CharacterDepthコンポーネントを取得
        depthComponent = GetComponent<CharacterDepth>();

        // 初期立ち絵を設定
        UpdateAppearance(appearance.GetCurrentVariation());

        // Buttonコンポーネントを取得または追加
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }

        // ボタンクリック時にOnInteractを呼び出す
        button.onClick.AddListener(OnInteract);

        // ハイライトは初期状態で非表示
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }
    }

    private void Start()
    {
        // プリセット位置を適用
        if (usePositionPreset)
        {
            ApplyPositionPreset(initialPosition);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnInteract);
        }
    }

    /// <summary>
    /// タップされた時の処理
    /// </summary>
    public void OnInteract()
    {
        if (!CanInteract())
        {
            Debug.LogWarning($"Character {characterId} is not interactable");
            return;
        }

        Debug.Log($"Interacting with character: {characterId}");

        // タップSEを再生
        AudioManager.Instance.PlaySE("button_tap");

        // NovelSceneに遷移
        SceneLoader.Instance.LoadNovelScene(scenarioLabel, "GameScene", characterId);
    }

    /// <summary>
    /// インタラクション可能かどうか
    /// </summary>
    public bool CanInteract()
    {
        return isInteractable;
    }

    /// <summary>
    /// GameObjectを取得
    /// </summary>
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    /// <summary>
    /// インタラクション可能状態を設定
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;

        // ボタンの有効/無効を切り替え
        if (button != null)
        {
            button.interactable = interactable;
        }

        // グレーアウト表示（オプション）
        if (characterImage != null)
        {
            characterImage.color = interactable ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    /// <summary>
    /// ハイライト表示を切り替え
    /// </summary>
    public void SetHighlight(bool highlight)
    {
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(highlight);
        }
    }

    /// <summary>
    /// 外見を変更（表情、衣装など）
    /// </summary>
    public void ChangeAppearance(string variationName)
    {
        appearance.SetVariation(variationName);
        UpdateAppearance(variationName);

        Debug.Log($"{characterId} appearance changed to: {variationName}");
    }

    /// <summary>
    /// 外見を実際に更新
    /// </summary>
    private void UpdateAppearance(string variationName)
    {
        Sprite sprite = appearance.GetSprite(variationName);

        if (characterImage != null && sprite != null)
        {
            characterImage.sprite = sprite;

            // 影のSpriteも更新
            if (depthComponent != null)
            {
                depthComponent.UpdateShadowSprite(sprite);
            }
        }
    }

    /// <summary>
    /// 外見バリエーションを追加（動的）
    /// </summary>
    public void AddAppearanceVariation(string variationName, Sprite sprite)
    {
        appearance.AddVariation(variationName, sprite);
    }

    /// <summary>
    /// 現在の外見バリエーション名を取得
    /// </summary>
    public string GetCurrentAppearance()
    {
        return appearance.GetCurrentVariation();
    }

    /// <summary>
    /// 位置プリセットを適用
    /// </summary>
    public void ApplyPositionPreset(CharacterPositionPreset.PositionPreset preset)
    {
        CharacterPositionPreset positionManager = FindObjectOfType<CharacterPositionPreset>();

        if (positionManager != null)
        {
            positionManager.ApplyPreset(gameObject, preset);
        }
        else
        {
            Debug.LogWarning("CharacterPositionPreset not found in scene!");
        }
    }

    /// <summary>
    /// 深度を設定
    /// </summary>
    public void SetDepth(CharacterDepth.DepthLevel depth)
    {
        if (depthComponent != null)
        {
            depthComponent.SetDepth(depth);
        }
    }

    /// <summary>
    /// 現在の深度を取得
    /// </summary>
    public CharacterDepth.DepthLevel GetDepth()
    {
        if (depthComponent != null)
        {
            return depthComponent.GetDepth();
        }

        return CharacterDepth.DepthLevel.Near;
    }

    /// <summary>
    /// 影の有効/無効を切り替え
    /// </summary>
    public void SetShadowEnabled(bool enabled)
    {
        if (depthComponent != null)
        {
            depthComponent.SetShadowEnabled(enabled);
        }
    }

    /// <summary>
    /// シナリオラベルを設定（行動管理用）
    /// </summary>
    public void SetScenarioLabel(string label)
    {
        if (!string.IsNullOrEmpty(label))
        {
            scenarioLabel = label;
        }
    }

    /// <summary>
    /// 現在のシナリオラベルを取得
    /// </summary>
    public string GetScenarioLabel()
    {
        return scenarioLabel;
    }

    /// <summary>
    /// キャラクターを初期化（動的生成用）
    /// </summary>
    public void Setup(string charId, string scenario, Sprite sprite)
    {
        characterId = charId;
        scenarioLabel = scenario;

        // 外見に追加
        appearance.AddVariation("normal", sprite);
        appearance.SetVariation("normal");

        if (characterImage != null && sprite != null)
        {
            characterImage.sprite = sprite;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // エディタで外見が変更された時に即座に反映
        if (characterImage == null)
        {
            characterImage = GetComponent<Image>();
        }

        if (characterImage != null)
        {
            Sprite sprite = appearance.GetCurrentSprite();
            if (sprite != null)
            {
                characterImage.sprite = sprite;
            }
        }
    }
#endif
}

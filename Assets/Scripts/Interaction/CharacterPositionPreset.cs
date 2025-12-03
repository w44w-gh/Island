using UnityEngine;

/// <summary>
/// キャラクターの配置プリセット
/// 左手前、中央奥などの位置をプリセットとして定義
/// </summary>
public class CharacterPositionPreset : MonoBehaviour
{
    public enum PositionPreset
    {
        // 手前（Near）
        LeftNear,       // 左手前
        CenterNear,     // 中央手前
        RightNear,      // 右手前

        // 中間（Middle）
        LeftMiddle,     // 左中間
        CenterMiddle,   // 中央中間
        RightMiddle,    // 右中間

        // 奥（Far）
        LeftFar,        // 左奥
        CenterFar,      // 中央奥
        RightFar,       // 右奥

        // カスタム
        Custom          // カスタム位置
    }

    [System.Serializable]
    public class PresetData
    {
        public PositionPreset preset;
        public Vector2 anchoredPosition;
        public CharacterDepth.DepthLevel depth;
    }

    [Header("Canvas Settings")]
    [SerializeField] private RectTransform canvasRect;  // Canvasの RectTransform

    [Header("Preset Definitions")]
    [SerializeField] private PresetData[] presets = new PresetData[]
    {
        // 手前（Near）- 下部に配置、大きい
        new PresetData { preset = PositionPreset.LeftNear, anchoredPosition = new Vector2(-400, -200), depth = CharacterDepth.DepthLevel.Near },
        new PresetData { preset = PositionPreset.CenterNear, anchoredPosition = new Vector2(0, -200), depth = CharacterDepth.DepthLevel.Near },
        new PresetData { preset = PositionPreset.RightNear, anchoredPosition = new Vector2(400, -200), depth = CharacterDepth.DepthLevel.Near },

        // 中間（Middle）- 中央に配置、中サイズ
        new PresetData { preset = PositionPreset.LeftMiddle, anchoredPosition = new Vector2(-300, -50), depth = CharacterDepth.DepthLevel.Middle },
        new PresetData { preset = PositionPreset.CenterMiddle, anchoredPosition = new Vector2(0, -50), depth = CharacterDepth.DepthLevel.Middle },
        new PresetData { preset = PositionPreset.RightMiddle, anchoredPosition = new Vector2(300, -50), depth = CharacterDepth.DepthLevel.Middle },

        // 奥（Far）- 上部に配置、小さい
        new PresetData { preset = PositionPreset.LeftFar, anchoredPosition = new Vector2(-200, 100), depth = CharacterDepth.DepthLevel.Far },
        new PresetData { preset = PositionPreset.CenterFar, anchoredPosition = new Vector2(0, 100), depth = CharacterDepth.DepthLevel.Far },
        new PresetData { preset = PositionPreset.RightFar, anchoredPosition = new Vector2(200, 100), depth = CharacterDepth.DepthLevel.Far },
    };

    private void Awake()
    {
        // CanvasRectが未設定の場合は親から取得
        if (canvasRect == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvasRect = canvas.GetComponent<RectTransform>();
            }
        }
    }

    /// <summary>
    /// プリセット位置を取得
    /// </summary>
    public Vector2 GetPresetPosition(PositionPreset preset)
    {
        foreach (var data in presets)
        {
            if (data.preset == preset)
            {
                return data.anchoredPosition;
            }
        }

        Debug.LogWarning($"Preset {preset} not found. Returning zero.");
        return Vector2.zero;
    }

    /// <summary>
    /// プリセット深度を取得
    /// </summary>
    public CharacterDepth.DepthLevel GetPresetDepth(PositionPreset preset)
    {
        foreach (var data in presets)
        {
            if (data.preset == preset)
            {
                return data.depth;
            }
        }

        Debug.LogWarning($"Preset {preset} not found. Returning Near.");
        return CharacterDepth.DepthLevel.Near;
    }

    /// <summary>
    /// キャラクターをプリセット位置に配置
    /// </summary>
    public void ApplyPreset(GameObject character, PositionPreset preset)
    {
        RectTransform rectTransform = character.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("Character does not have RectTransform!");
            return;
        }

        // 位置を設定
        Vector2 position = GetPresetPosition(preset);
        rectTransform.anchoredPosition = position;

        // 深度を設定
        CharacterDepth depthComponent = character.GetComponent<CharacterDepth>();
        if (depthComponent != null)
        {
            CharacterDepth.DepthLevel depth = GetPresetDepth(preset);
            depthComponent.SetDepth(depth);
        }

        Debug.Log($"Applied preset {preset} to {character.name}: Position={position}, Depth={GetPresetDepth(preset)}");
    }

    /// <summary>
    /// 複数のキャラクターを一括配置（シーン構成用）
    /// </summary>
    public void ApplySceneLayout(GameObject[] characters, PositionPreset[] presets)
    {
        if (characters.Length != presets.Length)
        {
            Debug.LogError("Characters and presets arrays must have the same length!");
            return;
        }

        for (int i = 0; i < characters.Length; i++)
        {
            ApplyPreset(characters[i], presets[i]);
        }
    }

    /// <summary>
    /// プリセットデータを取得
    /// </summary>
    public PresetData GetPresetData(PositionPreset preset)
    {
        foreach (var data in presets)
        {
            if (data.preset == preset)
            {
                return data;
            }
        }

        return null;
    }

#if UNITY_EDITOR
    /// <summary>
    /// エディタ用: 全てのプリセット位置を可視化
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (canvasRect == null) return;

        foreach (var data in presets)
        {
            // プリセット位置をGizmosで表示
            Vector3 worldPos = canvasRect.TransformPoint(data.anchoredPosition);

            // 深度に応じて色を変える
            switch (data.depth)
            {
                case CharacterDepth.DepthLevel.VeryFar:
                case CharacterDepth.DepthLevel.Far:
                    Gizmos.color = Color.blue;
                    break;
                case CharacterDepth.DepthLevel.Middle:
                    Gizmos.color = Color.yellow;
                    break;
                case CharacterDepth.DepthLevel.Near:
                case CharacterDepth.DepthLevel.VeryNear:
                    Gizmos.color = Color.red;
                    break;
            }

            Gizmos.DrawSphere(worldPos, 20f);
        }
    }
#endif
}

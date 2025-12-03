using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キャラクターの奥行き（深度）を管理するクラス
/// 奥にいるキャラは小さく、手前にいるキャラは大きく表示
/// 影も動的に生成して立体感を出す
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class CharacterDepth : MonoBehaviour
{
    public enum DepthLevel
    {
        VeryFar = 0,    // 最奥（50%サイズ）
        Far = 1,        // 奥（70%サイズ）
        Middle = 2,     // 中央（85%サイズ）
        Near = 3,       // 手前（100%サイズ）
        VeryNear = 4    // 最前（110%サイズ）
    }

    [Header("Depth Settings")]
    [SerializeField] private DepthLevel currentDepth = DepthLevel.Near;
    [SerializeField] private Vector3 baseScale = Vector3.one;  // 基準スケール

    [Header("Shadow Settings")]
    [SerializeField] private bool enableShadow = true;
    [SerializeField] private GameObject shadowObject;  // 影オブジェクト（自動生成可能）
    [SerializeField] private Color shadowColor = new Color(0, 0, 0, 0.3f);
    [SerializeField] private Vector2 shadowOffset = new Vector2(10, -10);  // 影のオフセット

    private RectTransform rectTransform;
    private Image shadowImage;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // 影を自動生成
        if (enableShadow && shadowObject == null)
        {
            CreateShadow();
        }

        // 初期深度を適用
        ApplyDepth();
    }

    /// <summary>
    /// 影を自動生成
    /// </summary>
    private void CreateShadow()
    {
        // 影用のGameObjectを作成
        shadowObject = new GameObject("Shadow");
        shadowObject.transform.SetParent(transform);

        // RectTransformを追加
        RectTransform shadowRect = shadowObject.AddComponent<RectTransform>();
        shadowRect.anchorMin = new Vector2(0.5f, 0.5f);
        shadowRect.anchorMax = new Vector2(0.5f, 0.5f);
        shadowRect.pivot = new Vector2(0.5f, 0.5f);
        shadowRect.sizeDelta = rectTransform.sizeDelta;
        shadowRect.anchoredPosition = shadowOffset;

        // Imageコンポーネントを追加
        shadowImage = shadowObject.AddComponent<Image>();

        // キャラクターのSpriteをコピー（親のImageから）
        Image parentImage = GetComponent<Image>();
        if (parentImage != null && parentImage.sprite != null)
        {
            shadowImage.sprite = parentImage.sprite;
        }

        shadowImage.color = shadowColor;

        // 影を最背面に
        shadowObject.transform.SetAsFirstSibling();

        Debug.Log($"Shadow created for {gameObject.name}");
    }

    /// <summary>
    /// 深度を設定
    /// </summary>
    public void SetDepth(DepthLevel depth)
    {
        currentDepth = depth;
        ApplyDepth();
    }

    /// <summary>
    /// 深度を適用
    /// </summary>
    private void ApplyDepth()
    {
        // スケールを設定
        float scaleFactor = GetScaleForDepth(currentDepth);
        transform.localScale = baseScale * scaleFactor;

        // 影の透明度を調整（手前ほど濃く）
        if (shadowImage != null)
        {
            Color newColor = shadowColor;
            newColor.a = shadowColor.a * scaleFactor;  // 奥ほど薄く
            shadowImage.color = newColor;

            // 影のオフセットも調整（奥ほど小さく）
            if (shadowObject != null)
            {
                RectTransform shadowRect = shadowObject.GetComponent<RectTransform>();
                shadowRect.anchoredPosition = shadowOffset * scaleFactor;
            }
        }

        // Canvasの描画順を調整（手前のキャラが前に）
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = (int)currentDepth;
        }

        Debug.Log($"{gameObject.name} depth set to {currentDepth} (scale: {scaleFactor})");
    }

    /// <summary>
    /// 深度に応じたスケール係数を取得
    /// </summary>
    private float GetScaleForDepth(DepthLevel depth)
    {
        switch (depth)
        {
            case DepthLevel.VeryFar:
                return 0.5f;  // 50%
            case DepthLevel.Far:
                return 0.7f;  // 70%
            case DepthLevel.Middle:
                return 0.85f; // 85%
            case DepthLevel.Near:
                return 1.0f;  // 100%
            case DepthLevel.VeryNear:
                return 1.1f;  // 110%
            default:
                return 1.0f;
        }
    }

    /// <summary>
    /// 現在の深度を取得
    /// </summary>
    public DepthLevel GetDepth()
    {
        return currentDepth;
    }

    /// <summary>
    /// 影の有効/無効を切り替え
    /// </summary>
    public void SetShadowEnabled(bool enabled)
    {
        enableShadow = enabled;

        if (shadowObject != null)
        {
            shadowObject.SetActive(enabled);
        }
        else if (enabled)
        {
            CreateShadow();
        }
    }

    /// <summary>
    /// 影の色を設定
    /// </summary>
    public void SetShadowColor(Color color)
    {
        shadowColor = color;

        if (shadowImage != null)
        {
            shadowImage.color = color;
        }
    }

    /// <summary>
    /// 影のオフセットを設定
    /// </summary>
    public void SetShadowOffset(Vector2 offset)
    {
        shadowOffset = offset;

        if (shadowObject != null)
        {
            RectTransform shadowRect = shadowObject.GetComponent<RectTransform>();
            shadowRect.anchoredPosition = offset;
        }
    }

    /// <summary>
    /// キャラクター画像が変更された時に影も更新
    /// </summary>
    public void UpdateShadowSprite(Sprite sprite)
    {
        if (shadowImage != null)
        {
            shadowImage.sprite = sprite;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タップ可能なアイテム
/// タップすると取得してインベントリに追加
/// </summary>
[RequireComponent(typeof(Image))]
public class InteractableItem : MonoBehaviour, IInteractable
{
    [Header("Item Settings")]
    [SerializeField] private string itemId;         // アイテムID（"wood", "stone"など）
    [SerializeField] private int quantity = 1;      // 取得個数
    [SerializeField] private Sprite itemSprite;     // アイテムの画像

    [Header("Interaction Settings")]
    [SerializeField] private bool isInteractable = true;  // インタラクション可能か

    [Header("Animation Settings")]
    [SerializeField] private float pickupAnimationDuration = 0.5f;  // 取得アニメーション時間
    [SerializeField] private AnimationCurve pickupCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Image itemImage;
    private Button button;
    private bool isPickedUp = false;

    private void Awake()
    {
        // Imageコンポーネントを取得
        itemImage = GetComponent<Image>();
        if (itemImage != null && itemSprite != null)
        {
            itemImage.sprite = itemSprite;
        }

        // Buttonコンポーネントを取得または追加
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }

        // ボタンクリック時にOnInteractを呼び出す
        button.onClick.AddListener(OnInteract);
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
        if (!CanInteract() || isPickedUp)
        {
            Debug.LogWarning($"Item {itemId} is not interactable or already picked up");
            return;
        }

        Debug.Log($"Picking up item: {itemId} x{quantity}");

        // タップSEを再生
        AudioManager.Instance.PlaySE("item_pickup");

        // フラグを立てる
        isPickedUp = true;

        // アイテムをインベントリに追加
        if (GameManager.Instance.State != null)
        {
            Item item = ItemDatabase.GetItem(itemId);
            if (item != null)
            {
                GameManager.Instance.State.Inventory.AddItem(item, quantity);
                Debug.Log($"Added {quantity}x {itemId} to inventory");
            }
            else
            {
                Debug.LogError($"Item '{itemId}' not found in ItemDatabase");
            }
        }

        // 取得アニメーション開始
        StartCoroutine(PickupAnimation());
    }

    /// <summary>
    /// 取得アニメーション
    /// </summary>
    private System.Collections.IEnumerator PickupAnimation()
    {
        Vector3 startPosition = transform.position;
        Vector3 startScale = transform.localScale;
        Color startColor = itemImage.color;

        float elapsed = 0f;

        while (elapsed < pickupAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pickupAnimationDuration;
            float curveValue = pickupCurve.Evaluate(t);

            // 上に移動
            transform.position = startPosition + Vector3.up * (curveValue * 50f);

            // スケールを小さく
            transform.localScale = startScale * (1f - curveValue);

            // フェードアウト
            Color newColor = startColor;
            newColor.a = 1f - curveValue;
            itemImage.color = newColor;

            yield return null;
        }

        // アニメーション終了後、オブジェクトを削除
        Destroy(gameObject);
    }

    /// <summary>
    /// インタラクション可能かどうか
    /// </summary>
    public bool CanInteract()
    {
        return isInteractable && !isPickedUp;
    }

    /// <summary>
    /// GameObjectを取得
    /// </summary>
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    /// <summary>
    /// アイテムを設定（動的生成用）
    /// </summary>
    public void Setup(string id, int qty, Sprite sprite)
    {
        itemId = id;
        quantity = qty;
        itemSprite = sprite;

        if (itemImage != null && sprite != null)
        {
            itemImage.sprite = sprite;
        }
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
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // エディタでSpriteが設定された時に即座に反映
        if (itemImage == null)
        {
            itemImage = GetComponent<Image>();
        }

        if (itemImage != null && itemSprite != null)
        {
            itemImage.sprite = itemSprite;
        }
    }
#endif
}

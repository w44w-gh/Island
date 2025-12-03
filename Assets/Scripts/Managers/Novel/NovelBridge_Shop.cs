using UnityEngine;

/// <summary>
/// NovelBridge - 店・購入・取引関連メソッド
/// </summary>
public partial class NovelBridge
{
    // ========== 店・購入・取引関連 ==========

    /// <summary>
    /// アイテムを食料で購入（宴のコマンドから呼び出し）
    /// currencyItemId: 支払う食料のアイテムID
    /// currencyAmount: 支払う食料の数量
    /// purchaseItemId: 購入するアイテムID
    /// purchaseAmount: 購入する数量
    /// </summary>
    public bool PurchaseItem(string currencyItemId, int currencyAmount, string purchaseItemId, int purchaseAmount)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return false;
        }

        // 支払う食料を持っているか確認
        if (!GameManager.Instance.State.Inventory.HasItem(currencyItemId, currencyAmount))
        {
            Debug.LogWarning($"[NovelBridge] 支払うアイテムが足りません: {currencyItemId} x{currencyAmount}");
            return false;
        }

        // 購入するアイテムを取得
        Item purchaseItem = ItemDatabase.GetItem(purchaseItemId);
        if (purchaseItem == null)
        {
            Debug.LogWarning($"[NovelBridge] 購入アイテムが見つかりません: {purchaseItemId}");
            return false;
        }

        // 支払いを実行
        bool removed = GameManager.Instance.State.Inventory.RemoveItem(currencyItemId, currencyAmount);
        if (!removed)
        {
            Debug.LogWarning($"[NovelBridge] 支払いに失敗: {currencyItemId} x{currencyAmount}");
            return false;
        }

        // アイテムを追加
        bool added = GameManager.Instance.State.Inventory.AddItem(purchaseItem, purchaseAmount);
        if (!added)
        {
            // 追加に失敗したら支払いを返却
            Item currencyItem = ItemDatabase.GetItem(currencyItemId);
            if (currencyItem != null)
            {
                GameManager.Instance.State.Inventory.AddItem(currencyItem, currencyAmount);
            }
            Debug.LogWarning($"[NovelBridge] アイテムの追加に失敗（インベントリが満杯）: {purchaseItemId}");
            return false;
        }

        Debug.Log($"[NovelBridge] 購入成功: {currencyItemId} x{currencyAmount} → {purchaseItemId} x{purchaseAmount}");
        return true;
    }

    /// <summary>
    /// 診療を受ける（HPとスタミナを回復、食料で支払い）
    /// </summary>
    public bool ReceiveTreatment(string currencyItemId, int currencyAmount, int hpRestore, int staminaRestore)
    {
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameStateが初期化されていません");
            return false;
        }

        // 支払う食料を持っているか確認
        if (!GameManager.Instance.State.Inventory.HasItem(currencyItemId, currencyAmount))
        {
            Debug.LogWarning($"[NovelBridge] 診療費が足りません: {currencyItemId} x{currencyAmount}");
            return false;
        }

        // 支払いを実行
        bool removed = GameManager.Instance.State.Inventory.RemoveItem(currencyItemId, currencyAmount);
        if (!removed)
        {
            Debug.LogWarning($"[NovelBridge] 支払いに失敗");
            return false;
        }

        // 回復処理
        if (hpRestore > 0)
        {
            GameManager.Instance.State.Player.Heal(hpRestore);
        }
        if (staminaRestore > 0)
        {
            GameManager.Instance.State.Player.RecoverStamina(staminaRestore);
        }

        Debug.Log($"[NovelBridge] 診療完了: HP+{hpRestore}, スタミナ+{staminaRestore}");
        return true;
    }
}

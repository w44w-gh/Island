using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 時間経過でアイテムをスポーンする
/// 時間帯や天候によってスポーンするアイテムが変わる
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableItem
    {
        public string itemId;           // アイテムID
        public Sprite itemSprite;       // アイテム画像
        public int minQuantity = 1;     // 最小個数
        public int maxQuantity = 3;     // 最大個数
        public float spawnChance = 0.5f; // スポーン確率（0.0～1.0）
    }

    [System.Serializable]
    public class SpawnCondition
    {
        public TimeOfDay timeOfDay = TimeOfDay.Morning;  // 時間帯
        public WeatherType weather = WeatherType.Sunny;   // 天候
        public List<SpawnableItem> items = new List<SpawnableItem>();  // スポーン可能アイテム
    }

    [Header("Spawn Settings")]
    [SerializeField] private GameObject itemPrefab;  // InteractableItemのPrefab
    [SerializeField] private Transform itemContainer; // アイテムの親オブジェクト
    [SerializeField] private List<SpawnCondition> spawnConditions = new List<SpawnCondition>();

    [Header("Spawn Positions")]
    [SerializeField] private List<RectTransform> spawnPoints = new List<RectTransform>();  // スポーン位置リスト

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 300f;  // スポーン間隔（秒）デフォルト5分
    [SerializeField] private int maxItemsOnField = 10;    // フィールド上の最大アイテム数

    private float nextSpawnTime = 0f;
    private List<GameObject> spawnedItems = new List<GameObject>();

    private void Start()
    {
        // 初回スポーン時刻を設定
        nextSpawnTime = Time.time + spawnInterval;

        Debug.Log($"ItemSpawner initialized. Next spawn in {spawnInterval} seconds");
    }

    private void Update()
    {
        // スポーン時刻チェック
        if (Time.time >= nextSpawnTime)
        {
            TrySpawnItems();
            nextSpawnTime = Time.time + spawnInterval;
        }

        // nullになったアイテム（取得済み）をリストから削除
        spawnedItems.RemoveAll(item => item == null);
    }

    /// <summary>
    /// アイテムのスポーンを試行
    /// </summary>
    private void TrySpawnItems()
    {
        // フィールド上のアイテム数が最大に達している場合はスキップ
        if (spawnedItems.Count >= maxItemsOnField)
        {
            Debug.Log("Max items on field reached. Skipping spawn.");
            return;
        }

        // 現在の時間帯と天候を取得
        if (GameManager.Instance.State == null)
        {
            Debug.LogWarning("GameState is null. Cannot spawn items.");
            return;
        }

        TimeOfDay currentTime = GameManager.Instance.GlobalGameTime.CurrentTimeOfDay;
        WeatherType currentWeather = GameManager.Instance.State.CurrentWeather;

        Debug.Log($"Trying to spawn items. Time: {currentTime.ToJapaneseString()}, Weather: {currentWeather.ToJapaneseString()}");

        // 条件に合致するスポーンリストを取得
        SpawnCondition condition = GetSpawnCondition(currentTime, currentWeather);

        if (condition == null || condition.items.Count == 0)
        {
            Debug.Log("No spawn condition found for current time and weather.");
            return;
        }

        // スポーン位置をランダムに選択
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points defined.");
            return;
        }

        // ランダムにアイテムをスポーン
        foreach (var spawnableItem in condition.items)
        {
            // スポーン確率チェック
            if (Random.value > spawnableItem.spawnChance)
            {
                continue;
            }

            // フィールド上のアイテム数チェック
            if (spawnedItems.Count >= maxItemsOnField)
            {
                break;
            }

            // スポーン位置をランダムに選択
            RectTransform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            // アイテムをスポーン
            SpawnItem(spawnableItem, spawnPoint);
        }
    }

    /// <summary>
    /// アイテムをスポーン
    /// </summary>
    private void SpawnItem(SpawnableItem spawnableItem, RectTransform spawnPoint)
    {
        if (itemPrefab == null)
        {
            Debug.LogError("Item prefab is not assigned!");
            return;
        }

        // Prefabからインスタンス化
        GameObject itemObj = Instantiate(itemPrefab, itemContainer != null ? itemContainer : transform);

        // RectTransformを取得
        RectTransform rectTransform = itemObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = spawnPoint.anchoredPosition;
            rectTransform.sizeDelta = new Vector2(100, 100); // アイテムサイズ
        }

        // InteractableItemコンポーネントを取得して設定
        InteractableItem interactableItem = itemObj.GetComponent<InteractableItem>();
        if (interactableItem != null)
        {
            int quantity = Random.Range(spawnableItem.minQuantity, spawnableItem.maxQuantity + 1);
            interactableItem.Setup(spawnableItem.itemId, quantity, spawnableItem.itemSprite);
        }

        // スポーン済みリストに追加
        spawnedItems.Add(itemObj);

        Debug.Log($"Spawned item: {spawnableItem.itemId} at position {spawnPoint.anchoredPosition}");

        // スポーンSEを再生
        AudioManager.Instance.PlaySE("item_spawn");
    }

    /// <summary>
    /// 現在の時間帯と天候に合致するスポーン条件を取得
    /// </summary>
    private SpawnCondition GetSpawnCondition(TimeOfDay time, WeatherType weather)
    {
        // 完全一致を探す
        foreach (var condition in spawnConditions)
        {
            if (condition.timeOfDay == time && condition.weather == weather)
            {
                return condition;
            }
        }

        // 時間帯だけ一致するものを探す
        foreach (var condition in spawnConditions)
        {
            if (condition.timeOfDay == time)
            {
                return condition;
            }
        }

        // 天候だけ一致するものを探す
        foreach (var condition in spawnConditions)
        {
            if (condition.weather == weather)
            {
                return condition;
            }
        }

        return null;
    }

    /// <summary>
    /// 全てのアイテムをクリア
    /// </summary>
    public void ClearAllItems()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }

        spawnedItems.Clear();
        Debug.Log("All items cleared");
    }

    /// <summary>
    /// 手動でスポーンを実行（デバッグ用）
    /// </summary>
    public void ForceSpawn()
    {
        TrySpawnItems();
    }
}

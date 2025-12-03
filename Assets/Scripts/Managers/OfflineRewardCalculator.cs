using System;
using UnityEngine;

/// <summary>
/// オフライン報酬計算クラス
/// プレイヤーがゲームをプレイしていない間の報酬を計算
/// </summary>
public class OfflineRewardCalculator
{
    private ItemSpawnManager itemSpawnManager;
    private MapState mapState;

    // 設定値
    private const int MIN_MINUTES_FOR_REWARD = 1;           // 報酬を得るための最小オフライン時間（分）
    private const int MAX_OFFLINE_HOURS = 24;               // オフライン報酬の上限時間（時間）
    private const int SPAWN_INTERVAL_MINUTES = 10;          // アイテムスポーンの間隔（分）

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OfflineRewardCalculator(ItemSpawnManager itemSpawnManager, MapState mapState)
    {
        this.itemSpawnManager = itemSpawnManager;
        this.mapState = mapState;
    }

    /// <summary>
    /// オフライン中の報酬を計算してマップに配置
    /// </summary>
    /// <param name="elapsed">オフライン経過時間</param>
    /// <returns>スポーン処理が実行された回数</returns>
    public int CalculateAndGrantRewards(TimeSpan elapsed)
    {
        // 経過時間が最小時間未満なら何もしない
        if (elapsed.TotalMinutes < MIN_MINUTES_FOR_REWARD)
        {
            Debug.Log("オフライン時間が短いため、報酬なし");
            return 0;
        }

        // 経過時間を分単位で計算
        int elapsedMinutes = (int)elapsed.TotalMinutes;

        // 上限を設定（最大24時間分）
        int cappedMinutes = Mathf.Min(elapsedMinutes, MAX_OFFLINE_HOURS * 60);

        Debug.Log($"オフライン報酬計算: {cappedMinutes}分（元: {elapsedMinutes}分）");

        // スポーン間隔ごとに1回のスポーン処理
        int spawnCount = cappedMinutes / SPAWN_INTERVAL_MINUTES;

        // アイテムをスポーン
        for (int i = 0; i < spawnCount; i++)
        {
            // 海岸にアイテムをランダム配置
            itemSpawnManager.SpawnItemsAtBeach();

            // 森にアイテムをランダム配置
            itemSpawnManager.SpawnItemsAtForest();

            // 将来的に追加: 山や川へのスポーン
            // itemSpawnManager.SpawnItemsAtMountain();
            // itemSpawnManager.SpawnItemsAtRiver();
        }

        Debug.Log($"オフライン報酬: {spawnCount}回のアイテム配置完了");
        Debug.Log(mapState.GetSummary());

        return spawnCount;
    }

    /// <summary>
    /// オフライン報酬の情報を取得（実際に付与せずに計算のみ）
    /// </summary>
    /// <param name="elapsed">オフライン経過時間</param>
    /// <returns>スポーン処理が実行される予定の回数</returns>
    public int PreviewRewardCount(TimeSpan elapsed)
    {
        if (elapsed.TotalMinutes < MIN_MINUTES_FOR_REWARD)
        {
            return 0;
        }

        int elapsedMinutes = (int)elapsed.TotalMinutes;
        int cappedMinutes = Mathf.Min(elapsedMinutes, MAX_OFFLINE_HOURS * 60);
        return cappedMinutes / SPAWN_INTERVAL_MINUTES;
    }

    /// <summary>
    /// オフライン報酬のサマリー文字列を取得
    /// </summary>
    /// <param name="elapsed">オフライン経過時間</param>
    /// <returns>サマリー文字列</returns>
    public string GetRewardSummary(TimeSpan elapsed)
    {
        int spawnCount = PreviewRewardCount(elapsed);

        if (spawnCount == 0)
        {
            return "オフライン時間が短いため、報酬はありません";
        }

        int cappedMinutes = Mathf.Min((int)elapsed.TotalMinutes, MAX_OFFLINE_HOURS * 60);
        int hours = cappedMinutes / 60;
        int minutes = cappedMinutes % 60;

        return $"オフライン期間: {hours}時間{minutes}分\nアイテム配置回数: {spawnCount}回";
    }
}

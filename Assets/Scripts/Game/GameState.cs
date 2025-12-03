using System;
using UnityEngine;

/// <summary>
/// ゲーム全体の状態を管理するクラス
/// 無人島サバイバルゲームの進行状態を保持
/// </summary>
public class GameState
{
    private int currentDay;
    private Weather weather;
    private GameTime gameTime;
    private PlayerStatus playerStatus;
    private MapState mapState;
    private CharacterManager characterManager;
    private Inventory inventory;
    private ConstructionState constructionState;
    private EventManager eventManager;
    private RivalEventManager rivalEventManager;
    private ItemSpawnManager itemSpawnManager;
    private OfflineRewardCalculator offlineRewardCalculator;
    private string marriedCharacterId; // 結婚相手のキャラクターID（null = 未婚）
    private FlagsSystem flagsSystem; // フラグ管理システム
    private string equippedAccessory1; // 装備中のアクセサリー1
    private string equippedAccessory2; // 装備中のアクセサリー2

    /// <summary>
    /// 経過日数（1日目から開始）
    /// </summary>
    public int CurrentDay => currentDay;

    /// <summary>
    /// 現在の天候
    /// </summary>
    public WeatherType CurrentWeather => weather.CurrentWeather;

    /// <summary>
    /// ゲーム時間
    /// </summary>
    public GameTime Time => gameTime;

    /// <summary>
    /// 主人公のステータス
    /// </summary>
    public PlayerStatus Player => playerStatus;

    /// <summary>
    /// マップの状態
    /// </summary>
    public MapState Map => mapState;

    /// <summary>
    /// キャラクター管理
    /// </summary>
    public CharacterManager Characters => characterManager;

    /// <summary>
    /// インベントリ
    /// </summary>
    public Inventory Inventory => inventory;

    /// <summary>
    /// 建築状態
    /// </summary>
    public ConstructionState Construction => constructionState;

    /// <summary>
    /// イベント管理
    /// </summary>
    public EventManager Events => eventManager;

    /// <summary>
    /// ライバルイベント管理
    /// </summary>
    public RivalEventManager RivalEvents => rivalEventManager;

    /// <summary>
    /// フラグ管理システム
    /// </summary>
    public FlagsSystem Flags => flagsSystem;

    /// <summary>
    /// 結婚相手のキャラクターID（null = 未婚）
    /// </summary>
    public string MarriedCharacterId => marriedCharacterId;

    /// <summary>
    /// 結婚しているか
    /// </summary>
    public bool IsMarried => !string.IsNullOrEmpty(marriedCharacterId);

    /// <summary>
    /// 結婚相手のキャラクターを取得
    /// </summary>
    public Character MarriedCharacter => IsMarried ? characterManager.GetCharacter(marriedCharacterId) : null;

    /// <summary>
    /// 日付が変わった時のイベント
    /// </summary>
    public event Action<int, WeatherType> OnDayChanged;

    /// <summary>
    /// 建築が完了した時のイベント
    /// </summary>
    public event Action<string> OnConstructionCompleted;

    /// <summary>
    /// スケジュール条件が変更された時のイベント
    /// （装備、好感度、フラグなどの変更時に発火）
    /// </summary>
    public event Action OnScheduleConditionChanged;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public GameState(GameTime gameTime, WeatherType initialWeather = WeatherType.Sunny)
    {
        this.gameTime = gameTime;
        this.currentDay = 1;
        this.weather = new Weather(initialWeather);
        this.playerStatus = new PlayerStatus();
        this.mapState = new MapState();
        this.itemSpawnManager = new ItemSpawnManager(mapState);
        this.offlineRewardCalculator = new OfflineRewardCalculator(itemSpawnManager, mapState);
        this.characterManager = new CharacterManager();
        this.inventory = new Inventory(20); // 初期容量20
        this.constructionState = new ConstructionState();
        this.eventManager = new EventManager();
        this.rivalEventManager = new RivalEventManager();
        this.flagsSystem = new FlagsSystem();

        // FlagsSystemのイベントを購読
        this.flagsSystem.OnFlagChanged += (flagId, value) => OnScheduleConditionChanged?.Invoke();

        // CharacterManagerのイベントを購読
        this.characterManager.OnAnyCharacterRomanceChanged += () => OnScheduleConditionChanged?.Invoke();

        Debug.Log($"GameState初期化: {currentDay}日目, 天候: {CurrentWeather.ToJapaneseString()}, {playerStatus.GetSummary()}");
    }

    /// <summary>
    /// 日付を進める（次の日へ）
    /// </summary>
    public void AdvanceDay()
    {
        currentDay++;
        weather.AdvanceToNextDay();

        Debug.Log($"日付更新: {currentDay}日目, 天候: {CurrentWeather.ToJapaneseString()}");
        OnDayChanged?.Invoke(currentDay, CurrentWeather);

        // ライバルイベントをチェック
        CheckRivalEvents();
    }

    /// <summary>
    /// ライバルイベントのチェック（日付変更時に自動実行）
    /// </summary>
    private void CheckRivalEvents()
    {
        System.Collections.Generic.List<RivalEvent> triggeredEvents = rivalEventManager.CheckForRivalEvents(this);

        foreach (RivalEvent rivalEvent in triggeredEvents)
        {
            Debug.Log($"[GameState] ライバルイベント発生: {rivalEvent.eventName} ({rivalEvent.eventId})");

            // イベント段階を進行
            rivalEventManager.AdvanceRivalEvent(rivalEvent.mainCharacterId, rivalEvent.eventStage);

            // TODO: Utageでシナリオを再生する処理を追加
            // NovelBridgeを使ってシナリオを再生する必要がある
        }
    }

    /// <summary>
    /// 天候を強制的に設定（デバッグ用）
    /// </summary>
    public void SetWeather(WeatherType weatherType)
    {
        weather.SetWeather(weatherType);
    }

    /// <summary>
    /// 経過日数のテキスト表示
    /// </summary>
    public string GetDayText()
    {
        return $"{currentDay}日目";
    }

    /// <summary>
    /// 天候のテキスト表示
    /// </summary>
    public string GetWeatherText()
    {
        return CurrentWeather.ToJapaneseString();
    }

    /// <summary>
    /// ゲーム状態のサマリー
    /// </summary>
    public string GetSummary()
    {
        return $"漂流{currentDay}日目 - {CurrentWeather.ToJapaneseString()} - {gameTime.CurrentTimeOfDayText}";
    }

    /// <summary>
    /// 特定の日数を経過したかチェック
    /// </summary>
    public bool HasPassedDays(int days)
    {
        return currentDay >= days;
    }

    /// <summary>
    /// 天候が指定したタイプかチェック
    /// </summary>
    public bool IsWeather(WeatherType weatherType)
    {
        return CurrentWeather == weatherType;
    }

    /// <summary>
    /// 晴れかどうか
    /// </summary>
    public bool IsSunny => CurrentWeather == WeatherType.Sunny;

    /// <summary>
    /// 曇りかどうか
    /// </summary>
    public bool IsCloudy => CurrentWeather == WeatherType.Cloudy;

    /// <summary>
    /// 雨かどうか
    /// </summary>
    public bool IsRainy => CurrentWeather == WeatherType.Rainy;

    /// <summary>
    /// 嵐かどうか
    /// </summary>
    public bool IsStormy => CurrentWeather == WeatherType.Stormy;

    /// <summary>
    /// 屋外活動が可能な天候か（晴れまたは曇り）
    /// </summary>
    public bool CanWorkOutdoors => IsSunny || IsCloudy;

    /// <summary>
    /// 危険な天候か（嵐）
    /// </summary>
    public bool IsDangerousWeather => IsStormy;

    /// <summary>
    /// セーブデータに変換
    /// </summary>
    public SaveData ToSaveData()
    {
        SaveData data = new SaveData
        {
            currentDay = this.currentDay,
            currentWeather = this.CurrentWeather.ToString(),
            savedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            playerHP = this.playerStatus.CurrentHP,
            playerStamina = this.playerStatus.CurrentStamina,
            saveVersion = 1
        };

        // マップ上のアイテムをセーブデータに追加
        foreach (MapLocation location in Enum.GetValues(typeof(MapLocation)))
        {
            var items = mapState.GetItemsAt(location);
            foreach (var item in items)
            {
                data.mapItems.Add(new SavedMapItem(location.ToString(), item.itemId, item.quantity));
            }
        }

        // キャラクターデータをセーブデータに追加
        foreach (var character in characterManager.AllCharacters)
        {
            data.characters.Add(new SavedCharacterData(character.id, character.Friendship, character.Romance, character.IsMarried));
        }

        // 結婚相手IDをセーブデータに追加
        data.marriedCharacterId = marriedCharacterId;

        // インベントリデータをセーブデータに追加
        foreach (var slot in inventory.Slots)
        {
            if (!slot.IsEmpty)
            {
                data.inventory.Add(new SavedInventorySlot(slot.item.id, slot.quantity));
            }
        }

        // 建築状態をセーブデータに追加
        data.constructionState = constructionState.ToSaveData();

        // マップ上の建築物をセーブデータに追加
        foreach (var construction in mapState.GetAllConstructions())
        {
            data.mapConstructions.Add(new SavedMapConstruction(construction.constructionId, construction.location.ToString()));
        }

        // イベント管理をセーブデータに追加
        data.eventManager = eventManager.ToSaveData();

        // ライバルイベント管理をセーブデータに追加
        data.rivalPairs = rivalEventManager.ToSaveData();

        // ライバルキャラクターデータをセーブデータに追加
        foreach (var rivalCharacter in characterManager.AllRivalCharacters)
        {
            data.rivalCharacters.Add(new SaveData.SavedRivalCharacterData(rivalCharacter.id, rivalCharacter.Friendship));
        }

        // イベント報酬の取得済みIDをセーブデータに追加
        if (EventRewardManager.Instance != null)
        {
            data.claimedEventIds = EventRewardManager.Instance.GetClaimedEventIds();
        }

        // フラグシステムをセーブデータに追加
        data.flags = flagsSystem.ToSaveData();

        // 装備をセーブデータに追加
        data.equippedAccessory1 = this.equippedAccessory1;
        data.equippedAccessory2 = this.equippedAccessory2;

        Debug.Log($"セーブデータ作成: {currentDay}日目, 天候: {data.currentWeather}, HP: {data.playerHP}, スタミナ: {data.playerStamina}, マップアイテム: {data.mapItems.Count}件, キャラクター: {data.characters.Count}人, インベントリ: {data.inventory.Count}件, 建築物: {data.mapConstructions.Count}件, イベント: {eventManager.GetTriggeredEventCount()}件, ライバルペア: {data.rivalPairs.Count}件, イベント報酬: {data.claimedEventIds.Count}件, フラグ: {flagsSystem.GetFlagCount()}件");
        return data;
    }

    /// <summary>
    /// セーブデータから復元
    /// </summary>
    public void LoadFromSaveData(SaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("セーブデータがnullです");
            return;
        }

        this.currentDay = data.currentDay;

        // 天候を復元
        if (Enum.TryParse(data.currentWeather, out WeatherType weatherType))
        {
            weather.SetWeather(weatherType);
        }
        else
        {
            Debug.LogWarning($"不明な天候: {data.currentWeather}、晴れに設定します");
            weather.SetWeather(WeatherType.Sunny);
        }

        // プレイヤーステータスを復元（直接フィールドに代入）
        playerStatus.SetHP(data.playerHP);
        playerStatus.SetStamina(data.playerStamina);

        // マップ上のアイテムを復元
        if (data.mapItems != null)
        {
            foreach (var mapItem in data.mapItems)
            {
                if (Enum.TryParse(mapItem.location, out MapLocation location))
                {
                    mapState.AddItemAt(location, mapItem.itemId, mapItem.quantity);
                }
            }
            Debug.Log($"マップアイテムを復元: {data.mapItems.Count}件");
        }

        // キャラクターデータを復元
        if (data.characters != null)
        {
            foreach (var charData in data.characters)
            {
                var character = characterManager.GetCharacter(charData.characterId);
                if (character != null)
                {
                    character.SetFriendship(charData.friendship);
                    character.SetRomance(charData.romance);
                    character.SetMarried(charData.isMarried);
                }
            }
            Debug.Log($"キャラクターデータを復元: {data.characters.Count}人");
        }

        // 結婚相手IDを復元
        if (!string.IsNullOrEmpty(data.marriedCharacterId))
        {
            SetMarriedCharacter(data.marriedCharacterId);
            Debug.Log($"結婚相手を復元: {data.marriedCharacterId}");
        }

        // インベントリデータを復元
        if (data.inventory != null)
        {
            foreach (var invSlot in data.inventory)
            {
                Item item = ItemDatabase.GetItem(invSlot.itemId);
                if (item != null)
                {
                    inventory.AddItem(item, invSlot.quantity);
                }
            }
            Debug.Log($"インベントリデータを復元: {data.inventory.Count}件");
        }

        // 建築状態を復元
        if (data.constructionState != null)
        {
            constructionState = ConstructionState.FromSaveData(data.constructionState);
            Debug.Log($"建築状態を復元: {(constructionState.IsConstructing ? "建築中" : "なし")}");
        }

        // マップ上の建築物を復元
        if (data.mapConstructions != null)
        {
            foreach (var construction in data.mapConstructions)
            {
                if (Enum.TryParse(construction.location, out MapLocation location))
                {
                    mapState.AddConstruction(construction.constructionId, location);
                }
            }
            Debug.Log($"マップ建築物を復元: {data.mapConstructions.Count}件");
        }

        // イベント管理を復元
        if (data.eventManager != null)
        {
            eventManager = EventManager.FromSaveData(data.eventManager);
            Debug.Log($"イベント管理を復元: {eventManager.GetTriggeredEventCount()}件の発生済みイベント");
        }

        // ライバルイベント管理を復元
        if (data.rivalPairs != null)
        {
            rivalEventManager.LoadFromSaveData(data.rivalPairs);
            Debug.Log($"ライバルイベント管理を復元: {data.rivalPairs.Count}件のライバルペア");
        }

        // ライバルキャラクターデータを復元
        if (data.rivalCharacters != null)
        {
            foreach (var rivalData in data.rivalCharacters)
            {
                var rivalCharacter = characterManager.GetRivalCharacter(rivalData.rivalCharacterId);
                if (rivalCharacter != null)
                {
                    rivalCharacter.IncreaseFriendship(rivalData.friendship);
                }
            }
            Debug.Log($"ライバルキャラクターデータを復元: {data.rivalCharacters.Count}人");
        }

        // イベント報酬の取得済みIDを復元
        if (data.claimedEventIds != null && EventRewardManager.Instance != null)
        {
            EventRewardManager.Instance.LoadClaimedEvents(data.claimedEventIds);
            Debug.Log($"イベント報酬データを復元: {data.claimedEventIds.Count}件");
        }

        // フラグシステムを復元
        if (data.flags != null)
        {
            flagsSystem.LoadFromSaveData(data.flags);
        }

        // 装備を復元
        this.equippedAccessory1 = data.equippedAccessory1;
        this.equippedAccessory2 = data.equippedAccessory2;

        Debug.Log($"セーブデータ読み込み: {currentDay}日目, 天候: {CurrentWeather.ToJapaneseString()}, HP: {data.playerHP}, スタミナ: {data.playerStamina}");
    }

    /// <summary>
    /// 最後にプレイしてからの経過時間を計算してオフライン処理を実行
    /// </summary>
    public void ProcessOfflineTime(long savedTimestamp)
    {
        // 現在のサーバー時刻を取得
        DateTime currentTime = NTPTimeManager.Instance.ServerTime;
        DateTime lastPlayTime = DateTimeOffset.FromUnixTimeSeconds(savedTimestamp).DateTime;

        // 経過時間を計算
        TimeSpan elapsed = currentTime - lastPlayTime;

        if (elapsed.TotalSeconds < 0)
        {
            Debug.LogWarning("セーブ時刻が未来の時刻です。端末の時刻が変更された可能性があります。");
            return;
        }

        Debug.Log($"オフライン経過時間: {elapsed.Days}日 {elapsed.Hours}時間 {elapsed.Minutes}分 {elapsed.Seconds}秒");

        // 建築完了チェック
        CheckConstructionCompletion();

        // 経過時間に応じたオフライン報酬を付与
        offlineRewardCalculator.CalculateAndGrantRewards(elapsed);
    }


    /// <summary>
    /// 建築完了をチェック（現在時刻で判定）
    /// </summary>
    public void CheckConstructionCompletion()
    {
        if (!constructionState.IsConstructing)
        {
            return;
        }

        DateTime currentTime = gameTime.CurrentTime;
        string completedId = constructionState.CheckAndCompleteConstruction(currentTime);

        if (completedId != null)
        {
            // 建築物をマップに配置
            ConstructionData data = ConstructionDatabase.GetConstruction(completedId);
            if (data != null)
            {
                mapState.AddConstruction(completedId, data.location);
                OnConstructionCompleted?.Invoke(completedId);
                Debug.Log($"建築完了: {data.name} が {data.location.ToJapaneseString()} に配置されました");
            }
        }
    }

    /// <summary>
    /// キャラクターと結婚する
    /// </summary>
    public bool MarryCharacter(string characterId)
    {
        // 既に結婚している場合は不可
        if (IsMarried)
        {
            Debug.LogWarning("既に結婚しています");
            return false;
        }

        // キャラクターを取得
        Character character = characterManager.GetCharacter(characterId);
        if (character == null)
        {
            Debug.LogWarning($"キャラクターが見つかりません: {characterId}");
            return false;
        }

        // 結婚可能かチェック
        if (!character.CanMarry())
        {
            Debug.LogWarning($"{character.name}とは結婚できません（恋愛度: {character.Romance}）");
            return false;
        }

        // 結婚処理
        marriedCharacterId = characterId;
        character.Marry();
        Debug.Log($"{character.name}と結婚しました！");

        // ライバルイベントを停止
        rivalEventManager.OnPlayerMarried(characterId);

        return true;
    }

    /// <summary>
    /// 結婚相手をセット（ロード時のみ使用）
    /// </summary>
    public void SetMarriedCharacter(string characterId)
    {
        marriedCharacterId = characterId;
    }

    /// <summary>
    /// アクセサリーを装備
    /// </summary>
    public void EquipAccessory(EquipmentSlot slot, string itemId)
    {
        if (slot == EquipmentSlot.Accessory1)
        {
            equippedAccessory1 = itemId;
            Debug.Log($"Accessory1 equipped: {itemId}");
        }
        else if (slot == EquipmentSlot.Accessory2)
        {
            equippedAccessory2 = itemId;
            Debug.Log($"Accessory2 equipped: {itemId}");
        }

        // スケジュール条件変更イベントを発火
        OnScheduleConditionChanged?.Invoke();
    }

    /// <summary>
    /// アクセサリーを外す
    /// </summary>
    public void UnequipAccessory(EquipmentSlot slot)
    {
        if (slot == EquipmentSlot.Accessory1)
        {
            equippedAccessory1 = null;
            Debug.Log("Accessory1 unequipped");
        }
        else if (slot == EquipmentSlot.Accessory2)
        {
            equippedAccessory2 = null;
            Debug.Log("Accessory2 unequipped");
        }

        // スケジュール条件変更イベントを発火
        OnScheduleConditionChanged?.Invoke();
    }

    /// <summary>
    /// 特定のアイテムを装備しているか
    /// </summary>
    public bool IsEquipped(string itemId)
    {
        return equippedAccessory1 == itemId || equippedAccessory2 == itemId;
    }

    /// <summary>
    /// 装備中のアクセサリー1を取得
    /// </summary>
    public string EquippedAccessory1 => equippedAccessory1;

    /// <summary>
    /// 装備中のアクセサリー2を取得
    /// </summary>
    public string EquippedAccessory2 => equippedAccessory2;
}

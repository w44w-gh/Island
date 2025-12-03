using System;
using System.Collections.Generic;

/// <summary>
/// セーブデータの構造
/// </summary>
[Serializable]
public class SaveData
{
    // ゲーム進行状況
    public int currentDay;              // 経過日数
    public string currentWeather;       // 現在の天候（enum名）
    public long savedTimestamp;         // セーブ時のタイムスタンプ（UnixTime）

    // プレイヤーステータス
    public int playerHP;                // HP
    public int playerStamina;           // スタミナ

    // インベントリ（将来の実装用）
    public List<SavedInventorySlot> inventory;

    // 装備（将来の実装用）
    public string equippedAccessory1;   // 装備中のアクセサリー1のID
    public string equippedAccessory2;   // 装備中のアクセサリー2のID

    // マップ上のアイテム
    public List<SavedMapItem> mapItems;

    // キャラクターデータ
    public List<SavedCharacterData> characters;

    // 建築データ
    public ConstructionStateData constructionState;
    public List<SavedMapConstruction> mapConstructions;

    // イベントデータ
    public EventManagerData eventManager;

    // イベント報酬データ
    public List<string> claimedEventIds; // 取得済みイベント報酬のID

    // 結婚データ
    public string marriedCharacterId; // 結婚相手のキャラクターID（null = 未婚）

    // ライバルデータ
    public List<RivalPairData> rivalPairs;
    public List<SavedRivalCharacterData> rivalCharacters;

    // フラグデータ
    public FlagsSystem.FlagsSaveData flags;

    // セーブデータのバージョン（将来の互換性対応用）
    public int saveVersion = 1;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SaveData()
    {
        inventory = new List<SavedInventorySlot>();
        mapItems = new List<SavedMapItem>();
        characters = new List<SavedCharacterData>();
        mapConstructions = new List<SavedMapConstruction>();
        rivalPairs = new List<RivalPairData>();
        rivalCharacters = new List<SavedRivalCharacterData>();
        claimedEventIds = new List<string>();
    }

    /// <summary>
    /// 建築状態のセーブデータ
    /// </summary>
    [Serializable]
    public class ConstructionStateData
    {
        public OngoingConstruction currentConstruction;
    }

    /// <summary>
    /// イベント管理のセーブデータ
    /// </summary>
    [Serializable]
    public class EventManagerData
    {
        public List<string> triggeredEvents;
    }

    /// <summary>
    /// ライバルペアのセーブデータ
    /// </summary>
    [Serializable]
    public class RivalPairData
    {
        public string mainCharacterId;      // 攻略可能キャラクターID
        public string rivalCharacterId;     // ライバルキャラクターID
        public int currentEventStage;       // 現在のライバルイベント段階
        public bool isMarried;              // ライバルペアが結婚済みか

        public RivalPairData(string mainCharacterId, string rivalCharacterId, int currentEventStage, bool isMarried)
        {
            this.mainCharacterId = mainCharacterId;
            this.rivalCharacterId = rivalCharacterId;
            this.currentEventStage = currentEventStage;
            this.isMarried = isMarried;
        }
    }

    /// <summary>
    /// ライバルキャラクターのセーブデータ
    /// </summary>
    [Serializable]
    public class SavedRivalCharacterData
    {
        public string rivalCharacterId;     // ライバルキャラクターID
        public int friendship;              // 友好度

        public SavedRivalCharacterData(string rivalCharacterId, int friendship)
        {
            this.rivalCharacterId = rivalCharacterId;
            this.friendship = friendship;
        }
    }
}

/// <summary>
/// キャラクターのセーブデータ
/// </summary>
[Serializable]
public class SavedCharacterData
{
    public string characterId;      // キャラクターID
    public int friendship;          // 友好度
    public int romance;             // 恋愛度
    public bool isMarried;          // 結婚しているか

    public SavedCharacterData(string characterId, int friendship, int romance, bool isMarried = false)
    {
        this.characterId = characterId;
        this.friendship = friendship;
        this.romance = romance;
        this.isMarried = isMarried;
    }
}

/// <summary>
/// マップ上のアイテムのセーブデータ
/// </summary>
[Serializable]
public class SavedMapItem
{
    public string location;     // 場所（enum名）
    public string itemId;       // アイテムID
    public int quantity;        // 数量

    public SavedMapItem(string location, string itemId, int quantity)
    {
        this.location = location;
        this.itemId = itemId;
        this.quantity = quantity;
    }
}

/// <summary>
/// インベントリスロットのセーブデータ
/// </summary>
[Serializable]
public class SavedInventorySlot
{
    public string itemId;       // アイテムID
    public int quantity;        // 数量

    public SavedInventorySlot(string itemId, int quantity)
    {
        this.itemId = itemId;
        this.quantity = quantity;
    }
}

/// <summary>
/// マップ上の建築物のセーブデータ
/// </summary>
[Serializable]
public class SavedMapConstruction
{
    public string constructionId;   // 建築物ID
    public string location;         // 場所（enum名）

    public SavedMapConstruction(string constructionId, string location)
    {
        this.constructionId = constructionId;
        this.location = location;
    }
}

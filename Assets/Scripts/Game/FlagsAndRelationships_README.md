# フラグ・好感度管理システム

条件付きスケジュールで使用する「フラグ」と「好感度」を管理するシステムです。

---

## 📁 システム構成

### 1. FlagsSystem
ゲーム内のフラグを管理します。
- イベント進行状況
- 建築中フラグ
- ストーリー進行フラグ
- 特殊条件フラグ

### 2. CharacterManager（既存システム）
キャラクターとの関係性を管理します。
- Romance（恋愛度 0～100）
- Friendship（友好度 0～100）
- IsMarried（結婚状態）

---

## 🛠️ 使い方

### フラグの操作

#### フラグを立てる
```csharp
// 建築中フラグをON
GameManager.Instance.State.Flags.SetFlag("building_in_progress", true);

// イベント完了フラグをON
GameManager.Instance.State.Flags.SetFlag("beach_event_completed");
```

#### フラグを確認
```csharp
if (GameManager.Instance.State.Flags.IsFlagEnabled("building_in_progress"))
{
    Debug.Log("建築中です");
}
```

#### フラグをOFFにする
```csharp
GameManager.Instance.State.Flags.SetFlag("building_in_progress", false);
```

#### フラグを削除
```csharp
GameManager.Instance.State.Flags.RemoveFlag("building_in_progress");
```

### 好感度の操作（CharacterManager使用）

#### 恋愛度を追加
```csharp
// Emilyを取得
var emily = GameManager.Instance.State.Characters.GetCharacter("char_01");

// 恋愛度を+10
emily.IncreaseRomance(10);

// 恋愛度を-5（マイナス値も可能）
emily.DecreaseRomance(5);
```

#### 恋愛度を取得
```csharp
var emily = GameManager.Instance.State.Characters.GetCharacter("char_01");
int romance = emily.Romance;
Debug.Log($"Emilyの恋愛度: {romance}");

// 友好度も取得可能
int friendship = emily.Friendship;
Debug.Log($"Emilyの友好度: {friendship}");
```

#### 結婚状態を確認
```csharp
var emily = GameManager.Instance.State.Characters.GetCharacter("char_01");
if (emily.IsMarried)
{
    Debug.Log("Emilyと結婚済み");
}
```

---

## 🎮 実践例

### 例1: 建築中のスケジュール変更

#### 1. 建築開始時にフラグをON
```csharp
// 建築開始時
public void StartConstruction(string constructionId)
{
    // 建築処理...

    // 建築中フラグを立てる
    GameManager.Instance.State.Flags.SetFlag("building_in_progress", true);

    // スケジュールを再評価
    var behaviorManager = FindObjectOfType<CharacterBehaviorManager>();
    behaviorManager?.ReevaluateConditionalSchedules();
}
```

#### 2. ConditionalScheduleLoaderで条件設定
```
Condition Type: FlagEnabled
Required Flag Id: "building_in_progress"
Conditional Schedule: CraftsmanBuildingSchedule
Schedule Mode: Override
```

#### 3. 建築完了時にフラグをOFF
```csharp
// 建築完了時
private void OnConstructionCompleted(string constructionId)
{
    // 建築中フラグを解除
    GameManager.Instance.State.Flags.SetFlag("building_in_progress", false);

    // スケジュールを再評価
    var behaviorManager = FindObjectOfType<CharacterBehaviorManager>();
    behaviorManager?.ReevaluateConditionalSchedules();
}
```

### 例2: 好感度によるスケジュール変更

#### 会話で恋愛度アップ
```csharp
// 会話終了時
public void OnConversationEnd(string characterId, int romanceGain)
{
    // キャラクターを取得
    var character = GameManager.Instance.State.Characters.GetCharacter(characterId);

    // 恋愛度を追加
    character.IncreaseRomance(romanceGain);

    // スケジュールを再評価（恋愛度が閾値を超えたかもしれない）
    var behaviorManager = FindObjectOfType<CharacterBehaviorManager>();
    behaviorManager?.ReevaluateConditionalSchedules();
}
```

#### ConditionalScheduleLoaderで条件設定
```
Condition Type: AffectionLevel
Required Affection: 80（恋愛度80以上）
Target Character Id: "char_01"（職人のID）
Conditional Schedule: CraftsmanHighAffectionSchedule
Schedule Mode: Override
```

### 例3: イベント進行によるスケジュール変更

#### イベント完了時にフラグを立てる
```csharp
// イベントシナリオ完了時（Utageから呼ばれる）
public void OnBeachEventCompleted()
{
    // イベント完了フラグを立てる
    GameManager.Instance.State.Flags.SetFlag("beach_event_completed", true);

    // 職人の恋愛度も上昇
    var craftsman = GameManager.Instance.State.Characters.GetCharacter("char_01");
    craftsman.IncreaseRomance(15);

    // スケジュールを再評価
    var behaviorManager = FindObjectOfType<CharacterBehaviorManager>();
    behaviorManager?.ReevaluateConditionalSchedules();
}
```

#### ConditionalScheduleLoaderで条件設定
```
Condition Type: FlagEnabled
Required Flag Id: "beach_event_completed"
Conditional Schedule: EmilyPostBeachSchedule
Schedule Mode: Override
```

---

## 💡 よくある使い方

### 建築中の職人
```csharp
// 建築開始
GameManager.Instance.State.Flags.SetFlag("building_in_progress", true);

// → 職人が朝・昼・夕すべて同じ場所で作業（Overrideモードで全時間帯定義）

// 建築完了
GameManager.Instance.State.Flags.SetFlag("building_in_progress", false);

// → 職人が通常スケジュールに戻る
```

### ストーリー進行
```csharp
// チュートリアル完了
GameManager.Instance.State.Flags.SetFlag("tutorial_completed", true);

// → キャラクターの会話内容が変わる
```

### 季節イベント
```csharp
// 夏イベント開始
GameManager.Instance.State.Flags.SetFlag("summer_event_active", true);

// → 全キャラクターが水着になる（Overrideで表情だけ変更）

// 夏イベント終了
GameManager.Instance.State.Flags.SetFlag("summer_event_active", false);
```

### デレ段階の変化
```csharp
// 恋愛度0～30: ツンツン
// 恋愛度31～60: 普通
// 恋愛度61～80: 少しデレ
// 恋愛度81～100: デレデレ

// ConditionalScheduleLoaderで複数条件を設定
Condition 1: Romance >= 81 → デレデレスケジュール
Condition 2: Romance >= 61 → 少しデレスケジュール
Condition 3: Romance >= 31 → 普通スケジュール
```

---

## 🔧 便利なメソッド

### フラグ関連
```csharp
// フラグをトグル（ON/OFF反転）
GameManager.Instance.State.Flags.ToggleFlag("debug_mode");

// 全フラグをクリア
GameManager.Instance.State.Flags.ClearAllFlags();

// フラグ数を取得
int count = GameManager.Instance.State.Flags.GetFlagCount();

// 全フラグを取得（デバッグ用）
var allFlags = GameManager.Instance.State.Flags.GetAllFlags();
foreach (var kvp in allFlags)
{
    Debug.Log($"{kvp.Key}: {kvp.Value}");
}
```

### キャラクター関連
```csharp
// キャラクターを取得
var character = GameManager.Instance.State.Characters.GetCharacter("char_01");

// 恋愛度を増やす
character.IncreaseRomance(10);

// 友好度を増やす
character.IncreaseFriendship(5);

// 結婚する
character.Marry();

// 結婚状態を確認
bool isMarried = character.IsMarried;

// 恋愛度を取得
int romance = character.Romance;

// 友好度を取得
int friendship = character.Friendship;
```

---

## 📝 セーブ/ロード

**フラグとキャラクターデータは自動的にセーブ/ロードされます**。

GameManagerの自動セーブ機能で：
- アプリが一時停止された時
- アプリがフォーカスを失った時
- 手動でSaveGame()を呼んだ時

に自動的に保存されます。

**保存される情報**:
- フラグ（FlagsSystem）
- キャラクターの恋愛度・友好度（CharacterManager）
- 結婚状態（CharacterManager）

---

## 🐛 トラブルシューティング

### フラグが反映されない
```csharp
// フラグを立てた後、必ずスケジュールを再評価
var behaviorManager = FindObjectOfType<CharacterBehaviorManager>();
behaviorManager?.ReevaluateConditionalSchedules();
```

### 恋愛度が反映されない
```csharp
// 恋愛度を変更した後、必ずスケジュールを再評価
var character = GameManager.Instance.State.Characters.GetCharacter("char_01");
character.IncreaseRomance(10);
behaviorManager?.ReevaluateConditionalSchedules();
```

### フラグIDのタイポに注意
```csharp
// ❌NG: タイポ
SetFlag("building_in_progres");  // 最後のsが抜けている

// ✅OK
SetFlag("building_in_progress");
```

---

## 🔗 関連ファイル

- `/Assets/Scripts/Game/FlagsSystem.cs` - フラグ管理
- `/Assets/Scripts/Game/Characters/CharacterManager.cs` - キャラクター管理（恋愛度・友好度）
- `/Assets/Scripts/Game/Characters/Character.cs` - キャラクタークラス
- `/Assets/Scripts/Game/GameState.cs` - ゲーム状態（Flags/CharactersManagerを含む）
- `/Assets/Scripts/Behavior/ConditionalScheduleLoader.cs` - 条件付きスケジュールローダー
- `/Assets/Scripts/Behavior/CharacterBehaviorManager.cs` - 行動管理マネージャー

---

## 💡 Tips

### GameManagerへのアクセスを短縮
```csharp
// よく使う場合はローカル変数に
var flags = GameManager.Instance.State.Flags;
var characters = GameManager.Instance.State.Characters;

flags.SetFlag("building_in_progress", true);

var craftsman = characters.GetCharacter("char_01");
craftsman.IncreaseRomance(10);
```

### デバッグコマンド（開発中に便利）
```csharp
// フラグを一覧表示
[ContextMenu("Show All Flags")]
public void ShowAllFlags()
{
    var allFlags = GameManager.Instance.State.Flags.GetAllFlags();
    foreach (var kvp in allFlags)
    {
        Debug.Log($"Flag: {kvp.Key} = {kvp.Value}");
    }
}

// 全キャラの恋愛度を表示
[ContextMenu("Show All Characters")]
public void ShowAllCharacters()
{
    var allCharacters = GameManager.Instance.State.Characters.AllCharacters;
    foreach (var character in allCharacters)
    {
        Debug.Log($"{character.name}: 恋愛度{character.Romance}, 友好度{character.Friendship}, 結婚{character.IsMarried}");
    }
}
```

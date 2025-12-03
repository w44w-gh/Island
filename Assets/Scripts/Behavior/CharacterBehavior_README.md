# キャラクター行動管理システム

時間帯ごとにキャラクターの行動を自動管理するシステムです。ScriptableObjectでスケジュールを定義し、マネージャーが一括制御します。

---

## 📁 構成

- **CharacterAction.cs** - 具体的な行動を定義（配置、表情、セリフなど）
- **CharacterSchedule.cs** - ScriptableObject形式のスケジュール定義
- **CharacterBehaviorManager.cs** - 全キャラクターの行動を一括管理

---

## 🎮 基本的な仕組み

### スケジュール定義（ScriptableObject）
エディタで編集可能なアセットとして、各キャラクターの1日のスケジュールを定義。

### 自動更新
時間帯が変わると、CharacterBehaviorManagerが全キャラクターの行動を自動更新。

### 特別イベント
通常スケジュールを上書きして、イベント専用の行動を実行。

---

## 🛠️ セットアップ

### 1. CharacterScheduleの作成（各キャラごと）

#### Emily用のスケジュール作成
1. Project ウィンドウで右クリック → Create → Island → Character Schedule
2. 名前を "EmilySchedule" に変更
3. Inspector で設定:

**Character Info**:
- Character Id: "emily"
- Character Name: "Emily"

**Daily Schedule**:
```
Actions:
  [0] Early Morning (早朝)
    - Time Of Day: EarlyMorning
    - Is Present: false（寝ている）
    - Appearance Variation: "sleep"

  [1] Morning (朝)
    - Time Of Day: Morning
    - Is Present: true
    - Position: LeftNear
    - Appearance Variation: "normal"
    - Is Interactable: true
    - Status Message: "おはよう！"
    - Scenario Label: "emily_morning"

  [2] Noon (昼)
    - Time Of Day: Noon
    - Is Present: true
    - Position: CenterMiddle
    - Appearance Variation: "cooking"
    - Is Interactable: true
    - Status Message: "お昼ごはんを作ってるの"
    - Scenario Label: "emily_noon"

  [3] Evening (夜)
    - Time Of Day: Evening
    - Is Present: true
    - Position: RightNear
    - Appearance Variation: "normal"
    - Is Interactable: true
    - Status Message: "夕日がきれいね"
    - Scenario Label: "emily_evening"

  [4] Midnight (深夜)
    - Time Of Day: Midnight
    - Is Present: false（寝ている）
    - Appearance Variation: "sleep"
```

**Special Events**（オプション）:
```
Special Events:
  [0] Beach Party
    - Event Id: "beach_party"
    - Action:
      - Time Of Day: Noon（イベント時は無視される）
      - Is Present: true
      - Position: CenterNear
      - Appearance Variation: "swimsuit"
      - Is Interactable: true
      - Status Message: "ビーチパーティーだよ！"
      - Scenario Label: "emily_beach_party"
```

#### デフォルトスケジュールの自動生成（便利機能）
Inspector で CharacterSchedule を開いた状態で:
1. 右上のメニュー（⋮）をクリック
2. "Generate Default Schedule" を選択
3. 基本的なスケジュールが自動生成される
4. 必要に応じて編集

#### 他のキャラクターも同様に作成
- AlexSchedule
- SarahSchedule
- ...

### 2. CharacterBehaviorManagerの配置

GameScene に配置:
1. Hierarchy で空のGameObject作成 → "CharacterBehaviorManager"
2. CharacterBehaviorManager.cs をアタッチ
3. Inspector で設定:

**Characters**:
```
Characters:
  [0] Emily
    - Character: Emily（InteractableCharacter）をドラッグ
    - Schedule: EmilySchedule をドラッグ

  [1] Alex
    - Character: Alex（InteractableCharacter）をドラッグ
    - Schedule: AlexSchedule をドラッグ

  [2] Sarah
    - Character: Sarah（InteractableCharacter）をドラッグ
    - Schedule: SarahSchedule をドラッグ
```

**Auto Update**:
- Auto Update On Time Change: チェックON（自動更新を有効化）

---

## 📝 使い方

### 基本的な流れ（自動）

1. **ゲーム開始**: 現在の時間帯に応じた行動を適用
2. **時間経過**: 時間帯が変わると自動的に全キャラの行動を更新
3. **イベント発生**: 特別イベント開始で全員がイベント行動に切り替わる
4. **イベント終了**: 通常スケジュールに戻る

### イベントの開始・終了（コードから）

```csharp
CharacterBehaviorManager manager = FindObjectOfType<CharacterBehaviorManager>();

// ビーチパーティーイベント開始
manager.StartEvent("beach_party");
// → 全キャラクターがビーチパーティー用の行動に切り替わる

// イベント終了
manager.EndEvent();
// → 通常スケジュールに戻る
```

### 手動で行動を更新

```csharp
CharacterBehaviorManager manager = FindObjectOfType<CharacterBehaviorManager>();

// 現在の時間帯で全員を更新
manager.ForceUpdateAllCharacters();

// 特定の時間帯で更新
manager.UpdateAllCharacters(TimeOfDay.Noon);
```

### キャラクターの動的追加

```csharp
// 新しいキャラクターを追加
CharacterSchedule newSchedule = Resources.Load<CharacterSchedule>("Schedules/NewCharacterSchedule");
InteractableCharacter newCharacter = Instantiate(characterPrefab);

manager.AddCharacter(newCharacter, newSchedule);
// → 即座に現在の時間帯の行動が適用される
```

---

## 🎨 実践例

### 例1: 全員の基本スケジュール

#### Emily（料理担当）
- **早朝**: いない（寝ている）
- **朝**: 左手前、通常の表情、「おはよう！」
- **昼**: 中央、料理中の表情、「お昼ごはんを作ってるの」
- **夜**: 右手前、通常の表情、「夕日がきれいね」
- **深夜**: いない（寝ている）

#### Alex（探検家）
- **早朝**: 中央奥、通常の表情、「朝の散歩中」
- **朝**: 右手前、通常の表情、「発見があったよ！」
- **昼**: いない（探検中）
- **夜**: 左手前、疲れた表情、「今日も充実してた」
- **深夜**: いない（寝ている）

#### Sarah（釣り好き）
- **早朝**: 右奥、通常の表情、「釣りの準備中」
- **朝**: いない（釣りに出かけている）
- **昼**: 中央手前、笑顔、「魚が釣れたよ！」
- **夜**: 左中間、通常の表情、「明日も頑張ろう」
- **深夜**: いない（寝ている）

### 例2: ビーチパーティーイベント

```csharp
// イベント開始
void StartBeachParty()
{
    manager.StartEvent("beach_party");

    // 全員が水着姿でビーチに集合
    // - Emily: 中央手前、水着、「楽しもう！」
    // - Alex: 左手前、水着、「泳ぎに行こうぜ！」
    // - Sarah: 右手前、水着、「最高の天気だね！」
}

// イベント終了
void EndBeachParty()
{
    manager.EndEvent();

    // 通常スケジュールに戻る
    // 現在の時間帯に応じた行動が再度適用される
}
```

### 例3: 天候による行動変化

```csharp
// 雨の日専用のイベントを開始
void OnRainyDay()
{
    manager.StartEvent("rainy_day");

    // 全員が屋内に避難
    // - Emily: キッチン、通常、「雨だから料理しよう」
    // - Alex: 読書中、通常、「本でも読もうかな」
    // - Sarah: 残念そう、悲しい、「釣りに行けない...」
}

void OnSunnyDay()
{
    manager.EndEvent();
    // 通常スケジュールに戻る
}
```

---

## ⚙️ カスタマイズ

### 行動の詳細設定

CharacterAction で設定できる項目:

#### 存在（Presence）
- **Is Present**: false = キャラクターを非表示（寝ている、出かけているなど）

#### 位置（Position）
- **Position**: プリセット位置（LeftNear, CenterFarなど）
- **Use Custom Position**: true = カスタム位置を使用
- **Custom Position**: 任意の座標

#### 外見（Appearance）
- **Appearance Variation**: 表情・衣装の名前（"normal", "happy", "swimsuit"など）

#### インタラクション（Interaction）
- **Is Interactable**: false = 話しかけられない（忙しい時など）
- **Scenario Label**: 会話シナリオのラベル（時間帯ごとに異なる会話）

#### メッセージ（Message）
- **Status Message**: ステータス表示用のメッセージ（「料理中」「釣りに出かけている」など）

### イベントIDの命名規則

推奨される命名規則:
- **beach_party** - ビーチパーティー
- **rainy_day** - 雨の日
- **birthday** - 誕生日
- **festival** - お祭り
- **storm** - 嵐

---

## 🐛 トラブルシューティング

### 時間帯が変わってもキャラが動かない
1. CharacterBehaviorManager の Auto Update On Time Change がチェックされているか確認
2. GameManager の OnTimeOfDayChanged イベントが発火しているか確認
3. Inspector の Characters リストにキャラとスケジュールが設定されているか確認

### スケジュールが見つからない
1. CharacterSchedule アセットが正しく作成されているか確認
2. Project ウィンドウで検索: "t:CharacterSchedule"
3. CharacterBehaviorManager の Schedule フィールドにドラッグされているか確認

### イベント行動が適用されない
1. Special Events に該当するイベントIDが登録されているか確認
2. StartEvent() の引数がイベントIDと一致しているか確認（大文字小文字も）
3. イベント終了時に EndEvent() を呼んでいるか確認

### 全ての時間帯の行動が定義されていない
Inspector で CharacterSchedule を開いた状態で:
1. 右上のメニュー（⋮）をクリック
2. "Validate All Schedules" を選択
3. コンソールにエラーが表示される

---

## 💡 Tips

### エディタでのデバッグ

#### スケジュールの検証
CharacterBehaviorManager の Inspector で:
- 右上のメニュー → "Validate All Schedules"
- 不足している時間帯がコンソールに表示される

#### 手動更新
CharacterBehaviorManager の Inspector で:
- 右上のメニュー → "Force Update All Characters"
- 現在の時間帯で全員を即座に更新

#### デフォルトスケジュール生成
CharacterSchedule の Inspector で:
- 右上のメニュー → "Generate Default Schedule"
- 基本的なスケジュールが自動生成される

### パフォーマンス最適化

- キャラクター数が多い場合は、時間帯変化時ではなく定期的に更新
- Is Present = false のキャラは GameObject.SetActive(false) で完全に無効化
- イベント中でないキャラは更新をスキップ

### 複数シーンでの使用

CharacterBehaviorManager は各シーンに配置:
- TitleScene: 不要
- GameScene: 必要（キャラクターがいる）
- SettingsScene: 不要

---

## 🔗 関連ファイル

- `/Assets/Scripts/Behavior/CharacterAction.cs` - 行動定義
- `/Assets/Scripts/Behavior/CharacterSchedule.cs` - スケジュール定義（ScriptableObject）
- `/Assets/Scripts/Behavior/CharacterBehaviorManager.cs` - 行動管理マネージャー
- `/Assets/Scripts/Interaction/InteractableCharacter.cs` - キャラクター本体
- `/Assets/Scripts/Managers/GameManager.cs` - 時間帯変化イベント

---

## 📚 今後の拡張案

### 天候連動
雨の日は自動的に rainy_day イベントを開始

### 好感度連動
好感度が高いキャラは話しかけやすい位置に移動

### ランダムイベント
確率で特別な行動をとる

### 複数キャラの連携
2人以上のキャラが同時に特定の場所にいる時は特別な会話

### アニメーション
位置移動時に歩行アニメーション

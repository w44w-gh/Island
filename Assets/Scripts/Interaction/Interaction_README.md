# インタラクションシステム（ダンガンロンパ２スタイル）

立ち絵のキャラクターをタップして会話、足元のアイテムをタップして拾うシステムです。

---

## 📁 構成

- **IInteractable.cs** - タップ可能オブジェクトのインターフェース
- **InteractableCharacter.cs** - タップで会話できるキャラクター
- **InteractableItem.cs** - タップで拾えるアイテム
- **ItemSpawner.cs** - 時間経過でアイテムをスポーン

---

## 🎮 基本的な仕組み

### キャラクター
- 立ち絵をそのまま配置
- タップするとNovelSceneに遷移して会話
- 好感度やフラグによってシナリオ分岐

### アイテム
- 足元にアイテムが落ちている
- タップすると取得アニメーション（上に浮いて消える）
- インベントリに自動追加
- 時間経過で新しいアイテムがスポーン

### スポーンシステム
- 時間帯と天候によってスポーンアイテムが変わる
- 例: 朝の晴れ → 木材、石材
- 例: 雨 → きのこ、魚
- スポーン間隔と最大数を設定可能

---

## 🛠️ Unity Editorでのセットアップ

### 1. Prefabの作成

#### InteractableCharacter Prefab
1. Hierarchy で右クリック → UI → Image
2. 名前: "InteractableCharacter"
3. InteractableCharacter.cs をアタッチ
4. Inspector で設定:
   - **Character Id**: キャラクターID（"emily", "alex"など）
   - **Scenario Label**: シナリオラベル（"emily_greeting"など）
   - **Character Sprite**: キャラクターの立ち絵
   - **Is Interactable**: チェックON
5. Prefab化: Project ウィンドウにドラッグ

#### InteractableItem Prefab
1. Hierarchy で右クリック → UI → Image
2. 名前: "InteractableItem"
3. InteractableItem.cs をアタッチ
4. Inspector で設定:
   - **Item Id**: アイテムID（"wood", "stone"など）
   - **Quantity**: 取得個数（1）
   - **Item Sprite**: アイテム画像
   - **Is Interactable**: チェックON
   - **Pickup Animation Duration**: 0.5秒
5. Prefab化: Project ウィンドウにドラッグ

### 2. GameSceneへの配置

#### Canvas構造
```
GameScene
  └─ Canvas
      ├─ GameUI（上部のステータス表示など）
      ├─ CharacterContainer（キャラクター配置用）
      │   ├─ Emily（InteractableCharacter）
      │   ├─ Alex（InteractableCharacter）
      │   └─ Sarah（InteractableCharacter）
      ├─ ItemContainer（アイテム配置用）
      │   └─ （動的にスポーン）
      └─ SpawnPoints（スポーン位置マーカー）
          ├─ SpawnPoint1
          ├─ SpawnPoint2
          ├─ SpawnPoint3
          └─ SpawnPoint4
```

#### CharacterContainerの作成
1. Canvas の下に空のGameObject作成 → "CharacterContainer"
2. RectTransform 設定:
   - Anchor: Stretch (全画面)
   - Width/Height: 0

#### キャラクターの配置
1. InteractableCharacter Prefabを CharacterContainer にドラッグ
2. 名前を "Emily" などに変更
3. RectTransform で位置とサイズを調整:
   - Width: 300, Height: 600（立ち絵サイズ）
   - Anchor Preset: Bottom Left/Center/Right（配置したい場所）
   - Position: 適切な位置に配置
4. Inspector で設定:
   - **Character Id**: "emily"
   - **Scenario Label**: "emily_greeting"
   - **Character Sprite**: Emilyの立ち絵をドラッグ
5. 他のキャラクターも同様に配置

#### ItemContainerの作成
1. Canvas の下に空のGameObject作成 → "ItemContainer"
2. RectTransform 設定:
   - Anchor: Stretch (全画面)
   - Width/Height: 0

#### SpawnPointsの作成
1. Canvas の下に空のGameObject作成 → "SpawnPoints"
2. 子オブジェクトとして空のGameObject作成 → "SpawnPoint1"
3. RectTransform で位置を設定（アイテムがスポーンする位置）
   - 例: Anchor: Bottom Left, Position: (150, 50, 0)
4. SpawnPoint2, SpawnPoint3... と複数作成
   - 足元の各所にアイテムが出現するイメージ

#### ItemSpawnerの設定
1. Hierarchy で空のGameObject作成 → "ItemSpawner"
2. ItemSpawner.cs をアタッチ
3. Inspector で設定:
   - **Item Prefab**: InteractableItem Prefabをドラッグ
   - **Item Container**: ItemContainer をドラッグ
   - **Spawn Points**: SpawnPoint1, SpawnPoint2... をリストに追加
   - **Spawn Interval**: 300（5分ごと）
   - **Max Items On Field**: 10

#### スポーン条件の設定
ItemSpawner の Inspector で `Spawn Conditions` を設定:

**条件1: 朝・晴れ**
- Time Of Day: Morning
- Weather: Sunny
- Items:
  - Item Id: "wood"
  - Item Sprite: 木材の画像
  - Min Quantity: 1, Max Quantity: 3
  - Spawn Chance: 0.7

  - Item Id: "stone"
  - Item Sprite: 石の画像
  - Min Quantity: 1, Max Quantity: 2
  - Spawn Chance: 0.5

**条件2: 雨**
- Time Of Day: Morning（任意）
- Weather: Rainy
- Items:
  - Item Id: "mushroom"
  - Item Sprite: きのこの画像
  - Min Quantity: 1, Max Quantity: 1
  - Spawn Chance: 0.8

---

## 🎨 レイアウト例

```
━━━━━━━━━━━━━━━━━━━━━━━━━
[時刻] [天候] [ステータス]     [設定]
━━━━━━━━━━━━━━━━━━━━━━━━━


        [Emily]         [Alex]


    🪵                      🪨

  🍄         [Sarah]


━━━━━━━━━━━━━━━━━━━━━━━━━
```

- 上部: ステータス表示
- 中央～下部: キャラクター立ち絵
- 足元: アイテム

---

## 📝 使い方

### キャラクターとの会話
```csharp
// 自動設定（InteractableCharacterで設定済み）
// タップすると自動的にNovelSceneに遷移
```

### アイテムの取得
```csharp
// 自動設定（InteractableItemで設定済み）
// タップすると自動的にインベントリに追加される
```

### プログラムから制御

#### キャラクターの有効/無効
```csharp
InteractableCharacter character = GetComponent<InteractableCharacter>();

// インタラクション無効化（グレーアウト）
character.SetInteractable(false);

// ハイライト表示
character.SetHighlight(true);
```

#### アイテムの手動スポーン
```csharp
ItemSpawner spawner = GetComponent<ItemSpawner>();

// 手動でスポーン実行
spawner.ForceSpawn();

// 全てのアイテムをクリア
spawner.ClearAllItems();
```

#### 動的にキャラクターを生成
```csharp
// Prefabからインスタンス化
GameObject charObj = Instantiate(characterPrefab, characterContainer.transform);

// セットアップ
InteractableCharacter character = charObj.GetComponent<InteractableCharacter>();
character.Setup("emily", "emily_greeting", emilySprite);

// 位置設定
RectTransform rect = charObj.GetComponent<RectTransform>();
rect.anchoredPosition = new Vector2(200, 100);
```

---

## ⚙️ カスタマイズ

### アイテムスポーン間隔の変更
```csharp
// ItemSpawner Inspector
Spawn Interval: 300  // 秒単位（300 = 5分）
```

### スポーン確率の調整
```csharp
// SpawnableItem の Spawn Chance を変更
0.0 = 0%（絶対出ない）
0.5 = 50%
1.0 = 100%（必ず出る）
```

### アニメーションの変更
```csharp
// InteractableItem Inspector
Pickup Animation Duration: 0.5  // 取得アニメーション時間
Pickup Curve: カーブを編集（Inspector上で）
```

### ハイライト効果の追加
1. InteractableCharacter の子オブジェクトとして空のGameObjectを作成
2. 名前: "HighlightEffect"
3. Outline や Glow などのエフェクトを追加
4. Inspector で `Highlight Effect` にドラッグ

---

## 🎵 必要なSE

### アイテム関連
- **item_pickup** - アイテム取得音
- **item_spawn** - アイテムスポーン音（オプション）

### キャラクター関連
- **button_tap** - キャラクタータップ音（既存）

---

## 💡 実装のポイント

### ダンガンロンパ２スタイル
- **立ち絵配置**: トップビューではなく、立ち絵をそのまま配置
- **シンプルなUI**: 複雑な3Dモデルは不要
- **タップ操作**: マウス/タッチで直感的に操作
- **会話イベント**: NovelSceneでストーリー展開

### スポーンシステム
- **時間帯連動**: ゲーム内時間に応じてアイテムが変化
- **天候連動**: 雨の日は特別なアイテムが出る
- **確率制御**: レアアイテムは低確率でスポーン

### パフォーマンス
- **最大アイテム数**: フィールド上のアイテム数を制限
- **Prefab使用**: 効率的なインスタンス化
- **オブジェクトプール**: 今後の拡張で実装可能

---

## 🐛 トラブルシューティング

### キャラクターをタップしても反応しない
1. Button コンポーネントが追加されているか確認
2. Interactable がチェックされているか確認
3. CharacterId と ScenarioLabel が設定されているか確認
4. Canvas に GraphicRaycaster コンポーネントがあるか確認

### アイテムが取得できない
1. Button コンポーネントが追加されているか確認
2. ItemId が正しく設定されているか確認
3. GameManager.Instance.State.Inventory が初期化されているか確認

### アイテムがスポーンしない
1. ItemSpawner の Item Prefab が設定されているか確認
2. Spawn Points が設定されているか確認
3. Spawn Conditions が設定されているか確認
4. 現在の時間帯・天候と条件が一致しているか確認
5. Max Items On Field に達していないか確認

### 立ち絵が表示されない
1. Character Sprite が設定されているか確認
2. Image コンポーネントがあるか確認
3. Canvas がカメラに表示されているか確認

---

## 🔗 関連ファイル

- `/Assets/Scripts/Interaction/IInteractable.cs` - インターフェース
- `/Assets/Scripts/Interaction/InteractableCharacter.cs` - キャラクター
- `/Assets/Scripts/Interaction/InteractableItem.cs` - アイテム
- `/Assets/Scripts/Interaction/ItemSpawner.cs` - スポーナー
- `/Assets/Scripts/Managers/SceneLoader.cs` - シーン遷移
- `/Assets/Scripts/Managers/AudioManager.cs` - 音響管理

---

## 📚 今後の拡張案

### キャラクター関連
- 時間帯によって移動する
- 好感度によって立ち絵が変わる
- ホバー時にセリフが表示される

### アイテム関連
- レアアイテムのエフェクト
- アイテムの種類を増やす
- オブジェクトプールで最適化

### スポーン関連
- イベント連動スポーン
- 特定の場所でのみスポーン
- プレイヤーの行動に応じたスポーン

### UI演出
- キャラクターのアニメーション（瞬き、揺れなど）
- アイテムの光るエフェクト
- タップ時のパーティクル

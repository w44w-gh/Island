# キャラクター拡張機能（立ち絵変更・遠近感・影）

キャラクターの外見を動的に変更し、立体感のある配置を実現する拡張機能です。

---

## 📁 新機能

### 1. 立ち絵の動的変更（CharacterAppearance）
- 表情や衣装を状況に応じて変更
- 複数のバリエーションを登録可能
- 例: "normal", "happy", "sad", "angry", "swimsuit"

### 2. 遠近感と影（CharacterDepth）
- 奥にいるキャラは小さく表示（50%～110%）
- 影を自動生成して立体感を演出
- 深度に応じて影の濃さも変化

### 3. 配置プリセット（CharacterPositionPreset）
- 左手前、中央奥など9つのプリセット
- 位置とサイズを一括設定
- エディタでGizmos表示

---

## 🎨 深度レベル

### DepthLevel（遠近感）
- **VeryFar** - 最奥（50%サイズ）- 背景に近い
- **Far** - 奥（70%サイズ）
- **Middle** - 中央（85%サイズ）
- **Near** - 手前（100%サイズ）- 標準
- **VeryNear** - 最前（110%サイズ）- 強調

### PositionPreset（配置プリセット）
#### 手前（Near）
- **LeftNear** - 左手前
- **CenterNear** - 中央手前
- **RightNear** - 右手前

#### 中間（Middle）
- **LeftMiddle** - 左中間
- **CenterMiddle** - 中央中間
- **RightMiddle** - 右中間

#### 奥（Far）
- **LeftFar** - 左奥
- **CenterFar** - 中央奥
- **RightFar** - 右奥

---

## 🛠️ セットアップ

### 1. CharacterPositionPresetの配置

GameScene に配置:
1. Hierarchy で空のGameObject作成 → "CharacterPositionPreset"
2. CharacterPositionPreset.cs をアタッチ
3. Inspector で設定:
   - **Canvas Rect**: Canvas の RectTransform をドラッグ
   - **Presets**: デフォルト値のまま（必要に応じて調整）

### 2. InteractableCharacterの設定

#### 基本設定
1. InteractableCharacter Prefabを作成または既存のものを編集
2. **必須コンポーネント**:
   - Image
   - CharacterDepth（自動追加される）
   - Button（自動追加される）

#### Appearance設定（立ち絵バリエーション）
Inspector の `Appearance` セクション:
```
Variations:
  - Variation Name: "normal"
    Sprite: Emily_Normal
  - Variation Name: "happy"
    Sprite: Emily_Happy
  - Variation Name: "sad"
    Sprite: Emily_Sad
  - Variation Name: "angry"
    Sprite: Emily_Angry
  - Variation Name: "swimsuit"
    Sprite: Emily_Swimsuit
```

#### Position & Depth設定
Inspector の `Position & Depth` セクション:
- **Initial Position**: CenterNear（またはお好みのプリセット）
- **Use Position Preset**: チェックON

#### CharacterDepth設定
CharacterDepthコンポーネント（自動追加）:
- **Current Depth**: Near（プリセットで自動設定）
- **Enable Shadow**: チェックON
- **Shadow Color**: (0, 0, 0, 0.3) - 半透明の黒
- **Shadow Offset**: (10, -10) - 右下に影

---

## 📝 使い方

### 立ち絵を変更

#### コードから変更
```csharp
InteractableCharacter character = GetComponent<InteractableCharacter>();

// 表情を変更
character.ChangeAppearance("happy");  // 笑顔に
character.ChangeAppearance("sad");    // 悲しい顔に
character.ChangeAppearance("angry");  // 怒り顔に

// 衣装を変更
character.ChangeAppearance("swimsuit");  // 水着に

// 通常に戻す
character.ChangeAppearance("normal");
```

#### 会話シーンから制御
NovelSceneから戻った後に変更する場合:
```csharp
void OnNovelSceneReturned()
{
    // 好感度によって表情を変える例
    if (relationship.affection > 80)
    {
        character.ChangeAppearance("happy");
    }
    else if (relationship.affection < 30)
    {
        character.ChangeAppearance("sad");
    }
}
```

### 配置を変更

#### プリセットを使用
```csharp
InteractableCharacter character = GetComponent<InteractableCharacter>();

// 位置プリセットを適用
character.ApplyPositionPreset(CharacterPositionPreset.PositionPreset.LeftFar);
// → 左奥に移動、小さく表示

character.ApplyPositionPreset(CharacterPositionPreset.PositionPreset.RightNear);
// → 右手前に移動、大きく表示
```

#### 深度のみ変更
```csharp
// 深度だけ変更（位置はそのまま）
character.SetDepth(CharacterDepth.DepthLevel.Far);
// → 小さく表示され、影も薄くなる

character.SetDepth(CharacterDepth.DepthLevel.VeryNear);
// → 大きく表示され、影も濃くなる
```

### 影を制御

```csharp
// 影を無効化
character.SetShadowEnabled(false);

// 影を有効化
character.SetShadowEnabled(true);
```

### シーン構成例

複数のキャラクターを一括配置:
```csharp
CharacterPositionPreset positionManager = FindObjectOfType<CharacterPositionPreset>();

GameObject[] characters = new GameObject[] { emily, alex, sarah };
CharacterPositionPreset.PositionPreset[] presets = new CharacterPositionPreset.PositionPreset[]
{
    CharacterPositionPreset.PositionPreset.LeftNear,    // Emily: 左手前
    CharacterPositionPreset.PositionPreset.CenterFar,   // Alex: 中央奥
    CharacterPositionPreset.PositionPreset.RightMiddle  // Sarah: 右中間
};

positionManager.ApplySceneLayout(characters, presets);
```

---

## 🎮 実践例

### 例1: 時間帯で表情を変える

```csharp
void OnTimeOfDayChanged(TimeOfDay current)
{
    if (current == TimeOfDay.Morning)
    {
        emily.ChangeAppearance("happy");  // 朝は元気
    }
    else if (current == TimeOfDay.Midnight)
    {
        emily.ChangeAppearance("tired");  // 深夜は疲れた顔
    }
}
```

### 例2: イベントで衣装を変える

```csharp
void OnBeachEvent()
{
    // ビーチイベント時は全員水着に
    emily.ChangeAppearance("swimsuit");
    alex.ChangeAppearance("swimsuit");
    sarah.ChangeAppearance("swimsuit");
}

void OnEventEnd()
{
    // イベント終了後は通常に戻す
    emily.ChangeAppearance("normal");
    alex.ChangeAppearance("normal");
    sarah.ChangeAppearance("normal");
}
```

### 例3: 会話に応じて配置を変える

```csharp
void StartConversation()
{
    // 会話開始: キャラクターを手前に移動
    emily.ApplyPositionPreset(CharacterPositionPreset.PositionPreset.CenterNear);
}

void EndConversation()
{
    // 会話終了: 元の位置に戻す
    emily.ApplyPositionPreset(CharacterPositionPreset.PositionPreset.RightMiddle);
}
```

### 例4: 動的な立ち絵追加

```csharp
// ランタイムで新しいバリエーションを追加
Sprite weddingDressSprite = Resources.Load<Sprite>("Characters/Emily_WeddingDress");
emily.AddAppearanceVariation("wedding_dress", weddingDressSprite);

// 使用
emily.ChangeAppearance("wedding_dress");
```

---

## ⚙️ カスタマイズ

### 深度のスケール係数を変更

CharacterDepth.cs の `GetScaleForDepth()` を編集:
```csharp
private float GetScaleForDepth(DepthLevel depth)
{
    switch (depth)
    {
        case DepthLevel.VeryFar:
            return 0.4f;  // より小さく（デフォルト: 0.5f）
        case DepthLevel.VeryNear:
            return 1.3f;  // より大きく（デフォルト: 1.1f）
        // ...
    }
}
```

### プリセット位置を調整

CharacterPositionPreset.cs の `presets` 配列を編集:
```csharp
new PresetData
{
    preset = PositionPreset.LeftNear,
    anchoredPosition = new Vector2(-500, -250),  // より左下に
    depth = CharacterDepth.DepthLevel.VeryNear   // より手前に
}
```

### 影の色や位置を変更

Inspector の CharacterDepth:
- **Shadow Color**: 色と透明度を調整
- **Shadow Offset**: X, Y のオフセットを調整

---

## 🎨 レイアウト例

### パターン1: 3人会話（手前・中央・奥）
```
        [Alex - Far]
             (小)

  [Emily - Near]   [Sarah - Middle]
      (大)              (中)
```

### パターン2: 主人公視点（全員手前）
```
[Emily]    [Alex]    [Sarah]
 (大)       (大)       (大)
```

### パターン3: 遠近感強調
```
                [Sarah - VeryFar]
                     (極小)

         [Alex - Middle]
              (中)

[Emily - VeryNear]
     (極大)
```

---

## 🐛 トラブルシューティング

### 立ち絵が変わらない
1. Appearance の Variations に目的のバリエーションが登録されているか確認
2. Sprite が null でないか確認
3. ChangeAppearance() の引数が正しいか確認（大文字小文字も一致）

### 影が表示されない
1. CharacterDepth コンポーネントがアタッチされているか確認
2. Enable Shadow がチェックされているか確認
3. Shadow Color の Alpha が 0 になっていないか確認

### プリセット位置が適用されない
1. CharacterPositionPreset がシーンに配置されているか確認
2. Canvas Rect が設定されているか確認
3. Use Position Preset がチェックされているか確認

### 影のSpriteが更新されない
立ち絵を変更した際、影も自動的に更新されますが、手動で更新する場合:
```csharp
CharacterDepth depth = GetComponent<CharacterDepth>();
depth.UpdateShadowSprite(newSprite);
```

---

## 💡 Tips

### 表情バリエーション名の命名規則

推奨される命名規則:
- **normal** - 通常
- **happy** - 笑顔
- **sad** - 悲しい
- **angry** - 怒り
- **surprised** - 驚き
- **tired** - 疲れた
- **embarrassed** - 照れ

衣装:
- **swimsuit** - 水着
- **casual** - カジュアル
- **formal** - フォーマル
- **pajamas** - パジャマ

### パフォーマンス最適化

- バリエーション数は10個以下推奨
- 使わないバリエーションはメモリから解放
- 影は必要ない場合は無効化

### エディタでのプレビュー

CharacterPositionPresetを選択すると、Scene Viewに各プリセット位置がGizmosで表示されます:
- 青: 奥（Far）
- 黄: 中間（Middle）
- 赤: 手前（Near）

---

## 🔗 関連ファイル

- `/Assets/Scripts/Interaction/CharacterAppearance.cs` - 外見管理
- `/Assets/Scripts/Interaction/CharacterDepth.cs` - 深度・影管理
- `/Assets/Scripts/Interaction/CharacterPositionPreset.cs` - 配置プリセット
- `/Assets/Scripts/Interaction/InteractableCharacter.cs` - キャラクター本体

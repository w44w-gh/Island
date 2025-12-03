# 条件付きスケジュールシステム

他のアプリのインストール状況やゲーム内フラグに応じて、キャラクターのスケジュールを動的に変更するシステムです。

---

## 📁 機能

### 1. 他アプリ連携
パッチアプリなど、特定のアプリがインストールされている場合に特別なスケジュールを適用。

### 2. フラグ連動
ゲーム内のイベントフラグに応じてスケジュールを変更。

### 3. 好感度連動
キャラクターとの好感度が一定以上の場合に特別なスケジュールを適用。

### 4. 日数範囲連動
ゲーム開始からの経過日数に応じて自動的にスケジュールを変更。

### 5. 装備アイテム連動
特定のアイテムを装備している場合に特別なスケジュールや会話を適用。

### 6. 複数条件のAND組み合わせ
複数の条件を同時にチェック可能（全て満たす必要あり）。

### 7. 優先度システム
複数の条件を満たす場合、優先度の高い条件を適用。

---

## 🛠️ セットアップ

### 1. ConditionalScheduleLoaderの作成

#### パッチアプリ連携の例
1. Project ウィンドウで右クリック → Create → Island → Conditional Schedule Loader
2. 名前を "EmilyConditionalLoader" に変更
3. Inspector で設定:

**Conditions**:
```
[0] パッチアプリがある場合
  - Condition Type: AppInstalled
  - Required App Package: "com.example.patchapp"
  - Conditional Schedule: EmilyPatchSchedule（パッチアプリ用スケジュール）
  - Schedule Mode: Override（上書きモード）← 重要！
  - Priority: 10（高優先度）

[1] 好感度80以上の場合
  - Condition Type: AffectionLevel
  - Required Affection: 80
  - Target Character Id: "emily"
  - Conditional Schedule: EmilyHighAffectionSchedule
  - Schedule Mode: Override（上書きモード）
  - Priority: 5（中優先度）
```

### 2. パッチアプリ用スケジュールの作成

通常と同じようにCharacterScheduleを作成:

1. Project → 右クリック → Create → Island → Character Schedule
2. 名前を "EmilyPatchSchedule" に変更
3. **Overrideモードの場合、変更したい時間帯だけ定義すればOK！**

```
Actions:
  [0] Morning（朝だけ定義）
    - Position: CenterNear
    - Appearance Variation: "special_outfit"
    - Is Interactable: true
    - Status Message: "パッチアプリ、ありがとう！"
    - Scenario Label: "emily_patch_greeting"

  [1] Noon（昼だけ定義）
    - Position: RightNear
    - Appearance Variation: "special_outfit"
    - Is Interactable: true
    - Status Message: "特別なイベントがあるよ！"
    - Scenario Label: "emily_patch_event"

  （夕方と夜は定義しない → 通常スケジュールがそのまま使われる）
```

**ポイント**: Overrideモードでは、定義していない時間帯は通常スケジュールが自動的に使われます。全時間帯を定義する必要はありません！

### 3. CharacterBehaviorManagerに設定

CharacterBehaviorManager の Inspector:
```
Characters:
  [0] Emily
    - Character: Emily（InteractableCharacter）
    - Schedule: EmilySchedule（基本スケジュール）
    - Conditional Loader: EmilyConditionalLoader ← ここに設定！
```

### 4. AndroidManifest.xmlの設定（重要！）

`Assets/Plugins/Android/AndroidManifest.xml` に以下を追加:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">

    <!-- 他のアプリの存在チェックに必要（Android 11以降） -->
    <queries>
        <!-- チェックしたいアプリのパッケージ名を列挙 -->
        <package android:name="com.example.patchapp" />
        <!-- 複数ある場合は追加 -->
        <package android:name="com.example.anotherapp" />
    </queries>

    <application>
        <!-- アプリの設定 -->
    </application>
</manifest>
```

**注意**: Android 11（API Level 30）以降では、`<queries>` タグでチェックするアプリを明示的に宣言する必要があります。

---

## 📝 使い方

### 基本的な流れ（自動）

1. **ゲーム開始時**: 条件を評価してスケジュールを選択
2. **パッチアプリがある**: 特別なスケジュールが適用される
3. **パッチアプリがない**: 通常のスケジュールが適用される
4. **条件の再評価**: `ReevaluateConditionalSchedules()` を呼ぶ

### エディタでのデバッグ

エディタではAndroid APIが使えないため、デバッグ用の設定を使用:

1. Project で ConditionalScheduleLoader を選択
2. Inspector の `Debug Installed Apps` に:
   ```
   - com.example.patchapp
   ```
   を追加すると、エディタでもパッチアプリが「インストール済み」として扱われる

### 条件の再評価

アプリがインストール/アンインストールされた可能性がある場合:

```csharp
CharacterBehaviorManager manager = FindObjectOfType<CharacterBehaviorManager>();

// 条件を再評価してスケジュールを更新
manager.ReevaluateConditionalSchedules();
```

### デバッグ: 現在のスケジュールを確認

```csharp
// Inspector で右上メニュー → "Show Active Schedules"
// または
manager.ShowActiveSchedules();
// → コンソールに各キャラの適用中スケジュール名が表示される
```

---

## 🎮 実践例

### 例1: パッチアプリ専用コンテンツ（Overrideモード）

```
通常スケジュール（EmilySchedule）:
  早朝: いない
  朝: LeftNear, normal, 「おはよう！」
  昼: CenterMiddle, normal, 「お昼ごはん！」
  夕: RightNear, normal, 「夕方だね」
  深夜: いない

パッチ追加スケジュール（EmilyPatchSchedule）:
  朝: CenterNear, special_outfit, 「パッチアプリありがとう！」
  昼: RightNear, special_outfit, 「特別なイベントがあるよ！」
  （早朝、夕、深夜は定義しない）

ConditionalScheduleLoader:
  Condition Type: AppInstalled
  Required App Package: "com.example.patchapp"
  Conditional Schedule: EmilyPatchSchedule
  Schedule Mode: Override ← ここ重要！
```

#### パッチアプリがある場合の結果
- 早朝: いない（通常のまま）
- **朝: special_outfit, 「パッチアプリありがとう！」（上書き）**
- **昼: special_outfit, 「特別なイベントがあるよ！」（上書き）**
- 夕: normal, 「夕方だね」（通常のまま）
- 深夜: いない（通常のまま）

#### パッチアプリがない場合
- 全て通常スケジュール

### 例2: 好感度による行動変化

#### 好感度80以上
```
ConditionalScheduleLoader:
  Condition Type: AffectionLevel
  Required Affection: 80
  Target Character Id: "emily"
  Conditional Schedule: EmilyHighAffectionSchedule
```

好感度が高いと:
- 常に話しかけやすい位置（手前）にいる
- 笑顔の表情が多い
- 特別な会話が発生

### 例3: フラグによるスケジュール変更

#### 特定イベント後
```
ConditionalScheduleLoader:
  Condition Type: FlagEnabled
  Required Flag Id: "emily_beach_event_completed"
  Conditional Schedule: EmilyPostBeachSchedule
```

ビーチイベント後は:
- ビーチ関連の話題が増える
- 水着のバリエーションが追加
- ビーチに行く時間帯が増える

### 例4: 日数範囲による会話変化

#### ゲーム序盤（1～3日目）
```
ConditionalScheduleLoader:
  Check Day Range: true
  Min Day: 1
  Max Day: 3
  Conditional Schedule: EmilyEarlyDaysSchedule
```

1～3日目は:
- 島の説明をしてくれる
- 基本的なチュートリアル的な会話
- まだ警戒している様子

#### 中盤（4～7日目）
```
ConditionalScheduleLoader:
  Check Day Range: true
  Min Day: 4
  Max Day: 7
  Conditional Schedule: EmilyMiddleDaysSchedule
```

4～7日目は:
- 打ち解けてきた会話
- 個人的な話題が増える

### 例5: 装備アイテムによる特別反応

#### 特定アクセサリー装備時
```
ConditionalScheduleLoader:
  Check Equipment: true
  Required Equipment Id: "lucky_charm"
  Conditional Schedule: EmilyLuckyCharmSchedule
```

ラッキーチャームを装備していると:
- 「それ、可愛いね！」という反応
- 特別な会話が発生
- 好感度が上がりやすくなる

### 例6: 複数条件の優先度（Override複数適用）

```
Conditions:
  [0] パッチアプリ
    - Condition Type: AppInstalled
    - Required App Package: "com.example.patchapp"
    - Schedule: EmilyPatchSchedule（朝と昼だけ定義）
    - Schedule Mode: Override
    - Priority: 10

  [1] 好感度80以上
    - Condition Type: AffectionLevel
    - Required Affection: 80
    - Schedule: EmilyHighAffectionSchedule（夕だけ定義）
    - Schedule Mode: Override
    - Priority: 5
```

**パッチアプリあり + 好感度80以上の場合**:
1. 通常スケジュールから開始
2. パッチ条件満たす → 朝と昼を上書き
3. 好感度条件満たす → 夕を上書き

**結果**:
- 早朝: 通常のまま
- 朝: パッチスケジュール（上書き）
- 昼: パッチスケジュール（上書き）
- 夕: 好感度スケジュール（上書き）
- 深夜: 通常のまま

**ポイント**: Overrideモードは複数の条件を重ねて適用できます！

---

## ⚙️ スケジュールモード

### Override（上書き）← おすすめ！
- **動作**: 定義された時間帯のみ上書き、残りは通常スケジュール
- **メリット**: 一部の時間帯だけ変更できる、重複が少ない、管理しやすい
- **使用例**: パッチアプリで朝と昼だけ特別な衣装、夕方と夜は通常通り

**例**:
```
通常スケジュール:
  朝: normal, 昼: normal, 夕: normal, 夜: いない

パッチ追加（Override）:
  朝: special_outfit（朝だけ定義）

結果:
  朝: special_outfit（上書き）
  昼: normal（通常のまま）
  夕: normal（通常のまま）
  夜: いない（通常のまま）
```

### Replace（完全置き換え）
- **動作**: 条件を満たしたら全時間帯を別スケジュールに置き換え
- **メリット**: 全く別の行動パターンを定義できる
- **使用例**: イベント期間中は完全に別のスケジュールを使用

**例**:
```
通常スケジュール:
  朝: normal, 昼: normal, 夕: normal, 夜: いない

イベント用（Replace）:
  朝: event_outfit, 昼: event_outfit, 夕: event_outfit, 夜: event_outfit

結果:
  朝: event_outfit（完全置き換え）
  昼: event_outfit
  夕: event_outfit
  夜: event_outfit（全て定義する必要がある）
```

---

## ⚙️ 条件タイプ

### AppInstalled（アプリインストール）
- **Required App Package**: パッケージ名（例: "com.example.patchapp"）
- **使用例**: パッチアプリ連携、コラボアプリ連携

### FlagEnabled（フラグ）
- **Required Flag Id**: フラグID（例: "beach_event_completed"）
- **使用例**: イベント後の行動変化、ストーリー進行による変化

### AffectionLevel（好感度）
- **Required Affection**: 必要な好感度（0～100）
- **Target Character Id**: 対象キャラクターID
- **使用例**: 好感度によるデレ度変化、特別イベント解放

### DayRange（日数範囲）
- **Min Day**: 最小日数（0 = 無制限）
- **Max Day**: 最大日数（0 = 無制限）
- **使用例**: ゲーム序盤限定の会話、特定期間のみのスケジュール

### Equipment（装備）
- **Required Equipment Id**: 必要なアイテムのID
- **使用例**: 特定アイテム装備時の特別な反応、アイテム関連イベント

### Custom（カスタム）
- 拡張ポイント
- ConditionalScheduleLoader.cs の `EvaluateCondition()` を編集して独自条件を追加可能

---

## 🔧 AndroidManifest.xml 詳細

### Android 11以降の制限

Android 11（API Level 30）以降では、セキュリティ強化のため、他のアプリの情報を取得するには明示的な宣言が必要です。

### 完全な例

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest
    xmlns:android="http://schemas.android.com/apk/res/android"
    package="com.yourcompany.islandgame">

    <!-- 他のアプリをチェックするための宣言 -->
    <queries>
        <!-- パッチアプリ -->
        <package android:name="com.example.patchapp" />

        <!-- その他の連携アプリ -->
        <package android:name="com.example.collabapp" />
    </queries>

    <application
        android:allowBackup="true"
        android:icon="@drawable/app_icon"
        android:label="@string/app_name">

        <activity android:name="com.unity3d.player.UnityPlayerActivity">
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
        </activity>
    </application>
</manifest>
```

### 配置場所

```
Assets/
  └─ Plugins/
      └─ Android/
          └─ AndroidManifest.xml
```

ファイルがない場合は新規作成してください。

---

## 🐛 トラブルシューティング

### パッチアプリがあるのに特別スケジュールにならない

#### 1. AndroidManifest.xmlを確認
```xml
<queries>
    <package android:name="com.example.patchapp" />
</queries>
```
が追加されているか確認。

#### 2. パッケージ名を確認
正しいパッケージ名を使用しているか確認:
```csharp
// コンソールに出力
Debug.Log(AndroidPackageChecker.IsPackageInstalled("com.example.patchapp"));
```

#### 3. 優先度を確認
他の条件の優先度が高くないか確認。優先度は高い順に評価される。

### エディタでテストできない

ConditionalScheduleLoader の `Debug Installed Apps` を使用:

1. Project で ConditionalScheduleLoader を選択
2. Inspector で Debug Installed Apps に "com.example.patchapp" を追加
3. プレイモードで再生

### 条件が変わっても反映されない

条件の再評価を呼ぶ:
```csharp
manager.ReevaluateConditionalSchedules();
```

### ConditionalLoaderが設定されているのに無視される

CharacterBehaviorManager の Inspector で:
- Conditional Loader フィールドに正しく設定されているか確認
- Conditional Schedule が null でないか確認

---

## 💡 Tips

### パッケージ名の調べ方

#### 実機での確認
```bash
# adbで実機のアプリ一覧を取得
adb shell pm list packages

# 特定のアプリを探す
adb shell pm list packages | grep patch
```

#### Google Playでの確認
Google PlayのアプリページURLから取得:
```
https://play.google.com/store/apps/details?id=com.example.patchapp
                                                ^^^^^^^^^^^^^^^^^^^^
                                                パッケージ名
```

### 複数のパッチアプリ対応

```
Conditions:
  [0] パッチアプリA
    - Required App Package: "com.example.patcha"
    - Conditional Schedule: EmilyPatchASchedule
    - Priority: 10

  [1] パッチアプリB
    - Required App Package: "com.example.patchb"
    - Conditional Schedule: EmilyPatchBSchedule
    - Priority: 10
```

### エディタとビルドの違い

- **エディタ**: Debug Installed Apps の設定を使用
- **実機ビルド**: 実際のインストール状況をチェック

---

## 🔗 関連ファイル

- `/Assets/Scripts/Managers/AndroidPackageChecker.cs` - アプリ存在チェック（既存）
- `/Assets/Scripts/Behavior/ConditionalScheduleLoader.cs` - 条件付きローダー
- `/Assets/Scripts/Behavior/CharacterBehaviorManager.cs` - 行動管理マネージャー
- `/Assets/Scripts/Behavior/CharacterSchedule.cs` - スケジュール定義
- `/Assets/Plugins/Android/AndroidManifest.xml` - Android設定

---

## 🎯 複数条件の組み合わせ（AND条件）

### AND条件の使い方

複数の条件を同時にチェック可能（全て満たす必要あり）:

```
ConditionalScheduleLoader:
  Check App Installed: true      ← チェック
  Required App Package: "com.example.patchapp"

  Check Affection Level: true    ← チェック
  Required Affection: 80
  Target Character Id: "emily"

  Check Day Range: true          ← チェック
  Min Day: 5
  Max Day: 0  (無制限)

  Conditional Schedule: EmilySpecialSchedule
```

この場合、以下の**全ての条件**を満たす必要があります:
- パッチアプリがインストールされている
- エミリーの好感度が80以上
- ゲーム開始から5日目以降

### 実例: 特別なイベント解放

```
ConditionalScheduleLoader:
  Check Equipment: true
  Required Equipment Id: "engagement_ring"

  Check Affection Level: true
  Required Affection: 100
  Target Character Id: "emily"

  Check Flag Enabled: true
  Required Flag Id: "beach_event_completed"

  Conditional Schedule: EmilyProposalSchedule
  Schedule Mode: Replace
  Priority: 100
```

条件:
- 婚約指輪を装備している
- 好感度が最大（100）
- ビーチイベントをクリア済み

→ 全て満たすとプロポーザルイベントのスケジュールに切り替わる

## 📚 今後の拡張案

### 時間帯連動条件
特定の時間帯のみ特別スケジュールを適用

### 天候連動条件
雨の日は別のスケジュールを使用

### アプリバージョンチェック
パッチアプリのバージョンに応じて異なるスケジュールを適用

### OR条件のサポート
「パッチアプリ OR コラボアプリ」など、複数条件のいずれかを満たせばOK

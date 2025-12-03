# EventReward システム

リアルイベント会場での限定配布システム。WiFiのSSID検出により、イベント会場にいるプレイヤーに限定アイテムを配布します。

**Firebase RemoteConfig対応**: イベント報酬データはサーバーサイドで管理され、アプリ更新なしに追加・変更が可能です。

## 📁 構成

- **EventReward.cs** - 報酬定義とデータベース（RemoteConfig対応）
- **WiFiScanManager.cs** - WiFiスキャン機能（Android専用）
- **EventRewardManager.cs** - イベント報酬管理クラス（Singleton）
- **RemoteConfigManager.cs** - Firebase RemoteConfig管理クラス（Singleton）

---

## 🎮 使い方

### 基本的な流れ

1. **プレイヤーがイベント会場に到着**
2. **ゲーム内で「イベント報酬」ボタンをタップ**
3. **WiFiスキャン実行**
   - WiFiがOFFの場合 → 設定画面を開くダイアログ表示
   - 位置情報許可がない場合 → 許可要求ダイアログ表示
4. **特定のSSIDを検出 → 報酬付与**
5. **アイテムがインベントリに追加される**

### コードからの呼び出し

```csharp
// イベントスキャン開始（ボタン押下時など）
EventRewardManager.Instance.StartEventScan();

// 報酬取得時のイベント登録
EventRewardManager.Instance.OnRewardClaimed += (reward) =>
{
    Debug.Log($"報酬取得: {reward.eventName}");
    // UIで報酬獲得演出を表示
};

// スキャン完了時のイベント登録
EventRewardManager.Instance.OnScanComplete += (message) =>
{
    Debug.Log($"スキャン完了: {message}");
    // 「報酬を取得しました」または「報酬が見つかりませんでした」
};

// エラー時のイベント登録
EventRewardManager.Instance.OnError += (errorMessage) =>
{
    Debug.LogError($"エラー: {errorMessage}");
    // エラーダイアログを表示
};
```

---

## 🎁 イベント報酬の追加方法

### Firebase RemoteConfigで追加（推奨）

Firebase Consoleから動的に追加できます。アプリの更新不要です。

1. **Firebase Consoleにアクセス**
   - プロジェクトを選択 → Remote Config

2. **パラメータを編集**
   - キー: `event_rewards`
   - 値: JSON形式（下記のフォーマット）

```json
{
  "events": [
    {
      "eventId": "event_animejapan2025",
      "eventName": "アニメジャパン2025",
      "ssid": "AnimeJapan2025_Island",
      "description": "アニメジャパン2025会場限定配布",
      "rewards": [
        { "itemId": "wood", "quantity": 100 },
        { "itemId": "stone", "quantity": 100 },
        { "itemId": "fish", "quantity": 50 }
      ]
    },
    {
      "eventId": "event_example2025",
      "eventName": "サンプルイベント2025",
      "ssid": "ExampleEvent2025_Island",
      "description": "サンプルイベント会場限定配布",
      "rewards": [
        { "itemId": "wood", "quantity": 200 },
        { "itemId": "stone", "quantity": 150 },
        { "itemId": "berry", "quantity": 100 }
      ]
    }
  ]
}
```

3. **変更を公開**
   - 「変更を公開」ボタンをクリック
   - ユーザーがアプリを起動すると自動的に新しいイベントが反映されます

### コードで追加（デフォルト値）

`EventReward.cs`の`LoadFromHardcodedData()`内で定義します。RemoteConfigが利用できない場合のフォールバックとして使用されます。

```csharp
// EventReward.cs: LoadFromHardcodedData()
var newEvent = new EventReward(
    "event_example2025",
    "サンプルイベント2025",
    "ExampleEvent2025_Island",
    "サンプルイベント会場限定配布"
);
newEvent.AddReward("wood", 200);
newEvent.AddReward("stone", 150);
newEvent.AddReward("berry", 100);
eventRewards.Add(newEvent);
```

---

## ⚙️ システムの仕組み

### 1. WiFi状態チェック

```csharp
// WiFiScanManager.cs
public bool IsWiFiEnabled()
```

- AndroidのWiFiManagerを使用してWiFiの状態を取得
- OFFの場合は設定画面を開くダイアログを表示（自動でONにはしない）

### 2. 位置情報パーミッションチェック

```csharp
// WiFiScanManager.cs
public bool HasLocationPermission()
public void RequestLocationPermission(Action<bool> callback)
```

- Android 6.0以降、WiFiスキャンには位置情報パーミッションが必須
- パーミッションがない場合のみ要求ダイアログを表示
- ユーザーが許可/拒否したらコールバックで通知

### 3. WiFiスキャン実行

```csharp
// WiFiScanManager.cs
public void ScanWiFiNetworks()
```

- AndroidのWiFiManager.startScan()を使用
- スキャン結果からSSIDリストを取得
- エディタでは仮のテストデータを返す

### 4. SSID照合と報酬付与

```csharp
// EventRewardManager.cs
private void OnWiFiScanComplete(List<string> ssidList)
```

- 検出されたSSIDをEventRewardDatabaseで照合
- 既に取得済みかチェック
- 未取得の場合のみ報酬を付与
- 報酬はGameState.Inventoryに追加

### 5. 保存と復元

- **保存**: `GameState.ToSaveData()` → SaveDataに含まれる
- **復元**: `GameState.LoadFromSaveData()` → EventRewardManagerに読み込み
- 取得済みイベントIDはHashSetで管理（重複取得を防止）

---

## 🛠️ セットアップ

### 1. Firebase SDKのインストール

1. **Firebase Unity SDKをダウンロード**
   - https://firebase.google.com/download/unity

2. **必要なパッケージをインポート**
   - `FirebaseRemoteConfig.unitypackage`
   - `FirebaseAnalytics.unitypackage`（依存関係）

3. **google-services.jsonを配置**
   - Firebase Consoleからダウンロード
   - `Assets/Plugins/Android/` に配置

### 2. Firebase RemoteConfigの設定

1. **Firebase Consoleで設定**
   - プロジェクトを選択 → Remote Config
   - 新しいパラメータを追加:
     - キー: `event_rewards`
     - 値: デフォルトJSON（下記参照）

```json
{
  "events": [
    {
      "eventId": "event_animejapan2025",
      "eventName": "アニメジャパン2025",
      "ssid": "AnimeJapan2025_Island",
      "description": "アニメジャパン2025会場限定配布",
      "rewards": [
        { "itemId": "wood", "quantity": 100 },
        { "itemId": "stone", "quantity": 100 },
        { "itemId": "fish", "quantity": 50 }
      ]
    },
    {
      "eventId": "event_comiket2025",
      "eventName": "コミケ2025",
      "ssid": "Comiket2025_Island",
      "description": "コミケ2025会場限定配布",
      "rewards": [
        { "itemId": "berry", "quantity": 100 },
        { "itemId": "coconut", "quantity": 50 }
      ]
    }
  ]
}
```

2. **変更を公開**

### 3. AndroidManifest.xmlに権限を追加

`Assets/Plugins/Android/AndroidManifest.xml`に以下を追加:

```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
<uses-permission android:name="android.permission.CHANGE_WIFI_STATE" />
<uses-permission android:name="android.permission.INTERNET" />
```

### 4. Unity Editor での確認

**WiFiスキャン**: エディタでは実際のWiFiスキャンは動作しません。代わりにテストデータを返します:

```csharp
// WiFiScanManager.cs: エディタ用テストデータ
List<string> testSSIDs = new List<string>
{
    "AnimeJapan2025_Island",  // アニメジャパン2025
    "TestWiFi_01",            // テスト用
    "HomeNetwork"             // テスト用
};
```

**RemoteConfig**: エディタではFirebase RemoteConfigは動作しません。代わりにハードコードされたデフォルト値を使用します。

### 5. UI実装

以下のダイアログを実装してください（現在はDebug.Logで代用）:

#### WiFi無効ダイアログ
```csharp
// EventRewardManager.cs:116-124
private void ShowWiFiDisabledDialog()
{
    // TODO: UI実装時に差し替え
    // 「WiFiが無効です。設定画面を開きますか？」
}
```

#### パーミッション要求ダイアログ
```csharp
// EventRewardManager.cs:129-148
private void ShowPermissionRequestDialog()
{
    // TODO: UI実装時に差し替え
    // 「イベント報酬を受け取るには位置情報の許可が必要です」
}
```

#### パーミッション拒否ダイアログ
```csharp
// EventRewardManager.cs:153-158
private void ShowPermissionDeniedDialog()
{
    // TODO: UI実装時に差し替え
    // 「位置情報の許可が拒否されました。設定から許可してください。」
}
```

#### 報酬取得通知
```csharp
// OnRewardClaimedイベントで実装
EventRewardManager.Instance.OnRewardClaimed += (reward) =>
{
    // TODO: 報酬取得演出UI
    // 「{reward.eventName}の報酬を取得しました！」
};
```

---

## 📝 注意事項

### 1. Android専用機能
- WiFiスキャンはAndroid専用です
- Unity Editorではテストデータを返します
- iOS版では別の実装が必要です

### 2. セキュリティ
- WiFi SSIDは簡単に偽装できるため、高価なアイテムは配布しないでください
- あくまで「会場の雰囲気を楽しむ」程度の報酬にとどめることを推奨

### 3. パーミッション
- Android 6.0以降、WiFiスキャンには位置情報パーミッションが必須
- ユーザーが拒否した場合、この機能は使用できません
- 設定画面から手動で許可する必要があります

### 4. WiFi設定
- WiFiがOFFの場合、設定画面を開くダイアログを表示
- 自動でWiFiをONにはしません（ユーザーの意思を尊重）

### 5. 重複取得の防止
- 取得済みイベントIDはHashSetで管理
- 同じイベントの報酬は1回のみ取得可能
- セーブデータに保存され、アプリ再起動後も保持

---

## 🐛 トラブルシューティング

### RemoteConfigからデータが取得できない
1. Firebase SDKが正しくインポートされているか確認
2. `google-services.json`が正しく配置されているか確認
3. インターネット接続があるか確認
4. Firebase Consoleで`event_rewards`パラメータが設定されているか確認
5. 変更を公開したか確認

→ エラー時はデフォルト値を使用するため、ゲームは継続できます

### WiFiスキャンが動作しない
1. AndroidManifest.xmlに権限が追加されているか確認
2. 位置情報パーミッションが許可されているか確認
3. WiFiがONになっているか確認

### 報酬が付与されない
1. SSIDが正しく登録されているか確認
   - Firebase Console → Remote Config → `event_rewards`
   - または`EventReward.cs`の`LoadFromHardcodedData()`
2. 既に取得済みでないか確認（`IsEventClaimed(eventId)`）
3. GameStateとInventoryが正しく動作しているか確認
4. RemoteConfigが正しく初期化されているか確認（ログを確認）

### エディタでテストしたい
```csharp
// WiFiScanManager.cs: 182-188行目
// エディタ用テストデータにイベントSSIDを追加
List<string> testSSIDs = new List<string>
{
    "AnimeJapan2025_Island",  // 既存
    "YourEventSSID_Island",   // 追加したイベントのSSID
};
```

---

## 🔄 RemoteConfigの動作フロー

```
[アプリ起動]
    ↓
[GameManager] InitializeTimeSync()
    ↓
[RemoteConfigManager] InitializeAndFetch()
    ↓
Firebase初期化 → RemoteConfigフェッチ → アクティベート
    ↓
[GameManager] OnConfigFetched コールバック
    ↓
[EventRewardDatabase] Reload()
    ↓
RemoteConfigからJSONを読み込み
    ↓
イベント報酬データ登録完了
```

**注意点**:
- RemoteConfigの初期化は非同期です
- 初期化完了前にEventRewardDatabaseがアクセスされた場合、デフォルト値を使用します
- ネットワークエラー時もデフォルト値にフォールバックします

---

## 📚 API リファレンス

### RemoteConfigManager

#### メソッド
- `InitializeAndFetch()` - RemoteConfigを初期化してデータをフェッチ
- `GetEventRewardsJson()` - イベント報酬のJSON文字列を取得
- `GetString(string key, string defaultValue)` - 文字列値を取得
- `GetLong(string key, long defaultValue)` - 整数値を取得
- `GetBool(string key, bool defaultValue)` - 真偽値を取得
- `GetDouble(string key, double defaultValue)` - 浮動小数点値を取得

#### プロパティ
- `IsInitialized` - 初期化完了フラグ
- `IsFetching` - フェッチ中フラグ

#### イベント
- `OnConfigFetched` - 設定取得完了時
- `OnConfigError` - エラー時（引数: string エラーメッセージ）

#### 定数
- `KEY_EVENT_REWARDS` - イベント報酬のRemoteConfigキー

### EventRewardDatabase

#### メソッド
- `Initialize()` - データベース初期化（RemoteConfigから読み込み）
- `Reload()` - データベースを再初期化（RemoteConfig更新時）
- `GetAllRewards()` - 全イベント報酬を取得
- `FindBySSID(string ssid)` - SSIDからイベント報酬を検索
- `GetById(string eventId)` - イベントIDからイベント報酬を取得

### EventRewardManager

#### メソッド
- `StartEventScan()` - イベントスキャン開始（ボタン押下時に呼び出し）
- `LoadClaimedEvents(List<string> eventIds)` - 取得済みイベントIDをロード
- `GetClaimedEventIds()` - 取得済みイベントIDを取得
- `IsEventClaimed(string eventId)` - イベントが取得済みかチェック
- `GetAvailableEvents()` - 取得可能なイベント一覧を取得
- `GetClaimedEvents()` - 取得済みイベント一覧を取得

#### イベント
- `OnRewardClaimed` - 報酬取得時（引数: EventReward）
- `OnScanComplete` - スキャン完了時（引数: string メッセージ）
- `OnError` - エラー時（引数: string エラーメッセージ）

### WiFiScanManager

#### メソッド
- `IsWiFiEnabled()` - WiFiが有効かチェック
- `HasLocationPermission()` - 位置情報パーミッションがあるかチェック
- `RequestLocationPermission(Action<bool> callback)` - 位置情報パーミッションを要求
- `OpenWiFiSettings()` - WiFi設定画面を開く
- `ScanWiFiNetworks()` - WiFiスキャンを実行

#### コールバック
- `OnScanComplete` - スキャン完了時（引数: List<string> SSIDリスト）
- `OnScanError` - スキャンエラー時（引数: string エラーメッセージ）

### EventRewardDatabase

#### メソッド
- `Initialize()` - データベース初期化
- `GetAllRewards()` - 全イベント報酬を取得
- `FindBySSID(string ssid)` - SSIDからイベント報酬を検索
- `GetById(string eventId)` - イベントIDからイベント報酬を取得

---

## 🎉 実装例：アニメジャパン2025

```csharp
// EventReward.cs: 73-83行目
var animeJapan = new EventReward(
    "event_animejapan2025",
    "アニメジャパン2025",
    "AnimeJapan2025_Island",
    "アニメジャパン2025会場限定配布"
);
animeJapan.AddReward("wood", 100);
animeJapan.AddReward("stone", 100);
animeJapan.AddReward("fish", 50);
eventRewards.Add(animeJapan);
```

**会場でのWiFi SSID**: `AnimeJapan2025_Island`

**配布アイテム**:
- 木材 x100
- 石 x100
- 魚 x50

プレイヤーがアニメジャパン2025会場でこのWiFiを検出すると、上記のアイテムが自動的にインベントリに追加されます。

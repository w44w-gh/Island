# DayTransition システム

ムジュラの仮面風の日付・時間帯・天候の遷移演出を表示するシステム。

## 📁 構成

- **DayTransitionUI.cs** - UI表示とアニメーション
- **DayTransitionManager.cs** - 管理クラス（Singleton）

---

## 🎮 使い方

### 基本的な呼び出し

```csharp
// GameStateから自動取得
DayTransitionManager.Instance.Show(gameState);

// 個別に指定
DayTransitionManager.Instance.Show(3, TimeOfDay.Morning, WeatherType.Sunny);
// → "3日目 朝 - 晴れ" と表示
```

### 初期化処理を待つ呼び出し（起動時・復帰時）

```csharp
// DayTransitionUIを表示しながら初期化処理を実行
DayTransitionManager.Instance.ShowWithInitialization(gameState, async () =>
{
    // NTP時刻同期
    await NTPTimeManager.Instance.Initialize();

    // Firebase RemoteConfig取得
    await InitializeRemoteConfigAsync();

    // その他の初期化処理...
});
```

**動作フロー**:
1. DayTransitionUIがフェードイン
2. 初期化処理を実行（非同期）
3. 完了後にフェードアウト
4. ゲーム開始

**メリット**:
- ロード時間を意味のあるコンテンツで埋められる
- NTP同期やRemoteConfig取得の待ち時間が自然
- ユーザー体験がスムーズ

### 推奨される呼び出し箇所

#### 1. ゲーム起動時（GameManager.InitializeTimeSync）

**現在の実装（推奨）**: 初期化処理を待つ
```csharp
private void ShowInitializationScreen()
{
    DayTransitionManager.Instance.ShowWithInitialization(
        gameState.CurrentDay,
        gameState.Time.CurrentTimeOfDay,
        gameState.CurrentWeather,
        async () =>
        {
            // NTP時刻同期
            await NTPTimeManager.Instance.Initialize();
            globalGameTime?.Capture();

            // オフライン経過時間を処理
            if (savedTimestamp > 0)
            {
                gameState.ProcessOfflineTime(savedTimestamp);
            }

            // Firebase RemoteConfigを初期化とフェッチ
            await InitializeRemoteConfigAsync();
        }
    );
}
```

**シンプルな表示のみ**:
```csharp
void Start()
{
    // ゲーム開始時に現在の状態を表示
    DayTransitionManager.Instance.Show(gameState);
}
```

#### 2. 日付変更時（GameState.AdvanceDay）
```csharp
public void AdvanceDay()
{
    currentDay++;
    weather.AdvanceToNextDay();

    // 日付遷移演出を表示
    DayTransitionManager.Instance.Show(this);

    Debug.Log($"日付更新: {currentDay}日目, 天候: {CurrentWeather.ToJapaneseString()}");
    OnDayChanged?.Invoke(currentDay, CurrentWeather);
}
```

#### 3. オフライン復帰時（GameManager.RefreshTimeWithFade）

**現在の実装（推奨）**: 初期化処理を待つ
```csharp
private void RefreshTimeWithFade()
{
    DayTransitionManager.Instance.ShowWithInitialization(
        gameState,
        async () =>
        {
            // NTP同期実行
            bool success = await NTPTimeManager.Instance.RefreshServerTimeAsync();

            if (success)
            {
                globalGameTime?.Capture();

                // オフライン経過時間を処理
                if (savedTimestamp > 0)
                {
                    gameState.ProcessOfflineTime(savedTimestamp);
                }
            }

            // RemoteConfigも再フェッチ
            await InitializeRemoteConfigAsync();
        }
    );
}
```

**シンプルな表示のみ**:
```csharp
public void ProcessOfflineTime(long savedTimestamp)
{
    // ... オフライン処理 ...

    // 復帰時に現在の状態を表示
    DayTransitionManager.Instance.Show(this);
}
```

#### 4. 時間帯変更時（任意）
```csharp
// 時間帯が変わったタイミングで表示
DayTransitionManager.Instance.Show(gameState);
```

---

## 🛠️ セットアップ

### 1. Unity エディタでの設定

#### Canvasの作成
1. Hierarchy で右クリック → UI → Canvas
2. Canvas の設定:
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920 x 1080

#### DayTransitionUI の作成
1. Canvas の下に空の GameObject を作成 → "DayTransitionUI"
2. DayTransitionUI にコンポーネントを追加:
   - Canvas Group
   - DayTransitionUI.cs (スクリプト)

3. Background Panel の作成:
   - DayTransitionUI の下に UI → Panel を作成
   - 名前: "Background"
   - 色: 黒 (R:0, G:0, B:0, A:200)

4. Day Text の作成:
   - Background の下に UI → Text を作成
   - 名前: "DayText"
   - 設定:
     - Text: "3日目" (サンプル)
     - Font Size: 80
     - Alignment: Center
     - Color: 白
     - Anchor: Center

5. TimeWeather Text の作成:
   - Background の下に UI → Text を作成
   - 名前: "TimeWeatherText"
   - 設定:
     - Text: "朝 - 晴れ" (サンプル)
     - Font Size: 50
     - Alignment: Center
     - Color: 白
     - Anchor: Center
     - Position: DayText の下に配置

#### DayTransitionUI スクリプトの設定
- Canvas Group: DayTransitionUI の Canvas Group をドラッグ
- Day Text: DayText をドラッグ
- Time Weather Text: TimeWeatherText をドラッグ
- Animation Settings:
  - Fade In Duration: 0.5
  - Display Duration: 2.0
  - Fade Out Duration: 0.5

#### Prefab化
1. DayTransitionUI を Project ウィンドウにドラッグしてPrefab化
2. 保存場所: `Assets/Prefabs/UI/DayTransitionUI.prefab`

### 2. DayTransitionManager の設定

#### シーンに配置（推奨）
1. Hierarchy で空の GameObject を作成 → "DayTransitionManager"
2. DayTransitionManager.cs をアタッチ
3. Inspector で設定:
   - Transition UI Prefab: 作成した DayTransitionUI Prefab をドラッグ

#### または、コードから自動生成
何も設定しなくても、最初の呼び出し時に自動生成されます。
（ただし Prefab は手動設定が必要）

---

## ⚙️ カスタマイズ

### アニメーション速度の変更
DayTransitionUI の Inspector で調整:
- `Fade In Duration`: フェードイン時間（デフォルト: 0.5秒）
- `Display Duration`: 表示時間（デフォルト: 2.0秒）
- `Fade Out Duration`: フェードアウト時間（デフォルト: 0.5秒）

### テキストスタイルの変更
- DayText / TimeWeatherText の Font, Size, Color を変更
- 日本語フォントを使用する場合は、Font を変更

### 背景の変更
- Background Panel の色や透明度を変更
- 画像を使用する場合は、Panel に Image コンポーネントを追加

---

## 🎨 レイアウト例

```
━━━━━━━━━━━━━━━━━━━━━━━━━
           3日目
         朝 - 晴れ
━━━━━━━━━━━━━━━━━━━━━━━━━
```

中央に大きく日付、その下に時間帯と天候を表示。
背景は半透明の黒で、画面全体を覆う。

---

## 📝 注意事項

1. **DontDestroyOnLoad**
   - DayTransitionManager と DayTransitionUI は DontDestroyOnLoad で永続化されます
   - シーン遷移しても消えません

2. **Singleton**
   - DayTransitionManager.Instance でどこからでもアクセス可能
   - 複数インスタンスは自動的に削除されます

3. **表示中の重複呼び出し**
   - 既に表示中に再度呼び出すと、前の表示を中断して新しい表示に切り替わります

4. **UI階層**
   - Canvas の Sort Order を調整して、他のUIより前面に表示されるようにしてください

---

## 🐛 トラブルシューティング

### 表示されない場合
1. DayTransitionManager.IsReady() で UI が設定されているか確認
2. Canvas が Scene に存在するか確認
3. Canvas の Sort Order が他の UI より高いか確認

### テキストが表示されない場合
1. DayText / TimeWeatherText が設定されているか確認
2. Font が設定されているか確認（日本語の場合は日本語フォント）

### アニメーションがおかしい場合
1. Canvas Group が設定されているか確認
2. Animation Settings の値を確認

---

## 📚 参考

### TimeOfDay の日本語変換
```csharp
public static string ToJapaneseString(this TimeOfDay timeOfDay)
{
    switch (timeOfDay)
    {
        case TimeOfDay.EarlyMorning: return "早朝";
        case TimeOfDay.Morning: return "朝";
        case TimeOfDay.Noon: return "昼";
        case TimeOfDay.Evening: return "夜";
        case TimeOfDay.Midnight: return "深夜";
        default: return "";
    }
}
```

### WeatherType の日本語変換
```csharp
public static string ToJapaneseString(this WeatherType weather)
{
    switch (weather)
    {
        case WeatherType.Sunny: return "晴れ";
        case WeatherType.Cloudy: return "曇り";
        case WeatherType.Rainy: return "雨";
        case WeatherType.Stormy: return "嵐";
        default: return "";
    }
}
```

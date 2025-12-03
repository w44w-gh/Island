# AudioManager

BGMとSEを一元管理するシングルトンマネージャー。シーン間で永続化され、フェードイン/アウト、ボリューム管理などの機能を提供します。

---

## 📁 主な機能

### BGM管理
- **フェードイン/アウト** - スムーズな曲の切り替え
- **ループ再生** - 自動ループ
- **重複防止** - 同じBGMが再生中の場合はスキップ
- **ボリューム管理** - PlayerPrefsで永続化

### SE管理
- **ワンショット再生** - 効果音の即時再生
- **ボリューム倍率** - SE毎にボリューム調整可能
- **ボリューム管理** - PlayerPrefsで永続化

### その他
- **DontDestroyOnLoad** - シーン遷移しても消えない
- **Singleton** - どこからでもアクセス可能
- **Inspector設定** - AudioClipDataでエディタから設定

---

## 🎮 使い方

### BGMを再生
```csharp
// 2秒かけてフェードイン
AudioManager.Instance.PlayBGM("title_theme", 2f);

// デフォルトのフェード時間で再生（1秒）
AudioManager.Instance.PlayBGM("game_morning");
```

### BGMを停止
```csharp
// 1秒かけてフェードアウト
AudioManager.Instance.StopBGM(1f);
```

### SEを再生
```csharp
// 通常ボリュームで再生
AudioManager.Instance.PlaySE("button_tap");

// ボリューム50%で再生
AudioManager.Instance.PlaySE("heal", 0.5f);
```

### ボリューム設定
```csharp
// BGMボリュームを80%に設定
AudioManager.Instance.SetBGMVolume(0.8f);

// SEボリュームを60%に設定
AudioManager.Instance.SetSEVolume(0.6f);

// 現在のボリュームを取得
float bgmVolume = AudioManager.Instance.GetBGMVolume();
float seVolume = AudioManager.Instance.GetSEVolume();
```

### ステータス確認
```csharp
// 現在再生中のBGM名を取得
string currentBGM = AudioManager.Instance.GetCurrentBGM();

// BGMが再生中かチェック
bool isPlaying = AudioManager.Instance.IsBGMPlaying();
```

---

## 🛠️ Unity Editorでのセットアップ

### 1. AudioManagerの作成（自動生成される）
AudioManager.Instanceを初めて呼び出すと、自動的にGameObjectが生成されます。

または手動で作成する場合：
1. Hierarchy で空の GameObject を作成 → "AudioManager"
2. AudioManager.cs をアタッチ
3. DontDestroyOnLoad が自動的に設定されます

### 2. AudioClipの設定

#### BGM Clipsの設定
Inspector の `Bgm Clips` 配列にBGMを追加：

| Name (識別用) | Clip (AudioClip) |
|--------------|------------------|
| title_theme | TitleTheme.mp3 |
| game_early_morning | GameEarlyMorning.mp3 |
| game_morning | GameMorning.mp3 |
| game_noon | GameNoon.mp3 |
| game_evening | GameEvening.mp3 |
| game_midnight | GameMidnight.mp3 |
| game_default | GameDefault.mp3 |

#### SE Clipsの設定
Inspector の `Se Clips` 配列にSEを追加：

| Name (識別用) | Clip (AudioClip) |
|--------------|------------------|
| button_tap | ButtonTap.wav |
| heal | Heal.wav |
| damage | Damage.wav |
| game_over | GameOver.wav |

### 3. デフォルト設定

Inspector の `Settings`:
- **Default Fade Duration**: 1.0 （デフォルトのフェード時間）

---

## 📝 必要なオーディオファイル

### BGM（音楽ファイル - .mp3 or .ogg 推奨）

#### タイトル
- **title_theme** - タイトル画面のテーマ曲

#### ゲーム画面（時間帯別・デフォルトエリア）
- **game_early_morning** - 早朝（3:00-6:00）の曲
- **game_morning** - 朝（6:00-12:00）の曲
- **game_noon** - 昼（12:00-18:00）の曲
- **game_evening** - 夜（18:00-21:00）の曲
- **game_midnight** - 深夜（21:00-3:00）の曲
- **game_default** - デフォルト曲（フォールバック用）

#### ゲーム画面（場所別BGM）
- **game_beach** - ビーチエリアの曲（波の音など）
- **game_forest_day** - 森エリアの昼の曲
- **game_forest_night** - 森エリアの夜の曲
- **game_cave** - 洞窟エリアの曲（暗い雰囲気）
- **game_camp_day** - キャンプ地の昼の曲
- **game_camp_night** - キャンプ地の夜の曲

**注意**: 雨・嵐の天候時はBGMが自動停止されます。

### SE（効果音 - .wav 推奨）

#### UI
- **button_tap** - ボタンタップ音

#### プレイヤー
- **heal** - HP回復音
- **damage** - ダメージ音

#### アイテム
- **item_pickup** - アイテム取得音
- **item_spawn** - アイテムスポーン音（オプション）

#### システム
- **game_over** - ゲームオーバー音
- **transition_start** - 日付遷移画面の開始音
- **transition_end** - 日付遷移画面の終了音
- **location_change** - 場所移動時の効果音

---

## 🎨 現在の実装箇所

### TitleSceneController
```csharp
void Start()
{
    // タイトルBGMを再生
    AudioManager.Instance.PlayBGM("title_theme", 2f);
}

void OnScreenTapped()
{
    // タップSEを再生
    AudioManager.Instance.PlaySE("button_tap");
}
```

### DayTransitionUI
```csharp
void ShowTransitionWithInitialization()
{
    // 現在のBGMを停止（NTP同期中など）
    AudioManager.Instance.StopBGM(1f);

    // 遷移開始SEを再生
    AudioManager.Instance.PlaySE("transition_start");

    // ... 初期化処理 ...

    // 遷移完了SEを再生
    AudioManager.Instance.PlaySE("transition_end");
}
```

### GameSceneController
```csharp
void Start()
{
    // 時間帯と場所に応じたBGMを再生
    PlayBGMForTimeOfDay(gameTime.CurrentTimeOfDay);
}

void OnTimeOfDayChanged(TimeOfDay previous, TimeOfDay current)
{
    // 時間帯が変わったらBGMを変更
    PlayBGMForTimeOfDay(current);
}

void OnDayChanged(int day, WeatherType weather)
{
    // 天候が変わったらBGMを再チェック（雨/嵐なら停止）
    PlayBGMForTimeOfDay(gameTime.CurrentTimeOfDay);
}

void PlayBGMForTimeOfDay(TimeOfDay timeOfDay)
{
    // 雨や嵐の時はBGMを停止
    if (weather == WeatherType.Rainy || weather == WeatherType.Stormy)
    {
        AudioManager.Instance.StopBGM(2f);
        return;
    }

    // 場所と時間帯に応じたBGMを取得
    string bgmName = GetBGMForLocation(currentLocation, timeOfDay);
    AudioManager.Instance.PlayBGM(bgmName, 2f);
}

// 場所を変更する（公開メソッド）
public void SetLocation(string locationName)
{
    currentLocation = locationName;
    AudioManager.Instance.PlaySE("location_change");
    PlayBGMForTimeOfDay(gameTime.CurrentTimeOfDay);
}

void OnHPChanged(int current, int change)
{
    if (change > 0)
        AudioManager.Instance.PlaySE("heal");
    else if (change < 0)
        AudioManager.Instance.PlaySE("damage");
}

void OnPlayerDeath()
{
    AudioManager.Instance.PlaySE("game_over");
    AudioManager.Instance.StopBGM(1f);
}
```

---

## ⚙️ カスタマイズ

### フェード時間の変更
```csharp
// Inspector の Default Fade Duration を変更
// または、個別に指定
AudioManager.Instance.PlayBGM("title_theme", 3f); // 3秒フェード
```

### ボリューム設定の保存
ボリューム設定は自動的にPlayerPrefsに保存されます。
- BGMボリューム: `BGMVolume` キー
- SEボリューム: `SEVolume` キー

---

## 🐛 トラブルシューティング

### BGMが再生されない
1. AudioClipData の Name が正しく設定されているか確認
2. Clip が null でないか確認
3. BGMボリュームが0になっていないか確認（GetBGMVolume()）

### SEが聞こえない
1. AudioClipData の Name が正しく設定されているか確認
2. Clip が null でないか確認
3. SEボリュームが0になっていないか確認（GetSEVolume()）

### フェードがおかしい
1. Default Fade Duration の値を確認
2. Time.deltaTime が正常か確認（Time.timeScale = 0だとフェードしない）

### 同じBGMが何度も再生される
AudioManager は既に再生中のBGMは自動的にスキップします。強制的に再生したい場合は、一度StopBGM()してから再生してください。

---

## 📚 内部実装

### AudioClipData
```csharp
[Serializable]
public class AudioClipData
{
    public string name;         // BGM/SE名（識別用）
    public AudioClip clip;      // AudioClip
}
```

### フェード処理
- **FadeBGM** - 現在のBGMをフェードアウト → 新しいBGMをフェードイン
- **FadeOutBGM** - BGMをフェードアウトして停止

### ボリューム管理
- `bgmVolume` - BGMのマスターボリューム（0.0 ～ 1.0）
- `seVolume` - SEのマスターボリューム（0.0 ～ 1.0）
- PlayerPrefs で永続化され、起動時に自動ロード

---

## 🎮 場所別BGMの使い方

GameSceneControllerには、場所によってBGMを切り替える機能が実装されています。

### 利用可能な場所

- **"default"** - デフォルトエリア（島の一般エリア）- 時間帯に応じて変化
- **"beach"** - ビーチエリア - 常に同じBGM（波の音など）
- **"forest"** - 森エリア - 昼と夜で変化
- **"cave"** - 洞窟エリア - 常に同じBGM（暗い雰囲気）
- **"camp"** - キャンプ地 - 昼と夜で変化

### 場所を変更する方法

```csharp
// GameSceneControllerの公開メソッドを呼び出す
GameSceneController controller = FindObjectOfType<GameSceneController>();
controller.SetLocation("beach");  // ビーチに移動
```

プレイヤーが移動した時に呼び出すことで、自動的にBGMが切り替わります。

### 天候の影響

雨や嵐の天候時は、どの場所でもBGMが自動停止されます。晴れや曇りに戻ると、自動的にBGMが再生されます。

---

## 💡 今後の拡張案

### BGMプレイリスト
複数の曲をランダムまたは順番に再生する機能

### オーディオミキサー
より高度な音量管理（リバーブ、エコーなど）

### 3D音響
距離減衰やステレオパンニング

### オーディオプール
SEの同時再生数を制限してパフォーマンス向上

### 環境音
雨の音、波の音、風の音などの環境音レイヤー

---

## 🔗 関連ファイル

- `/Assets/Scripts/Managers/AudioManager.cs` - 本体
- `/Assets/Scripts/Scenes/TitleSceneController.cs` - タイトル画面での使用例
- `/Assets/Scripts/Scenes/GameSceneController.cs` - ゲーム画面での使用例（場所別BGM対応）
- `/Assets/Scripts/UI/DayTransition/DayTransitionUI.cs` - 日付遷移画面でのBGM制御

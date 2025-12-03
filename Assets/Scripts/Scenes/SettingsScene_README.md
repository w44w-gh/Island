# SettingsScene（設定画面）

BGM/SEのボリューム調整などの設定を行う画面です。

---

## 📁 機能

### 音量調整
- **BGMボリューム** - スライダーで0%～100%に調整
- **SEボリューム** - スライダーで0%～100%に調整
- **リアルタイム反映** - スライダーを動かすと即座に音量が変わる
- **自動保存** - PlayerPrefsに自動保存され、次回起動時も設定を維持

### テストボタン
- **BGMテスト** - 現在の音量でBGMを試聴
- **SEテスト** - 現在の音量でSEを試聴

### その他
- **戻るボタン** - タイトル画面に戻る

---

## 🛠️ Unity Editorでのセットアップ

### 1. SettingsSceneの作成

#### 新しいシーンを作成
1. File → New Scene
2. 名前を「SettingsScene」として保存
3. Build Settings に追加（File → Build Settings → Add Open Scenes）

#### Canvasの作成
1. Hierarchy で右クリック → UI → Canvas
2. Canvas の設定:
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920 x 1080

### 2. UI要素の配置

#### タイトルテキスト
1. Canvas の下に UI → Text を作成
2. 名前: "TitleText"
3. 設定:
   - Text: "設定"
   - Font Size: 60
   - Alignment: Center
   - Color: 白
   - Anchor: Top Center
   - Position: (0, -100, 0)

#### BGMボリュームセクション

**BGMラベルテキスト**:
1. Canvas の下に UI → Text を作成
2. 名前: "BGMLabel"
3. 設定:
   - Text: "BGM音量"
   - Font Size: 40
   - Alignment: Left
   - Color: 白
   - Anchor: Middle Left
   - Position: (200, 100, 0)

**BGMボリュームスライダー**:
1. Canvas の下に UI → Slider を作成
2. 名前: "BGMVolumeSlider"
3. 設定:
   - Min Value: 0
   - Max Value: 1
   - Value: 1
   - Whole Numbers: OFF
   - Width: 600
   - Anchor: Middle Center
   - Position: (0, 100, 0)

**BGMボリュームテキスト（%表示）**:
1. Canvas の下に UI → Text を作成
2. 名前: "BGMVolumeText"
3. 設定:
   - Text: "100%"
   - Font Size: 40
   - Alignment: Left
   - Color: 白
   - Anchor: Middle Right
   - Position: (-150, 100, 0)

#### SEボリュームセクション

**SEラベルテキスト**:
1. Canvas の下に UI → Text を作成
2. 名前: "SELabel"
3. 設定:
   - Text: "SE音量"
   - Font Size: 40
   - Alignment: Left
   - Color: 白
   - Anchor: Middle Left
   - Position: (200, 0, 0)

**SEボリュームスライダー**:
1. Canvas の下に UI → Slider を作成
2. 名前: "SEVolumeSlider"
3. 設定:
   - Min Value: 0
   - Max Value: 1
   - Value: 1
   - Whole Numbers: OFF
   - Width: 600
   - Anchor: Middle Center
   - Position: (0, 0, 0)

**SEボリュームテキスト（%表示）**:
1. Canvas の下に UI → Text を作成
2. 名前: "SEVolumeText"
3. 設定:
   - Text: "100%"
   - Font Size: 40
   - Alignment: Left
   - Color: 白
   - Anchor: Middle Right
   - Position: (-150, 0, 0)

#### テストボタン（オプション）

**BGMテストボタン**:
1. Canvas の下に UI → Button を作成
2. 名前: "TestBGMButton"
3. 設定:
   - Text: "BGM試聴"
   - Font Size: 30
   - Width: 200, Height: 60
   - Anchor: Middle Center
   - Position: (-150, -100, 0)

**SEテストボタン**:
1. Canvas の下に UI → Button を作成
2. 名前: "TestSEButton"
3. 設定:
   - Text: "SE試聴"
   - Font Size: 30
   - Width: 200, Height: 60
   - Anchor: Middle Center
   - Position: (150, -100, 0)

#### 戻るボタン

1. Canvas の下に UI → Button を作成
2. 名前: "BackButton"
3. 設定:
   - Text: "戻る"
   - Font Size: 40
   - Width: 300, Height: 80
   - Anchor: Bottom Center
   - Position: (0, 100, 0)

### 3. SettingsSceneControllerの設定

1. Hierarchy で空の GameObject を作成 → "SettingsSceneController"
2. SettingsSceneController.cs をアタッチ
3. Inspector で各UI要素をドラッグ:
   - **Bgm Volume Slider**: BGMVolumeSlider をドラッグ
   - **Se Volume Slider**: SEVolumeSlider をドラッグ
   - **Bgm Volume Text**: BGMVolumeText をドラッグ
   - **Se Volume Text**: SEVolumeText をドラッグ
   - **Back Button**: BackButton をドラッグ
   - **Test BGM Button**: TestBGMButton をドラッグ（オプション）
   - **Test SE Button**: TestSEButton をドラッグ（オプション）

### 4. TitleSceneの設定

TitleSceneに設定ボタンを追加します。

1. TitleScene を開く
2. Canvas の下に UI → Button を作成
3. 名前: "SettingsButton"
4. 設定:
   - Text: "設定"
   - Font Size: 30
   - Width: 150, Height: 60
   - Anchor: Bottom Right
   - Position: (-100, 100, 0)
5. TitleSceneController の Inspector で:
   - **Settings Button**: SettingsButton をドラッグ

---

## 🎮 使い方

### プレイヤー視点

1. タイトル画面で「設定」ボタンをクリック
2. スライダーを動かしてBGM/SEの音量を調整
3. SEスライダーを動かすと、その場でSEが鳴って確認できる
4. テストボタンで音量を確認（オプション）
5. 「戻る」ボタンでタイトル画面に戻る

設定は自動保存され、次回起動時も維持されます。

### 開発者視点

```csharp
// 設定画面を開く
SceneLoader.Instance.LoadSettingsScene();

// AudioManagerから音量を取得（0.0～1.0）
float bgmVolume = AudioManager.Instance.GetBGMVolume();
float seVolume = AudioManager.Instance.GetSEVolume();

// AudioManagerで音量を設定（0.0～1.0）
AudioManager.Instance.SetBGMVolume(0.8f);
AudioManager.Instance.SetSEVolume(0.6f);
```

---

## 📝 実装の詳細

### SettingsSceneController.cs

#### 初期化（Start）
```csharp
void Start()
{
    // AudioManagerから現在の音量を取得してスライダーに反映
    bgmVolumeSlider.value = AudioManager.Instance.GetBGMVolume();
    seVolumeSlider.value = AudioManager.Instance.GetSEVolume();

    // スライダーのイベントリスナーを登録
    bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
    seVolumeSlider.onValueChanged.AddListener(OnSEVolumeChanged);
}
```

#### BGMボリューム変更
```csharp
void OnBGMVolumeChanged(float value)
{
    // AudioManagerに反映
    AudioManager.Instance.SetBGMVolume(value);

    // テキスト表示を更新（0%～100%）
    bgmVolumeText.text = $"{value * 100:F0}%";
}
```

#### SEボリューム変更
```csharp
void OnSEVolumeChanged(float value)
{
    // AudioManagerに反映
    AudioManager.Instance.SetSEVolume(value);

    // テキスト表示を更新（0%～100%）
    seVolumeText.text = $"{value * 100:F0}%";

    // スライダーを動かしたときにSEを再生して確認
    AudioManager.Instance.PlaySE("button_tap");
}
```

#### 戻るボタン
```csharp
void OnBackButtonClicked()
{
    AudioManager.Instance.PlaySE("button_tap");
    SceneLoader.Instance.LoadTitleScene();
}
```

### AudioManagerとの連携

AudioManagerは設定された音量をPlayerPrefsに自動保存します:
- キー: `"BGMVolume"`, `"SEVolume"`
- 値: 0.0～1.0の浮動小数点数

次回起動時、AudioManagerが自動的にPlayerPrefsから音量を読み込みます。

---

## 🎨 レイアウト例

```
━━━━━━━━━━━━━━━━━━━━━━━━━
          設定
━━━━━━━━━━━━━━━━━━━━━━━━━

BGM音量  ━━━━━━●━━━━  80%

SE音量   ━━━━━━━━●━━  90%

  [BGM試聴]  [SE試聴]


        [戻る]
━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

## ⚙️ カスタマイズ

### スライダーの見た目を変更
- Slider の Background, Fill, Handle の色を変更
- Handle のサイズを変更

### テキストスタイルの変更
- Font を変更（日本語フォント推奨）
- Font Size や Color を調整

### 追加の設定項目
SettingsSceneControllerに新しい設定を追加できます:
- 画面の明るさ
- 言語設定
- 難易度設定
- データリセット機能

---

## 🐛 トラブルシューティング

### スライダーが動かない
1. Slider コンポーネントが正しく設定されているか確認
2. Min Value: 0, Max Value: 1 になっているか確認
3. Whole Numbers が OFF になっているか確認

### 音量が反映されない
1. AudioManager が正しく初期化されているか確認
2. スライダーのイベントリスナーが登録されているか確認
3. SettingsSceneController の Inspector で各UI要素が設定されているか確認

### 設定が保存されない
PlayerPrefsは自動的に保存されますが、エディタで動作確認する場合:
- Unity エディタを終了すると保存される
- または `PlayerPrefs.Save()` を明示的に呼ぶ（AudioManagerが自動実行）

### SettingsSceneが見つからない
1. Build Settings に SettingsScene が追加されているか確認
2. シーン名が正確に "SettingsScene" になっているか確認

---

## 🔗 関連ファイル

- `/Assets/Scripts/Scenes/SettingsSceneController.cs` - 設定画面コントローラー
- `/Assets/Scripts/Managers/AudioManager.cs` - 音量管理
- `/Assets/Scripts/Managers/SceneLoader.cs` - シーン遷移
- `/Assets/Scripts/Scenes/TitleSceneController.cs` - タイトル画面から設定画面へ

---

## 💡 今後の拡張案

### 追加の音響設定
- マスターボリューム
- ボイスボリューム（キャラクターボイス）
- 環境音ボリューム

### グラフィック設定
- 画面解像度
- フレームレート上限
- アンチエイリアシング

### ゲームプレイ設定
- 難易度
- 字幕のON/OFF
- チュートリアルの再表示

### データ管理
- セーブデータの削除
- クラウドセーブとの同期
- データのエクスポート/インポート

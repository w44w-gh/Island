using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 設定シーンを制御するクラス
/// BGM/SEのボリューム調整などの設定を管理
/// </summary>
public class SettingsSceneController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider seVolumeSlider;
    [SerializeField] private TextMeshProUGUI bgmVolumeText;
    [SerializeField] private TextMeshProUGUI seVolumeText;
    [SerializeField] private Button backButton;

    [Header("Test Buttons")]
    [SerializeField] private Button testBGMButton;
    [SerializeField] private Button testSEButton;

    private void Start()
    {
        // 現在の音量を取得してスライダーに反映
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = AudioManager.Instance.GetBGMVolume();
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        if (seVolumeSlider != null)
        {
            seVolumeSlider.value = AudioManager.Instance.GetSEVolume();
            seVolumeSlider.onValueChanged.AddListener(OnSEVolumeChanged);
        }

        // 戻るボタン
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        // テストボタン
        if (testBGMButton != null)
        {
            testBGMButton.onClick.AddListener(OnTestBGMButtonClicked);
        }

        if (testSEButton != null)
        {
            testSEButton.onClick.AddListener(OnTestSEButtonClicked);
        }

        // 初期表示を更新
        UpdateVolumeText();

        Debug.Log("SettingsScene ready");
    }

    private void OnDestroy()
    {
        // イベントリスナーを解除
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
        }

        if (seVolumeSlider != null)
        {
            seVolumeSlider.onValueChanged.RemoveListener(OnSEVolumeChanged);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        if (testBGMButton != null)
        {
            testBGMButton.onClick.RemoveListener(OnTestBGMButtonClicked);
        }

        if (testSEButton != null)
        {
            testSEButton.onClick.RemoveListener(OnTestSEButtonClicked);
        }
    }

    /// <summary>
    /// BGMボリュームが変更された時の処理
    /// </summary>
    private void OnBGMVolumeChanged(float value)
    {
        // AudioManagerに反映
        AudioManager.Instance.SetBGMVolume(value);

        // テキスト表示を更新
        UpdateVolumeText();

        Debug.Log($"BGM Volume changed: {value * 100:F0}%");
    }

    /// <summary>
    /// SEボリュームが変更された時の処理
    /// </summary>
    private void OnSEVolumeChanged(float value)
    {
        // AudioManagerに反映
        AudioManager.Instance.SetSEVolume(value);

        // テキスト表示を更新
        UpdateVolumeText();

        // スライダーを動かしたときにSEを再生して確認できるようにする
        AudioManager.Instance.PlaySE("button_tap");

        Debug.Log($"SE Volume changed: {value * 100:F0}%");
    }

    /// <summary>
    /// ボリューム表示テキストを更新
    /// </summary>
    private void UpdateVolumeText()
    {
        if (bgmVolumeText != null && bgmVolumeSlider != null)
        {
            bgmVolumeText.text = $"{bgmVolumeSlider.value * 100:F0}%";
        }

        if (seVolumeText != null && seVolumeSlider != null)
        {
            seVolumeText.text = $"{seVolumeSlider.value * 100:F0}%";
        }
    }

    /// <summary>
    /// 戻るボタンがクリックされた時の処理
    /// </summary>
    private void OnBackButtonClicked()
    {
        Debug.Log("Back button clicked - Returning to Title");

        // ボタンSEを再生
        AudioManager.Instance.PlaySE("button_tap");

        // タイトルシーンに戻る
        SceneLoader.Instance.LoadTitleScene();
    }

    /// <summary>
    /// BGMテストボタンがクリックされた時の処理
    /// </summary>
    private void OnTestBGMButtonClicked()
    {
        Debug.Log("Test BGM button clicked");

        // ボタンSEを再生
        AudioManager.Instance.PlaySE("button_tap");

        // テスト用BGMを再生（タイトルテーマを使用）
        AudioManager.Instance.PlayBGM("title_theme", 1f);
    }

    /// <summary>
    /// SEテストボタンがクリックされた時の処理
    /// </summary>
    private void OnTestSEButtonClicked()
    {
        Debug.Log("Test SE button clicked");

        // テスト用SEを再生
        AudioManager.Instance.PlaySE("button_tap");
    }
}

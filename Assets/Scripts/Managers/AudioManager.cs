using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGMとSEを管理するクラス（Singleton）
/// シーン間で永続化され、音楽の再生・停止・フェードを管理
/// </summary>
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AudioManager");
                _instance = go.AddComponent<AudioManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Header("Audio Sources")]
    private AudioSource bgmSource;      // BGM用
    private AudioSource seSource;       // SE用

    [Header("Audio Clips")]
    [SerializeField] private AudioClipData[] bgmClips;
    [SerializeField] private AudioClipData[] seClips;

    [Header("Settings")]
    [SerializeField] private float defaultFadeDuration = 1f;

    // ボリューム設定
    private float bgmVolume = 1f;
    private float seVolume = 1f;

    // フェード処理用
    private Coroutine fadeCoroutine;
    private string currentBGM = "";

    // ボリューム保存キー
    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SE_VOLUME_KEY = "SEVolume";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        // BGM用 AudioSource を作成
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        // SE用 AudioSource を作成
        seSource = gameObject.AddComponent<AudioSource>();
        seSource.loop = false;
        seSource.playOnAwake = false;

        // 保存されているボリュームを読み込み
        LoadVolume();

        Debug.Log("AudioManager initialized");
    }

    /// <summary>
    /// BGMを再生
    /// </summary>
    /// <param name="bgmName">BGM名（BGMClipのname）</param>
    /// <param name="fadeDuration">フェード時間（秒）</param>
    public void PlayBGM(string bgmName, float fadeDuration = -1)
    {
        if (fadeDuration < 0)
        {
            fadeDuration = defaultFadeDuration;
        }

        // 既に同じBGMが再生中の場合はスキップ
        if (currentBGM == bgmName && bgmSource.isPlaying)
        {
            return;
        }

        AudioClip clip = GetBGMClip(bgmName);
        if (clip == null)
        {
            Debug.LogWarning($"BGM '{bgmName}' not found!");
            return;
        }

        // フェード処理を開始
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeBGM(clip, bgmName, fadeDuration));
    }

    /// <summary>
    /// BGMを停止
    /// </summary>
    /// <param name="fadeDuration">フェード時間（秒）</param>
    public void StopBGM(float fadeDuration = -1)
    {
        if (fadeDuration < 0)
        {
            fadeDuration = defaultFadeDuration;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOutBGM(fadeDuration));
    }

    /// <summary>
    /// SEを再生
    /// </summary>
    /// <param name="seName">SE名（SEClipのname）</param>
    /// <param name="volume">ボリューム倍率（0.0～1.0、デフォルトは1.0）</param>
    public void PlaySE(string seName, float volume = 1f)
    {
        AudioClip clip = GetSEClip(seName);
        if (clip == null)
        {
            Debug.LogWarning($"SE '{seName}' not found!");
            return;
        }

        seSource.PlayOneShot(clip, seVolume * volume);
    }

    /// <summary>
    /// BGMのフェード処理
    /// </summary>
    private IEnumerator FadeBGM(AudioClip nextClip, string nextBGMName, float duration)
    {
        // 現在のBGMをフェードアウト
        if (bgmSource.isPlaying)
        {
            float startVolume = bgmSource.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }
        }

        // 新しいBGMに切り替え
        bgmSource.clip = nextClip;
        bgmSource.Play();
        currentBGM = nextBGMName;

        // フェードイン
        float targetVolume = bgmVolume;
        float elapsed2 = 0f;

        while (elapsed2 < duration)
        {
            elapsed2 += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, elapsed2 / duration);
            yield return null;
        }

        bgmSource.volume = targetVolume;
        fadeCoroutine = null;

        Debug.Log($"BGM started: {nextBGMName}");
    }

    /// <summary>
    /// BGMのフェードアウト
    /// </summary>
    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = bgmVolume;
        currentBGM = "";
        fadeCoroutine = null;

        Debug.Log("BGM stopped");
    }

    /// <summary>
    /// BGMボリュームを設定
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmVolume);
        PlayerPrefs.Save();

        Debug.Log($"BGM Volume set to: {bgmVolume}");
    }

    /// <summary>
    /// SEボリュームを設定
    /// </summary>
    public void SetSEVolume(float volume)
    {
        seVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SE_VOLUME_KEY, seVolume);
        PlayerPrefs.Save();

        Debug.Log($"SE Volume set to: {seVolume}");
    }

    /// <summary>
    /// BGMボリュームを取得
    /// </summary>
    public float GetBGMVolume() => bgmVolume;

    /// <summary>
    /// SEボリュームを取得
    /// </summary>
    public float GetSEVolume() => seVolume;

    /// <summary>
    /// ボリュームをロード
    /// </summary>
    private void LoadVolume()
    {
        bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        seVolume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, 1f);

        bgmSource.volume = bgmVolume;

        Debug.Log($"Volume loaded - BGM: {bgmVolume}, SE: {seVolume}");
    }

    /// <summary>
    /// BGMクリップを取得
    /// </summary>
    private AudioClip GetBGMClip(string bgmName)
    {
        if (bgmClips == null) return null;

        foreach (var data in bgmClips)
        {
            if (data.name == bgmName)
            {
                return data.clip;
            }
        }

        return null;
    }

    /// <summary>
    /// SEクリップを取得
    /// </summary>
    private AudioClip GetSEClip(string seName)
    {
        if (seClips == null) return null;

        foreach (var data in seClips)
        {
            if (data.name == seName)
            {
                return data.clip;
            }
        }

        return null;
    }

    /// <summary>
    /// 現在再生中のBGM名を取得
    /// </summary>
    public string GetCurrentBGM() => currentBGM;

    /// <summary>
    /// BGMが再生中かチェック
    /// </summary>
    public bool IsBGMPlaying() => bgmSource.isPlaying;
}

/// <summary>
/// AudioClipとその名前を紐づけるデータ
/// </summary>
[Serializable]
public class AudioClipData
{
    public string name;         // BGM/SE名（識別用）
    public AudioClip clip;      // AudioClip
}

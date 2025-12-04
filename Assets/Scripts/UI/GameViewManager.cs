using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// GameScene内のMapViewとLocationViewを管理
/// シームレスな画面切り替えを提供
/// </summary>
public class GameViewManager : MonoBehaviour
{
    private static GameViewManager _instance;
    public static GameViewManager Instance => _instance;

    [Header("Views")]
    [SerializeField] private GameObject mapView;        // マップ画面
    [SerializeField] private GameObject locationView;   // 場所画面

    [Header("Location View Components")]
    [SerializeField] private UnityEngine.UI.Image locationBackground;  // 場所の背景
    [SerializeField] private Transform charactersContainer;            // キャラ配置用コンテナ
    [SerializeField] private GameObject characterPrefab;               // キャラ立ち絵のPrefab

    [Header("Character Behavior")]
    [SerializeField] private CharacterBehaviorManager behaviorManager; // キャラ行動管理

    [Header("Transition")]
    [SerializeField] private LocationTransitionUI transitionUI;  // 遷移演出
    [SerializeField] private float transitionDuration = 0.3f;

    [Header("Location Backgrounds")]
    [SerializeField] private Sprite beachBackground;
    [SerializeField] private Sprite forestBackground;
    [SerializeField] private Sprite mountainBackground;
    [SerializeField] private Sprite riverBackground;

    // 現在の状態
    private MapLocation? currentLocation = null;
    private bool isTransitioning = false;

    /// <summary>
    /// 現在表示中の場所（nullならマップ画面）
    /// </summary>
    public MapLocation? CurrentLocation => currentLocation;

    /// <summary>
    /// マップ画面を表示中か
    /// </summary>
    public bool IsShowingMap => currentLocation == null;

    /// <summary>
    /// 場所変更時のイベント
    /// </summary>
    public event Action<MapLocation?> OnLocationChanged;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        // 初期状態はマップ画面
        ShowMapView();
    }

    /// <summary>
    /// マップ画面を表示
    /// </summary>
    public void ShowMapView()
    {
        if (mapView != null) mapView.SetActive(true);
        if (locationView != null) locationView.SetActive(false);

        currentLocation = null;
        OnLocationChanged?.Invoke(null);

        Debug.Log("Showing MapView");
    }

    /// <summary>
    /// 場所画面に移動（演出なし）
    /// </summary>
    public void ShowLocationView(MapLocation location)
    {
        // 背景を設定
        SetLocationBackground(location);

        // キャラを配置
        SpawnCharactersAtLocation(location);

        // 画面切り替え
        if (mapView != null) mapView.SetActive(false);
        if (locationView != null) locationView.SetActive(true);

        currentLocation = location;
        OnLocationChanged?.Invoke(location);

        // BGM変更
        if (GameSceneController.Instance != null)
        {
            GameSceneController.Instance.SetLocation(location);
        }
    }

    /// <summary>
    /// 指定場所にいるキャラを動的に配置
    /// </summary>
    private void SpawnCharactersAtLocation(MapLocation location)
    {
        // 既存のキャラをクリア
        ClearCharacters();

        if (behaviorManager == null || characterPrefab == null || charactersContainer == null)
        {
            return;
        }

        // 現在の時間帯を取得
        TimeOfDay currentTime = TimeOfDay.Morning;
        if (GameManager.Instance != null && GameManager.Instance.GlobalGameTime != null)
        {
            currentTime = GameManager.Instance.GlobalGameTime.CurrentTimeOfDay;
        }

        // この場所にいるキャラを取得（ScriptableObject版）
        var charactersHere = behaviorManager.GetCharacterDataAtLocation(location, currentTime);

        // キャラごとに立ち絵を生成
        for (int i = 0; i < charactersHere.Count; i++)
        {
            var charInfo = charactersHere[i];
            SpawnCharacterFromData(charInfo, i, charactersHere.Count);
        }
    }

    /// <summary>
    /// キャラの立ち絵を生成（CharacterData版）
    /// </summary>
    private void SpawnCharacterFromData(CharacterBehaviorManager.CharacterDataLocationInfo charInfo, int index, int totalCount)
    {
        if (charInfo.data == null) return;

        // Prefabから生成
        GameObject charObj = Instantiate(characterPrefab, charactersContainer);

        // InteractableCharacterにデータを設定
        InteractableCharacter spawnedChar = charObj.GetComponent<InteractableCharacter>();

        if (spawnedChar != null)
        {
            // CharacterDataからセットアップ
            spawnedChar.Setup(
                charInfo.data.characterId,
                charInfo.data.defaultScenarioLabel,
                charInfo.data.defaultSprite
            );
        }

        // 位置を設定（複数キャラの場合は横に並べる）
        RectTransform rectTransform = charObj.GetComponent<RectTransform>();
        if (rectTransform != null && totalCount > 1)
        {
            float spacing = 300f;  // キャラ間の間隔
            float offsetX = (index - (totalCount - 1) * 0.5f) * spacing;
            rectTransform.anchoredPosition = new Vector2(offsetX, rectTransform.anchoredPosition.y);
        }
    }

    /// <summary>
    /// 場所画面に移動（ズーム演出付き）
    /// </summary>
    public void MoveToLocation(MapLocation location, RectTransform iconTransform = null)
    {
        if (isTransitioning) return;

        StartCoroutine(MoveToLocationCoroutine(location, iconTransform));
    }

    private IEnumerator MoveToLocationCoroutine(MapLocation location, RectTransform iconTransform)
    {
        isTransitioning = true;

        // SE再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE("location_change");
        }

        // ズーム演出（TransitionUIがある場合）
        if (transitionUI != null && iconTransform != null)
        {
            yield return StartCoroutine(transitionUI.ZoomToLocation(iconTransform, location));

            // 暗い画面で少し待機
            yield return new WaitForSeconds(0.3f);

            // 場所画面を表示（画面が黒い状態で切り替え）
            ShowLocationView(location);

            // もう少し待ってからフェードイン
            yield return new WaitForSeconds(0.2f);

            // フェードイン
            yield return StartCoroutine(transitionUI.FadeIn());
        }
        else
        {
            // 演出なしの場合は簡易フェード
            yield return StartCoroutine(SimpleFadeTransition());

            // 場所画面を表示
            ShowLocationView(location);
        }

        isTransitioning = false;
    }

    /// <summary>
    /// マップ画面に戻る
    /// </summary>
    public void ReturnToMap()
    {
        if (isTransitioning) return;

        StartCoroutine(ReturnToMapCoroutine());
    }

    private IEnumerator ReturnToMapCoroutine()
    {
        isTransitioning = true;

        // SE再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE("button_tap");
        }

        // ズームアウト演出（TransitionUIがある場合）
        if (transitionUI != null)
        {
            // フェードアウト
            yield return StartCoroutine(transitionUI.FadeOut());

            // 暗い画面で少し待機
            yield return new WaitForSeconds(0.3f);

            // マップ画面を表示（画面が黒い状態で切り替え）
            ShowMapView();

            // もう少し待ってからズームアウト + フェードイン
            yield return new WaitForSeconds(0.2f);

            // ズームアウトしながらフェードイン
            yield return StartCoroutine(transitionUI.ZoomOutAndFadeInPublic());
        }
        else
        {
            // 簡易フェード
            yield return StartCoroutine(SimpleFadeTransition());

            // マップ画面を表示
            ShowMapView();
        }

        isTransitioning = false;
    }

    /// <summary>
    /// 簡易フェード遷移
    /// </summary>
    private IEnumerator SimpleFadeTransition()
    {
        // SceneFadeManagerを使用
        if (SceneFadeManager.Instance != null)
        {
            SceneFadeManager.Instance.FadeIn(transitionDuration);
            yield return new WaitForSeconds(transitionDuration);

            SceneFadeManager.Instance.FadeOut(transitionDuration);
            yield return new WaitForSeconds(transitionDuration * 0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 場所に応じた背景を設定
    /// </summary>
    private void SetLocationBackground(MapLocation location)
    {
        if (locationBackground == null) return;

        Sprite bg = location switch
        {
            MapLocation.Beach => beachBackground,
            MapLocation.Forest => forestBackground,
            MapLocation.Mountain => mountainBackground,
            MapLocation.River => riverBackground,
            _ => null
        };

        if (bg != null)
        {
            locationBackground.sprite = bg;
        }
        else
        {
            Debug.LogWarning($"Background not set for {location}");
        }
    }

    /// <summary>
    /// キャラクター配置コンテナを取得
    /// </summary>
    public Transform GetCharactersContainer()
    {
        return charactersContainer;
    }

    /// <summary>
    /// 現在の場所のキャラクターをクリア
    /// </summary>
    public void ClearCharacters()
    {
        if (charactersContainer == null) return;

        foreach (Transform child in charactersContainer)
        {
            Destroy(child.gameObject);
        }
    }
}

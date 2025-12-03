using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マップビューの管理
/// 各場所アイコンのキャラ表示を更新
/// </summary>
public class MapViewController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CharacterBehaviorManager behaviorManager;

    [Header("Location Icons")]
    [SerializeField] private List<LocationIconUI> locationIcons = new List<LocationIconUI>();

    [Header("Auto Update")]
    [SerializeField] private bool autoUpdateOnTimeChange = true;

    private void Start()
    {
        // 初期表示
        UpdateAllLocationIcons();

        // 時間帯変化イベントを購読
        if (autoUpdateOnTimeChange && GameManager.Instance != null)
        {
            GameManager.Instance.OnTimeOfDayChanged += OnTimeOfDayChanged;
        }
    }

    private void OnDestroy()
    {
        // イベント購読解除
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimeOfDayChanged -= OnTimeOfDayChanged;
        }
    }

    /// <summary>
    /// 時間帯が変わった時
    /// </summary>
    private void OnTimeOfDayChanged(TimeOfDay previous, TimeOfDay current)
    {
        Debug.Log($"[MapViewController] Time changed: {previous.ToJapaneseString()} → {current.ToJapaneseString()}");
        UpdateAllLocationIcons();
    }

    /// <summary>
    /// 全場所アイコンのキャラ表示を更新
    /// </summary>
    public void UpdateAllLocationIcons()
    {
        if (behaviorManager == null)
        {
            Debug.LogWarning("BehaviorManager is not set");
            return;
        }

        if (GameManager.Instance == null || GameManager.Instance.GlobalGameTime == null)
        {
            Debug.LogWarning("GameManager or GlobalGameTime is not available");
            return;
        }

        TimeOfDay currentTime = GameManager.Instance.GlobalGameTime.CurrentTimeOfDay;

        foreach (var locationIcon in locationIcons)
        {
            if (locationIcon == null)
            {
                continue;
            }

            // この場所にいるキャラのリストを取得
            var charactersHere = behaviorManager.GetCharactersAtLocation(locationIcon.Location, currentTime);

            // アイコンを更新
            locationIcon.UpdateCharacterIcons(charactersHere);
        }

        Debug.Log($"[MapViewController] Updated location icons for {currentTime.ToJapaneseString()}");
    }

    /// <summary>
    /// 手動で更新（デバッグ用）
    /// </summary>
    [ContextMenu("Force Update Location Icons")]
    public void ForceUpdateLocationIcons()
    {
        UpdateAllLocationIcons();
    }

    /// <summary>
    /// 特定の場所のアイコンを取得
    /// </summary>
    public LocationIconUI GetLocationIcon(MapLocation location)
    {
        foreach (var icon in locationIcons)
        {
            if (icon != null && icon.Location == location)
            {
                return icon;
            }
        }
        return null;
    }
}

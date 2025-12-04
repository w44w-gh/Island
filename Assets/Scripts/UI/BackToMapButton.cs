using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マップに戻るボタン
/// LocationViewに配置して使用
/// </summary>
[RequireComponent(typeof(Button))]
public class BackToMapButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (GameViewManager.Instance != null)
        {
            GameViewManager.Instance.ReturnToMap();
        }
    }
}

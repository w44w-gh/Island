using UnityEngine;

/// <summary>
/// 宴とゲームデータの橋渡しクラス（メイン）
/// 宴のシナリオスクリプトから呼び出せるメソッドを提供
/// partial classで機能ごとに分割されています
/// </summary>
public partial class NovelBridge : MonoBehaviour
{
    private static NovelBridge _instance;
    public static NovelBridge Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("NovelBridge");
                _instance = go.AddComponent<NovelBridge>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

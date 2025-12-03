using UnityEngine;

/// <summary>
/// タップ可能なオブジェクトのインターフェース
/// キャラクター、アイテムなどが実装する
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// タップされた時の処理
    /// </summary>
    void OnInteract();

    /// <summary>
    /// インタラクション可能かどうか
    /// </summary>
    bool CanInteract();

    /// <summary>
    /// インタラクション範囲（画面上のコライダーなど）
    /// </summary>
    GameObject GetGameObject();
}

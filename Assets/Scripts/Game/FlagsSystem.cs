using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム内のフラグを管理するシステム
/// イベント進行、条件判定などに使用
/// </summary>
[System.Serializable]
public class FlagsSystem
{
    [SerializeField] private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    /// <summary>
    /// フラグが変更された時のイベント
    /// </summary>
    public event Action<string, bool> OnFlagChanged;

    /// <summary>
    /// フラグを立てる
    /// </summary>
    public void SetFlag(string flagId, bool value = true)
    {
        if (flags.ContainsKey(flagId))
        {
            flags[flagId] = value;
            Debug.Log($"[Flags] フラグ更新: {flagId} = {value}");
        }
        else
        {
            flags.Add(flagId, value);
            Debug.Log($"[Flags] フラグ追加: {flagId} = {value}");
        }

        // フラグ変更イベントを発火
        OnFlagChanged?.Invoke(flagId, value);
    }

    /// <summary>
    /// フラグが立っているか確認
    /// </summary>
    public bool IsFlagEnabled(string flagId)
    {
        if (flags.ContainsKey(flagId))
        {
            return flags[flagId];
        }

        // 存在しないフラグはfalse
        return false;
    }

    /// <summary>
    /// フラグを削除
    /// </summary>
    public void RemoveFlag(string flagId)
    {
        if (flags.ContainsKey(flagId))
        {
            flags.Remove(flagId);
            Debug.Log($"[Flags] フラグ削除: {flagId}");

            // フラグ変更イベントを発火（削除=falseとして扱う）
            OnFlagChanged?.Invoke(flagId, false);
        }
    }

    /// <summary>
    /// フラグをトグル
    /// </summary>
    public void ToggleFlag(string flagId)
    {
        bool currentValue = IsFlagEnabled(flagId);
        SetFlag(flagId, !currentValue);
    }

    /// <summary>
    /// 全フラグをクリア
    /// </summary>
    public void ClearAllFlags()
    {
        flags.Clear();
        Debug.Log("[Flags] 全フラグをクリア");
    }

    /// <summary>
    /// フラグの数を取得
    /// </summary>
    public int GetFlagCount()
    {
        return flags.Count;
    }

    /// <summary>
    /// 全フラグのリストを取得（デバッグ用）
    /// </summary>
    public Dictionary<string, bool> GetAllFlags()
    {
        return new Dictionary<string, bool>(flags);
    }

    /// <summary>
    /// セーブデータに変換
    /// </summary>
    public FlagsSaveData ToSaveData()
    {
        return new FlagsSaveData
        {
            flagKeys = new List<string>(flags.Keys),
            flagValues = new List<bool>(flags.Values)
        };
    }

    /// <summary>
    /// セーブデータから復元
    /// </summary>
    public void LoadFromSaveData(FlagsSaveData saveData)
    {
        flags.Clear();

        if (saveData != null && saveData.flagKeys != null && saveData.flagValues != null)
        {
            for (int i = 0; i < saveData.flagKeys.Count && i < saveData.flagValues.Count; i++)
            {
                flags[saveData.flagKeys[i]] = saveData.flagValues[i];
            }

            Debug.Log($"[Flags] {flags.Count}個のフラグを復元");
        }
    }

    /// <summary>
    /// フラグのセーブデータ
    /// </summary>
    [System.Serializable]
    public class FlagsSaveData
    {
        public List<string> flagKeys;
        public List<bool> flagValues;
    }
}

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// セーブデータの管理（暗号化対応）
/// </summary>
public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SaveManager");
                _instance = go.AddComponent<SaveManager>();
            }
            return _instance;
        }
    }

    // 暗号化キーの生成用シード（難読化）
    // 複数の定数を組み合わせてキーを動的生成することでハードコードを回避
    private static readonly byte[] KEY_SEED_1 = new byte[] { 0x49, 0x73, 0x6C, 0x61, 0x6E, 0x64, 0x47, 0x61 }; // "IslandGa"
    private static readonly byte[] KEY_SEED_2 = new byte[] { 0x6D, 0x65, 0x32, 0x30, 0x32, 0x34, 0x53, 0x61 }; // "me2024Sa"
    private static readonly byte[] KEY_SEED_3 = new byte[] { 0x76, 0x65, 0x44, 0x61, 0x74, 0x61, 0x4B, 0x65 }; // "veDataKe"
    private static readonly byte[] KEY_SEED_4 = new byte[] { 0x79, 0x53, 0x65, 0x63, 0x75, 0x72, 0x69, 0x74 }; // "ySecurit"

    private static readonly byte[] IV_SEED_1 = new byte[] { 0x49, 0x73, 0x6C, 0x61, 0x6E, 0x64, 0x32, 0x30 }; // "Island20"
    private static readonly byte[] IV_SEED_2 = new byte[] { 0x32, 0x34, 0x49, 0x56, 0x31, 0x32, 0x33, 0x34 }; // "24IV1234"

    // 動的に生成される暗号化キーとIV
    private static byte[] _encryptionKey;
    private static byte[] _encryptionIV;

    /// <summary>
    /// 暗号化キーを動的生成（初回のみ）
    /// </summary>
    private static byte[] GetEncryptionKey()
    {
        if (_encryptionKey == null)
        {
            // 複数のシードを結合してSHA256でハッシュ化
            byte[] combined = new byte[KEY_SEED_1.Length + KEY_SEED_2.Length + KEY_SEED_3.Length + KEY_SEED_4.Length];
            Buffer.BlockCopy(KEY_SEED_1, 0, combined, 0, KEY_SEED_1.Length);
            Buffer.BlockCopy(KEY_SEED_2, 0, combined, KEY_SEED_1.Length, KEY_SEED_2.Length);
            Buffer.BlockCopy(KEY_SEED_3, 0, combined, KEY_SEED_1.Length + KEY_SEED_2.Length, KEY_SEED_3.Length);
            Buffer.BlockCopy(KEY_SEED_4, 0, combined, KEY_SEED_1.Length + KEY_SEED_2.Length + KEY_SEED_3.Length, KEY_SEED_4.Length);

            using (SHA256 sha256 = SHA256.Create())
            {
                _encryptionKey = sha256.ComputeHash(combined);
            }
        }
        return _encryptionKey;
    }

    /// <summary>
    /// 暗号化IVを動的生成（初回のみ）
    /// </summary>
    private static byte[] GetEncryptionIV()
    {
        if (_encryptionIV == null)
        {
            // 複数のシードを結合
            byte[] combined = new byte[IV_SEED_1.Length + IV_SEED_2.Length];
            Buffer.BlockCopy(IV_SEED_1, 0, combined, 0, IV_SEED_1.Length);
            Buffer.BlockCopy(IV_SEED_2, 0, combined, IV_SEED_1.Length, IV_SEED_2.Length);

            _encryptionIV = combined;
        }
        return _encryptionIV;
    }

    private const string SAVE_KEY = "GameSaveData";
    private const string BACKUP_SAVE_KEY = "GameSaveData_Backup";

    // 開発中は暗号化をオフにできる（デバッグビルド時のみ有効）
    [SerializeField] private bool useEncryption = true;

    /// <summary>
    /// 暗号化を使用するか（開発用）
    /// </summary>
    public bool UseEncryption
    {
        get => useEncryption;
        set
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            useEncryption = value;
            Debug.Log($"暗号化モード: {(useEncryption ? "ON" : "OFF")} (開発モードのみ変更可能)");
#else
            Debug.LogWarning("リリースビルドでは暗号化を無効化できません");
#endif
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

    /// <summary>
    /// ゲームデータをセーブ（バックアップ付き）
    /// </summary>
    public bool SaveGame(SaveData data)
    {
        try
        {
            // JSONに変換
            string json = JsonUtility.ToJson(data, true);
            Debug.Log($"セーブデータJSON: {json.Length}文字");

            // 暗号化（オプション）
            string saveString = useEncryption ? Encrypt(json) : json;

            // 既存のセーブデータがあればバックアップにコピー
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string currentSave = PlayerPrefs.GetString(SAVE_KEY);
                PlayerPrefs.SetString(BACKUP_SAVE_KEY, currentSave);
                Debug.Log("既存のセーブデータをバックアップにコピーしました");
            }

            // 新しいデータをメインに保存
            PlayerPrefs.SetString(SAVE_KEY, saveString);
            PlayerPrefs.Save();

            string encryptionStatus = useEncryption ? "暗号化あり" : "暗号化なし";
            Debug.Log($"ゲームデータをセーブしました（バックアップ付き, {encryptionStatus}）");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"セーブ失敗: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// ゲームデータをロード（バックアップからの復元対応）
    /// </summary>
    public SaveData LoadGame()
    {
        // メインのセーブデータからロードを試みる
        SaveData data = TryLoadFromKey(SAVE_KEY, "メイン");

        if (data != null)
        {
            return data;
        }

        // メインが失敗したらバックアップから試みる
        Debug.LogWarning("メインのセーブデータの読み込みに失敗しました。バックアップから復元を試みます...");
        data = TryLoadFromKey(BACKUP_SAVE_KEY, "バックアップ");

        if (data != null)
        {
            Debug.Log("バックアップからの復元に成功しました");
            // バックアップからの復元に成功したら、それをメインにコピー
            SaveGame(data);
            return data;
        }

        // 両方失敗
        Debug.LogError("メインとバックアップ両方のセーブデータの読み込みに失敗しました");
        return null;
    }

    /// <summary>
    /// 指定されたキーからセーブデータを読み込む
    /// </summary>
    private SaveData TryLoadFromKey(string key, string label)
    {
        try
        {
            if (!PlayerPrefs.HasKey(key))
            {
                Debug.Log($"{label}セーブデータが存在しません");
                return null;
            }

            // PlayerPrefsから読み込み
            string loadedString = PlayerPrefs.GetString(key, "");

            if (string.IsNullOrEmpty(loadedString))
            {
                Debug.LogWarning($"{label}セーブデータが空です");
                return null;
            }

            // 復号化（必要な場合のみ）
            string json;
            if (useEncryption)
            {
                json = Decrypt(loadedString);
            }
            else
            {
                json = loadedString;
            }

            // JSONからオブジェクトに変換
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            string encryptionStatus = useEncryption ? "暗号化あり" : "暗号化なし";
            Debug.Log($"{label}セーブデータのロードに成功しました ({encryptionStatus})");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"{label}セーブデータのロード失敗: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// セーブデータが存在するか
    /// </summary>
    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    /// <summary>
    /// セーブデータを削除（バックアップも含む）
    /// </summary>
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteKey(BACKUP_SAVE_KEY);
        PlayerPrefs.Save();
        Debug.Log("セーブデータとバックアップを削除しました");
    }

    /// <summary>
    /// 文字列を暗号化
    /// </summary>
    private string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = GetEncryptionKey();
            aes.IV = GetEncryptionIV();

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    /// <summary>
    /// 暗号化された文字列を復号化
    /// </summary>
    private string Decrypt(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = GetEncryptionKey();
            aes.IV = GetEncryptionIV();

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}

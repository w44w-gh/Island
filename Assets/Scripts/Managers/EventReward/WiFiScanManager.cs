using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

/// <summary>
/// WiFiスキャン管理クラス（Android専用）
/// リアルイベント配布用のSSID検出機能
/// </summary>
public class WiFiScanManager
{
    private const string PERMISSION_FINE_LOCATION = "android.permission.ACCESS_FINE_LOCATION";

    // コールバック
    public Action<List<string>> OnScanComplete;
    public Action<string> OnScanError;

    /// <summary>
    /// WiFiが有効かチェック
    /// </summary>
    public bool IsWiFiEnabled()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
            using (AndroidJavaObject wifiManager = context.Call<AndroidJavaObject>("getSystemService", "wifi"))
            {
                return wifiManager.Call<bool>("isWifiEnabled");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"WiFi状態取得エラー: {e.Message}");
            return false;
        }
#else
        Debug.LogWarning("WiFiScanManager: Android専用機能です（エディタでは動作しません）");
        return false;
#endif
    }

    /// <summary>
    /// 位置情報パーミッションがあるかチェック
    /// </summary>
    public bool HasLocationPermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return Permission.HasUserAuthorizedPermission(Permission.FineLocation);
#else
        return false;
#endif
    }

    /// <summary>
    /// 位置情報パーミッションを要求
    /// </summary>
    public void RequestLocationPermission(Action<bool> callback)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionGranted += (permission) =>
        {
            Debug.Log($"パーミッション許可: {permission}");
            callback?.Invoke(true);
        };
        callbacks.PermissionDenied += (permission) =>
        {
            Debug.LogWarning($"パーミッション拒否: {permission}");
            callback?.Invoke(false);
        };
        callbacks.PermissionDeniedAndDontAskAgain += (permission) =>
        {
            Debug.LogWarning($"パーミッション拒否（今後表示しない）: {permission}");
            callback?.Invoke(false);
        };

        Permission.RequestUserPermission(Permission.FineLocation, callbacks);
#else
        Debug.LogWarning("WiFiScanManager: Android専用機能です");
        callback?.Invoke(false);
#endif
    }

    /// <summary>
    /// WiFi設定画面を開く
    /// </summary>
    public void OpenWiFiSettings()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
            using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.settings.WIFI_SETTINGS"))
            {
                currentActivity.Call("startActivity", intent);
                Debug.Log("WiFi設定画面を開きました");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"WiFi設定画面を開けませんでした: {e.Message}");
        }
#else
        Debug.LogWarning("WiFiScanManager: Android専用機能です");
#endif
    }

    /// <summary>
    /// WiFiスキャンを実行してSSIDリストを取得
    /// </summary>
    public void ScanWiFiNetworks()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
            using (AndroidJavaObject wifiManager = context.Call<AndroidJavaObject>("getSystemService", "wifi"))
            {
                // スキャン開始
                bool scanStarted = wifiManager.Call<bool>("startScan");

                if (!scanStarted)
                {
                    Debug.LogWarning("WiFiスキャンを開始できませんでした");
                    OnScanError?.Invoke("スキャンを開始できませんでした");
                    return;
                }

                // スキャン結果取得
                AndroidJavaObject scanResults = wifiManager.Call<AndroidJavaObject>("getScanResults");

                if (scanResults == null)
                {
                    Debug.LogWarning("WiFiスキャン結果が取得できませんでした");
                    OnScanError?.Invoke("スキャン結果が取得できませんでした");
                    return;
                }

                // SSIDリストを作成
                List<string> ssidList = new List<string>();
                int size = scanResults.Call<int>("size");

                for (int i = 0; i < size; i++)
                {
                    using (AndroidJavaObject scanResult = scanResults.Call<AndroidJavaObject>("get", i))
                    {
                        string ssid = scanResult.Get<string>("SSID");

                        // SSIDが空でない場合のみ追加
                        if (!string.IsNullOrEmpty(ssid) && ssid != "<unknown ssid>")
                        {
                            // ダブルクォートを除去
                            ssid = ssid.Trim('"');
                            ssidList.Add(ssid);
                        }
                    }
                }

                Debug.Log($"WiFiスキャン完了: {ssidList.Count}件のネットワークを検出");
                OnScanComplete?.Invoke(ssidList);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"WiFiスキャンエラー: {e.Message}");
            OnScanError?.Invoke($"スキャンエラー: {e.Message}");
        }
#else
        Debug.LogWarning("WiFiScanManager: Android専用機能です（エディタではテストデータを返します）");

        // エディタ用のテストデータ
        List<string> testSSIDs = new List<string>
        {
            "AnimeJapan2025_Island",
            "TestWiFi_01",
            "HomeNetwork"
        };
        OnScanComplete?.Invoke(testSSIDs);
#endif
    }
}

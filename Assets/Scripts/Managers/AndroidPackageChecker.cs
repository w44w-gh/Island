using UnityEngine;

/// <summary>
/// Androidのパッケージ存在確認クラス
/// </summary>
public static class AndroidPackageChecker
{
    /// <summary>
    /// 指定されたパッケージIDのアプリがインストールされているか確認
    /// </summary>
    public static bool IsPackageInstalled(string packageId)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

            // パッケージ情報を取得（存在しなければ例外が発生）
            AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageId, 0);

            Debug.Log($"[AndroidPackageChecker] パッケージ '{packageId}' が見つかりました");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log($"[AndroidPackageChecker] パッケージ '{packageId}' は見つかりませんでした: {e.Message}");
            return false;
        }
#else
        // エディタやAndroid以外のプラットフォームでは常にfalse
        Debug.Log($"[AndroidPackageChecker] エディタまたは非Androidプラットフォームのため、パッケージチェックをスキップ: {packageId}");
        return false;
#endif
    }

    /// <summary>
    /// 指定されたパッケージIDのアプリを起動
    /// </summary>
    public static bool LaunchPackage(string packageId)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

            // アプリの起動Intent取得
            AndroidJavaObject launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", packageId);

            if (launchIntent != null)
            {
                currentActivity.Call("startActivity", launchIntent);
                Debug.Log($"[AndroidPackageChecker] パッケージ '{packageId}' を起動しました");
                return true;
            }
            else
            {
                Debug.LogWarning($"[AndroidPackageChecker] パッケージ '{packageId}' の起動Intentが取得できませんでした");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AndroidPackageChecker] パッケージ '{packageId}' の起動に失敗: {e.Message}");
            return false;
        }
#else
        Debug.Log($"[AndroidPackageChecker] エディタまたは非Androidプラットフォームのため、起動をスキップ: {packageId}");
        return false;
#endif
    }

    /// <summary>
    /// 特定のActivityを起動（パッケージID + アクティビティ名）
    /// </summary>
    public static bool LaunchActivity(string packageId, string activityClassName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Intentを作成
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
            AndroidJavaObject componentName = new AndroidJavaObject(
                "android.content.ComponentName",
                packageId,
                activityClassName
            );

            intent.Call<AndroidJavaObject>("setComponent", componentName);
            currentActivity.Call("startActivity", intent);

            Debug.Log($"[AndroidPackageChecker] Activity '{packageId}/{activityClassName}' を起動しました");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AndroidPackageChecker] Activity '{packageId}/{activityClassName}' の起動に失敗: {e.Message}");
            return false;
        }
#else
        Debug.Log($"[AndroidPackageChecker] エディタまたは非Androidプラットフォームのため、起動をスキップ: {packageId}/{activityClassName}");
        return false;
#endif
    }
}

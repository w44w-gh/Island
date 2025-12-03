using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// NTPサーバーからサーバー時刻を取得・管理するマネージャー
/// 端末の時刻改ざん対策として使用
/// </summary>
public class NTPTimeManager : MonoBehaviour
{
    private static NTPTimeManager _instance;
    public static NTPTimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("NTPTimeManager");
                _instance = go.AddComponent<NTPTimeManager>();
            }
            return _instance;
        }
    }

    // NTPサーバー（複数のバックアップを用意）
    private readonly string[] ntpServers = new string[]
    {
        "time.google.com",      // Googleの公開NTPサーバー
        "ntp.nict.jp",          // 日本の標準時サーバー
        "pool.ntp.org",         // NTP Pool Project
        "time.cloudflare.com"   // Cloudflareの公開NTPサーバー
    };

    // サーバー時刻とローカル時刻の差分（ミリ秒）
    private long serverTimestampOffset = 0;
    private bool isInitialized = false;
    private int currentServerIndex = 0;

    /// <summary>
    /// サーバー時刻が取得済みかどうか
    /// </summary>
    public bool IsReady => isInitialized;

    /// <summary>
    /// 現在のサーバー時刻を取得（日本時間 JST = UTC+9）
    /// </summary>
    public DateTime ServerTime
    {
        get
        {
            if (!isInitialized)
            {
                Debug.LogWarning("Server time not initialized yet. Returning local time.");
                return DateTime.UtcNow.AddHours(9);
            }

            // ローカル時刻 + サーバーとの差分 = サーバー時刻の推定値（UTC）
            long localMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long estimatedServerMillis = localMillis + serverTimestampOffset;

            // UTC時刻に9時間追加して日本時間に変換
            DateTime utcTime = DateTimeOffset.FromUnixTimeMilliseconds(estimatedServerMillis).DateTime;
            return utcTime.AddHours(9);
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
    /// 初期化（自動的に時刻取得）
    /// </summary>
    public async Task Initialize()
    {
        await RefreshServerTimeAsync();
    }

    /// <summary>
    /// サーバー時刻を再取得する（非同期）
    /// </summary>
    public async Task<bool> RefreshServerTimeAsync()
    {
        // すべてのNTPサーバーを試す
        for (int i = 0; i < ntpServers.Length; i++)
        {
            string server = ntpServers[(currentServerIndex + i) % ntpServers.Length];

            try
            {
                Debug.Log($"Attempting to sync time with NTP server: {server}");

                DateTime ntpTime = await GetNetworkTimeAsync(server);
                DateTime localTime = DateTime.UtcNow;

                // サーバー時刻とローカル時刻の差分を計算
                long ntpMillis = new DateTimeOffset(ntpTime).ToUnixTimeMilliseconds();
                long localMillis = new DateTimeOffset(localTime).ToUnixTimeMilliseconds();
                serverTimestampOffset = ntpMillis - localMillis;

                isInitialized = true;
                currentServerIndex = (currentServerIndex + i) % ntpServers.Length;

                Debug.Log($"NTP time synchronized with {server}");
                Debug.Log($"Server time: {ServerTime:yyyy/MM/dd HH:mm:ss}");
                Debug.Log($"Offset: {serverTimestampOffset}ms");

                return true;  // 成功
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to sync with {server}: {e.Message}");
            }
        }

        Debug.LogError("Failed to sync time with all NTP servers!");
        return false;  // 失敗
    }

    /// <summary>
    /// サーバー時刻を再取得する（同期的に開始、非ブロッキング）
    /// </summary>
    public void RefreshServerTime()
    {
        _ = RefreshServerTimeAsync();
    }

    /// <summary>
    /// NTPサーバーから時刻を取得
    /// </summary>
    private async Task<DateTime> GetNetworkTimeAsync(string ntpServer)
    {
        const int NTP_PORT = 123;
        const int TIMEOUT_MS = 5000;

        // NTPリクエストパケット（48バイト）
        byte[] ntpData = new byte[48];
        ntpData[0] = 0x1B; // LI = 0 (no warning), VN = 3 (IPv4), Mode = 3 (Client)

        IPAddress[] addresses = await Dns.GetHostAddressesAsync(ntpServer);
        IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], NTP_PORT);

        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            socket.ReceiveTimeout = TIMEOUT_MS;
            socket.SendTimeout = TIMEOUT_MS;

            await socket.ConnectAsync(ipEndPoint);
            await socket.SendAsync(new ArraySegment<byte>(ntpData), SocketFlags.None);

            ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(ntpData);
            await socket.ReceiveAsync(receiveBuffer, SocketFlags.None);

            // NTPタイムスタンプは40バイト目から8バイト（秒部分4バイト + 少数部分4バイト）
            ulong intPart = SwapEndianness(BitConverter.ToUInt32(ntpData, 40));
            ulong fractPart = SwapEndianness(BitConverter.ToUInt32(ntpData, 44));

            ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            // NTPタイムスタンプは1900年1月1日からの経過時間
            DateTime ntpEpoch = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return ntpEpoch.AddMilliseconds(milliseconds);
        }
    }

    /// <summary>
    /// エンディアン変換
    /// </summary>
    private uint SwapEndianness(uint x)
    {
        return ((x & 0x000000ff) << 24) +
               ((x & 0x0000ff00) << 8) +
               ((x & 0x00ff0000) >> 8) +
               ((x & 0xff000000) >> 24);
    }

    /// <summary>
    /// タイムスタンプ（Unix時刻ミリ秒）を取得
    /// </summary>
    public long GetServerTimestamp()
    {
        return new DateTimeOffset(ServerTime).ToUnixTimeMilliseconds();
    }
}

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ARSportsSync.Runtime;
using UnityEngine;

namespace ARSportsSync.Networking
{
    public sealed class RealtimePostClient: MonoBehaviour
    {
        [SerializeField] private string serverUrl = "ws://192.168.1.10:8080";
        [SerializeField] private RealtimePoseTarget target;
        [SerializeField] private bool connectOnStart = true;
        [SerializeField] private float pingIntervalSeconds = 1f;

        private readonly ConcurrentQueue<string> incoming = new ConcurrentQueue<string>();
        private ClientWebSocket socket;
        private CancellationTokenSource cancellation;

        private int messageCount;
        private int staleCount;
        private double lastRoundTripMs;
        private float nextPingTime;
        private string status = "Idle";

        public string Status => status;
        public int MessageCount => messageCount;
        public int StaleCount => staleCount;
        public double LastRoundTripMs => lastRoundTripMs;

        private async void Start()
        {
            if (connectOnStart)
            {
                await ConnectAsync();
            }
        }

        private async Task ConnectAsync()
        {
            if (socket != null && socket.State == WebSocketState.Open)
            {
                return;
            }

            cancellation = new CancellationTokenSource();
            socket = new ClientWebSocket();
            status = "Connecting";

            try
            {
                await socket.ConnectAsync(new Uri(serverUrl), cancellation.Token);
                status = "Connected";
                _ = Task.Run(() => ReceiveLoop(cancellation.Token));
            }
            catch (Exception ex)
            {
                status = $"Connect failed: {ex.Message}";
            }
        }

        private async Task ReceiveLoop(CancellationToken token)
        {
            byte[] buffer = new byte[8192];

            using MemoryStream stream = new MemoryStream();

            while (!token.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                ArraySegment<byte> segment = new ArraySegment<byte>(buffer);

                try
                {
                    result = await socket.ReceiveAsync(segment, token);                    
                }
                catch (Exception ex)
                {
                    status = $"Receive failed: {ex.Message}";
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    status = "Closed";
                    break;
                }

                stream.Write(buffer, 0, result.Count);

                if (!result.EndOfMessage)
                {
                    continue;
                }

                string json = Encoding.UTF8.GetString(stream.ToArray());
                stream.SetLength(0);
                incoming.Enqueue(json);
            }
        }

        private void Update()
        {
            if (socket != null && socket.State == WebSocketState.Open && Time.unscaledTime >= nextPingTime)
            {
                nextPingTime = Time.unscaledTime + pingIntervalSeconds;
                _ = SendPingAsync();
            }

            while (incoming.TryDequeue(out string json))
            {
                HandleMessage(json);
            }
        }

        private void HandleMessage(string json)
        {
            if (json.Contains("\"type\":\"pong\""))
            {
                PongMessage pong = JsonUtility.FromJson<PongMessage>(json);
                lastRoundTripMs = Time.realtimeSinceStartupAsDouble * 1000.0 - pong.clientTimeMs;
                return;
            }

            if (!json.Contains("\"type\":\"pose\""))
            {
                return;
            }

            PoseSnapshot snapshot = JsonUtility.FromJson<PoseSnapshot>(json);
            if (snapshot == null)
            {
                return;
            }

            messageCount++;

            if (target == null)
            {
                return;
            }

            bool applied = target.ApplySnapshot(snapshot);
            if (!applied)
            {
                staleCount++;
            }
        }

        private async Task SendPingAsync()
        {
            if (socket == null || socket.State != WebSocketState.Open)
            {
                return;
            }

            string json = $"{{\"type\":\"ping\",\"clientTimeMs\":{Time.realtimeSinceStartupAsDouble * 1000.0:0.###}}}";
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            try
            {
                await socket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    cancellation.Token
                );
            }
            catch (Exception ex)
            {
                status = $"Ping failed: {ex.Message}";
            }
        }

        private async void OnDestroy()
        {
            cancellation?.Cancel();

            if (socket != null && socket.State == WebSocketState.Open)
            {
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Unity client closing",
                    CancellationToken.None
                );
            }

            socket?.Dispose();
            cancellation?.Dispose();
        }                    
    }
}
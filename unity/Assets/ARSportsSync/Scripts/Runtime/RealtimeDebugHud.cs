using ARSportsSync.Networking;
using TMPro;
using UnityEngine;

namespace ARSportsSync.Runtime
{
    public sealed class RealtimeDebugHud : MonoBehaviour
    {
        [SerializeField] private RealtimePoseClient client;
        [SerializeField] private RealtimePoseTarget target;
        [SerializeField] private TMP_Text text;

        private float fps;

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            fps = Mathf.Lerp(
                fps,
                1f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f),
                0.1f
            );

            if (text == null || client == null)
            {
                return;
            }

            int seq = target == null ? -1 : target.LastSeq;

            text.text =
                $"Status: {client.Status}\n" +
                $"FPS: {fps:0}\n" +
                $"Messages: {client.MessageCount}\n" +
                $"Stale: {client.StaleCount}\n" +
                $"RTT: {client.LastRoundTripMs:0.0} ms\n" +
                $"Seq: {seq}";
        }
    }
}
using UnityEngine;

namespace PS1.Scripts.Network
{
    /// <summary>
    /// We send 16-bit packed shorts (48 bits!) not 32-bit floats (96 bits).
    /// Short range: [ -32,768 to 32,767 ]
    /// Pros:
    /// 1. Reduces bandwidth usage by 50% (48 bits vs 96 bits).
    /// 2. Improves network performance and reduces latency.
    /// Cons:
    /// 1. It allows only 0.01 unit precision.
    /// 2. A range of ±327.68 units.
    /// </summary>
    public struct PositionPayload
    {
        public short x;
        public short y;
        public short z;
    }

    public static class NetworkPositionSerializer
    {
        private const float QuantizationScale = 100f;

        public static PositionPayload Serialize(Vector3 position) => new PositionPayload
        {
            // --- DATA COMPRESSION LAYER (Bit-Packing Float to 16-bit Short) ---
            // ex: position.x is 327.681 | payload.x = (short)roundInt(327.681 * 100) => 32768
            x = (short)Mathf.RoundToInt(position.x * QuantizationScale),
            y = (short)Mathf.RoundToInt(position.y * QuantizationScale),
            z = (short)Mathf.RoundToInt(position.z * QuantizationScale)
        };

        public static Vector3 Deserialize(PositionPayload payload) => new Vector3
        (
            // --- DECOMPRESSION LAYER (Decode 16-bit Shorts back to 32-bit Float) ---
            // ex: payload.x is 32768 | position.x = 32768 / 100 => 327.68
            x: payload.x / QuantizationScale,
            y: payload.y / QuantizationScale,
            z: payload.z / QuantizationScale
        );
    }
}
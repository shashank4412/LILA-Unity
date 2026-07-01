using PS1.Scripts.Network;
using UnityEngine;

namespace PS1.Scripts.Movement
{
    /// <summary>
    /// Handles remote player movement by receiving network positions and interpolating between them for smooth visual updates.
    /// </summary>
    public class RemotePlayer : Player
    {
        [Header("Material Settings")]
        [SerializeField] private Material playerSkin;

        [Header("Offset Settings")]
        [SerializeField] private bool enableOffset = false;
        [SerializeField] private Vector3 offsetValue = Vector3.zero;

        [Header("Interpolation Settings")]
        [SerializeField] private bool enableInterpolation = true;

        [Range(0f, 1f)][SerializeField] private float interpolationSpeed = 0.3f; // Step-percentage value

        private Vector3 networkTargetPosition;

        public override void Init()
        {
            base.Init();
            GetComponent<Renderer>().material = playerSkin;
            networkTargetPosition = transform.position;

            // Apply the initial visual position shift instantly on spawn
            if (enableOffset) transform.position = networkTargetPosition + offsetValue;
        }

        private void Update()
        {
            if (!isReady) return;

            // 1. Resolve true target position by checking the local offset rules
            Vector3 finalTargetPos = enableOffset ? networkTargetPosition + offsetValue : networkTargetPosition;

            // 2. Process transform frames through the step configurations
            if (enableInterpolation)
            {
                // Decouples the 0-1 percentage step value from the client frame rate
                float blendStep = 1f - Mathf.Exp(-interpolationSpeed * Time.deltaTime * 60f);

                transform.position = Vector3.Lerp(transform.position, finalTargetPos, blendStep);
            }
            else
            {
                transform.position = finalTargetPos;
            }
        }

        public void SetNetworkPosition(PositionPayload payload)
        {
            // Deserialize the compressed network position
            networkTargetPosition = NetworkPositionSerializer.Deserialize(payload);
            Debug.Log($"[RECEIVED] Deserialized Target Pos: {networkTargetPosition}");
        }
    }
}
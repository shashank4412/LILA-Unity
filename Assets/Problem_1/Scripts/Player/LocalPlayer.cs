using System;
using PS1.Scripts.Network;
using UnityEngine;


namespace PS1.Scripts.Movement
{
    /// <summary>
    /// Handles local player movement input and broadcasts transform updates to the network.
    /// </summary>
    public class LocalPlayer : Player
    {
        [Header("Material Settings")]
        [SerializeField] private Material playerSkin;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f; // flat-velocity (m/sec)

        public override void Init()
        {
            base.Init();
            GetComponent<Renderer>().material = playerSkin;
        }

        private Vector3 direction;
        private Vector3 GetInput()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            return new Vector3(moveX, 0, moveZ);
        }
        private void Move(Vector3 direction)
        {
            Vector3 movement = direction * moveSpeed * Time.fixedDeltaTime;
            transform.position += movement;
        }

        private void Update()
        {
            if (!isReady) return;

            // 0. Get raw axis inputs
            direction = GetInput();
        }
        private void FixedUpdate()
        {
            if (direction == Vector3.zero) return;

            // 1. Accumulate movement vector based on requested velocity math
            Move(direction);

            // 2. Immediately broadcast clean transform data down the line
            BroadcastTransform();
        }

        public event Action<PositionPayload> OnPositionBroadcast;
        private void BroadcastTransform()
        {
            PositionPayload payload = NetworkPositionSerializer.Serialize(transform.position);
            OnPositionBroadcast?.Invoke(payload);

            // Calculation: 1short == 2bytes == 16bit | 3short => 48bits (less than 96 bits!)
            int dataSizeInBits = sizeof(short) * 3 * 8;
            Debug.Log($"[SENT] Raw Pos: {transform.position} | Packed Bits: {dataSizeInBits} bits | Payload: ({payload.x}, {payload.y}, {payload.z})");
        }
    }
}
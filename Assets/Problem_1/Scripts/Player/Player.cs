using UnityEngine;


namespace PS1.Scripts.Movement
{
    /// <summary>
    /// Handles player movement input and provides base functionality for both local and remote players.
    /// </summary>
    public abstract class Player : MonoBehaviour
    {
        protected bool isReady = false;

        protected virtual void Awake()
        {
            enabled = false;
        }

        public virtual void Init()
        {
            isReady = true;
            enabled = true;
        }

        public virtual void DeInit()
        {
            isReady = false;
            enabled = false;
        }
    }
}

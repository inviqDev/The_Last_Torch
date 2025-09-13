using System;
using UnityEngine;

namespace Runtime
{
    public class EnemyTriggerDetector : MonoBehaviour
    {
        public Action<Character> OnTriggerWithPlayer;
        
        [Header("Player model layer")]
        [SerializeField] private LayerMask playerLayerMask;

        private void OnTriggerEnter(Collider other)
        {
            if ((playerLayerMask.value & 1 << other.gameObject.layer) == 0) return;
            
            UnityEngine.Assertions.Assert.IsNotNull(GameManager.Instance?.Player);
            if (other.gameObject != GameManager.Instance?.Player.gameObject) return;
            
            var player = GameManager.Instance?.Player;
            UnityEngine.Assertions.Assert.IsNotNull(player, "Player is null");
            
            OnTriggerWithPlayer?.Invoke(player);
        }
    }
}

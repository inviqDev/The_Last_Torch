using System;
using UnityEngine;

namespace Runtime
{
    public class EnemyAttackTrigger : MonoBehaviour
    {
        public Action<PlayerModel> OnPlayerEnter;
        public Action<PlayerModel> OnPlayerExit;
        
        [Header("Layer masks")]
        [SerializeField] protected LayerMask playerLayerMask;

        [SerializeField] private EnemyModel enemyModel;
        [SerializeField] private Collider detectorCol;

        private PlayerModel _player;
        
        private void OnEnable()
        {
            enemyModel.OnConfigLoaded += OnConfigLoaded;
        }

        private void OnConfigLoaded(EnemyConfig config)
        {
            if (detectorCol is SphereCollider col)
            {
                col.radius = config.stoppingDistance;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((playerLayerMask.value & 1 << other.gameObject.layer) == 0) return;
            
            UnityEngine.Assertions.Assert.IsNotNull(GameManager.Instance?.Player);
            if (other.gameObject != GameManager.Instance?.Player.gameObject) return;
            
            _player = GameManager.Instance?.Player;
            UnityEngine.Assertions.Assert.IsNotNull(_player, "Player is null");
            
            OnPlayerEnter?.Invoke(_player);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!_player) return;
            if (other.gameObject != _player.gameObject) return;

            _player = null;
            OnPlayerExit?.Invoke(_player);
        }
    }
}
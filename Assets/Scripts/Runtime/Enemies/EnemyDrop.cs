using UnityEngine;

namespace Runtime
{
    public class EnemyDrop : MonoBehaviour, IPoolable
    {
        [Header("Unique Key in pool dictionary")]
        [SerializeField] protected string uniquePoolKey;
        public string UniquePoolKey => uniquePoolKey;
        
        [SerializeField] private LayerMask playerLayerMask;
        private float _expGained;
        
        private void OnTriggerEnter(Collider other)
        {
            if (OtherIsNotPlayer(other, out var player)) return;
            
            player.CollectExp(_expGained);
            gameObject.SetActive(false);
        }

        public void SetExpGainedAmount(float amountFromConfig)
        {
            _expGained = amountFromConfig;
        }
        
        private bool OtherIsNotPlayer(Collider other, out PlayerModel player)
        {
            if ((playerLayerMask.value & 1 << other.gameObject.layer) != 0)
            {
                if (other.transform.root.TryGetComponent(out player)) return false;
            }

            player = null;
            return true;
        }

        public void OnGetFromPool()
        {
            transform.SetParent(null);
            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            gameObject.SetActive(false);
            transform.SetParent(Pool.Instance?.transform);
        }
    }
}

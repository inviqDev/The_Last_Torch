using UnityEngine;

namespace Runtime
{
    public class EnemyDrop : MonoBehaviour, IPoolable
    {
        [Header("Unique Key in pool dictionary")] 
        [SerializeField] protected string uniquePoolKey;
        public string UniquePoolKey => uniquePoolKey;

        [SerializeField] private LayerMask playerLayerMask;

        private MeshRenderer meshRenderer;
        public MeshRenderer MeshRenderer => meshRenderer;
        
        private float _expGained;
        private float _healthBoost;
        private float _damageBoost;
        private float _moveSpeed;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (OtherIsNotPlayer(other, out var player)) return;

            player.CollectExp(_expGained);
            
            /*OMG DECISION*/ /*OMG DECISION*/ /*OMG DECISION*/ /*OMG DECISION*/
            player.ChangeStats(_healthBoost, _moveSpeed, _damageBoost);
            
            Pool.Instance?.ReturnToPool(this);
        }
        
        public void SetUpConfigValues(EnemyDropConfig dropConfig)
        {
            _expGained = dropConfig.expGained;
            _healthBoost = dropConfig.healthBoost;
            _damageBoost = dropConfig.damageBoost;
            _moveSpeed = dropConfig.moveSpeedBoost;
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
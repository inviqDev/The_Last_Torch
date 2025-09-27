using UnityEngine;

namespace Runtime
{
    public class EnemyDrop : MonoBehaviour, IPoolable
    {
        [Header("Unique Key in pool dictionary")] 
        [SerializeField] protected string uniquePoolKey;
        public string UniquePoolKey => uniquePoolKey;

        [SerializeField] private LayerMask playerLayerMask;
        public MeshRenderer MeshRenderer { get; private set; }
        
        private DropAnimation dropAnimation;

        private float _expGained;
        private float _healthBoost;
        private float _damageBoost;
        private float _moveSpeed;

        private void Awake()
        {
            MeshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (OtherIsNotPlayer(other, out var player)) return;

            player.CollectExp(_expGained);
            
            /*OMG DECISION*/ /*OMG DECISION*/ /*OMG DECISION*/ /*OMG DECISION*/
            player.ChangeStats(_healthBoost, _moveSpeed, _damageBoost);
            
            Pool.Instance?.ReturnToPool(this);
        }
        
        public void SetUpDropFromConfig(EnemyDropConfig dropConfig, Vector3 initPos)
        {
            MeshRenderer ??= GetComponent<MeshRenderer>();
            MyAssertions.EnsureIsNotNull(MeshRenderer);
            MeshRenderer.material = dropConfig.dropMaterial;
            
            dropAnimation ??= GetComponent<DropAnimation>();
            MyAssertions.EnsureIsNotNull(dropAnimation);
            dropAnimation.StartAnimation(initPos);
            
            _expGained = dropConfig.expGained;
            _healthBoost = dropConfig.healthBoost;
            _moveSpeed = dropConfig.moveSpeedBoost;
            _damageBoost = dropConfig.damageBoost;
        }

        private bool OtherIsNotPlayer(Collider other, out Player player)
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
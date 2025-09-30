using UnityEngine;

namespace Runtime
{
    public class EnemyDrop : MonoBehaviour, IPoolable
    {
        [Header("Unique Key in pool dictionary")] 
        [SerializeField] protected string uniquePoolKey;
        public string UniquePoolKey => uniquePoolKey;

        [SerializeField] private LayerMask playerLayerMask;
        
        private MeshRenderer _meshRenderer;
        private DropAnimation dropAnimation;

        private float _expGained;
        private float _healthBoost;
        private float _damageBoost;
        private float _moveSpeed;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        
        public void SetUpDropFromConfig(EnemyDropConfig dropConfig, Vector3 initPos)
        {
            transform.position = initPos;
            
            _meshRenderer ??= GetComponent<MeshRenderer>();
            UnityEngine.Assertions.Assert.IsNotNull(_meshRenderer,
                "mesh renderer component is missing");
            _meshRenderer.material = dropConfig.dropMaterial;
            
            dropAnimation ??= GetComponent<DropAnimation>();
            UnityEngine.Assertions.Assert.IsNotNull(dropAnimation,
                "drop animation renderer component is missing");
            dropAnimation.StartAnimation(true, true);
            
            _expGained = dropConfig.expGained;
            _healthBoost = dropConfig.healthBoost;
            _moveSpeed = dropConfig.moveSpeedBoost;
            _damageBoost = dropConfig.damageBoost;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (OtherIsNotPlayer(other, out var player)) return;
            
            /*OMG DECISION*/ /*OMG DECISION*/ /*OMG DECISION*/ /*OMG DECISION*/
            player.ChangeStats(_healthBoost, _moveSpeed, _damageBoost);
            player.CollectExp(_expGained);
            
            Pool.Instance?.ReturnToPool(this);
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
        
        private int _counter;
        public void OnGetFromPool()
        {
            transform.SetParent(null);
            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            transform.SetParent(Pool.Instance?.DropRoot);
            gameObject.SetActive(false);
        }
    }
}
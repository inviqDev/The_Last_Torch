using System;
using System.Collections;
using UnityEngine;

namespace Runtime
{
    public class Projectile : MonoBehaviour, IPoolable
    {
        public Action<PlayerModel> OnPlayerDamaged;
        public Action<Projectile> OnMoveToPool;

        [Header("Unique Key in pool dictionary")] 
        [SerializeField] private string uniquePoolKey;
        public string UniquePoolKey => uniquePoolKey;

        [Header("Player layer mask")] 
        [SerializeField] private LayerMask playerLayerMask;
        [SerializeField] private float moveSpeed = 12f;
        [SerializeField] private float lifeTime = 2f;

        private Coroutine _routine;
        private Vector3 _direction;

        public void LaunchProjectile(Vector3 origin, Vector3 targetPos)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            transform.position = origin;
            _direction = (targetPos - origin).normalized;

            if (_direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_direction);
            }

            _routine = StartCoroutine(FlyRoutine());
        }

        private IEnumerator FlyRoutine()
        {
            var t = 0f;
            while (t < lifeTime)
            {
                transform.Translate(_direction * (moveSpeed * Time.deltaTime), Space.World);
                t += Time.deltaTime;

                yield return null;
            }

            Pool.Instance?.ReturnToPool(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((playerLayerMask.value & (1 << other.gameObject.layer)) == 0) return;
            UnityEngine.Assertions.Assert.IsNotNull(GameManager.Instance?.Player);
            
            if (other.gameObject != GameManager.Instance?.Player.gameObject) return;
            
            var player = GameManager.Instance?.Player;
            UnityEngine.Assertions.Assert.IsNotNull(player, "Player is null");
            
            OnPlayerDamaged?.Invoke(player);
            Pool.Instance?.ReturnToPool(this);
        }

        public void ChangeProjectileBasicSettings(ProjectileSettings newSettings)
        {
            transform.localScale = Vector3.one * newSettings.sizeMultiplier;
            moveSpeed = newSettings.moveSpeed;
        }

        // ===== Pool =====

        public void OnGetFromPool()
        {
            transform.SetParent(null);
            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            OnMoveToPool?.Invoke(this);
            
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            transform.localScale = Vector3.one;
            gameObject.SetActive(false);
            transform.SetParent(Pool.Instance?.transform);
        }
    }
}
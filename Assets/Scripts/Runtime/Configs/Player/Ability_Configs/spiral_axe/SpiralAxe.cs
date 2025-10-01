using System.Collections;
using UnityEngine;

namespace Runtime
{
    public class SpiralAxe : AbilityVFX
    {
        [SerializeField] private DamageCollider _hitCollider;

        [Header("Spiral Settings")] [SerializeField]
        private float radiusGrowth; // how fast radius grows

        [SerializeField] private float moveSpeed; // angular speed (radians/sec)
        [SerializeField] private float lifeTime;

        [SerializeField] private bool useLocalForward = true;

        [SerializeField] private Particle trailParticles;
        [SerializeField] private Particle hitParticle;

        private Player _player;
        private Ability _ability;

        private IEnumerator _trailCoroutine;
        private Particle _pooledTrail;

        private void OnTriggerDetected(Character enemy)
        {
            if (!enemy) return;
            
            UnityEngine.Assertions.Assert.IsNotNull(hitParticle, $"particles {hitParticle} is missing");
            GameManager.Instance?.ParticlesManager?.PlayParticle(hitParticle.UniquePoolKey, enemy);
            
            enemy.TakeDamage(_ability.Damage);
        }

        protected override void OnPlay(AbilityContext ctx)
        {
            if (_trailCoroutine != null)
            {
                StopCoroutine(_trailCoroutine);
            }

            _trailCoroutine = SpiralRoutine(ctx);
            StartCoroutine(_trailCoroutine);
        }

        private IEnumerator SpiralRoutine(AbilityContext ctx)
        {
            _player ??= ctx.Player;
            _ability ??= ctx.Ability;

            if (!_player)
            {
                Debug.LogWarning("[SpiralAxe] Player is null in context.");
                RaiseFinished(ctx.Ability);
                yield break;
            }
            
            var startPosY = _player.transform.localPosition.y * 0.5f;
            var startPos = new Vector3(_player.transform.position.x, startPosY, _player.transform.position.z);
            
            var forwardDir = useLocalForward ? ctx.Player.transform.forward : Vector3.forward;
            forwardDir.y = 0;
            forwardDir.Normalize();
            
            var rightDir = Vector3.Cross(Vector3.up, forwardDir).normalized;

            _pooledTrail = GameManager.Instance?.ParticlesManager?.PlayParticle(trailParticles.UniquePoolKey);
            if (_pooledTrail)
            {
                var mainModule = _pooledTrail.ParticleSystem.main;
                mainModule.stopAction = ParticleSystemStopAction.Callback;
                mainModule.startLifetime = lifeTime;
                mainModule.loop = true;
            }
            
            var elapsed = 0f;
            while (elapsed < lifeTime)
            {
                elapsed += Time.deltaTime;

                var angle = elapsed * moveSpeed;
                var radius = elapsed * radiusGrowth;

                // spiral offset around startPos in horizontal plane
                var offsetX = Mathf.Cos(angle) * radius;
                var offsetZ = Mathf.Sin(angle) * radius;

                // build offset in world space relative to chosen forward
                var offset = rightDir * offsetX + forwardDir * offsetZ;

                // final pos = center + offset, Y fixed
                var pos = startPos + offset;
                pos.y = startPos.y;

                transform.position = pos;

                if (_pooledTrail)
                {
                    _pooledTrail.transform.position = pos;
                }
                
                yield return null;
            }

            if (_pooledTrail)
            {
                var mainModule = _pooledTrail.ParticleSystem.main;
                mainModule.loop = false;
                _pooledTrail.ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                _pooledTrail = null;
            }

            _trailCoroutine = null;
            RaiseFinished(ctx.Ability);
        }
        
        private void OnEnable()
        {
            UnityEngine.Assertions.Assert.IsNotNull(trailParticles, $"particles {trailParticles} is missing");
            UnityEngine.Assertions.Assert.IsNotNull(hitParticle, $"particles {hitParticle} is missing");

            if (!_hitCollider) return;
            
            _hitCollider.OnTriggerDetected -= OnTriggerDetected;
            _hitCollider.OnTriggerDetected += OnTriggerDetected;
        }

        private void OnDisable()
        {
            if (_trailCoroutine != null)
            {
                StopCoroutine(_trailCoroutine);
                _trailCoroutine = null;
            }
            
            if (!_hitCollider) return;
            
            _hitCollider.OnTriggerDetected -= OnTriggerDetected;
        }
    }
}
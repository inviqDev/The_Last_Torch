using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Particle : MonoBehaviour, IPoolable
    {
        [Header("Unique Key in pool dictionary")] [SerializeField]
        private string uniquePoolKey;
        
        [SerializeField] private float scaleMultiplier = 1f;

        private ParticleSystem _particleSystem;
        private Character _particleOwner;

        public string UniquePoolKey => uniquePoolKey;

        public void PlayParticleEffect(Character owner = null)
        {
            _particleSystem ??= GetComponent<ParticleSystem>();

            if (!_particleSystem)
            {
                UnityEngine.Assertions.Assert.IsNotNull(_particleSystem,
                    $"{uniquePoolKey} particle system is missing");
                Pool.Instance?.ReturnToPool(this);
                return;
            }

            if (owner)
            {
                _particleOwner = owner;
                
                _particleSystem.transform.parent = _particleOwner.transform;
                transform.localPosition = Vector3.zero;
                transform.localScale = _particleOwner.transform.localScale * scaleMultiplier;
                
                _particleOwner.OnCharacterDeath += c => _particleSystem.transform.parent = null;
            }
            else
            {
                _particleSystem.transform.parent = null;
            }

            _particleSystem.Play();
        }

        private void OnParticleSystemStopped()
        {
            _particleOwner.OnCharacterDeath -= c => _particleSystem.transform.parent = null;
            
            Pool.Instance?.ReturnToPool(this);
        }

        public void OnGetFromPool()
        {
            _particleSystem ??= GetComponent<ParticleSystem>();

            if (!_particleSystem)
            {
                UnityEngine.Assertions.Assert.IsNotNull(_particleSystem,
                    $"{gameObject.name} : particle system {uniquePoolKey} is missing");

                Pool.Instance?.ReturnToPool(this);
                return;
            }

            _particleSystem.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear);
            _particleSystem.Clear(true);

            var main = _particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;

            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_particleSystem,
                $"{gameObject.name} : particle system {uniquePoolKey} is missing");
            if (!_particleSystem) return;

            transform.SetParent(Pool.Instance?.ParticlesRoot);
            gameObject.SetActive(false);
        }
    }
}
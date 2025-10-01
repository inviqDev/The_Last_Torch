using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Particle : MonoBehaviour, IPoolable
    {
        [Header("Unique Key in pool dictionary")]
        [SerializeField] private string uniquePoolKey;
        
        [SerializeField] private float scaleMultiplier = 1f;

        private Character _particleOwner;

        public string UniquePoolKey => uniquePoolKey;
        public ParticleSystem ParticleSystem { get; private set; }

        public void PlayParticleEffect(Character owner = null)
        {
            ParticleSystem ??= GetComponent<ParticleSystem>();
            
            if (!ParticleSystem)
            {
                UnityEngine.Assertions.Assert.IsNotNull(ParticleSystem, $"{uniquePoolKey} particle system is missing");
                
                Pool.Instance?.ReturnToPool(this);
                return;
            }

            var mainModule = ParticleSystem.main;
            mainModule.stopAction = ParticleSystemStopAction.Callback;
            
            transform.SetParent(null);
            
            if (owner)
            {
                _particleOwner = owner;
                
                transform.SetParent(_particleOwner.transform);
                transform.localPosition = Vector3.zero;
                transform.localScale = _particleOwner.transform.localScale * scaleMultiplier;
                
                _particleOwner.OnCharacterDeath -= UnparentParticleOnOwnerDeath;
                _particleOwner.OnCharacterDeath += UnparentParticleOnOwnerDeath;
            }
            else
            {
                transform.localScale = Vector3.one * scaleMultiplier;
            }

            ParticleSystem.Play();
        }

        private void UnparentParticleOnOwnerDeath(Character owner)
        {
            transform.SetParent(null);
        }

        private void OnParticleSystemStopped()
        {
            if (_particleOwner)
            {
                _particleOwner.OnCharacterDeath -= UnparentParticleOnOwnerDeath;
                _particleOwner = null;
            }
            
            Pool.Instance?.ReturnToPool(this);
        }

        public void OnGetFromPool()
        {
            ParticleSystem ??= GetComponent<ParticleSystem>();
            
            if (!ParticleSystem)
            {
                UnityEngine.Assertions.Assert.IsNotNull(ParticleSystem,
                    $"{gameObject.name}  :  particle system {uniquePoolKey} is missing");

                Pool.Instance?.ReturnToPool(this);
                return;
            }

            if (_particleOwner)
            {
                _particleOwner.OnCharacterDeath -= UnparentParticleOnOwnerDeath;
                _particleOwner = null;
            }

            ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            UnityEngine.Assertions.Assert.IsNotNull(ParticleSystem,
                $"{gameObject.name} : particle system {uniquePoolKey} is missing");
            if (!ParticleSystem) return;

            ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            
            transform.SetParent(Pool.Instance?.ParticlesRoot);
            gameObject.SetActive(false); 
        }
    }
}
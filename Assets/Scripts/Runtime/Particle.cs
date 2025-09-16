using System;
using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Particle : MonoBehaviour, IPoolable
    {
        [Header("Unique Key in pool dictionary")] 
        [SerializeField] private string uniquePoolKey;
        
        private ParticleSystem _particleSystem;

        public string UniquePoolKey => uniquePoolKey;

        public void PlayStaticParticleEffect(Vector3 position)
        {
            UnityEngine.Assertions.Assert.IsNotNull(_particleSystem, "particle system component is missing");
            
            transform.position = position;
            _particleSystem.Play();
        }
        
        public void PlayMovableParticleEffect(Character owner)
        {
            _particleSystem ??= GetComponent<ParticleSystem>();
            
            if (!_particleSystem)
            {
                UnityEngine.Assertions.Assert.IsNotNull(_particleSystem, 
                    $"{uniquePoolKey} particle system is missing");
                Pool.Instance?.ReturnToPool(this);
                return;
            }
            
            _particleSystem.gameObject.transform.SetParent(owner.transform);
            transform.localPosition = Vector3.zero;
            
            owner.OnCharacterDeath -= OnCharacterDeath;
            owner.OnCharacterDeath += OnCharacterDeath;
            
            _particleSystem.Play();
        }

        private void OnCharacterDeath(Character owner)
        {
            Pool.Instance?.ReturnToPool(this);
            owner.OnCharacterDeath -= OnCharacterDeath;
        }

        private void OnParticleSystemStopped()
        {
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
            main.loop = false;
            
            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_particleSystem, 
                $"{gameObject.name} : particle system {uniquePoolKey} is missing");
            if (!_particleSystem) return;
            
            gameObject.SetActive(false);
            transform.SetParent(Pool.Instance?.ParticlesRoot);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class ParticlesManager : Singleton<ParticlesManager>
    {
        [SerializeField] private List<Particle> particles;
        
        private Dictionary<string, Particle> _particlesDictionary;

        public void Init()
        {
            _particlesDictionary = new Dictionary<string, Particle>();
            foreach (var p in particles)
            {
                var key = p.UniquePoolKey;
                _particlesDictionary.Add(key, p);
            }
        }

        public void PlayParticle(string uniquePoolKey, Character owner)
        {
            if (!_particlesDictionary.TryGetValue(uniquePoolKey, out var p))
            {
                UnityEngine.Assertions.Assert.IsNotNull(p, $"particle {uniquePoolKey} is not found");
                return;
            }

            var particle = Pool.Instance?.TryGetObjectFromPool(p);
            particle?.PlayMovableParticleEffect(owner);
        }
    }
}

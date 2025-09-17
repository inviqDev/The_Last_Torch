using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private List<Sound> sounds;
        
        private Dictionary<string, Sound> _soundDictionary;

        public void Init()
        {
            _soundDictionary = new Dictionary<string, Sound>();
            foreach (var s in sounds)
            {
                var key = s.UniquePoolKey;
                _soundDictionary.Add(key, s);
            }
        }

        public void PlaySound(string uniquePoolKey, Vector3 position)
        {
            if (!_soundDictionary.TryGetValue(uniquePoolKey, out var s))
            {
                UnityEngine.Assertions.Assert.IsNotNull(s, "SoundManager: sound is not found");
                return;
            }

            var sound = Pool.Instance?.TryGetObjectFromPool(s);
            sound?.PlayAudioClip(position);
        }
    }
}
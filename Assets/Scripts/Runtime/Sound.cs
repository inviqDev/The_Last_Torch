using System.Collections;
using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(AudioSource))]
    public class Sound : MonoBehaviour, IPoolable
    {
        [Header("Unique Key in pool dictionary")] 
        [SerializeField] private string uniquePoolKey;

        [Header("Trim / Envelope (normalized)")]
        [Tooltip("Какую часть клипа проигрывать [0..1]. Например 0.35 = 35% длины клипа.")]
        [SerializeField, Range(0f, 1f)] private float playablePartInPercents = 0.35f;

        [Tooltip("Доля окна, которую держим на полном уровне, остальное — затухание.")]
        [SerializeField, Range(0f, 1f)] private float peakHoldPercent = 0.10f;

        [Tooltip("Произвольная кривая затухания от 1 (начало фейда) до 0 (конец). Если пусто — линейная.")]
        [SerializeField] private AnimationCurve fadeCurve;

        [SerializeField] private AudioClip audioClip;
        [SerializeField] private bool playFullClip; 

        private AudioSource _audioSource;
        private IEnumerator _playRoutine;
        private float _baseVolume;

        public string UniquePoolKey => uniquePoolKey;
        
        public void PlayAudioClip(Vector3 position)
        {
            UnityEngine.Assertions.Assert.IsNotNull(_audioSource, "audio source component is missing");
            
            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
            }
            
            ConfigureAudioSource(position);
            
            if (_audioSource.loop)
            {
                _audioSource.Play();
            }
            else
            {
                _playRoutine = playFullClip ? PlayFullClipLength() : PlayPartOfAudioClip();
                StartCoroutine(_playRoutine);
            }
        }

        private IEnumerator PlayFullClipLength()
        {
            var clipLength = _audioSource.clip.length;
            yield return new WaitForSeconds(clipLength);
            
            StopAndRelease();
        }
        
        private IEnumerator PlayPartOfAudioClip()
        {
            var baseVol = Mathf.Max(0f, _baseVolume);
            var totalClipLength = _audioSource.clip.length;
            var playableLength = Mathf.Clamp01(playablePartInPercents) * totalClipLength;

            if (playableLength < 0.01f)
            {
                UnityEngine.Assertions.Assert.IsTrue(playableLength >= 0.01f, "playable clip part is too small");
                
                StopAndRelease();
                yield break;
            }

            var loudPart = Mathf.Clamp01(peakHoldPercent) * playableLength;
            var fadeDuration = Mathf.Max(0f, playableLength - loudPart);

            
            _audioSource.Play();

            var t = 0f;
            while (t < loudPart)
            {
                if (!_audioSource.isPlaying)
                {
                    StopAndRelease(); 
                    yield break;
                }
                
                t += Time.deltaTime;
                yield return null;
            }

            t = 0f;
            while (t < fadeDuration)
            {
                if (!_audioSource.isPlaying)
                {
                    StopAndRelease(); 
                    yield break;
                }
                
                t += Time.deltaTime;

                var norm = fadeDuration > 0.0001f 
                    ? Mathf.Clamp01(t / fadeDuration) 
                    : 1f;
                
                var env = fadeCurve != null && fadeCurve.length > 0 
                    ? Mathf.Clamp01(fadeCurve.Evaluate(norm)) 
                    : (1f - norm);
                
                _audioSource.volume = baseVol * env;

                yield return null;
            }

            _audioSource.volume = 0f;
            _audioSource.Stop();

            StopAndRelease();
        }
        
        private void ConfigureAudioSource(Vector3 position)
        {
            if (!audioClip)
            {
                UnityEngine.Assertions.Assert.IsNotNull(audioClip, $"{uniquePoolKey} clip is not found");
                Pool.Instance.ReturnToPool(this);
                return;
            }
            
            _audioSource ??= GetComponent<AudioSource>();
            _audioSource.clip ??= audioClip;
            
            _audioSource.time = 0f;
            _baseVolume = _audioSource.volume;
            _audioSource.playOnAwake = false;
            transform.position = position;
            
            if (fadeCurve == null || fadeCurve.length == 0)
            {
                fadeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
            }
        }
        
        private void StopAndRelease()
        {
            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
                _playRoutine = null;
            }
            
            _audioSource.Stop();
            _audioSource.volume = _baseVolume;
            
            Pool.Instance.ReturnToPool(this);
        }

        public void OnGetFromPool()
        {
            _audioSource ??= GetComponent<AudioSource>();
            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            gameObject.SetActive(false);
            transform.SetParent(Pool.Instance?.SoundsRoot);
        }
    }
}
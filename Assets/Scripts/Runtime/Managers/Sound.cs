using System.Collections;
using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(AudioSource))]
    public class Sound : MonoBehaviour, IPoolable
    {
        [Header("Unique Key in pool dictionary")] [SerializeField]
        private string uniquePoolKey;

        [Header("Trim / Envelope (normalized)")]
        [Tooltip("Какую часть клипа проигрывать [0..1]. Например 0.35 = 35% длины клипа.")]
        [SerializeField, Range(0f, 1f)] private float playablePartInPercents = 0.35f;

        [Tooltip("Доля окна, которую держим на полном уровне, остальное — затухание.")]
        [SerializeField, Range(0f, 1f)] private float peakHoldPercent = 0.10f;

        [Tooltip("Произвольная кривая затухания от 1 (начало фейда) до 0 (конец). Если пусто — линейная.")]
        [SerializeField] private AnimationCurve fadeCurve;

        [SerializeField] private AudioClip audioClip;
        [SerializeField] private bool playFullClip; 

        private AudioSource audioSource;
        private IEnumerator _playRoutine;
        private float _baseVolume;

        public string UniquePoolKey => uniquePoolKey;
        
        public void Play(Vector3 position)
        {
            UnityEngine.Assertions.Assert.IsNotNull(audioSource, "audioSource component is missing");
            
            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
            }
            
            ConfigureAudioSource(position);
            
            if (audioSource.loop)
            {
                audioSource.Play();
            }
            else
            {
                _playRoutine = playFullClip ? PlayFullClipLength() : PlayPartOfAudioClip();
                StartCoroutine(_playRoutine);
            }
        }

        private IEnumerator PlayFullClipLength()
        {
            var clipLength = audioSource.clip.length;
            yield return new WaitForSeconds(clipLength);
            
            StopAndRelease();
        }
        
        private IEnumerator PlayPartOfAudioClip()
        {
            var baseVol = Mathf.Max(0f, _baseVolume);
            var totalClipLength = audioSource.clip.length;
            var playableLength = Mathf.Clamp01(playablePartInPercents) * totalClipLength;

            if (playableLength < 0.01f)
            {
                UnityEngine.Assertions.Assert.IsTrue(playableLength >= 0.01f, "playable clip part is too small");
                
                StopAndRelease();
                yield break;
            }

            var loudPart = Mathf.Clamp01(peakHoldPercent) * playableLength;
            var fadeDuration = Mathf.Max(0f, playableLength - loudPart);

            
            audioSource.Play();

            var t = 0f;
            while (t < loudPart)
            {
                if (!audioSource.isPlaying)
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
                if (!audioSource.isPlaying)
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
                
                audioSource.volume = baseVol * env;

                yield return null;
            }

            audioSource.volume = 0f;
            audioSource.Stop();

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
            
            audioSource ??= GetComponent<AudioSource>();
            audioSource.clip ??= audioClip;
            
            audioSource.time = 0f;
            _baseVolume = audioSource.volume;
            audioSource.playOnAwake = false;
            transform.position = position;
            
            if (fadeCurve == null || fadeCurve.length == 0)
            {
                fadeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
            }
        }
        
        private void StopAndRelease()
        {
            audioSource.Stop();
            audioSource.volume = _baseVolume;
            gameObject.transform.parent = Pool.Instance.transform;
            
            Pool.Instance.ReturnToPool(this);
        }

        public void OnGetFromPool()
        {
            audioSource ??= GetComponent<AudioSource>();
            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
                _playRoutine = null;
            }

            gameObject.SetActive(false);
        }
    }
}
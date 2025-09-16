using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

using MyAsserts = UnityEngine.Assertions.Assert;

namespace Runtime
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingOverlay : MonoBehaviour
    {
        [SerializeField] private int sortingOrder = 10000;

        [Header("UI references")] 
        [SerializeField] private Slider sliderProgressBar;

        [SerializeField] private Image circleProgressBar;
        [SerializeField] private TextMeshProUGUI percentsText;

        [SerializeField] private Canvas canvas;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            RunSecureAssertions();
            SetDefaultValues();
        }

        private void RunSecureAssertions()
        {
            MyAsserts.IsNotNull(sliderProgressBar, "slider progress bar is missing");
            MyAsserts.IsNotNull(circleProgressBar, "circle progress bar image is missing");
            MyAsserts.IsNotNull(percentsText, "loading percents text field is missing");
        }

        public void SetDefaultValues()
        {
            RunSecureAssertions();

            canvas.overrideSorting = true;
            canvas.sortingOrder = sortingOrder;

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 0f;

            var startLoadingValue = 0f;
            SetProgress(startLoadingValue);
        }

        public void SetProgress(float value)
        {
            var v = Mathf.Clamp01(value);
            if (sliderProgressBar)
            {
                sliderProgressBar.minValue = 0f;
                sliderProgressBar.maxValue = 1f;
                sliderProgressBar.value = v;
            }

            circleProgressBar.fillAmount = v;
            percentsText.text = $"{Mathf.RoundToInt(v * 100f)}%";

        }

        public IEnumerator ShowLoadingProgress(float duration = 0.25f)
        {
            if (!_canvasGroup)
            {
                yield break;
            }

            _canvasGroup.blocksRaycasts = true;

            var t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);
                
                yield return null;
            }

            _canvasGroup.alpha = 1f;
        }

        public IEnumerator Hide(float duration = 0.25f)
        {
            if (!_canvasGroup)
            {
                yield break;
            }

            var t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
                
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}
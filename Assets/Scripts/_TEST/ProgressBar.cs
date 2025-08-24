using UnityEngine;
using UnityEngine.UI;

public abstract class ProgressBar : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image fillArea;
    private readonly float _defaultFillAmountValue = 0.0f;

    protected virtual void Start()
    {
        HideProgressBar();
    }

    protected void ShowProgressBar(float currentFillAmount)
    {
        if (canvas.isActiveAndEnabled) return;
        
        canvas.gameObject.SetActive(true);
        fillArea.fillAmount = currentFillAmount;
    }

    protected void HideProgressBar()
    {
        fillArea.fillAmount = _defaultFillAmountValue;
        canvas.gameObject.SetActive(false);
    }

    protected void FillProgressBar(float fillAmount) => fillArea.fillAmount = fillAmount;
}
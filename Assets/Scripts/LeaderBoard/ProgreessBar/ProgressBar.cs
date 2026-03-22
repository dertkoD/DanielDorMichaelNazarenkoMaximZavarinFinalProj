using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image progressImage; 
    [SerializeField] private float progressStep = 0.17f; 

    private float currentProgress = 0f;

    private void Start()
    {
        UpdateBar();
    }

    public void AddProgress()
    {
        currentProgress += progressStep;
        currentProgress = Mathf.Clamp01(currentProgress);

        UpdateBar();
    }

    private void UpdateBar()
    {
        progressImage.fillAmount = currentProgress;
    }
}

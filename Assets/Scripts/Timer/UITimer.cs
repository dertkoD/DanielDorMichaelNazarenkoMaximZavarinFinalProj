using TMPro;
using UnityEngine;

public class UITimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    public void UpdateTimerUI(float newTime)
    {
        float timeAsInt = Mathf.FloorToInt(newTime);
        timerText.text = timeAsInt.ToString();
    }
    
}

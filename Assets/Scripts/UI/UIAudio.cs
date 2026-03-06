using UnityEngine;
using UnityEngine.UI;

public class UIAudio : MonoBehaviour
{
    
    [Header("Sliders")] 
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    
    
    public void ChangeGeneralUISlider(float newValue)
    {
        volumeSlider.SetValueWithoutNotify(1 - (newValue / -40f));
    }
    public void ChangeMusicUISlider(float newValue)
    {
        musicSlider.SetValueWithoutNotify(1 - (newValue / -40f));
    }
    public void ChangeSoundUISlider(float newValue)
    {
        soundSlider.SetValueWithoutNotify(1 - (newValue / -40f));
    }
    
}

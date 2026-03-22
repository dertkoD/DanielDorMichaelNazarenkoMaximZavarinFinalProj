using UnityEngine;

public class FinishTransition : MonoBehaviour
{
    const string SHOOTER_SCENE_NAME = "ShootScene";
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private Timer _timer;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            progressBar.AddProgress();
            _timer.StopTimerOnFinish();
            SceneTransitionManager.Instance.TransitionToScene(SHOOTER_SCENE_NAME);
        }
    }
}

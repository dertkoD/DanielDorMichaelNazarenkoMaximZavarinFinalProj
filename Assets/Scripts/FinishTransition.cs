using UnityEngine;

public class FinishTransition : MonoBehaviour
{
    const string SHOOTER_SCENE_NAME = "ShootScene";
    [SerializeField] private ProgressBar progressBar;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            progressBar.AddProgress();
            SceneTransitionManager.Instance.TransitionToScene(SHOOTER_SCENE_NAME);
        }
    }
}

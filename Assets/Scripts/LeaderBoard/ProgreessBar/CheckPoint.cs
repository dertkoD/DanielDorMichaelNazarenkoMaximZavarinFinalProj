using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private ProgressBar progressBar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            progressBar.AddProgress();
            gameObject.SetActive(false); 
        }
    }
}

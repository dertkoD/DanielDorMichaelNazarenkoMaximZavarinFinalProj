using UnityEngine;

public class DoorsDisabler : MonoBehaviour
{
    [SerializeField] private GameObject doors;
    private void Awake()
    {
        if (doors)
        {
            doors.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (doors)
        {
            doors.SetActive(false);
        }
    }
}

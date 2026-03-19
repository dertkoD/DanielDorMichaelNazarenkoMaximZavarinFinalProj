using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private string playerName = "DefaultName";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    
    public void SetPlayerName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
            playerName = name;
    }
}

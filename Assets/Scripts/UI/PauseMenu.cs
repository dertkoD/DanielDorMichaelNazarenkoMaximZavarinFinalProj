using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")] [SerializeField] private GameObject pauseMenuRoot;

    [Header("Scenes")] [SerializeField] private string mainMenuSceneName = "MainMenu";
    
    [Header("Input System")] [SerializeField] private InputActionReference pauseAction;

    private bool isPaused;

    [SerializeField] private Timer _timer;

    private void Start()
    {
        ResumeGame();
    }

    private void OnEnable()
    {
        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.performed += OnPausePressed;
            pauseAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.performed -= OnPausePressed;
            pauseAction.action.Disable();
        }
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        
        if (_timer)
            _timer.StopTimer();

        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(false);

        if (_timer)
            _timer.ResumeTimer();
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        UnpauseBeforeSceneChange();

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.TransitionToScene(currentSceneName);
        else
            SceneManager.LoadScene(currentSceneName);
    }

    public void LoadMainMenu()
    {
        UnpauseBeforeSceneChange();

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.TransitionToScene(mainMenuSceneName);
        else
            SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenSettings()
    {

    }

    public void QuitGame()
    {
        UnpauseBeforeSceneChange();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void UnpauseBeforeSceneChange()
    {
        isPaused = false;

        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(false);

        Time.timeScale = 1f;
    }
}
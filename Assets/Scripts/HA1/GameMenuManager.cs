using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject endMenuPanel;

    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private TextMeshProUGUI victoryText;

    [Header("Events")]
    [SerializeField] private LevelResultActionChannelSO levelResultAction;

    [Header("Game Systems")]
    [SerializeField] private CursorAgentMovement raceManager;
    [SerializeField] private CameraRail cameraRail;

    private bool isGameStarted;
    private string sceneName;

    private void OnEnable()
    {
        if (levelResultAction != null)
            levelResultAction.OnEvent += OnLevelResult;
    }

    private void OnDisable()
    {
        if (levelResultAction != null)
            levelResultAction.OnEvent -= OnLevelResult;
    }

    private void Start()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        isGameStarted = false;

        if (startMenuPanel != null) startMenuPanel.SetActive(true);
        if (endMenuPanel != null) endMenuPanel.SetActive(false);

        if (victoryText != null)
            victoryText.text = "";

        if (raceManager != null)
            raceManager.SetGameActive(false);

        if (cameraRail != null)
            cameraRail.SetGameActive(false);

        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        SetInstructionsText();
    }

    private void OnLevelResult(bool isVictory)
    {
        if (victoryText != null)
            victoryText.text = isVictory ? "You Win!" : "You Lose!";

        if (endMenuPanel != null)
            endMenuPanel.SetActive(true);

        if (raceManager != null)
            raceManager.SetGameActive(false);

        if (cameraRail != null)
            cameraRail.SetGameActive(false);
    }

    public void StartGame()
    {
        if (isGameStarted) return;

        isGameStarted = true;

        if (startMenuPanel != null)
            startMenuPanel.SetActive(false);

        if (victoryText != null)
            victoryText.text = "";

        if (raceManager != null)
            raceManager.SetGameActive(true);

        if (cameraRail != null)
            cameraRail.SetGameActive(true);
    }

    public void RestartGame()
    {
        sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    private void SetInstructionsText()
    {
        if (instructionsText == null) return;

        instructionsText.text =
            "OBJECTIVE:\n" +
            "Defeat all enemies and survive.\n\n" +
            "Click START to begin";
    }
}
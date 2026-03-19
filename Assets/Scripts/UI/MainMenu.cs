using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Text warningText;

    private const int MaxNameLength = 12;
    private static readonly Regex validNameRegex = new Regex("^[a-zA-Z0-9]+$");

    public void StartNewGame()
    {
        string enteredName = nameInputField.text.Trim();

        if (enteredName.Length > MaxNameLength ||
            !validNameRegex.IsMatch(enteredName))
        {
            warningText.text = "Name must be letters/numbers only (max 12).";
        }
        else
        {
            warningText.text = "";
            GameManager.Instance.SetPlayerName(enteredName);
            SceneTransitionManager.Instance.TransitionToScene("MainScene");
            //GameManager.Instance.SetElapsedTime(0f);
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;


    public string GetPlayerName()
    {
        return nameInput.text;
    }

    public void CleanInput()
    {
        nameInput.text = "";
    }
    
}

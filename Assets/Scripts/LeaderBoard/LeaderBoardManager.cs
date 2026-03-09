using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class LeaderBoardManager : MonoBehaviour
{
    private Dictionary<string, float> scoreTable = new Dictionary<string, float>();
    [SerializeField] private PlayerNameInput _playerNameInput;
    [SerializeField] private UILeaderBoardManager uiLeader;
    [SerializeField] private SaveLeaderBoard saveSystem;

    private string lastName = "";
    
    
    void Start()
    {
        scoreTable = saveSystem.LoadLeaderBoardData();
        
        if (!string.IsNullOrEmpty(GameSession.CurrentPlayerName))
        {
            SavePlayerScoreInDictionary(
                GameSession.CurrentPlayerName,
                GameSession.CurrentPlayerScore
            );
        }
        
        uiLeader.ShowLeaderboard(scoreTable);
    }
    
    public void SaveNewNameInDictionary()
    {
        string playerName = _playerNameInput.GetPlayerName().ToLower();

        GameSession.CurrentPlayerName = playerName;
        GameSession.CurrentPlayerScore = 0;
        //lastName = playerName;
        _playerNameInput.CleanInput();
        
        uiLeader.UpdateCurrentPlayerStats(playerName,  GameSession.CurrentPlayerScore);
        
        if (!scoreTable.ContainsKey(playerName))
        {
            scoreTable.Add(playerName, 0);
            
            saveSystem.SaveLeaderBoardData(scoreTable);
            
            uiLeader.AddNewMemberToLeaderBoard(scoreTable);
        }
    }
    
    private void SavePlayerScoreInDictionary(string playerName, float score)
    {
        if (score > scoreTable[playerName])
        {
            scoreTable[playerName] = score;
        }

        saveSystem.SaveLeaderBoardData(scoreTable);
        
        uiLeader.UpdateCurrentPlayerStats(playerName, scoreTable[playerName]);
        uiLeader.ShowLeaderboard(scoreTable);
    }

    // private void Update()
    // {
    //     if (Keyboard.current.spaceKey.wasPressedThisFrame)
    //     {
    //         uiLeader.ShowLeaderboard(scoreTable);
    //     }
    // }
    
    
    // For tests
    // public void AddTestScore(float testvalue)
    // {
    //     currentPLayerScore += testvalue;
    //     SavePlayerScoreInDictionary(lastName, currentPLayerScore);
    //     uiLeader.UpdateCurrentPlayerStats(lastName, currentPLayerScore);
    // }

    public void ReloadScene()
    {
        SceneManager.LoadScene("M_Test");
    }


    public void CleanTheLeaderBoard()
    {
        scoreTable.Clear();
        saveSystem.ClearLeaderBoardData();
        
        uiLeader.ShowLeaderboard(scoreTable);
    }
    
    
}

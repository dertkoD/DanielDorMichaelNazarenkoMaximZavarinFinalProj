using TMPro;
using UnityEngine;

public class LeaderBoardUIRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI placeText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void SetData(int place, string playerName, float score)
    {
        placeText.text = place.ToString();
        nameText.text = playerName;
        scoreText.text = score.ToString();
    }
}

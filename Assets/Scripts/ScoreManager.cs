using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] PlayerData data;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;

    private void Start()
    {
        highScoreText.text = data.highScore.ToString();
        data.score = 0;
    }

    private void Update()
    {
        scoreText.text = data.score.ToString();
        if (data.currentHP > 0) return;
        if(data.highScore < data.score)
        {
            data.highScore = data.score;
        }

    }
}

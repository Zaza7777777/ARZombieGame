using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text ScoreText;
    public Text LivesText;
    public GameObject GameOverPanel;
    public Text GameOverScoreText;

    void Update()
    {
        if (GameManager.Instance == null) return;

       
        if (ScoreText != null)
            ScoreText.text = "Score: " + GameManager.Instance.Score;

        if (LivesText != null)
            LivesText.text = "Lives: " + GameManager.Instance.Lives;

       
        if (GameManager.Instance.IsGameOver())
        {
            if (GameOverPanel != null && !GameOverPanel.activeSelf)
            {
                GameOverPanel.SetActive(true);
                if (GameOverScoreText != null)
                    GameOverScoreText.text = "Final Score: " + GameManager.Instance.Score;
            }
        }
    }
}
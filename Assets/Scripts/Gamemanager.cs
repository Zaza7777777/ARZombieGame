using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int Score = 0;
    public int Lives = 5;
    public int MaxLives = 5;
    public int CurrentWave = 1;
    public int ZombiesPerWave = 3;
    public int MaxWaves = 3;
    public int ZombiesRemainingInWave;
    public bool WaveInProgress = false;
    public TMP_Text ScoreText;
    public TMP_Text LivesText;
    public TMP_Text WaveText;
    public GameObject GameOverPanel;
    public GameObject WaveCompletePanel;
    public GameObject WaveStartPanel;
    public GameObject WinPanel;
    public TMP_Text WaveStartText;
    private bool gameOver = false;
    void Awake() { Instance = this; }
    void Start()
    {
        if (GameOverPanel != null) GameOverPanel.SetActive(false);
        if (WaveCompletePanel != null) WaveCompletePanel.SetActive(false);
        if (WaveStartPanel != null) WaveStartPanel.SetActive(false);
        if (WinPanel != null) WinPanel.SetActive(false);
        StartWave();
    }
    public void StartWave()
    {
        if (CurrentWave > MaxWaves) { TriggerWin(); return; }
        ZombiesRemainingInWave = ZombiesPerWave + (CurrentWave - 1) * 2;
        WaveInProgress = false;
        if (WaveCompletePanel != null) WaveCompletePanel.SetActive(false);
        if (WaveStartPanel != null)
        {
            if (WaveStartText != null) WaveStartText.text = "Wave " + CurrentWave;
            WaveStartPanel.SetActive(true);
            Invoke("HideWaveStartPanel", 2f);
        }
        else
        {
            WaveInProgress = true;
        }
        Debug.LogWarning("Wave " + CurrentWave + " started! Zombies: " + ZombiesRemainingInWave);
        UpdateUI();
    }
    void HideWaveStartPanel()
    {
        if (WaveStartPanel != null) WaveStartPanel.SetActive(false);
        WaveInProgress = true;
    }
    public void ZombieKilled()
    {
        if (gameOver) return;
        Score += 10 * CurrentWave;
        ZombiesRemainingInWave--;
        UpdateUI();
        if (ZombiesRemainingInWave <= 0) WaveComplete();
    }
    void WaveComplete()
    {
        if (!WaveInProgress) return;
        WaveInProgress = false;
        CurrentWave++;
        if (CurrentWave > MaxWaves) { TriggerWin(); return; }
        if (WaveCompletePanel != null) WaveCompletePanel.SetActive(true);
        Invoke("StartWave", 3f);
    }
    public void LoseLife()
    {
        if (gameOver) return;
        Lives--;
        UpdateUI();
        Debug.LogWarning("Lives remaining: " + Lives);
        if (Lives <= 0) TriggerGameOver();
    }
    public void AddScore(int amount)
    {
        if (gameOver) return;
        Score += amount;
        UpdateUI();
    }
    void UpdateUI()
    {
        if (ScoreText != null) ScoreText.text = "Score: " + Score;
        if (LivesText != null) LivesText.text = "Lives: " + Lives;
        if (WaveText != null) WaveText.text = "Wave: " + CurrentWave;
    }
    void TriggerGameOver()
    {
        gameOver = true;
        WaveInProgress = false;
        if (GameOverPanel != null) GameOverPanel.SetActive(true);
    }
    void TriggerWin()
    {
        gameOver = true;
        WaveInProgress = false;
        if (WaveCompletePanel != null) WaveCompletePanel.SetActive(false);
        if (WaveStartPanel != null) WaveStartPanel.SetActive(false);
        if (WinPanel != null) WinPanel.SetActive(true);
        Debug.Log("You win!");
    }
    public bool IsGameOver() { return gameOver; }
}
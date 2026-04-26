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
    public TMP_Text KillCountText;          // <-- ADD THIS
    public GameObject GameOverPanel;
    public GameObject WaveCompletePanel;
    public GameObject WaveStartPanel;
    public GameObject WinPanel;
    public TMP_Text WaveStartText;

    [Header("Audio")]
    public AudioClip takeDamageClip;
    public AudioClip collectLifeClip;
    private AudioSource audioSource;
    private bool gameOver = false;

    // Kill count for minigun unlock
    private int killCount = 0;
    private MinigunMode minigunMode;         // <-- ADD THIS

    void Awake() { Instance = this; }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        minigunMode = FindObjectOfType<MinigunMode>();  // <-- ADD THIS

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

        killCount++;
        Debug.Log("Kill count: " + killCount);

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
        if (takeDamageClip != null)
            audioSource.PlayOneShot(takeDamageClip);
        UpdateUI();
        Debug.LogWarning("Lives remaining: " + Lives);
        if (Lives <= 0) TriggerGameOver();
    }

    public void GainLife()
    {
        if (gameOver) return;
        if (Lives >= MaxLives)
        {
            Debug.Log("Already at max lives, med kit wasted.");
            return;
        }
        Lives = Mathf.Min(Lives + 1, MaxLives);
        if (collectLifeClip != null)
            audioSource.PlayOneShot(collectLifeClip);
        UpdateUI();
        Debug.Log("Med kit collected! Lives: " + Lives);
    }

    public void AddScore(int amount)
    {
        if (gameOver) return;
        Score += amount;
        UpdateUI();
    }

    public int GetKillCount() { return killCount; }
    public void ResetKillCount()
    {
        killCount = 0;
        UpdateUI();                          // <-- ADD THIS so counter resets on screen
    }

    void UpdateUI()
    {
        if (ScoreText != null) ScoreText.text = "Score: " + Score;
        if (LivesText != null) LivesText.text = "Lives: " + Lives;
        if (WaveText != null) WaveText.text = "Wave: " + CurrentWave;

        // Kill counter with minigun goal                // <-- ADD THIS BLOCK
        if (KillCountText != null)
        {
            if (minigunMode != null)
                KillCountText.text = "Kills: " + killCount + " / " + minigunMode.killsRequired;
            else
                KillCountText.text = "Kills: " + killCount;
        }
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
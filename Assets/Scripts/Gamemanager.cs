using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

  
    public int Score = 0;
    public int Lives = 5;
    public int MaxLives = 5;
    public int CurrentWave = 1;
    public int ZombiesPerWave = 3;
    public int MaxWaves = 5;          
    public int ZombiesRemainingInWave;
    public bool WaveInProgress = false;


    public TMP_Text ScoreText;
    public TMP_Text LivesText;
    public TMP_Text WaveText;
    public TMP_Text KillCountText;

   
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
    private int killCount = 0;
    private MinigunMode minigunMode;

  
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
  
        if (GameConfig.Instance != null)
            MaxWaves = GameConfig.Instance.TotalWaves;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        minigunMode = FindObjectOfType<MinigunMode>();

    
        SetPanel(GameOverPanel, false);
        SetPanel(WaveCompletePanel, false);
        SetPanel(WaveStartPanel, false);
        SetPanel(WinPanel, false);

        StartWave();
    }

   
    public void StartWave()
    {
        if (CurrentWave > MaxWaves) { TriggerWin(); return; }

        ZombiesRemainingInWave = ZombiesPerWave + (CurrentWave - 1) * 2;
        WaveInProgress = false;

        SetPanel(WaveCompletePanel, false);

        if (WaveStartPanel != null)
        {
            if (WaveStartText != null)
                WaveStartText.text = "Wave " + CurrentWave;
            SetPanel(WaveStartPanel, true);
            Invoke(nameof(HideWaveStartPanel), 2f);
        }
        else
        {
            WaveInProgress = true;
        }

        Debug.LogWarning("Wave " + CurrentWave + " started! Enemies: " + ZombiesRemainingInWave);
        UpdateUI();
    }

    void HideWaveStartPanel()
    {
        SetPanel(WaveStartPanel, false);
        WaveInProgress = true;
    }

    void WaveComplete()
    {
        if (!WaveInProgress) return;
        WaveInProgress = false;
        CurrentWave++;

        if (CurrentWave > MaxWaves) { TriggerWin(); return; }

        SetPanel(WaveCompletePanel, true);
        Invoke(nameof(StartWave), 3f);
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

    public void GolemKilled()
    {
        if (gameOver) return;
        Score += 25 * CurrentWave;
        ZombiesRemainingInWave--;
        killCount++;
        Debug.Log("Golem kill! Kill count: " + killCount);
        UpdateUI();
        if (ZombiesRemainingInWave <= 0) WaveComplete();
    }

    public void KillAllZombies()
    {
        if (gameOver) return;

        ZombieBehaviour[] zombies = FindObjectsOfType<ZombieBehaviour>();
        foreach (ZombieBehaviour z in zombies) z.KillInstantly();

        GolemBehaviour[] golems = FindObjectsOfType<GolemBehaviour>();
        foreach (GolemBehaviour g in golems) g.KillInstantly();

        Debug.Log($"NUKE killed {zombies.Length} zombies and {golems.Length} golems!");
    }



    public void LoseLife()
    {
        if (gameOver) return;
        Lives--;
        PlayClip(takeDamageClip);
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
        PlayClip(collectLifeClip);
        UpdateUI();
        Debug.Log("Med kit collected! Lives: " + Lives);
    }

    public void AddScore(int amount)
    {
        if (gameOver) return;
        Score += amount;
        UpdateUI();
    }

  

    void TriggerGameOver()
    {
        gameOver = true;
        WaveInProgress = false;
        SetPanel(GameOverPanel, true);
    }

    void TriggerWin()
    {
        gameOver = true;
        WaveInProgress = false;
        SetPanel(WaveCompletePanel, false);
        SetPanel(WaveStartPanel, false);
        SetPanel(WinPanel, true);
        Debug.Log("You win!");
    }


    public void RestartToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public void RestartImmediate()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public bool IsGameOver() { return gameOver; }

    public int GetKillCount() { return killCount; }
    public void ResetKillCount() { killCount = 0; UpdateUI(); }

    void UpdateUI()
    {
        if (ScoreText != null) ScoreText.text = "Score: " + Score;
        if (LivesText != null) LivesText.text = "Lives: " + Lives;
        if (WaveText != null) WaveText.text = "Wave: " + CurrentWave + " / " + MaxWaves;

        if (KillCountText != null)
        {
            if (minigunMode != null)
                KillCountText.text = "Minigun: " + killCount + " / " + minigunMode.killsRequired;
            else
                KillCountText.text = "Minigun: " + killCount;
        }
    }

    void SetPanel(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }

    void PlayClip(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
using UnityEngine;


public class GameConfig : MonoBehaviour
{
    public static GameConfig Instance { get; private set; }

    [HideInInspector] public int TotalWaves = 5;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }
}
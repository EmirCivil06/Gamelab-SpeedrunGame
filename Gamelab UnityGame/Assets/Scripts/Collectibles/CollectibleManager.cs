using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    [SerializeField]
    private int currentScore;
    
    
    public TextMeshProUGUI scoreText;

    public int CurrentScore => currentScore;

    public event Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void RegisterCollect(int amount)
    {
        currentScore += amount;
        scoreText.text = currentScore.ToString();

        // event sistemi henüz eklenmedi
        //OnScoreChanged?.Invoke(currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
        //OnScoreChanged?.Invoke(currentScore);
    }
}


using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;


public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }
    private Collectible _collectible;

    [SerializeField]
    private int currentScore;
    [SerializeField] private UIDocument gameUI;
    private Label scoreText;
    public int CurrentScore => currentScore;

    public event Action<int> OnScoreChanged;
    private Collectible[] _collectibles;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        scoreText = gameUI.rootVisualElement.Q("Score") as Label;

        Instance = this;
        _collectibles = FindObjectsByType<Collectible>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);
        foreach (var collectible in _collectibles)
        {
            if (collectible != null)
            {
                collectible.OnCollectibleAnimation += RegisterCollect;
            }
        }
        
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (_collectibles == null)
        {
            return;
        }

        foreach (var collectible in _collectibles)
        {
            if (collectible != null)
            {
                collectible.OnCollectibleAnimation -= RegisterCollect;
            }
        }
    }

    public void RegisterCollect(int amount)
    {
        currentScore += amount;
        scoreText.text = ": " + currentScore.ToString();
    }

    public void ResetScore()
    {
        currentScore = 0;
        //OnScoreChanged?.Invoke(currentScore);
    }
}


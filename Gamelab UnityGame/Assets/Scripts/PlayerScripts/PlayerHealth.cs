using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Player References")]
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private Rigidbody2D playerRigidbody;

    [Header("Health Settings")]
    [SerializeField]
    private int maxHealth = 3;

    [SerializeField]
    private bool resetVelocityOnRespawn = true;

    [SerializeField]
    private UnityEvent onPlayerRespawned;

    private int _currentHealth;
    private Vector3 _spawnPosition;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
    

    public MainGameUIEvents uiEvents;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        ResolvePlayerReferences();
        CacheSpawnPosition();
        uiEvents = GameObject.FindObjectOfType<MainGameUIEvents>();
    }

    private void OnEnable()
    {
        _currentHealth = Mathf.Max(1, maxHealth);
        NotifyHealthChanged();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || _currentHealth <= 0 || playerTransform == null)
        {
            return;
        }

        _currentHealth = Mathf.Max(_currentHealth - amount, 0);
        NotifyHealthChanged();

        RespawnPlayer();
    }

    public void GameOver()
    {
        _currentHealth = Mathf.Max(1, maxHealth);
        NotifyHealthChanged();
        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    public void SetSpawnPoint(Vector3 position)
    {
        _spawnPosition = position;
    }

    public void AssignPlayer(Transform targetTransform, Rigidbody2D targetRigidbody = null)
    {
        playerTransform = targetTransform;
        playerRigidbody = targetRigidbody ?? playerTransform.GetComponent<Rigidbody2D>();
        CacheSpawnPosition();
    }

    private void RespawnPlayer()
    {
        if (playerTransform == null)
        {
            return;
        }

        playerTransform.position = _spawnPosition;

        if (resetVelocityOnRespawn && playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        onPlayerRespawned?.Invoke();
    }

    private void NotifyHealthChanged()
    {
        //onHealthChanged?.Invoke(_currentHealth);
        if (EnsureUIEventsReference())
        {
            uiEvents.UpdateHealth(_currentHealth);
        }
        if (_currentHealth <= 0)
        {
            Debug.Log("Game Over");
            var player = GameObject.FindGameObjectWithTag("Player");
            Destroy(player);
            GameOver();
        }
    }

    private void CacheSpawnPosition()
    {
        if (playerTransform != null)
        {
            _spawnPosition = playerTransform.position;
        }
    }

    private bool EnsureUIEventsReference()
    {
        if (uiEvents == null)
        {
            uiEvents = FindObjectOfType<MainGameUIEvents>();
        }

        return uiEvents != null;
    }

    private void ResolvePlayerReferences()
    {
        if (playerTransform == null)
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }

        if (playerTransform != null && playerRigidbody == null)
        {
            playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();
        }
    }
}

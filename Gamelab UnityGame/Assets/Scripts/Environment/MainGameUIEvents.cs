using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class MainGameUIEvents : MonoBehaviour
{
    private UIDocument mainGameUI;
    private ProgressBar timer;
    private AudioSource clockTicking;
    private float soundInterval = 1f;
    private float soundTimer = 0f;

    [Header("Health Bar değişkenleri")]
    [SerializeField] private Sprite fullHealth;
    [SerializeField] private Sprite twoHealth;
    [SerializeField] private Sprite oneHealth;
    [SerializeField] private Sprite noHealth;
    private VisualElement healthBar;
    void Awake()
    {
        mainGameUI = GetComponent<UIDocument>();
        timer = mainGameUI.rootVisualElement.Q("ProgressBar") as ProgressBar;
        clockTicking = GetComponent<AudioSource>();
        EnsureUIReferences();
    }

    void Update()
    {
        timer.value -= Time.deltaTime;
        soundTimer += Time.deltaTime;
        if (soundTimer >= soundInterval)
        {
             clockTicking.PlayOneShot(clockTicking.clip);
             soundTimer = 0f;
        }
        if (!EnsureUIReferences() || timer == null)
        {
            return;
        }

        
    }
    private bool EnsureUIReferences()
    {
        if (mainGameUI == null)
        {
            mainGameUI = GetComponent<UIDocument>();
        }
        if (mainGameUI == null || mainGameUI.rootVisualElement == null)
        {
            return false;
        }
        var root = mainGameUI.rootVisualElement;
        if (healthBar == null && root != null)
        {
            healthBar = root.Q<VisualElement>("HealthBar");
        }
        if (timer == null && root != null)
        {
            timer = root.Q("ProgressBar") as ProgressBar;
        }
        return true;
    }
    public void UpdateHealth(int hp)
    {
        if (!EnsureUIReferences() || healthBar == null)
        {
            Debug.LogWarning("Health bar UI hazır olmadığı için UpdateHealth çağrısı atlandı.");
            return;
        }
        
        switch (hp)
        {
            case 3: 
                healthBar.style.backgroundImage = new StyleBackground(fullHealth.texture);
                break;
            case 2:
                healthBar.style.backgroundImage = new StyleBackground(twoHealth.texture);
                break;
            case 1:
                healthBar.style.backgroundImage = new StyleBackground(oneHealth.texture);
                break;
            case 0:
                healthBar.style.backgroundImage = new StyleBackground(noHealth.texture);
                break;
            default:
                Debug.Log("No sprite found for that HP");
                break;
        }
    }





















}

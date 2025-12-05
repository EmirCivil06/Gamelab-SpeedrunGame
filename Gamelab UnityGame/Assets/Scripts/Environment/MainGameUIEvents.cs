using System.Collections;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainGameUIEvents : MonoBehaviour
{
    private UIDocument mainGameUI;
    private ProgressBar timer;
    private AudioSource clockTicking;

    [Header("Game Over Değişkenleri")]
    private VisualElement deathScreen;
    private Button quit, tryAgain;

    [Header("Timer Değişkenleri")]
    private float soundInterval = 1f;
    private float soundTimer = 0f;

    [Header("Health Bar değişkenleri")]
    [SerializeField] private Sprite fullHealth;
    [SerializeField] private Sprite twoHealth;
    [SerializeField] private Sprite oneHealth;
    [SerializeField] private Sprite noHealth;
    private VisualElement healthBar;

    [SerializeField] private AudioSource click;
    void Awake()
    {
        mainGameUI = GetComponent<UIDocument>();
        timer = mainGameUI.rootVisualElement.Q("ProgressBar") as ProgressBar;
        clockTicking = GetComponent<AudioSource>();

        deathScreen = mainGameUI.rootVisualElement.Q("DeathScreen");
        quit = mainGameUI.rootVisualElement.Q("Quit") as Button;
        tryAgain = mainGameUI.rootVisualElement.Q("Retry") as Button;

        quit.RegisterCallback<ClickEvent>(OnQuitClick);
        tryAgain.RegisterCallback<ClickEvent>(OnRetryClick);

        EnsureUIReferences();
    }

    private void OnDisable()
    {
        quit.UnregisterCallback<ClickEvent>(OnQuitClick);
        tryAgain.UnregisterCallback<ClickEvent>(OnRetryClick);
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

    public void ChangeToGameOver()
    {
        deathScreen.RemoveFromClassList("invis");
    }

    public void OnQuitClick(ClickEvent evt)
    {
        click.Play();
        Application.Quit();
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #endif
    }

    public void OnRetryClick(ClickEvent evt)
    {
        click.Play();
        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
        ResumeNonInterface();
    }

    private void ResumeNonInterface()
    {
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            Time.timeScale = 1f;
            if (!source.CompareTag("UIAudio")) 
                source.mute = false;
        }
    }
}
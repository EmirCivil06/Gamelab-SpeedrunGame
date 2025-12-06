using System.Collections;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
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

    // Pause menüsü değişkenleri
    [SerializeField] private AudioMixer mainAudioMixer;
    [SerializeField] private InputActionReference escapeKey;
    private Button continiue;
    private Slider sound, music;
    private VisualElement pauseMenu;
    public SettingsData settingsData;

    [SerializeField] private AudioSource click;
    void Awake()
    {
        mainGameUI = GetComponent<UIDocument>();
        timer = mainGameUI.rootVisualElement.Q("ProgressBar") as ProgressBar;

        pauseMenu = mainGameUI.rootVisualElement.Q("PauseMenu");

        sound = pauseMenu.Q("SFX") as Slider;
        music = pauseMenu.Q("Music") as Slider;

        sound.RegisterValueChangedCallback(evt =>
        {
            settingsData.sfxVolume = evt.newValue;
            ApplySoundChanges();
            SaveSettings();
        });

        music.RegisterValueChangedCallback(evt =>
        {
            settingsData.musicVolume = evt.newValue;
            ApplyMusicChanges();
            SaveSettings();
        });

        clockTicking = GetComponent<AudioSource>();

        deathScreen = mainGameUI.rootVisualElement.Q("DeathScreen");
        quit = mainGameUI.rootVisualElement.Q("Quit") as Button;
        tryAgain = mainGameUI.rootVisualElement.Q("Retry") as Button;
        continiue = pauseMenu.Q("Continue") as Button;

        quit.RegisterCallback<ClickEvent>(OnQuitClick);
        tryAgain.RegisterCallback<ClickEvent>(OnRetryClick);
        continiue.RegisterCallback<ClickEvent>(OnContiniueClick);
        EnsureUIReferences();
    }

    void Start()
    {
        ApplyMusicChanges();
        ApplySoundChanges();
    }

    void OnEnable()
    {
        escapeKey.action.performed += TogglePauseMenu;
        escapeKey.action.Enable();
    }

    private void OnDisable()
    {
        quit.UnregisterCallback<ClickEvent>(OnQuitClick);
        tryAgain.UnregisterCallback<ClickEvent>(OnRetryClick);
        continiue.UnregisterCallback<ClickEvent>(OnContiniueClick);

        escapeKey.action.performed -= TogglePauseMenu;
        escapeKey.action.Disable();
    }
    // Sayaç için update
    void Update()
    {
        if (!Flag.sceneBeforIsActive)
        {
            timer.value -= Time.deltaTime;
            soundTimer += Time.deltaTime; 
        }
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
    // Semihin eklediği bölge
    private bool EnsureUIReferences()
    {
        // Doküman ve obje kontrollerini yapıyor
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

    // Semihin eklediği bölge
    public void UpdateHealth(int hp)
    {
        if (!EnsureUIReferences() || healthBar == null)
        {
            Debug.LogWarning("Health bar UI hazır olmadığı için UpdateHealth çağrısı atlandı.");
            return;
        }
        // Sağlık kontrolü için switch case şeysi
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
    // Kaybetme ekranına geçiş
    public void ChangeToGameOver()
    {
        deathScreen.style.display = DisplayStyle.Flex;
        deathScreen.RemoveFromClassList("invis");
    }
    // Düğme ataması için metot
    public void OnQuitClick(ClickEvent evt)
    {
        click.PlayOneShot(click.clip);
        Application.Quit();
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #endif
    }
    // Düğme ataması için metot
    public void OnRetryClick(ClickEvent evt)
    {
        click.PlayOneShot(click.clip);
        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
        ResumeNonInterface();
    }
    // Düğme ataması için metot
    public void OnContiniueClick(ClickEvent evt)
    {
        pauseMenu.AddToClassList("invis");
        Invoke("SetDisplay", 0.8f);
        ResumeNonInterface();
    }
    // --------------------------------- AYARLARIN KAYDEDİLME BÖLÜMÜ -----------------------------
    public void ApplySoundChanges()
    {
        mainAudioMixer.SetFloat("SFXVolume", Mathf.Log10(settingsData.sfxVolume) * 20f);
    }

    public void ApplyMusicChanges()
    {
        mainAudioMixer.SetFloat("MusicVolume", Mathf.Log10(settingsData.sfxVolume) * 20f);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("musicVolume", settingsData.musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", settingsData.sfxVolume);
    }
    // --------------------------------------------------------------------------------------------

    // Bu şey çalışıyor olması lazım
    private void ResumeNonInterface()
    {
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            Time.timeScale = 1f;
            if (!source.CompareTag("UIAudio")) 
                source.mute = false;
        }
    }
    // Invoke kullanabilmek için atamayı ayrı bir metoda aktardık
    private void SetDisplay()
    {
        pauseMenu.style.display = DisplayStyle.None;
    }

    // TO:DO: Bu metot şuan niyeyse çalışmıyor (Debug dışında)
    public void TogglePauseMenu(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Debug.Log("ESC");
        if (!EnsureUIReferences() || pauseMenu == null) return;
        
        if (pauseMenu.style.display == DisplayStyle.None)
        {
            pauseMenu.style.display = DisplayStyle.Flex;
            pauseMenu.RemoveFromClassList("invis");
            Time.timeScale = 0f;
            foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
            {
                if (!source.CompareTag("UIAudio"))
                    source.mute = true;
            }
            return;
        }
    }
}
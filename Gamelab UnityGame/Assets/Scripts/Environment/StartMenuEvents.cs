using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class StartMenuEvents : MonoBehaviour
{   
    // -------- ANA BAŞLANGIÇ MENÜSÜ ---------
    private UIDocument mainMenuDocument;
    private Button _start;
    private Button _options;
    private Button _quit;
    // -------- GEÇİŞ EKRANI ---------
    private VisualElement fadeScreen;
    // -------- SEÇENEKLER MENÜSÜ ---------
    private VisualElement optionsContainer;
    private Slider sound, music;
    private Toggle beepToggle;
    private Button returner;
    // ---------- SEÇENEKLER AYARLARI İÇİN SO FIELD'I ---------------
    public SettingsData settingsData;
    //-------------------------------------
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource beep;

    // Dokümanı ve butonları alabilmek için awake metodu
    private void Awake()
    {
        Flag.sceneBeforIsActive = true;
        mainMenuDocument = GetComponent<UIDocument>();

        _start = mainMenuDocument.rootVisualElement.Q("Start") as Button;
        _options = mainMenuDocument.rootVisualElement.Q("Settings") as Button;
        _quit = mainMenuDocument.rootVisualElement.Q("Quit") as Button;
        fadeScreen = mainMenuDocument.rootVisualElement.Q("FadeScreen");

        optionsContainer = mainMenuDocument.rootVisualElement.Q("SettingsContainer");
        sound = mainMenuDocument.rootVisualElement.Q("SFXVolume") as Slider;
        music = mainMenuDocument.rootVisualElement.Q("MusicVolume") as Slider;
        beepToggle = mainMenuDocument.rootVisualElement.Q("Beep") as Toggle;
        returner = mainMenuDocument.rootVisualElement.Q("Return") as Button;

        sound.value = settingsData.sfxVolume;
        music.value = settingsData.musicVolume;

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

        Invoke("FadeIn", 0.7f);

        _start.RegisterCallback<ClickEvent>(OnStartClick);
        _options.RegisterCallback<ClickEvent>(OnOptionsClick);
        _quit.RegisterCallback<ClickEvent>(OnQuitClick);
        returner.RegisterCallback<ClickEvent>(OnReturnClick);

        beepToggle.RegisterValueChangedCallback(OnToggleValueChanged);
    }

    void Start()
    {
        ApplySoundChanges();
        ApplyMusicChanges();
    }

    // Oyun devre dışı kaldığında metodun butona yapılan atanmasını kaldırır (???)
    private void OnDisable()
    {
        _start.UnregisterCallback<ClickEvent>(OnStartClick);
        _options.UnregisterCallback<ClickEvent>(OnOptionsClick);
        _quit.UnregisterCallback<ClickEvent>(OnQuitClick);
        beepToggle.UnregisterValueChangedCallback(OnToggleValueChanged);
    }

    // Başlama butonu
    private void OnStartClick(ClickEvent evt)
    {
        FadeOut(1.5f);
        audioSource.Play();
    }

    // Seçenekler butonu
    private void OnOptionsClick(ClickEvent evt)
    {
        audioSource.Play();
        optionsContainer.style.display = DisplayStyle.Flex;
    } 

    // Çıkış butonu
    private void OnQuitClick(ClickEvent evt)
    {
        audioSource.Play();
        Application.Quit();
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #endif
    } 

    private void OnReturnClick(ClickEvent evt)
    {
        audioSource.Play();
        optionsContainer.style.display = DisplayStyle.None;
    }

    private void FadeIn()
    {
        fadeScreen.AddToClassList("invis");
        PauseGame();
    }

    private IEnumerator HideAfterRealTime(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        ResumeGame();
        Hide();
    }

    private void FadeOut(float secondsLater)
    {
        fadeScreen.RemoveFromClassList("invis");
        StartCoroutine(HideAfterRealTime(secondsLater));

    }

    // -----------------------------------------------------------------------

    private void Hide()
    {
        mainMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    // ---------------------------------------------------------------------------------

    private void PauseGame()
    {
        Time.timeScale = 0f;
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (!source.CompareTag("UIAudio")) 
                source.mute = true;
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (!source.CompareTag("UIAudio")) 
                source.mute = false;
        }
    }
    // ------------------------------------------------------------------------------------------------------------
    private void OnToggleValueChanged(ChangeEvent<bool> evt)
    {
        if (evt.newValue == true) beep.Play();
    }
    // ----------------------------------------------------------------------------------------------
    private void ApplyMusicChanges()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(settingsData.musicVolume) * 20f);
    }

    private void ApplySoundChanges()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(settingsData.sfxVolume) * 20f);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("musicVolume", settingsData.musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", settingsData.sfxVolume);
    }
}

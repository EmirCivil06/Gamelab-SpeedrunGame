using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StartMenuEvents : MonoBehaviour
{   
    private UIDocument mainMenuDocument;
    private Button _start;
    private Button _options;
    private Button _quit;
    private VisualElement fadeScreen;
    private AudioSource audioSource;

    // Dokümanı ve butonları alabilmek için awake metodu
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        mainMenuDocument = GetComponent<UIDocument>();

        _start = mainMenuDocument.rootVisualElement.Q("Start") as Button;
        _options = mainMenuDocument.rootVisualElement.Q("Settings") as Button;
        _quit = mainMenuDocument.rootVisualElement.Q("Quit") as Button;
        fadeScreen = mainMenuDocument.rootVisualElement.Q("FadeScreen");
        Invoke("FadeIn", 0.5f);

        _start.RegisterCallback<ClickEvent>(OnStartClick);
        _options.RegisterCallback<ClickEvent>(OnOptionsClick);
        _quit.RegisterCallback<ClickEvent>(OnQuitClick);
    }

    // Oyun devre dışı kaldığında metodun butona yapılan atanmasını kaldırır (???)
    private void OnDisable()
    {
        _start.UnregisterCallback<ClickEvent>(OnStartClick);
        _options.UnregisterCallback<ClickEvent>(OnOptionsClick);
        _quit.UnregisterCallback<ClickEvent>(OnQuitClick);
    }

    // Başlama butonu
    private void OnStartClick(ClickEvent evt)
    {
        Debug.Log("Calisiyo");
        FadeOut(1.5f);
        audioSource.Play();
    }

    // Seçenekler butonu
    private void OnOptionsClick(ClickEvent evt)
    {
        audioSource.Play();
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
}

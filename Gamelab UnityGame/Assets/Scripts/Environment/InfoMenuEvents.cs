using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class InfoMenuEvents : MonoBehaviour
{
    private UIDocument InfoUI;
    private VisualElement image1, image2, image3;
    private Label text;
    [SerializeField] private GameObject nextUI;

    private void Awake()
    {
        InfoUI = GetComponent<UIDocument>();

        image1 = InfoUI.rootVisualElement.Q("imageOne");
        image2 = InfoUI.rootVisualElement.Q("imageTwo");
        image3 = InfoUI.rootVisualElement.Q("imageThree");
        text = InfoUI.rootVisualElement.Q("Story") as Label;

        InfoUI.rootVisualElement.focusable = true;
        InfoUI.rootVisualElement.Focus();
        InfoUI.rootVisualElement.RegisterCallback<ClickEvent>(OnEnterClick);

        DisplayElementIn(0.75f, image1);
        DisplayElementIn(1.75f, image2);
        DisplayElementIn(2.8f, image3);
        DisplayElementIn(0.5f, text);

        PauseGame();
    }

    private void DisplayElementIn(float seconds, VisualElement element)
    {
        StartCoroutine(WaitAndDisplay(seconds, element));
    }

    private IEnumerator WaitAndDisplay(float seconds, VisualElement element)
    {
        yield return new WaitForSeconds(seconds);
        element.RemoveFromClassList("invis");
    }

    IEnumerator whyNoLambdaTho()
    {
        float duration = 0.5f;
        float time = 0f;

        VisualElement root = InfoUI.rootVisualElement;

        // disable input during fade
        root.pickingMode = PickingMode.Ignore;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            root.style.opacity = 1f - t;
            yield return null;
        }

        // optionally hide completely
        ResumeGame();
        nextUI.SetActive(true);
        root.style.display = DisplayStyle.None;
    }

    private void OnEnterClick(ClickEvent evt)
    {   
        StartCoroutine(whyNoLambdaTho());  
    }

    private void PauseGame()
    {
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (!source.CompareTag("UIAudio")) 
                source.mute = true;
        }
    }

    private void ResumeGame()
    {
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (!source.CompareTag("UIAudio")) 
                source.mute = false;
        }
    }

}

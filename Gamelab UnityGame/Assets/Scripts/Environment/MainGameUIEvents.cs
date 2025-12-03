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

    void Awake()
    {
        mainGameUI = GetComponent<UIDocument>();
        timer = mainGameUI.rootVisualElement.Q("ProgressBar") as ProgressBar;
        clockTicking = GetComponent<AudioSource>();
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
    }

}

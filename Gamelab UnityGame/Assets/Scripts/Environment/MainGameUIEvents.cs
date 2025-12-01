using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class MainGameUIEvents : MonoBehaviour
{
    private UIDocument mainGameUI;
    private ProgressBar timer;

    void Awake()
    {
        mainGameUI = GetComponent<UIDocument>();
        timer = mainGameUI.rootVisualElement.Q("ProgressBar") as ProgressBar;
    }

    void Update()
    {
       timer.value -= Time.deltaTime;
       timer.title = timer.value.ToString(); 
    }

}

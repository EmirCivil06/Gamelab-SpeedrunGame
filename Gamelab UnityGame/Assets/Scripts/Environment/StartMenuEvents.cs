using UnityEngine;
using UnityEngine.UIElements;

public class StartMenuEvents : MonoBehaviour
{   
    private UIDocument mainMenuDocument;
    private Button _button;

    // Dokümanı alabilmek için awake metodu
    private void Awake()
    {
        mainMenuDocument = GetComponent<UIDocument>();
        _button = mainMenuDocument.rootVisualElement.Q("Start") as Button;
        _button.RegisterCallback<ClickEvent>(OnStartClick);
    }

    // Oyun devre dışı kaldığında metodun butona yapılan atanmasını kaldırır (???)
    private void OnDisable()
    {
        _button.UnregisterCallback<ClickEvent>(OnStartClick);
    }

    private void OnStartClick(ClickEvent evt)
    {
        Debug.Log("Başlama butonuna basıldı");
    }
}

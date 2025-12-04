using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class MainGameUIEvents : MonoBehaviour
{
    private UIDocument mainGameUI;
    private ProgressBar timer;

    [Header("Health Bar değişkenleri")]
    public Sprite fullHealth;
    public Sprite twoHealth;
    public Sprite oneHealth;
    public Sprite noHealth;
    private VisualElement healthBar;


    void Awake()
    {
        EnsureUIReferences();
    }

    private void OnEnable()
    {
        EnsureUIReferences();
    }

    void Update()
    {
        if (!EnsureUIReferences() || timer == null)
        {
            return;
        }
        
        timer.value = Mathf.Max(0f, timer.value - Time.deltaTime);
        timer.title = Mathf.CeilToInt(timer.value).ToString();
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
}

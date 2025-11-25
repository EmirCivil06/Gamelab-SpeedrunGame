using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    [SerializeField]
    private int scoreValue = 1;

    [SerializeField]
    private bool destroyOnCollect = true;

    private Collider2D _collider;


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        if (_collider != null && !_collider.isTrigger)
        {
            _collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Collect();
    }

    private void Collect()
    {
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.RegisterCollect(scoreValue);
        }
        else
        {
            Debug.LogWarning($"Collectible picked up but no {nameof(CollectibleManager)} found in the scene.", this);
        }

        Destroy(gameObject);
    }
}


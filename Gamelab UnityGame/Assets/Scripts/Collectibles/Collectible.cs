using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class Collectible : MonoBehaviour
{
    [SerializeField] private int scoreValue = 1;
    public int ScoreValue
    {
        get => scoreValue;
    }

    private Collider2D _collider;
    public Animator _animator;

    public delegate void CollectibleAnimationHandler(int score);
    public event CollectibleAnimationHandler OnCollectibleAnimation;
    
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        if (_collider != null && !_collider.isTrigger)
        {
            _collider.isTrigger = true;
        }
        // burası collectible'ın 'Is trigger'ı tiklemeyi unutulursa diye eklendi
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return; // Player hariç çarpışmalarda hiç bir şey yapma
        }
        
        Collect();
    }

    public void Collect()
    {
        OnCollectibleAnimation?.Invoke(scoreValue); // Collectible üzerinden collectible animasyonu çalıştırılması için event ateşliyorum
        Destroy(gameObject);
        
    }

    private void OnDestroy()
    {
        _animator.SetBool("onDestroy", true);
    }
}


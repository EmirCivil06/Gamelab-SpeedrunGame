using System;
using System.Collections;
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
    private bool _isCollected;

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
        if (_isCollected)
        {
            return;
        }

        _isCollected = true;

        if (_collider != null)
        {
            _collider.enabled = false;
        }
        
        OnCollectibleAnimation?.Invoke(scoreValue); // Collectible üzerinden collectible animasyonu çalıştırılması için event ateşliyorum

        if (_animator != null && gameObject.activeInHierarchy)
        {
            StartCoroutine(DestroyAfterAnimation());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        _animator.SetBool("onDestroy", true);

        // Animator'ın yeni state'e geçmesi için bir frame bekliyorum
        yield return null;

        //float waitTime = GetCurrentAnimationLength()
        
        const float waitTime = 0.5f; // animation üzerinde tüm collectible destroy animasyonları 30 fps ile oluşturulduğu için 0.5f saniye bekliyorum
        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }

        Destroy(gameObject);
    }

    private float GetCurrentAnimationLength()
    {
        var clipInfos = _animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfos.Length > 0)
        {
            return clipInfos[0].clip.length / Mathf.Max(_animator.speed, 0.01f);
        }

        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float stateSpeed = Mathf.Max(stateInfo.speed, 0.01f);
        return stateInfo.length / stateSpeed;
    }
}


using UnityEngine;

public class bullet : MonoBehaviour
{
    public float omurSuresi = 5f;

    void Start()
    {
     
        Destroy(gameObject, omurSuresi);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            if (PlayerHealth.Instance != null)
            {
                PlayerHealth.Instance.TakeDamage(1);
            }
            Destroy(gameObject); 

        }


        else if (other.CompareTag("Ground")) 
        {

            Destroy(gameObject);
        }
    }
}
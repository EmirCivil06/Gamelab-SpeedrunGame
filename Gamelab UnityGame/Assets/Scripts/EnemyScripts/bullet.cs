using UnityEngine;

public class bullet : MonoBehaviour
{
    public float omurSuresi = 5f; // Mermi ne kadar s�re sonra yok olacak

    void Start()
    {
        // Belirtilen s�re sonunda mermiyi yok et
        Destroy(gameObject, omurSuresi);
    }

    // Bu fonksiyon, mermi ba�ka bir Collider2D'ye temas etti�inde �a�r�l�r
    void OnTriggerEnter2D(Collider2D other)
    {
        // E�er mermi Lama'ya �arparsa
        if (other.CompareTag("Player")) // Lama'n�n Tag'i "Player" olmal�
        {
            if (PlayerHealth.Instance != null)
            {
                PlayerHealth.Instance.TakeDamage(1);
            }

            Destroy(gameObject); // Mermiyi yok et

            // E�er Lama'ya �zg� bir hasar efekti vb. varsa buraya ekleyebilirsin
        }

        // E�er mermi bir zemine, duvara veya ba�ka bir engere �arparsa
        /*else if (other.CompareTag("Zemin") || other.CompareTag("Duvar")) 
        {
            Debug.Log("Mermi zemine/duvara �arpt�!");
            Destroy(gameObject);
        }*/

        // Di�er bir ihtimalle ba�ka mermiye veya d��mana �arparsa da yok olsun diyebiliriz.
         else if (other.CompareTag("Enemies") || other.CompareTag("Bullets"))
         {
             Destroy(gameObject);
         }
    }
}
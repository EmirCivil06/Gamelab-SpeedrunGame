using UnityEngine;

public class bullet : MonoBehaviour
{
    public float omurSuresi = 5f; // Mermi ne kadar süre sonra yok olacak

    void Start()
    {
        // Belirtilen süre sonunda mermiyi yok et
        Destroy(gameObject, omurSuresi);
    }

    // Bu fonksiyon, mermi baþka bir Collider2D'ye temas ettiðinde çaðrýlýr
    void OnTriggerEnter2D(Collider2D other)
    {
        // Eðer mermi Lama'ya çarparsa
        if (other.CompareTag("Player")) // Lama'nýn Tag'i "Player" olmalý
        {
            //lamanýn sabrýný azaltma kodu buraya gelecek
            Debug.Log("Mermi Lama'ya çarptý!");

            Destroy(gameObject); // Mermiyi yok et

            // Eðer Lama'ya özgü bir hasar efekti vb. varsa buraya ekleyebilirsin
        }

        // Eðer mermi bir zemine, duvara veya baþka bir engere çarparsa
        /*else if (other.CompareTag("Zemin") || other.CompareTag("Duvar")) 
        {
            Debug.Log("Mermi zemine/duvara çarptý!");
            Destroy(gameObject);
        }*/

        // Diðer bir ihtimalle baþka mermiye veya düþmana çarparsa da yok olsun diyebiliriz.
         else if (other.CompareTag("Enemy") || other.CompareTag("Bullet"))
         {
             Destroy(gameObject);
         }
    }
}
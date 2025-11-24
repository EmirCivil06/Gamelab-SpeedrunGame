using UnityEngine;

public class HorozAI : MonoBehaviour {

public enum EnemyType {
        normalHoroz,
        bornozluHoroz
}
public EnemyType enemyType;

    public enum EnemyState{
        idle,
        //patrol,
        attack
}

    public EnemyState currentState;
    /*
    public Transform pointA;
    public Transform pointB;
    private Transform targetPoint;
    */
    public float movementSpeed = 3f;
    private SpriteRenderer spriteRenderer;
    public float gorusMesafesi = 10f;      // Lamay� ne kadar uzaktan g�rebilece�i
    public LayerMask lamaLayer;           // Lamay� filtrelemek i�in (Inspector'dan "Player" layer'�n� se�)
    public Transform atesNoktasi;         // Merminin ��kaca�� yer (Horozun elinin ucu gibi)
    public GameObject okeyTasiPrefab;
    public GameObject ps5KoluPrefab;

    public float saldiriAraligi = 2f;     // Saniyede ka� kez ate� edece�i
    private float sonrakiAtisZamani = 0f; // Cooldown sayac�

    private Transform lamaTransform; //lama konumu


    void Start(){
        currentState = EnemyState.idle;

        //targetPoint = pointB;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null) {
        Debug.Log("horozun spriterenderer'i yok kingo.");
        }
    }

    private void Update()
    {
        switch (currentState) {
            case EnemyState.idle:
                Idle();
                break;

            case EnemyState.attack:
                Attack();
                break;
        }
    }

    void Idle() {
        LamayiAra();
    }

    void Attack()
    {
        // CASUS 1: Attack moduna girdik mi?
        // Debug.Log("1. Attack döngüsü başladı."); 

        if (lamaTransform == null)
        {
            Debug.Log("HATA: Lama kayboldu! (lamaTransform null)");
            currentState = EnemyState.idle;
            return;
        }

        Vector2 yon = (lamaTransform.position - transform.position).normalized;

        // Burada tekrar Raycast atıyoruz, acaba bu mu başarısız oluyor?
        RaycastHit2D hit = Physics2D.Raycast(transform.position, yon, gorusMesafesi, lamaLayer);

        if (hit.collider == null || hit.transform != lamaTransform)
        {
            // CASUS 2: İkinci bakışta göremedik
            Debug.Log("2. Attack içindeki Raycast Lamayı göremedi! Engel var veya açı yanlış.");
            lamaTransform = null;
            currentState = EnemyState.idle;
            return;
        }

        Turn(yon.x < 0);

        // CASUS 3: Zaman doldu mu?
        if (Time.time >= sonrakiAtisZamani)
        {
            Debug.Log("3. Süre doldu, tetiğe basılıyor!");
            AtesEt();
            sonrakiAtisZamani = Time.time + saldiriAraligi;
        }
        else
        {
            // Çok spam yapmasın diye bunu kapalı tutabilirsin
            // Debug.Log("Cooldown bekleniyor...");
        }
    }

    void AtesEt()
    {
        GameObject mermiPrefab = null;

        // Hangi mermiyi atacağız?
        if (enemyType == EnemyType.normalHoroz)
            mermiPrefab = okeyTasiPrefab;
        else
            mermiPrefab = ps5KoluPrefab;

        if (mermiPrefab != null && atesNoktasi != null)
        {
            // 1. Mermiyi Yarat
            GameObject mermi = Instantiate(mermiPrefab, atesNoktasi.position, Quaternion.identity);

            // 2. Merminin üzerindeki Rigidbody2D'yi bul (Fizik motoru)
            Rigidbody2D rb = mermi.GetComponent<Rigidbody2D>();

            if (rb != null && lamaTransform != null)
            {
                // 3. Yönü Hesapla (Hedefin pozisyonu - Ateş noktasının pozisyonu)
                Vector2 yon = (lamaTransform.position - atesNoktasi.position).normalized;

                float mermiHizi = 10f; // Hızı buradan değiştirebilirsin

                // 4. Hızı Ver (Unity 6 için linearVelocity, eskiler için velocity)
                rb.linearVelocity = yon * mermiHizi;
            }
            else
            {
                Debug.LogError("HATA: Mermi prefabında Rigidbody2D YOK veya Lama kayıp!");
            }
        }
    }

    void Turn (bool facingLeft) {
        if (spriteRenderer != null) {
            spriteRenderer.flipX = facingLeft;
        }
    }//diger tarafa donduruyo

    void LamayiAra()
    {

        // Yönü belirle
        Vector2 yon = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        // --- HATA AYIKLAMA ÇİZGİSİ (Scene ekranında KIRMIZI bir çizgi çizer) ---
        // Oyun çalışırken Scene sekmesine geçip Horoz'a bak, kırmızı çizgiyi gör.
        Debug.DrawRay(transform.position, yon * gorusMesafesi, Color.red);
        // -----------------------------------------------------------------------

        // Işını at
        RaycastHit2D hit = Physics2D.Raycast(transform.position, yon, gorusMesafesi, lamaLayer);

        // Bir şeye çarptı mı?
        if (hit.collider != null)
        {
            // Konsola neye çarptığını yazdıralım.
            // Eğer "Lama" yazıyorsa ama saldırmıyorsa sorun koddadır.
            // Eğer "Horoz" (kendisi) yazıyorsa sorun ayardadır.
            Debug.Log("Horoz şuna bakıyor: " + hit.collider.name);

            lamaTransform = hit.transform;

            // LayerMask kullandığımız için tekrar tag kontrolüne gerek yok ama emin olalım
            // (Burada Player tag'ine sahip mi diye de bakabilirsin ekstra güvenlik için)
            currentState = EnemyState.attack;
        }
    }


    /*void Patrol()
  {
      if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f) {

          if (targetPoint == pointB)
          {
              targetPoint = pointA;
              Turn(true);
          }

          else {
              targetPoint = pointB;
              Turn(false);
          }

      }

      transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, movementSpeed * Time.deltaTime); 
      //lama gorduk mu buranin asagisinda bak ona gore hareket edecek.
  }*/
}


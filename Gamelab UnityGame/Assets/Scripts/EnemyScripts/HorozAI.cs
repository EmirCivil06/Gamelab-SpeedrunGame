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
        // Lama hala g�r��te mi diye kontrol et
        if (lamaTransform == null)
        {
            currentState = EnemyState.idle; // Lama yoksa bo�a ge�
            return;
        }

        // Lama ile aram�zdaki mesafeyi ve y�n� tekrar kontrol edelim
        Vector2 yon = (lamaTransform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, yon, gorusMesafesi, lamaLayer);

        // E�er ���n art�k Lamaya �arpm�yorsa (araya engel girdi veya uza�a ka�t�)
        if (hit.collider == null || hit.transform != lamaTransform)
        {
            Debug.Log("Lama g�r��ten kayboldu!");
            lamaTransform = null; // Laman�n kayd�n� sil
            currentState = EnemyState.idle; // Bo�a ge�
            return;
        }

        // Lama g�r��teyse, ona do�ru d�n
        Turn(yon.x < 0);

        // Sald�r� cooldown'�n� (bekleme s�resi) kontrol et
        if (Time.time >= sonrakiAtisZamani)
        {
            // Ate� etme zaman� geldi
            AtesEt();
            // Sonraki at�� zaman�n� ayarla
            sonrakiAtisZamani = Time.time + saldiriAraligi;
        }
    }

    void AtesEt()
    {
        GameObject mermiPrefab;

        // Hangi horoz oldu�umuza bak�p do�ru mermiyi se�elim
        if (enemyType == EnemyType.normalHoroz)
        {
            mermiPrefab = okeyTasiPrefab;
        }
        else // (tipi == HorozTipi.Bornozlu)
        {
            mermiPrefab = ps5KoluPrefab;
        }

        if (mermiPrefab != null && atesNoktasi != null)
        {
            // Mermiyi (prefab�) "Ate� Noktas�" pozisyonunda yarat
            GameObject mermi = Instantiate(mermiPrefab, atesNoktasi.position, Quaternion.identity);

            // Mermiye h�z ver (Lama'ya do�ru)
            // (E�er mermilerin kendi script'i olacaksa bu k�s�m de�i�ebilir)
            Rigidbody2D rb = mermi.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 yon = (lamaTransform.position - atesNoktasi.position).normalized;
                float mermiHizi = 10f; // Mermi h�z�n� buradan ayarla
                rb.linearVelocity = yon * mermiHizi;
            }
        }
    }

    void Turn (bool facingLeft) {
        if (spriteRenderer != null) {
            spriteRenderer.flipX = facingLeft;
        }
    }//diger tarafa donduruyo

    void LamayiAra() {
    
    Vector2 yon = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        // I��n� at ve neye �arpt���na bak
        RaycastHit2D hit = Physics2D.Raycast(transform.position, yon, gorusMesafesi, lamaLayer);

        // E�er ���n bir �eye �arpt�ysa VE �arpt��� �ey "Lama" ise
        if (hit.collider != null)
        {
            Debug.Log("Lama g�r�ld�!");
            lamaTransform = hit.transform; // Laman�n yerini kaydet
            currentState = EnemyState.attack; // Sald�r� moduna ge�
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


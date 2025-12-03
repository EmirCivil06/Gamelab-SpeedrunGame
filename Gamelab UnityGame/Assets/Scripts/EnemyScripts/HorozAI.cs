using Unity.VisualScripting;
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
    public float gorusMesafesi = 10f;
    public float duymaMesafesi = 3f;
    public float mermiGecikmesi = 0.3f;

    public LayerMask lamaLayer;           
    public Transform atesNoktasi;         
    public GameObject okeyTasiPrefab;
    public GameObject ps5KoluPrefab;

    public float saldiriAraligi = 2f;     
    private float sonrakiAtisZamani = 0f;
    public float tepkiSuresi = 0.5f;
    private Animator animator;

    private Transform lamaTransform; 

    private AudioSource throwing;


    void Start(){
        currentState = EnemyState.idle;

        //targetPoint = pointB;

        animator = GetComponentInChildren<Animator>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        throwing = GetComponentInChildren<AudioSource>();

        if (spriteRenderer == null) {
        Debug.Log("horozun spriterenderer'i yok kingo.");
        }

        animator.Play(0, -1, Random.Range(0f, 1f));
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
        if (lamaTransform == null)
        {
            currentState = EnemyState.idle;
            return;
        }

        Vector2 yon = (lamaTransform.position - transform.position).normalized;
        Turn(yon.x < 0);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, yon, gorusMesafesi, lamaLayer);

        if (hit.collider == null || hit.transform != lamaTransform)
        {
            lamaTransform = null;
            currentState = EnemyState.idle;
            return;
        }

        if (Time.time >= sonrakiAtisZamani)
        {
            AtesEt();
            sonrakiAtisZamani = Time.time + saldiriAraligi;
        }
    }

    void AtesEt()
    {
        StartCoroutine(AtesEtmeIslemi());
    }

    System.Collections.IEnumerator AtesEtmeIslemi() {

        if (animator != null) {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(mermiGecikmesi);

        GameObject mermiPrefab = null;

        if (enemyType == EnemyType.normalHoroz)
            mermiPrefab = okeyTasiPrefab;
        else
            mermiPrefab = ps5KoluPrefab;

        if (mermiPrefab != null && atesNoktasi != null) {
            GameObject mermi = Instantiate(mermiPrefab, atesNoktasi.position, Quaternion.identity);
            throwing.PlayOneShot(throwing.clip);

            Rigidbody2D rb = mermi.GetComponent<Rigidbody2D>();

            if (rb != null && lamaTransform != null) {

                Vector2 yon = (lamaTransform.position - atesNoktasi.position).normalized;
                float mermiHizi = 10f;
                rb.linearVelocity = yon * mermiHizi;
            }
        }
    }

    void Turn (bool facingLeft) {

        if (facingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        else {

            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void LamayiAra()
    {
        Collider2D arkaHit = Physics2D.OverlapCircle(transform.position, duymaMesafesi, lamaLayer);

        if (arkaHit != null) {
            TespitEt(arkaHit.transform);
            return;
        }


        Vector2 yon = -transform.right;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, yon, gorusMesafesi, lamaLayer);

        if (hit.collider != null)
        {
            TespitEt(hit.transform);
        }
    }

    void TespitEt(Transform hedef) {
        lamaTransform = hedef;
        sonrakiAtisZamani = Time.time + tepkiSuresi;
        currentState = EnemyState.attack;
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
using UnityEngine;

public class HorozAI : MonoBehaviour {

public enum EnemyState{
        patrol,
        attack
}

    public EnemyState currentState;

    public Transform pointA;
    public Transform pointB;
    public float movementSpeed = 3f;
    private Transform targetPoint;
    private SpriteRenderer spriteRenderer;


    void Start(){
        currentState = EnemyState.patrol;

        targetPoint = pointB;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null) {
        Debug.Log("horozun spriterenderer'i yok kingo.");
        }
    }

    private void Update()
    {
        switch (currentState) {
            case EnemyState.patrol:
                Patrol();
                break;

            case EnemyState.attack:
                Attack();
                break;
        }
    }

    void Patrol()
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
    }

    void Attack() {
        Debug.Log("LAMA'YA SALDIRIYORUM!");
        //Lama hala goruste mi diye kontrol etsin.
    }

    void Turn (bool facingLeft) {
        if (spriteRenderer != null) {
            spriteRenderer.flipX = facingLeft;
        }
    }//diger tarafa donduruyo
}


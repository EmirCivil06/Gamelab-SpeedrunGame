using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    // NOT: Unity'nin yeni input sistemini kullandık. public metotları player'a verdiğimiz 
    // input komponentinde bir takım atamalar için oluşturduk
    [Header("Komponentler")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Ayarlar")]
    [SerializeField] private float movingSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Yer Kontrolü")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // Hareket eylemi sonucu oluşan yön float değerini tutmak için
    private float horizontal;

    // Yatay hareket için
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * movingSpeed, rb.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    // Zıplama metodu
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    // Tekrar tekrar zıplamayı engellemek için yere değiyor mu değmiyor mu diye bakıyoz (yer kontrolü yani)
    private bool isGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }
    // NOT!!!!
    // Player objesine yer kontrolü için bir adet Capsule Collider 2D, yer objeleri için (LayerMask için) 
    // yeni bir "Ground" layerı ve OYUNCU DUVARA YAPIŞMASIN DİYE bir Pyhsics2D materyali ekledik.
    // Bunları MANTIK ÇERÇEVESİNDE elleyebilirsiniz (capsule collider hariç).
    // LEVEL TASARIMINDA "Ground" LAYER'I KULLANMANIZ GEREKİR!!!!
}

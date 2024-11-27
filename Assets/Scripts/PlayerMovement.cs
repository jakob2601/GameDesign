using UnityEngine;

public class TopDownPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Normale Geschwindigkeit des Spielers
    public float dashSpeed = 10f; // Geschwindigkeit während des Dashes
    public float dashDuration = 0.2f; // Dauer des Dashes in Sekunden
    public float dashCooldown = 1f; // Abklingzeit zwischen Dashes
    public Animator animator;  // Animation für Character

    private Rigidbody2D rb; // Rigidbody2D-Komponente
    private Vector2 moveInput; // Bewegungseingabe
    private Vector2 lastMoveDirection; // letzte Bewegungseingabe
    private bool isDashing = false; // Ob der Spieler aktuell dashen kann
    private float dashCooldownTimer = 0f; // Zeit bis zum nächsten Dash
    private bool isFacingRight = false; // der Spieler wendet sich rechte Seite zu


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D zuweisen

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ProccessInputs();
        Animate();

        if(moveInput.x <0 && !isFacingRight || moveInput.x > 0 && isFacingRight)
        {
            flip();
        }

        // Dash-Mechanik prüfen
        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0f && !isDashing && moveInput != Vector2.zero)
        {
            StartCoroutine(Dash());
        }

        // Dash-Cooldown herunterzählen
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    void ProccessInputs()
{
    float moveX = Input.GetAxisRaw("Horizontal");
    float moveY = Input.GetAxisRaw("Vertical");

    // Toleranz für kleine Eingabewerte
    if (Mathf.Abs(moveX) < 0.1f) moveX = 0;
    if (Mathf.Abs(moveY) < 0.1f) moveY = 0;

    if (moveX != 0 || moveY != 0)
    {
        lastMoveDirection = new Vector2(moveX, moveY).normalized;
    }

    moveInput = new Vector2(moveX, moveY).normalized;
}


    void Animate()
    {
        animator.SetFloat("Horizontal", moveInput.x); // Setzen horizontale Bewegung zur Animation
        animator.SetFloat("Vertical", moveInput.y); // Setzen verticale Bewegung zur Animation
        animator.SetFloat("Speed", moveInput.sqrMagnitude); // Bewegungsgeschwindigkeit
        animator.SetFloat("StayHorizontal", lastMoveDirection.x);  
        animator.SetFloat("StayVertical", lastMoveDirection.y);
    }

    void flip()
    {
        // flip Sprite sheet,  Spieler sich nach links bewegen kann 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        isFacingRight = !isFacingRight;
    }

    private void FixedUpdate()
    {
    // Normale Bewegung, wenn nicht gedasht wird und Bewegungseingabe vorhanden ist
    if (!isDashing && moveInput.magnitude > 0.01f) // Schwellenwert für Bewegung
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
    }

    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        Vector2 dashDirection = moveInput.normalized; // Richtung des Dashes basierend auf der Eingabe

        float dashTime = 0f;
        while (dashTime < dashDuration)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            dashTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate(); // Warte auf das nächste Physik-Update
        }

        isDashing = false;

        // Starte den Dash-Cooldown
        dashCooldownTimer = dashCooldown;
    }
}

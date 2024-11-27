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
        //Speicher letze Position des Spielers
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        if((moveX == 0 && moveY == 0) && (moveInput.x != 0 || moveInput.y != 0))
        {
            lastMoveDirection = moveInput;
        }
        // Bewegungseingabe für x- und y-Achse
        moveInput.x = Input.GetAxis("Horizontal"); // A/D oder Pfeiltasten (x)
        moveInput.y = Input.GetAxis("Vertical");   // W/S oder Pfeiltasten (y)
        moveInput.Normalize(); // Alle Bewegungsrichtung in der gleichen Geschwind.
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
        // Normale Bewegung, wenn nicht gedasht wird
        if (!isDashing)
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
